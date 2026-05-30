Imports System.Threading
Imports System.Threading.Tasks
Imports System.Reflection
Imports System.Speech.Synthesis
Imports Microsoft.Win32
Imports System.Net.NetworkInformation

Public Class MainForm

    ' Windows API
    Declare Function timeBeginPeriod Lib "winmm.dll" (uPeriod As Integer) As Integer
    Declare Function timeEndPeriod Lib "winmm.dll" (uPeriod As Integer) As Integer
    Declare Function RegisterHotKey Lib "user32" (hWnd As IntPtr, id As Integer, fsModifiers As UInteger, vk As UInteger) As Boolean
    Declare Function UnregisterHotKey Lib "user32" (hWnd As IntPtr, id As Integer) As Boolean
    Declare Function ReleaseCapture Lib "user32" () As Boolean
    Declare Function SendMessage Lib "user32" Alias "SendMessageW" (hWnd As IntPtr, msg As Integer, wParam As IntPtr, lParam As IntPtr) As IntPtr

    ' WndProc message constants
    Private Const WM_NCHITTEST As Integer = &H84
    Private Const WM_NCLBUTTONDOWN As Integer = &HA1
    Private Const WM_ENTERSIZEMOVE As Integer = &H231
    Private Const WM_EXITSIZEMOVE As Integer = &H232
    Private Const WM_SIZING As Integer = &H214
    Private Const WM_HOTKEY As Integer = &H312
    Private Const HTCLIENT As Integer = 1
    Private Const HTLEFT As Integer = 10
    Private Const HTRIGHT As Integer = 11
    Private Const MOD_CONTROL As UInteger = 2UI

    ' Hotkey IDs
    Private Const HK_CTRL_F1 As Integer = 1
    Private Const HK_CTRL_F2 As Integer = 2
    Private Const HK_CTRL_F3 As Integer = 3
    Private Const HK_CTRL_F4 As Integer = 4
    Private Const HK_CTRL_F5 As Integer = 5
    Private Const HK_CTRL_F6 As Integer = 6
    Private Const HK_CTRL_F7 As Integer = 7
    Private Const HK_CTRL_F8 As Integer = 8
    Private Const HK_CTRL_F12 As Integer = 9

    ' Form state
    Private _formIsClosing As Boolean = False
    Private _move As Boolean = False
    Private _isResizing As Boolean = False
    Private _originalFormColor As Color
    Private ReadOnly _resizePadding As Integer = 3
    Private ReadOnly _autoSnapDistance As Integer = 10
    Private _dragGrabX As Integer = 0
    Private _dragGrabY As Integer = 0
    Private _resetSessionStatsNextCycle As Integer = 1  ' 1 = true, 0 = false; accessed via Interlocked
    Private _colourChangerRunning As Integer = 0        ' 1 = running; gate via Interlocked.CompareExchange
    Private ReadOnly _cts As New CancellationTokenSource()

    Public Shared _unfocusedOpacity As Double = 0.5

    ' TTS — single shared instance, skip if already speaking
    Private ReadOnly _sapi As New SpeechSynthesizer
    Private ReadOnly _sapiLock As New Object

    Private ReadOnly _totalDownloadUploadToolTip As New ToolTip()

    ' Cached label tag strings — set once at load, never change
    Private _tagDLDTotal As String, _tagDLDSession As String, _tagDLDCurrent As String, _tagDLDAverage As String
    Private _tagULDTotal As String, _tagULDSession As String, _tagULDCurrent As String, _tagULDAverage As String

    ' Adapter re-enumeration throttle: check every 10 cycles, or immediately when adapter is lost
    Private _adapterCheckCountdown As Integer = 0

    Private Sub MainForm_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Try
            timeBeginPeriod(1)
            SetMainFormDefaults()

            RegistryEditor.CreateRegistryKeys()
            RegistryEditor.LoadRegistryKeys()

            _sapi.SelectVoiceByHints(VoiceGender.Female, VoiceAge.NotSet)
            RegisterHotKeys()
            GenerateEvents()
            CacheLabelTags()

            ThreadPool.QueueUserWorkItem(AddressOf NetworkLoop)
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

    Private Sub SetMainFormDefaults()
        Size = New Size(200, 254)
        Location = New Point(My.Computer.Screen.WorkingArea.Width - Width, My.Computer.Screen.WorkingArea.Height - Height)
    End Sub

    Private Sub RegisterHotKeys()
        RegisterHotKey(Handle, HK_CTRL_F1, MOD_CONTROL, CUInt(Keys.F1))
        RegisterHotKey(Handle, HK_CTRL_F2, MOD_CONTROL, CUInt(Keys.F2))
        RegisterHotKey(Handle, HK_CTRL_F3, MOD_CONTROL, CUInt(Keys.F3))
        RegisterHotKey(Handle, HK_CTRL_F4, MOD_CONTROL, CUInt(Keys.F4))
        RegisterHotKey(Handle, HK_CTRL_F5, MOD_CONTROL, CUInt(Keys.F5))
        RegisterHotKey(Handle, HK_CTRL_F6, MOD_CONTROL, CUInt(Keys.F6))
        RegisterHotKey(Handle, HK_CTRL_F7, MOD_CONTROL, CUInt(Keys.F7))
        RegisterHotKey(Handle, HK_CTRL_F8, MOD_CONTROL, CUInt(Keys.F8))
        RegisterHotKey(Handle, HK_CTRL_F12, MOD_CONTROL, CUInt(Keys.F12))
    End Sub

    Private Sub UnregisterHotKeys()
        For i As Integer = HK_CTRL_F1 To HK_CTRL_F12
            UnregisterHotKey(Handle, i)
        Next
    End Sub

    Private Sub GenerateEvents()
        AddHandler MouseDown, AddressOf Everything_MouseDown
        AddHandler MouseUp, AddressOf Everything_MouseUp
        AddHandler MouseMove, AddressOf Everything_MouseMove
        AddHandler MouseClick, AddressOf Everything_MouseClick
        AddHandler MouseEnter, AddressOf Everything_MouseEnter
        AddHandler MouseLeave, AddressOf Everything_MouseLeave
        AddHandler KeyDown, AddressOf Everything_KeyDown
        For Each ctrl As Control In GetAllControls(Me)
            AddHandler ctrl.MouseDown, AddressOf Everything_MouseDown
            AddHandler ctrl.MouseUp, AddressOf Everything_MouseUp
            AddHandler ctrl.MouseMove, AddressOf Everything_MouseMove
            AddHandler ctrl.MouseClick, AddressOf Everything_MouseClick
            AddHandler ctrl.MouseEnter, AddressOf Everything_MouseEnter
            AddHandler ctrl.MouseLeave, AddressOf Everything_MouseLeave
        Next
    End Sub

    Private Sub CacheLabelTags()
        _tagDLDTotal = LabelHeaderDLDTotal.Tag.ToString()
        _tagDLDSession = LabelHeaderDLDSession.Tag.ToString()
        _tagDLDCurrent = LabelHeaderDLDCurrent.Tag.ToString()
        _tagDLDAverage = LabelHeaderDLDAverage.Tag.ToString()
        _tagULDTotal = LabelHeaderULDTotal.Tag.ToString()
        _tagULDSession = LabelHeaderULDSession.Tag.ToString()
        _tagULDCurrent = LabelHeaderULDCurrent.Tag.ToString()
        _tagULDAverage = LabelHeaderULDAverage.Tag.ToString()
    End Sub

    Private Iterator Function GetAllControls(parent As Control) As IEnumerable(Of Control)
        For Each ctrl As Control In parent.Controls
            Yield ctrl
            For Each child In GetAllControls(ctrl)
                Yield child
            Next
        Next
    End Function

    Protected Overrides Sub WndProc(ByRef m As Message)
        Select Case m.Msg
            Case WM_NCHITTEST
                ' Show resize cursor when mouse is over the form background near an edge
                MyBase.WndProc(m)
                If m.Result.ToInt32() = HTCLIENT Then
                    Dim x As Integer = Cursor.Position.X
                    If x >= Left AndAlso x < Left + _resizePadding Then
                        m.Result = New IntPtr(HTLEFT)
                    ElseIf x < Right AndAlso x >= Right - _resizePadding Then
                        m.Result = New IntPtr(HTRIGHT)
                    End If
                End If
                Return

            Case WM_ENTERSIZEMOVE
                ' Fires when Windows starts a resize drag (from SendMessage HTLEFT/HTRIGHT)
                _originalFormColor = BackColor
                _isResizing = True
                PanelStats.Visible = False
                DisplayerLabel.Visible = True
                If Interlocked.CompareExchange(_colourChangerRunning, 1, 0) = 0 Then
                    ThreadPool.QueueUserWorkItem(AddressOf ColourChanger)
                End If

            Case WM_EXITSIZEMOVE
                _isResizing = False
                RegistryEditor.SaveRegistryKeys()

            Case WM_SIZING
                DisplayerLabel.Text = Width & " px"

            Case WM_HOTKEY
                Dim hotkeyId As Integer = m.WParam.ToInt32()
                Task.Run(Sub() SpeakHotKey(hotkeyId))
        End Select
        MyBase.WndProc(m)
    End Sub

    Private Sub SpeakHotKey(id As Integer)
        If Not Monitor.TryEnter(_sapiLock) Then Return
        Try
            Dim dlTotal, dlSession, dlCurrent, dlAverage As Decimal
            Dim ulTotal, ulSession, ulCurrent, ulAverage As Decimal
            SyncLock StatsSync.Lock
                dlTotal = Download.Total : dlSession = Download.Session
                dlCurrent = Download.Current : dlAverage = Download.Average
                ulTotal = Upload.Total : ulSession = Upload.Session
                ulCurrent = Upload.Current : ulAverage = Upload.Average
            End SyncLock

            Dim text As String = String.Empty
            Select Case id
                Case HK_CTRL_F1 : text = "Total Download: " & FixSizesWithLongWords(dlTotal)
                Case HK_CTRL_F2 : text = "Session Download: " & FixSizesWithLongWords(dlSession)
                Case HK_CTRL_F3 : text = "Current Download: " & FixSizesWithLongWords(dlCurrent) & " a second"
                Case HK_CTRL_F4 : text = "Average Download: " & FixSizesWithLongWords(dlAverage) & " a second"
                Case HK_CTRL_F5 : text = "Total Upload: " & FixSizesWithLongWords(ulTotal)
                Case HK_CTRL_F6 : text = "Session Upload: " & FixSizesWithLongWords(ulSession)
                Case HK_CTRL_F7 : text = "Current Upload: " & FixSizesWithLongWords(ulCurrent) & " a second"
                Case HK_CTRL_F8 : text = "Average Upload: " & FixSizesWithLongWords(ulAverage) & " a second"
                Case HK_CTRL_F12
                    Interlocked.Exchange(_resetSessionStatsNextCycle, 1)
                    text = "Resetting Session Stats."
            End Select
            If Not String.IsNullOrEmpty(text) Then
                _sapi.Speak(text)
            End If
        Finally
            Monitor.Exit(_sapiLock)
        End Try
    End Sub

    Private Sub ResetSessionStats()
        SyncLock StatsSync.Lock
            Download.Total = Network.GetDownloadedBytes()
            Download.SessionOld = Download.Total
            Download.TotalOld = Download.Total
            Download.AverageCount = 1
            Upload.Total = Network.GetUploadedBytes()
            Upload.SessionOld = Upload.Total
            Upload.TotalOld = Upload.Total
            Upload.AverageCount = 1
        End SyncLock
    End Sub

    Private Sub NetworkLoop()
        Try
            ResetSessionStats()
            Dim sw As New Stopwatch
            While Not _cts.IsCancellationRequested
                sw.Restart()
                If Interlocked.CompareExchange(_resetSessionStatsNextCycle, 0, 1) = 1 Then
                    ResetSessionStats()
                Else
                    _adapterCheckCountdown -= 1
                    Dim networkIDsMatched As Boolean = Network.ActiveInterface IsNot Nothing AndAlso _adapterCheckCountdown > 0
                    If Not networkIDsMatched Then
                        _adapterCheckCountdown = 10
                        Dim interfaces = Network.GetOperationalInterfaces("Loopback", "Teredo")
                        For Each ni As NetworkInterface In interfaces
                            If Network.ActiveInterface IsNot Nothing AndAlso Network.ActiveInterface.Id = ni.Id Then
                                networkIDsMatched = True
                                Exit For
                            End If
                        Next
                        If Not networkIDsMatched Then
                            Network.ActiveInterface = Nothing
                            For Each ni As NetworkInterface In interfaces
                                If ni.OperationalStatus = OperationalStatus.Up Then
                                    Network.ActiveInterface = ni
                                    ResetSessionStats()
                                    Exit For
                                End If
                            Next
                        End If
                    End If

                    If Network.ActiveInterface IsNot Nothing AndAlso networkIDsMatched Then
                        Dim dlBytes = Network.GetDownloadedBytes()
                        Dim ulBytes = Network.GetUploadedBytes()

                        SyncLock StatsSync.Lock
                            Download.Total = dlBytes
                            Download.Current = Download.Total - Download.TotalOld
                            Download.Session = Download.Total - Download.SessionOld
                            Upload.Total = ulBytes
                            Upload.Current = Upload.Total - Upload.TotalOld
                            Upload.Session = Upload.Total - Upload.SessionOld

                            Download.Average = Download.Session / Download.AverageCount
                            Download.AverageCount += 1
                            Download.TotalOld = Download.Total
                            Upload.Average = Upload.Session / Upload.AverageCount
                            Upload.AverageCount += 1
                            Upload.TotalOld = Upload.Total
                        End SyncLock

                        If Not _cts.IsCancellationRequested Then
                            Try
                                BeginInvoke(Sub()
                                                If Not _cts.IsCancellationRequested Then
                                                    RefreshLabels()
                                                    RefreshGraphs()
                                                End If
                                            End Sub)
                            Catch ex As ObjectDisposedException
                                Return
                            End Try
                        End If
                    End If
                End If

                Dim toSleep As Long = 1000 - sw.ElapsedMilliseconds
                If toSleep > 0 Then
                    Thread.Sleep(CInt(Math.Min(toSleep, 1000)))
                End If
            End While
        Catch ex As Exception When Not _cts.IsCancellationRequested
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
            ThreadPool.QueueUserWorkItem(AddressOf NetworkLoop)
        End Try
    End Sub

    Private Sub ColourChanger(state As Object)
        Try
            Dim x As Integer = 0, y As Integer = 0, z As Integer = 0
            Dim rgb As String = "r"
            Const increment As Integer = 5
            While _move Or _isResizing
                Dim color As Color = Color.FromArgb(255, x, y, z)
                BeginInvoke(Sub() BackColor = color)
                Select Case rgb
                    Case "r" : x += increment : If x >= 255 Then rgb = "g"
                    Case "g" : y += increment : If y >= 255 Then rgb = "b"
                    Case "b" : z += increment : If z >= 255 Then rgb = "R"
                    Case "R" : x -= increment : If x <= 0 Then rgb = "G"
                    Case "G" : y -= increment : If y <= 0 Then rgb = "B"
                    Case "B" : z -= increment : If z <= 0 Then rgb = "r"
                End Select
                Thread.Sleep(30)
            End While
            If Not _cts.IsCancellationRequested Then
                Try
                    Invoke(Sub()
                               BackColor = _originalFormColor
                               PanelStats.Visible = True
                               DisplayerLabel.Visible = False
                           End Sub)
                Catch ex As ObjectDisposedException
                End Try
            End If
            Interlocked.Exchange(_colourChangerRunning, 0)
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

    Private Sub RefreshLabels()
        Try
            Dim dlTotal, dlSession, dlCurrent, dlAverage As Decimal
            Dim ulTotal, ulSession, ulCurrent, ulAverage As Decimal
            SyncLock StatsSync.Lock
                dlTotal = Download.Total : dlSession = Download.Session
                dlCurrent = Download.Current : dlAverage = Download.Average
                ulTotal = Upload.Total : ulSession = Upload.Session
                ulCurrent = Upload.Current : ulAverage = Upload.Average
            End SyncLock

            Dim r As Tuple(Of Decimal, String, String)

            r = ConvertBytes(dlTotal) : LabelDLDTotal.Text = Math.Round(r.Item1, 2).ToString() : LabelHeaderDLDTotal.Text = _tagDLDTotal.Replace("XX", r.Item2)
            r = ConvertBytes(dlSession) : LabelDLDSession.Text = Math.Round(r.Item1, 2).ToString() : LabelHeaderDLDSession.Text = _tagDLDSession.Replace("XX", r.Item2)
            r = ConvertBytes(dlCurrent) : LabelDLDCurrent.Text = Math.Round(r.Item1, 2).ToString() : LabelHeaderDLDCurrent.Text = _tagDLDCurrent.Replace("XX", r.Item2)
            r = ConvertBytes(dlAverage) : LabelDLDAverage.Text = Math.Round(r.Item1, 2).ToString() : LabelHeaderDLDAverage.Text = _tagDLDAverage.Replace("XX", r.Item2)

            r = ConvertBytes(ulTotal) : LabelULDTotal.Text = Math.Round(r.Item1, 2).ToString() : LabelHeaderULDTotal.Text = _tagULDTotal.Replace("XX", r.Item2)
            r = ConvertBytes(ulSession) : LabelULDSession.Text = Math.Round(r.Item1, 2).ToString() : LabelHeaderULDSession.Text = _tagULDSession.Replace("XX", r.Item2)
            r = ConvertBytes(ulCurrent) : LabelULDCurrent.Text = Math.Round(r.Item1, 2).ToString() : LabelHeaderULDCurrent.Text = _tagULDCurrent.Replace("XX", r.Item2)
            r = ConvertBytes(ulAverage) : LabelULDAverage.Text = Math.Round(r.Item1, 2).ToString() : LabelHeaderULDAverage.Text = _tagULDAverage.Replace("XX", r.Item2)
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

    Private Sub RefreshGraphs()
        Try
            Dim dlCurrent, ulCurrent As Decimal
            SyncLock StatsSync.Lock
                dlCurrent = Download.Current
                ulCurrent = Upload.Current
            End SyncLock
            GraphyDownload.AddValue(CSng(dlCurrent))
            GraphyUpload.AddValue(CSng(ulCurrent))
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

    Private Shared Function ConvertBytes(bytes As Decimal) As Tuple(Of Decimal, String, String)
        If bytes >= 1125899906842624D Then
            Return Tuple.Create(bytes / 1125899906842624D, "PB", "Petabytes")
        ElseIf bytes >= 1099511627776D Then
            Return Tuple.Create(bytes / 1099511627776D, "TB", "Terabytes")
        ElseIf bytes >= 1073741824D Then
            Return Tuple.Create(bytes / 1073741824D, "GB", "Gigabytes")
        ElseIf bytes >= 1048576D Then
            Return Tuple.Create(bytes / 1048576D, "MB", "Megabytes")
        ElseIf bytes >= 1024D Then
            Return Tuple.Create(bytes / 1024D, "KB", "Kilobytes")
        Else
            Return Tuple.Create(bytes, "B", "Bytes")
        End If
    End Function

    Private Shared Function FixSizesWithLongWords(bytes As Decimal, Optional roundTo As Integer = 2) As String
        Dim r = ConvertBytes(bytes)
        Return $"{Math.Round(r.Item1, roundTo)} {r.Item3}"
    End Function

    Private Sub Everything_MouseDown(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left Then
            Dim x As Integer = Cursor.Position.X
            If x >= Left AndAlso x < Left + _resizePadding Then
                ReleaseCapture()
                SendMessage(Handle, WM_NCLBUTTONDOWN, New IntPtr(HTLEFT), IntPtr.Zero)
            ElseIf x < Right AndAlso x >= Right - _resizePadding Then
                ReleaseCapture()
                SendMessage(Handle, WM_NCLBUTTONDOWN, New IntPtr(HTRIGHT), IntPtr.Zero)
            Else
                _dragGrabX = Cursor.Position.X - Left
                _dragGrabY = Cursor.Position.Y - Top
                _move = True
                _originalFormColor = BackColor
                Cursor = Cursors.SizeAll
                PanelStats.Visible = False
                DisplayerLabel.Visible = True
                If Interlocked.CompareExchange(_colourChangerRunning, 1, 0) = 0 Then
                    ThreadPool.QueueUserWorkItem(AddressOf ColourChanger)
                End If
            End If
        End If
    End Sub

    Private Sub Everything_MouseUp(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Left AndAlso _move Then
            _move = False
            Cursor = Cursors.Default
            RegistryEditor.SaveRegistryKeys()
        End If
    End Sub

    Private Sub Everything_MouseMove(sender As Object, e As MouseEventArgs)
        If _move Then
            Dim x As Integer = Cursor.Position.X
            Dim y As Integer = Cursor.Position.Y
            Dim workingArea As Rectangle = Screen.GetWorkingArea(Cursor.Position)

            ' Snap zone: two-sided, centred on the cursor position where the window
            ' edge would land flush with the screen edge (original logic).
            If x < workingArea.Left + _dragGrabX + _autoSnapDistance AndAlso x > workingArea.Left + _dragGrabX - _autoSnapDistance Then
                Left = workingArea.Left
            ElseIf x > workingArea.Right - (Width - _dragGrabX) - _autoSnapDistance AndAlso x < workingArea.Right - (Width - _dragGrabX) + _autoSnapDistance Then
                Left = workingArea.Right - Width
            Else
                Left = x - _dragGrabX
            End If

            If y < workingArea.Top + _dragGrabY + _autoSnapDistance AndAlso y > workingArea.Top + _dragGrabY - _autoSnapDistance Then
                Top = workingArea.Top
            ElseIf y > workingArea.Bottom - (Height - _dragGrabY) - _autoSnapDistance AndAlso y < workingArea.Bottom - (Height - _dragGrabY) + _autoSnapDistance Then
                Top = workingArea.Bottom - Height
            Else
                Top = y - _dragGrabY
            End If

            DisplayerLabel.Text = $"X = {Location.X}{vbNewLine}Y = {Location.Y}"
        ElseIf Not _isResizing Then
            Dim x As Integer = Cursor.Position.X
            If x >= Left AndAlso x < Left + _resizePadding Then
                Cursor = Cursors.SizeWE
            ElseIf x < Right AndAlso x >= Right - _resizePadding Then
                Cursor = Cursors.SizeWE
            Else
                Cursor = Cursors.Default
            End If
        End If
    End Sub

    Private Sub Everything_MouseClick(sender As Object, e As MouseEventArgs)
        If e.Button = MouseButtons.Right Then
            RightClickMenu.Show(Cursor.Position)

            NetworkAdaptersMenuItem.DropDownItems.Clear()
            For Each ni As NetworkInterface In Network.GetOperationalInterfaces("Loopback", "Teredo")
                Dim newMenuItem As New ToolStripMenuItem With {.Text = ni.Name, .Tag = ni}
                AddHandler newMenuItem.Click, AddressOf NetworkAdapterSelected
                NetworkAdaptersMenuItem.DropDownItems.Add(newMenuItem)
            Next
            If Network.ActiveInterface IsNot Nothing Then
                For Each menuItem As ToolStripMenuItem In NetworkAdaptersMenuItem.DropDownItems
                    menuItem.Checked = (menuItem.Text = Network.ActiveInterface.Name)
                Next
            End If

            For Each item As ToolStripMenuItem In UnfocusedOpacityToolStripMenuItem.DropDownItems
                item.Checked = (_unfocusedOpacity = CDbl(item.Tag) / 100)
            Next
        End If
    End Sub

    Private Sub NetworkAdapterSelected(sender As Object, e As EventArgs)
        Try
            Dim menuItem As ToolStripMenuItem = sender
            Interlocked.Exchange(_resetSessionStatsNextCycle, 1)
            Network.ActiveInterface = CType(menuItem.Tag, NetworkInterface)
            RegistryEditor.SaveRegistryKeys()
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

    Private Sub Everything_MouseEnter(sender As Object, e As EventArgs)
        Try
            If Not _formIsClosing Then
                Opacity = 1
            End If
            If TypeOf sender Is Label Then
                If CType(sender, Label).Name.Contains("Total") Then
                    Dim combined As Decimal
                    SyncLock StatsSync.Lock
                        combined = Download.Total + Upload.Total
                    End SyncLock
                    Dim r = ConvertBytes(combined)
                    _totalDownloadUploadToolTip.SetToolTip(sender, $"Total Download and Upload: {Math.Round(r.Item1, 2)} {r.Item2}")
                End If
            End If
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

    Private Sub Everything_MouseLeave(sender As Object, e As EventArgs)
        Try
            If Not _formIsClosing AndAlso Not _move AndAlso Not _isResizing AndAlso Not Bounds.Contains(Cursor.Position) Then
                Opacity = _unfocusedOpacity
            End If
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

    Private Sub Everything_KeyDown(sender As Object, e As KeyEventArgs)
        If e.KeyCode = Keys.Escape Then
            _formIsClosing = True
            RegistryEditor.SaveRegistryKeys()
            Close()
        End If
    End Sub

    Private Sub TopMostToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles TopMostToolStripMenuItem.Click
        TopMost = Not TopMost
        TopMostToolStripMenuItem.Checked = TopMost
        RegistryEditor.SaveRegistryKeys()
    End Sub

    Private Sub TwentyFivePercent_Click(sender As Object, e As EventArgs) Handles TwentyFivePercent.Click
        _unfocusedOpacity = 0.25
        Opacity = _unfocusedOpacity
        RegistryEditor.SaveRegistryKeys()
    End Sub

    Private Sub FiftyPercent_Click(sender As Object, e As EventArgs) Handles FiftyPercent.Click
        _unfocusedOpacity = 0.5
        Opacity = _unfocusedOpacity
        RegistryEditor.SaveRegistryKeys()
    End Sub

    Private Sub SeventyFivePercent_Click(sender As Object, e As EventArgs) Handles SeventyFivePercent.Click
        _unfocusedOpacity = 0.75
        Opacity = _unfocusedOpacity
        RegistryEditor.SaveRegistryKeys()
    End Sub

    Private Sub OnehundredPercent_Click(sender As Object, e As EventArgs) Handles OnehundredPercent.Click
        _unfocusedOpacity = 1
        Opacity = _unfocusedOpacity
        RegistryEditor.SaveRegistryKeys()
    End Sub

    Private Sub ExitToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ExitToolStripMenuItem.Click
        _formIsClosing = True
        RegistryEditor.SaveRegistryKeys()
        Close()
    End Sub

    Protected Overrides Sub OnFormClosed(e As FormClosedEventArgs)
        _cts.Cancel()
        UnregisterHotKeys()
        _sapi.Dispose()
        _cts.Dispose()
        timeEndPeriod(1)
        MyBase.OnFormClosed(e)
    End Sub

End Class
