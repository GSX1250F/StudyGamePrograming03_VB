<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Game
    Inherits System.Windows.Forms.Form

    'フォームがコンポーネントの一覧をクリーンアップするために dispose をオーバーライドします。
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Windows フォーム デザイナーで必要です。
    Private components As System.ComponentModel.IContainer

    'メモ: 以下のプロシージャは Windows フォーム デザイナーで必要です。
    'Windows フォーム デザイナーを使用して変更できます。  
    'コード エディターを使って変更しないでください。
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        components = New ComponentModel.Container()
        PictureBox = New PictureBox()
        Timer1 = New Timer(components)
        CType(PictureBox, ComponentModel.ISupportInitialize).BeginInit()
        SuspendLayout()
        ' 
        ' PictureBox
        ' 
        PictureBox.BackColor = SystemColors.ActiveCaptionText
        PictureBox.ImeMode = ImeMode.NoControl
        PictureBox.Location = New Point(0, 0)
        PictureBox.Margin = New Padding(6, 6, 6, 6)
        PictureBox.Name = "PictureBox"
        PictureBox.Size = New Size(1902, 1638)
        PictureBox.TabIndex = 1
        PictureBox.TabStop = False
        ' 
        ' Timer1
        ' 
        ' 
        ' Game
        ' 
        AutoScaleDimensions = New SizeF(13F, 32F)
        AutoScaleMode = AutoScaleMode.Font
        ClientSize = New Size(1904, 1643)
        Controls.Add(PictureBox)
        DoubleBuffered = True
        Margin = New Padding(6, 6, 6, 6)
        Name = "Game"
        Text = "Game"
        CType(PictureBox, ComponentModel.ISupportInitialize).EndInit()
        ResumeLayout(False)
    End Sub

    Friend WithEvents PictureBox As PictureBox
    Friend WithEvents Timer1 As Timer
End Class
