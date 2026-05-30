<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class MainForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(MainForm))
        Me.PanelStats = New System.Windows.Forms.Panel()
        Me.LabelULDTotal = New System.Windows.Forms.Label()
        Me.LabelHeaderULDTotal = New System.Windows.Forms.Label()
        Me.LabelDLDTotal = New System.Windows.Forms.Label()
        Me.LabelHeaderDLDTotal = New System.Windows.Forms.Label()
        Me.LabelULDAverage = New System.Windows.Forms.Label()
        Me.LabelULDCurrent = New System.Windows.Forms.Label()
        Me.LabelULDSession = New System.Windows.Forms.Label()
        Me.LabelHeaderULDAverage = New System.Windows.Forms.Label()
        Me.LabelHeaderULDCurrent = New System.Windows.Forms.Label()
        Me.LabelHeaderULDSession = New System.Windows.Forms.Label()
        Me.LabelDLDAverage = New System.Windows.Forms.Label()
        Me.LabelDLDCurrent = New System.Windows.Forms.Label()
        Me.LabelDLDSession = New System.Windows.Forms.Label()
        Me.LabelHeaderDLDAverage = New System.Windows.Forms.Label()
        Me.LabelHeaderDLDCurrent = New System.Windows.Forms.Label()
        Me.LabelHeaderDLDSession = New System.Windows.Forms.Label()
        Me.DisplayerLabel = New System.Windows.Forms.Label()
        Me.PanelGraphs = New System.Windows.Forms.Panel()
        Me.GraphyDownload = New Graphy.Graphy()
        Me.GraphyUpload = New Graphy.Graphy()
        Me.RightClickMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.TopMostToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.NetworkAdaptersMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.UnfocusedOpacityToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TwentyFivePercent = New System.Windows.Forms.ToolStripMenuItem()
        Me.FiftyPercent = New System.Windows.Forms.ToolStripMenuItem()
        Me.SeventyFivePercent = New System.Windows.Forms.ToolStripMenuItem()
        Me.OnehundredPercent = New System.Windows.Forms.ToolStripMenuItem()
        Me.ExitToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.PanelStats.SuspendLayout()
        Me.PanelGraphs.SuspendLayout()
        Me.RightClickMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'PanelStats
        '
        Me.PanelStats.Anchor = System.Windows.Forms.AnchorStyles.Top
        Me.PanelStats.BackColor = System.Drawing.Color.Transparent
        Me.PanelStats.Controls.Add(Me.LabelULDTotal)
        Me.PanelStats.Controls.Add(Me.LabelHeaderULDTotal)
        Me.PanelStats.Controls.Add(Me.LabelDLDTotal)
        Me.PanelStats.Controls.Add(Me.LabelHeaderDLDTotal)
        Me.PanelStats.Controls.Add(Me.LabelULDAverage)
        Me.PanelStats.Controls.Add(Me.LabelULDCurrent)
        Me.PanelStats.Controls.Add(Me.LabelULDSession)
        Me.PanelStats.Controls.Add(Me.LabelHeaderULDAverage)
        Me.PanelStats.Controls.Add(Me.LabelHeaderULDCurrent)
        Me.PanelStats.Controls.Add(Me.LabelHeaderULDSession)
        Me.PanelStats.Controls.Add(Me.LabelDLDAverage)
        Me.PanelStats.Controls.Add(Me.LabelDLDCurrent)
        Me.PanelStats.Controls.Add(Me.LabelDLDSession)
        Me.PanelStats.Controls.Add(Me.LabelHeaderDLDAverage)
        Me.PanelStats.Controls.Add(Me.LabelHeaderDLDCurrent)
        Me.PanelStats.Controls.Add(Me.LabelHeaderDLDSession)
        Me.PanelStats.Location = New System.Drawing.Point(0, 0)
        Me.PanelStats.Name = "PanelStats"
        Me.PanelStats.Size = New System.Drawing.Size(200, 54)
        Me.PanelStats.TabIndex = 19
        '
        'LabelULDTotal
        '
        Me.LabelULDTotal.AutoSize = True
        Me.LabelULDTotal.ForeColor = System.Drawing.Color.RoyalBlue
        Me.LabelULDTotal.Location = New System.Drawing.Point(-1, 40)
        Me.LabelULDTotal.Name = "LabelULDTotal"
        Me.LabelULDTotal.Size = New System.Drawing.Size(46, 13)
        Me.LabelULDTotal.TabIndex = 32
        Me.LabelULDTotal.Text = "0000,00"
        '
        'LabelHeaderULDTotal
        '
        Me.LabelHeaderULDTotal.AutoSize = True
        Me.LabelHeaderULDTotal.ForeColor = System.Drawing.Color.RoyalBlue
        Me.LabelHeaderULDTotal.Location = New System.Drawing.Point(-1, 27)
        Me.LabelHeaderULDTotal.Name = "LabelHeaderULDTotal"
        Me.LabelHeaderULDTotal.Size = New System.Drawing.Size(39, 13)
        Me.LabelHeaderULDTotal.TabIndex = 31
        Me.LabelHeaderULDTotal.Tag = "TU.XX"
        Me.LabelHeaderULDTotal.Text = "TU.XX"
        '
        'LabelDLDTotal
        '
        Me.LabelDLDTotal.AutoSize = True
        Me.LabelDLDTotal.ForeColor = System.Drawing.Color.Green
        Me.LabelDLDTotal.Location = New System.Drawing.Point(-1, 14)
        Me.LabelDLDTotal.Name = "LabelDLDTotal"
        Me.LabelDLDTotal.Size = New System.Drawing.Size(46, 13)
        Me.LabelDLDTotal.TabIndex = 30
        Me.LabelDLDTotal.Text = "0000,00"
        '
        'LabelHeaderDLDTotal
        '
        Me.LabelHeaderDLDTotal.AutoSize = True
        Me.LabelHeaderDLDTotal.ForeColor = System.Drawing.Color.Green
        Me.LabelHeaderDLDTotal.Location = New System.Drawing.Point(-1, 1)
        Me.LabelHeaderDLDTotal.Name = "LabelHeaderDLDTotal"
        Me.LabelHeaderDLDTotal.Size = New System.Drawing.Size(39, 13)
        Me.LabelHeaderDLDTotal.TabIndex = 29
        Me.LabelHeaderDLDTotal.Tag = "TD.XX"
        Me.LabelHeaderDLDTotal.Text = "TD.XX"
        '
        'LabelULDAverage
        '
        Me.LabelULDAverage.AutoSize = True
        Me.LabelULDAverage.ForeColor = System.Drawing.Color.RoyalBlue
        Me.LabelULDAverage.Location = New System.Drawing.Point(155, 40)
        Me.LabelULDAverage.Name = "LabelULDAverage"
        Me.LabelULDAverage.Size = New System.Drawing.Size(46, 13)
        Me.LabelULDAverage.TabIndex = 28
        Me.LabelULDAverage.Text = "0000,00"
        '
        'LabelULDCurrent
        '
        Me.LabelULDCurrent.AutoSize = True
        Me.LabelULDCurrent.ForeColor = System.Drawing.Color.RoyalBlue
        Me.LabelULDCurrent.Location = New System.Drawing.Point(103, 40)
        Me.LabelULDCurrent.Name = "LabelULDCurrent"
        Me.LabelULDCurrent.Size = New System.Drawing.Size(46, 13)
        Me.LabelULDCurrent.TabIndex = 27
        Me.LabelULDCurrent.Text = "0000,00"
        '
        'LabelULDSession
        '
        Me.LabelULDSession.AutoSize = True
        Me.LabelULDSession.ForeColor = System.Drawing.Color.RoyalBlue
        Me.LabelULDSession.Location = New System.Drawing.Point(51, 40)
        Me.LabelULDSession.Name = "LabelULDSession"
        Me.LabelULDSession.Size = New System.Drawing.Size(46, 13)
        Me.LabelULDSession.TabIndex = 26
        Me.LabelULDSession.Text = "0000,00"
        '
        'LabelHeaderULDAverage
        '
        Me.LabelHeaderULDAverage.AutoSize = True
        Me.LabelHeaderULDAverage.ForeColor = System.Drawing.Color.RoyalBlue
        Me.LabelHeaderULDAverage.Location = New System.Drawing.Point(155, 27)
        Me.LabelHeaderULDAverage.Name = "LabelHeaderULDAverage"
        Me.LabelHeaderULDAverage.Size = New System.Drawing.Size(41, 13)
        Me.LabelHeaderULDAverage.TabIndex = 25
        Me.LabelHeaderULDAverage.Tag = "A.XX/s"
        Me.LabelHeaderULDAverage.Text = "A.XX/s"
        '
        'LabelHeaderULDCurrent
        '
        Me.LabelHeaderULDCurrent.AutoSize = True
        Me.LabelHeaderULDCurrent.ForeColor = System.Drawing.Color.RoyalBlue
        Me.LabelHeaderULDCurrent.Location = New System.Drawing.Point(103, 27)
        Me.LabelHeaderULDCurrent.Name = "LabelHeaderULDCurrent"
        Me.LabelHeaderULDCurrent.Size = New System.Drawing.Size(41, 13)
        Me.LabelHeaderULDCurrent.TabIndex = 24
        Me.LabelHeaderULDCurrent.Tag = "C.XX/s"
        Me.LabelHeaderULDCurrent.Text = "C.XX/s"
        '
        'LabelHeaderULDSession
        '
        Me.LabelHeaderULDSession.AutoSize = True
        Me.LabelHeaderULDSession.ForeColor = System.Drawing.Color.RoyalBlue
        Me.LabelHeaderULDSession.Location = New System.Drawing.Point(51, 27)
        Me.LabelHeaderULDSession.Name = "LabelHeaderULDSession"
        Me.LabelHeaderULDSession.Size = New System.Drawing.Size(32, 13)
        Me.LabelHeaderULDSession.TabIndex = 23
        Me.LabelHeaderULDSession.Tag = "U.XX"
        Me.LabelHeaderULDSession.Text = "U.XX"
        '
        'LabelDLDAverage
        '
        Me.LabelDLDAverage.AutoSize = True
        Me.LabelDLDAverage.ForeColor = System.Drawing.Color.Green
        Me.LabelDLDAverage.Location = New System.Drawing.Point(155, 14)
        Me.LabelDLDAverage.Name = "LabelDLDAverage"
        Me.LabelDLDAverage.Size = New System.Drawing.Size(46, 13)
        Me.LabelDLDAverage.TabIndex = 22
        Me.LabelDLDAverage.Text = "0000,00"
        '
        'LabelDLDCurrent
        '
        Me.LabelDLDCurrent.AutoSize = True
        Me.LabelDLDCurrent.ForeColor = System.Drawing.Color.Green
        Me.LabelDLDCurrent.Location = New System.Drawing.Point(103, 14)
        Me.LabelDLDCurrent.Name = "LabelDLDCurrent"
        Me.LabelDLDCurrent.Size = New System.Drawing.Size(46, 13)
        Me.LabelDLDCurrent.TabIndex = 21
        Me.LabelDLDCurrent.Text = "0000,00"
        '
        'LabelDLDSession
        '
        Me.LabelDLDSession.AutoSize = True
        Me.LabelDLDSession.ForeColor = System.Drawing.Color.Green
        Me.LabelDLDSession.Location = New System.Drawing.Point(51, 14)
        Me.LabelDLDSession.Name = "LabelDLDSession"
        Me.LabelDLDSession.Size = New System.Drawing.Size(46, 13)
        Me.LabelDLDSession.TabIndex = 20
        Me.LabelDLDSession.Text = "0000,00"
        '
        'LabelHeaderDLDAverage
        '
        Me.LabelHeaderDLDAverage.AutoSize = True
        Me.LabelHeaderDLDAverage.ForeColor = System.Drawing.Color.Green
        Me.LabelHeaderDLDAverage.Location = New System.Drawing.Point(155, 1)
        Me.LabelHeaderDLDAverage.Name = "LabelHeaderDLDAverage"
        Me.LabelHeaderDLDAverage.Size = New System.Drawing.Size(41, 13)
        Me.LabelHeaderDLDAverage.TabIndex = 19
        Me.LabelHeaderDLDAverage.Tag = "A.XX/s"
        Me.LabelHeaderDLDAverage.Text = "A.XX/s"
        '
        'LabelHeaderDLDCurrent
        '
        Me.LabelHeaderDLDCurrent.AutoSize = True
        Me.LabelHeaderDLDCurrent.ForeColor = System.Drawing.Color.Green
        Me.LabelHeaderDLDCurrent.Location = New System.Drawing.Point(103, 1)
        Me.LabelHeaderDLDCurrent.Name = "LabelHeaderDLDCurrent"
        Me.LabelHeaderDLDCurrent.Size = New System.Drawing.Size(41, 13)
        Me.LabelHeaderDLDCurrent.TabIndex = 18
        Me.LabelHeaderDLDCurrent.Tag = "C.XX/s"
        Me.LabelHeaderDLDCurrent.Text = "C.XX/s"
        '
        'LabelHeaderDLDSession
        '
        Me.LabelHeaderDLDSession.AutoSize = True
        Me.LabelHeaderDLDSession.ForeColor = System.Drawing.Color.Green
        Me.LabelHeaderDLDSession.Location = New System.Drawing.Point(51, 1)
        Me.LabelHeaderDLDSession.Name = "LabelHeaderDLDSession"
        Me.LabelHeaderDLDSession.Size = New System.Drawing.Size(32, 13)
        Me.LabelHeaderDLDSession.TabIndex = 17
        Me.LabelHeaderDLDSession.Tag = "D.XX"
        Me.LabelHeaderDLDSession.Text = "D.XX"
        '
        'DisplayerLabel
        '
        Me.DisplayerLabel.BackColor = System.Drawing.Color.Transparent
        Me.DisplayerLabel.Dock = System.Windows.Forms.DockStyle.Fill
        Me.DisplayerLabel.Font = New System.Drawing.Font("Microsoft Sans Serif", 30.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.DisplayerLabel.ForeColor = System.Drawing.Color.DodgerBlue
        Me.DisplayerLabel.ImageKey = "(none)"
        Me.DisplayerLabel.Location = New System.Drawing.Point(0, 0)
        Me.DisplayerLabel.Name = "DisplayerLabel"
        Me.DisplayerLabel.Size = New System.Drawing.Size(200, 254)
        Me.DisplayerLabel.TabIndex = 20
        Me.DisplayerLabel.Tag = ""
        Me.DisplayerLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.DisplayerLabel.Visible = False
        '
        'PanelGraphs
        '
        Me.PanelGraphs.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.PanelGraphs.BackColor = System.Drawing.Color.Transparent
        Me.PanelGraphs.Controls.Add(Me.GraphyDownload)
        Me.PanelGraphs.Controls.Add(Me.GraphyUpload)
        Me.PanelGraphs.Location = New System.Drawing.Point(0, 54)
        Me.PanelGraphs.Name = "PanelGraphs"
        Me.PanelGraphs.Size = New System.Drawing.Size(200, 200)
        Me.PanelGraphs.TabIndex = 21
        '
        'GraphyDownload
        '
        Me.GraphyDownload.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GraphyDownload.BackColor = System.Drawing.Color.Transparent
        Me.GraphyDownload.EnableAntiAliasing = True
        Me.GraphyDownload.IndexIndicatorColour = System.Drawing.Color.White
        Me.GraphyDownload.LineColour = System.Drawing.Color.Green
        Me.GraphyDownload.Location = New System.Drawing.Point(0, 0)
        Me.GraphyDownload.MinimumSize = New System.Drawing.Size(100, 50)
        Me.GraphyDownload.Name = "GraphyDownload"
        Me.GraphyDownload.OverlayColour = System.Drawing.Color.Red
        Me.GraphyDownload.OverlayText = "DB: "
        Me.GraphyDownload.PaddingHeight = 0.9!
        Me.GraphyDownload.ShowTextOverlay = True
        Me.GraphyDownload.Size = New System.Drawing.Size(200, 100)
        Me.GraphyDownload.TabIndex = 17
        Me.GraphyDownload.Type = Graphy.GraphType.Gradient
        Me.GraphyDownload.VerticalLineInterval = 10
        Me.GraphyDownload.VerticalLineIntervalColour = System.Drawing.Color.LightBlue
        '
        'GraphyUpload
        '
        Me.GraphyUpload.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GraphyUpload.BackColor = System.Drawing.Color.Transparent
        Me.GraphyUpload.EnableAntiAliasing = True
        Me.GraphyUpload.IndexIndicatorColour = System.Drawing.Color.White
        Me.GraphyUpload.LineColour = System.Drawing.Color.RoyalBlue
        Me.GraphyUpload.Location = New System.Drawing.Point(0, 100)
        Me.GraphyUpload.MinimumSize = New System.Drawing.Size(100, 50)
        Me.GraphyUpload.Name = "GraphyUpload"
        Me.GraphyUpload.OverlayColour = System.Drawing.Color.Red
        Me.GraphyUpload.OverlayText = "UB: "
        Me.GraphyUpload.PaddingHeight = 0.9!
        Me.GraphyUpload.ShowTextOverlay = True
        Me.GraphyUpload.Size = New System.Drawing.Size(200, 100)
        Me.GraphyUpload.TabIndex = 18
        Me.GraphyUpload.Type = Graphy.GraphType.Gradient
        Me.GraphyUpload.VerticalLineInterval = 10
        Me.GraphyUpload.VerticalLineIntervalColour = System.Drawing.Color.LightBlue
        '
        'RightClickMenu
        '
        Me.RightClickMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TopMostToolStripMenuItem, Me.ToolStripSeparator1, Me.NetworkAdaptersMenuItem, Me.UnfocusedOpacityToolStripMenuItem, Me.ExitToolStripMenuItem})
        Me.RightClickMenu.Name = "ContextMenuStrip1"
        Me.RightClickMenu.ShowCheckMargin = True
        Me.RightClickMenu.ShowImageMargin = False
        Me.RightClickMenu.Size = New System.Drawing.Size(176, 98)
        '
        'TopMostToolStripMenuItem
        '
        Me.TopMostToolStripMenuItem.Checked = True
        Me.TopMostToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked
        Me.TopMostToolStripMenuItem.Name = "TopMostToolStripMenuItem"
        Me.TopMostToolStripMenuItem.Size = New System.Drawing.Size(175, 22)
        Me.TopMostToolStripMenuItem.Text = "Top Most"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(172, 6)
        '
        'NetworkAdaptersMenuItem
        '
        Me.NetworkAdaptersMenuItem.Name = "NetworkAdaptersMenuItem"
        Me.NetworkAdaptersMenuItem.Size = New System.Drawing.Size(175, 22)
        Me.NetworkAdaptersMenuItem.Text = "Network Adapters"
        '
        'UnfocusedOpacityToolStripMenuItem
        '
        Me.UnfocusedOpacityToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.TwentyFivePercent, Me.FiftyPercent, Me.SeventyFivePercent, Me.OnehundredPercent})
        Me.UnfocusedOpacityToolStripMenuItem.Name = "UnfocusedOpacityToolStripMenuItem"
        Me.UnfocusedOpacityToolStripMenuItem.Size = New System.Drawing.Size(175, 22)
        Me.UnfocusedOpacityToolStripMenuItem.Text = "Unfocused Opacity"
        '
        'TwentyFivePercent
        '
        Me.TwentyFivePercent.Name = "TwentyFivePercent"
        Me.TwentyFivePercent.Size = New System.Drawing.Size(180, 22)
        Me.TwentyFivePercent.Tag = "25"
        Me.TwentyFivePercent.Text = "25%"
        '
        'FiftyPercent
        '
        Me.FiftyPercent.Name = "FiftyPercent"
        Me.FiftyPercent.Size = New System.Drawing.Size(180, 22)
        Me.FiftyPercent.Tag = "50"
        Me.FiftyPercent.Text = "50%"
        '
        'SeventyFivePercent
        '
        Me.SeventyFivePercent.Name = "SeventyFivePercent"
        Me.SeventyFivePercent.Size = New System.Drawing.Size(180, 22)
        Me.SeventyFivePercent.Tag = "75"
        Me.SeventyFivePercent.Text = "75%"
        '
        'OnehundredPercent
        '
        Me.OnehundredPercent.Name = "OnehundredPercent"
        Me.OnehundredPercent.Size = New System.Drawing.Size(180, 22)
        Me.OnehundredPercent.Tag = "100"
        Me.OnehundredPercent.Text = "100%"
        '
        'ExitToolStripMenuItem
        '
        Me.ExitToolStripMenuItem.Name = "ExitToolStripMenuItem"
        Me.ExitToolStripMenuItem.Size = New System.Drawing.Size(175, 22)
        Me.ExitToolStripMenuItem.Text = "Exit"
        '
        'MainForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.Black
        Me.ClientSize = New System.Drawing.Size(200, 254)
        Me.Controls.Add(Me.DisplayerLabel)
        Me.Controls.Add(Me.PanelGraphs)
        Me.Controls.Add(Me.PanelStats)
        Me.DoubleBuffered = True
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.KeyPreview = True
        Me.MaximizeBox = False
        Me.MinimumSize = New System.Drawing.Size(200, 154)
        Me.Name = "MainForm"
        Me.Opacity = 0.5R
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "Network Monitor"
        Me.TopMost = True
        Me.PanelStats.ResumeLayout(False)
        Me.PanelStats.PerformLayout()
        Me.PanelGraphs.ResumeLayout(False)
        Me.RightClickMenu.ResumeLayout(False)
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents GraphyDownload As Graphy.Graphy
    Friend WithEvents GraphyUpload As Graphy.Graphy
    Friend WithEvents PanelStats As Panel
    Friend WithEvents LabelULDTotal As Label
    Friend WithEvents LabelHeaderULDTotal As Label
    Friend WithEvents LabelDLDTotal As Label
    Friend WithEvents LabelHeaderDLDTotal As Label
    Friend WithEvents LabelULDAverage As Label
    Friend WithEvents LabelULDCurrent As Label
    Friend WithEvents LabelULDSession As Label
    Friend WithEvents LabelHeaderULDAverage As Label
    Friend WithEvents LabelHeaderULDCurrent As Label
    Friend WithEvents LabelHeaderULDSession As Label
    Friend WithEvents LabelDLDAverage As Label
    Friend WithEvents LabelDLDCurrent As Label
    Friend WithEvents LabelDLDSession As Label
    Friend WithEvents LabelHeaderDLDAverage As Label
    Friend WithEvents LabelHeaderDLDCurrent As Label
    Friend WithEvents LabelHeaderDLDSession As Label
    Friend WithEvents DisplayerLabel As Label
    Friend WithEvents PanelGraphs As Panel
    Friend WithEvents RightClickMenu As ContextMenuStrip
    Friend WithEvents TopMostToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents NetworkAdaptersMenuItem As ToolStripMenuItem
    Friend WithEvents UnfocusedOpacityToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ExitToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TwentyFivePercent As ToolStripMenuItem
    Friend WithEvents FiftyPercent As ToolStripMenuItem
    Friend WithEvents SeventyFivePercent As ToolStripMenuItem
    Friend WithEvents OnehundredPercent As ToolStripMenuItem
End Class
