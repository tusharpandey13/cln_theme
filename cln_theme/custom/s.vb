Imports System.Runtime.InteropServices

Public Class s : Inherits Form
    Public shadowsize%
    Private pfh As IntPtr
    Sub New(ByRef fh As IntPtr, s%)
        SetStyle(ControlStyles.AllPaintingInWmPaint Or ControlStyles.OptimizedDoubleBuffer Or ControlStyles.UserPaint Or ControlStyles.ResizeRedraw, 1)
        DoubleBuffered = 1
        pfh = fh
        shadowsize = s
        FormBorderStyle = 0
        ShowInTaskbar = 0
        ControlBox = 0
        Text = ""

        AddOwnedForm(Form.FromHandle(fh))

    End Sub

End Class
