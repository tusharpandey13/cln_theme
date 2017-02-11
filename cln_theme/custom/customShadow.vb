Imports System.Runtime.InteropServices

Public Class ShadowForm
    Inherits Form

    Private Shadows ParentForm As Form
    Public ShadowSize As Integer

#Region "Ctor"
    Public Sub New(ByRef Parent As Form, ByVal ShadowSize As Integer)

        SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ContainerControl Or ControlStyles.SupportsTransparentBackColor Or ControlStyles.UserMouse Or ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        SetStyle(ControlStyles.Selectable Or ControlStyles.StandardClick Or ControlStyles.StandardDoubleClick Or ControlStyles.Opaque, False)
        DoubleBuffered = True

        Me.ParentForm = Parent
        Me.ShadowSize = ShadowSize

        FormBorderStyle = FormBorderStyle.None
        ShowInTaskbar = False
        ControlBox = False
        Text = ""


        AddHandler Parent.Resize, Sub() UpdateBounds()
        AddHandler Parent.Move, Sub() UpdateBounds()
        AddHandler Parent.Layout, Sub() UpdateBounds()

        AddOwnedForm(ParentForm)


    End Sub
#End Region
    Protected Overrides Sub OnPaintBackground(e As System.Windows.Forms.PaintEventArgs)
    End Sub
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        e.Graphics.InterpolationMode = 7
        e.Graphics.SmoothingMode = 2
    End Sub


    Public Shadows Sub UpdateBounds()
        ' Location = ParentForm.Location - New Point(ShadowSize, ShadowSize - 5)
        'MyBase.Size = ParentForm.ClientSize + New Size(ShadowSize * 2 + 1, ShadowSize * 2 + 0)
        SetWindowPos(Handle, ParentForm.Handle, ParentForm.Left - 20, ParentForm.Top - 20, ParentForm.Width + 40, ParentForm.Height + 40, SetWindowPosFlags.FrameChanged)
    End Sub

    Protected Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            Dim cParms As CreateParams = MyBase.CreateParams
            cParms.ExStyle = cParms.ExStyle Or WindowStyles.WS_EX_LAYERED Or
                                               WindowStyles.WS_EX_TOOLWINDOW Or
                                               WindowStyles.WS_DISABLED
            Return cParms

        End Get
    End Property

    Public Sub SetBits(B As Bitmap)
        If Not IsHandleCreated Or DesignMode Then Exit Sub

        If Not Bitmap.IsCanonicalPixelFormat(B.PixelFormat) OrElse Not Bitmap.IsAlphaPixelFormat(B.PixelFormat) Then
            Throw New ApplicationException("The picture must be 32bit picture with alpha channel.")
        End If

        Dim oldBits As IntPtr = IntPtr.Zero
        Dim screenDC As IntPtr = win32.GetDC(IntPtr.Zero)
        Dim hBitmap As IntPtr = IntPtr.Zero
        Dim memDc As IntPtr = win32.CreateCompatibleDC(screenDC)

        Try
            Dim topLoc As New win32.Point32(Left, Top)
            Dim bitMapSize As New win32.Size32(B.Width, B.Height)
            Dim blendFunc As New win32.BLENDFUNCTION()
            Dim srcLoc As New win32.Point32(0, 0)

            hBitmap = B.GetHbitmap(Color.FromArgb(0))
            oldBits = win32.SelectObject(memDc, hBitmap)

            blendFunc.BlendOp = win32.AC_SRC_OVER
            blendFunc.SourceConstantAlpha = 250
            blendFunc.AlphaFormat = win32.AC_SRC_ALPHA
            blendFunc.BlendFlags = 0

            win32.UpdateLayeredWindow(Handle, screenDC, topLoc, bitMapSize, memDc, srcLoc,
             0, blendFunc, win32.ULW_ALPHA)
        Finally
            If hBitmap <> IntPtr.Zero Then
                win32.SelectObject(memDc, oldBits)
                win32.DeleteObject(hBitmap)
            End If
            win32.ReleaseDC(IntPtr.Zero, screenDC)
            win32.DeleteDC(memDc)
        End Try
    End Sub
End Class

