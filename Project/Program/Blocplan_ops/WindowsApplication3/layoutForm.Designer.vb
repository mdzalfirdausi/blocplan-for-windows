﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class layoutForm
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
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

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.layoutPanel = New System.Windows.Forms.Panel()
        Me.SuspendLayout()
        '
        'layoutPanel
        '
        Me.layoutPanel.Location = New System.Drawing.Point(25, 13)
        Me.layoutPanel.Name = "layoutPanel"
        Me.layoutPanel.Size = New System.Drawing.Size(247, 237)
        Me.layoutPanel.TabIndex = 0
        '
        'layoutForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(284, 262)
        Me.Controls.Add(Me.layoutPanel)
        Me.Name = "layoutForm"
        Me.Text = "layoutForm"
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents layoutPanel As System.Windows.Forms.Panel
End Class
