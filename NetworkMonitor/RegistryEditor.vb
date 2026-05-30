Imports System.ComponentModel
Imports System.Net.NetworkInformation
Imports System.Reflection
Imports Microsoft.Win32

Public Class RegistryEditor

    Private Const _registryParentFolder As String = "NetworkMonitor"
    Private Const _registryMainForm As String = _registryParentFolder & "\MainForm"

    Private Const VERSION_KEY As String = "Version"
    Private Const SIZE_KEY As String = "Size"
    Private Const LOCATION_KEY As String = "Location"
    Private Const TOPMOST_KEY As String = "TopMost"
    Private Const OPACITY_KEY As String = "Opacity"
    Private Const NETWORK_ADAPTER_NAME_KEY As String = "NetworkAdapterName"

    Public Shared Sub CreateRegistryKeys()
        Try
            Registry.CurrentUser.CreateSubKey(_registryParentFolder)
            Registry.CurrentUser.CreateSubKey(_registryMainForm)
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

    Public Shared Sub LoadRegistryKeys()
        Try
            ' Version check — reset all keys if version changed or missing
            Using parentKey = Registry.CurrentUser.OpenSubKey(_registryParentFolder, True)
                If parentKey Is Nothing Then Return
                Dim currentVersion As String = My.Application.Info.Version.ToString()
                Dim savedVersion As String = parentKey.GetValue(VERSION_KEY)?.ToString()
                If savedVersion Is Nothing OrElse savedVersion <> currentVersion Then
                    ResetRegistryKeys()
                    CreateRegistryKeys()
                    Using fresh = Registry.CurrentUser.OpenSubKey(_registryParentFolder, True)
                        fresh?.SetValue(VERSION_KEY, currentVersion)
                    End Using
                    Return  ' Keys were just reset; defaults will be saved on next close
                End If
            End Using

            Using key = Registry.CurrentUser.OpenSubKey(_registryMainForm, True)
                If key Is Nothing Then Return

                ' Size
                Dim sizeRaw = key.GetValue(SIZE_KEY)
                If sizeRaw Is Nothing Then
                    key.SetValue(SIZE_KEY, $"{MainForm.Width},{MainForm.Height}")
                Else
                    Dim savedSize As Size = TypeDescriptor.GetConverter(GetType(Size)).ConvertFromString(sizeRaw.ToString())
                    If savedSize.Width >= MainForm.MinimumSize.Width AndAlso savedSize.Height >= MainForm.MinimumSize.Height Then
                        MainForm.Size = savedSize
                    Else
                        MainForm.Size = MainForm.MinimumSize
                        key.SetValue(SIZE_KEY, $"{MainForm.MinimumSize.Width},{MainForm.MinimumSize.Height}")
                    End If
                End If

                ' Location
                Dim locationRaw = key.GetValue(LOCATION_KEY)
                If locationRaw Is Nothing Then
                    key.SetValue(LOCATION_KEY, $"{MainForm.Left},{MainForm.Top}")
                Else
                    Dim savedLocation As Point = TypeDescriptor.GetConverter(GetType(Point)).ConvertFromString(locationRaw.ToString())
                    Dim rect As New Rectangle(savedLocation, MainForm.Size)
                    If FormOnScreen(rect) IsNot Nothing Then
                        MainForm.Location = savedLocation
                    Else
                        If MainForm.Width > My.Computer.Screen.WorkingArea.Width Then
                            MainForm.Width = My.Computer.Screen.WorkingArea.Width
                            key.SetValue(SIZE_KEY, $"{MainForm.Width},{MainForm.Height}")
                        End If
                        MainForm.Location = New Point(My.Computer.Screen.WorkingArea.Width - MainForm.Width, My.Computer.Screen.WorkingArea.Height - MainForm.Height)
                        key.SetValue(LOCATION_KEY, $"{MainForm.Left},{MainForm.Top}")
                    End If
                End If

                ' TopMost
                Dim topMostRaw = key.GetValue(TOPMOST_KEY)
                If topMostRaw Is Nothing Then
                    key.SetValue(TOPMOST_KEY, $"{MainForm.TopMost}")
                Else
                    Dim savedTopMost As Boolean = TypeDescriptor.GetConverter(GetType(Boolean)).ConvertFromString(topMostRaw.ToString())
                    MainForm.TopMost = savedTopMost
                    MainForm.TopMostToolStripMenuItem.Checked = savedTopMost
                End If

                ' Network adapter
                Dim adapterRaw = key.GetValue(NETWORK_ADAPTER_NAME_KEY)
                If adapterRaw Is Nothing Then
                    key.SetValue(NETWORK_ADAPTER_NAME_KEY, "Ethernet")
                Else
                    Dim savedName As String = adapterRaw.ToString()
                    For Each ni As NetworkInterface In Network.GetOperationalInterfaces("Loopback", "Teredo")
                        If savedName = ni.Name Then
                            Network.ActiveInterface = ni
                            Exit For
                        End If
                    Next
                End If

                ' Opacity
                Dim opacityRaw = key.GetValue(OPACITY_KEY)
                If opacityRaw Is Nothing Then
                    key.SetValue(OPACITY_KEY, "0.5")
                Else
                    Dim savedOpacity As Double = TypeDescriptor.GetConverter(GetType(Double)).ConvertFromString(opacityRaw.ToString())
                    MainForm.Opacity = savedOpacity
                    MainForm._unfocusedOpacity = savedOpacity
                End If
            End Using
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

    Private Shared Function FormOnScreen(rect As Rectangle) As Screen
        For Each s As Screen In Screen.AllScreens
            If s.Bounds.Contains(rect) Then
                Return s
            End If
        Next
        Return Nothing
    End Function

    Public Shared Sub SaveRegistryKeys()
        Try
            Using key = Registry.CurrentUser.OpenSubKey(_registryMainForm, True)
                If key Is Nothing Then Return
                key.SetValue(SIZE_KEY, $"{MainForm.Width},{MainForm.Height}")
                key.SetValue(LOCATION_KEY, $"{MainForm.Left},{MainForm.Top}")
                key.SetValue(TOPMOST_KEY, $"{MainForm.TopMost}")
                key.SetValue(OPACITY_KEY, $"{MainForm._unfocusedOpacity}")
                If Network.ActiveInterface IsNot Nothing Then
                    key.SetValue(NETWORK_ADAPTER_NAME_KEY, Network.ActiveInterface.Name)
                End If
            End Using
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

    Public Shared Sub ResetRegistryKeys()
        Try
            If Registry.CurrentUser.OpenSubKey(_registryParentFolder) IsNot Nothing Then
                Registry.CurrentUser.DeleteSubKeyTree(_registryParentFolder)
            End If
        Catch ex As Exception
            Logger.Log(ex, MethodBase.GetCurrentMethod().Name)
        End Try
    End Sub

End Class
