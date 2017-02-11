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

        'AddOwnedForm(ParentForm)

        ' AddHandler Parent.Move, AddressOf UpdateBounds
        ' AddHandler Parent.Layout, AddressOf UpdateBounds


    End Sub

#End Region


#Region "DWM"
    Private dwmMargins As MARGINS
    Private _marginOk As Boolean
    Protected Overloads Overrides Sub WndProc(ByRef m As Message)

        Dim result As IntPtr
        Dim dwmHandled As Integer = DwmDefWindowProc(m.HWnd, m.Msg, m.WParam, m.LParam, result)

        If dwmHandled = 1 Then
            m.Result = result
            Exit Sub
        End If

        If m.Msg = WindowsMessages.WmNcCalcSize AndAlso CInt(m.WParam) = 1 Then
            Dim nccsp As NCCALCSIZE_PARAMS = Marshal.PtrToStructure(m.LParam, GetType(NCCALCSIZE_PARAMS))

            ' Adjust (shrink) the client rectangle to accommodate the border:
            nccsp.rect0.top += 0
            nccsp.rect0.bottom += 0
            nccsp.rect0.left += 0
            nccsp.rect0.right += 0

            If Not _marginOk Then
                'Set what client area would be for passing to 
                'DwmExtendIntoClientArea. Also remember that at least 
                'one of these values NEEDS TO BE > 1, else it won't work.
                dwmMargins.cyTopHeight = 6
                dwmMargins.cxLeftWidth = 6
                dwmMargins.cyBottomHeight = 6
                dwmMargins.cxRightWidth = 6
                _marginOk = True
            End If

            Marshal.StructureToPtr(nccsp, m.LParam, False)

            m.Result = IntPtr.Zero

        Else : MyBase.WndProc(m)
        End If
    End Sub
    Private Function HitTestNCA(ByVal hwnd As IntPtr, ByVal wparam _
                                      As IntPtr, ByVal lparam As IntPtr) As IntPtr

        Dim p As New Point(LoWord(CInt(lparam)), HiWord(CInt(lparam)))

        Dim topleft As Rectangle = RectangleToScreen(New Rectangle(0, 0, dwmMargins.cxLeftWidth, dwmMargins.cxLeftWidth))
        Dim topright As Rectangle = RectangleToScreen(New Rectangle(Width - dwmMargins.cxRightWidth, 0, dwmMargins.cxRightWidth, dwmMargins.cxRightWidth))
        Dim botleft As Rectangle = RectangleToScreen(New Rectangle(0, Height - dwmMargins.cyBottomHeight, dwmMargins.cxLeftWidth, dwmMargins.cyBottomHeight))
        Dim botright As Rectangle = RectangleToScreen(New Rectangle(Width - dwmMargins.cxRightWidth, Height - dwmMargins.cyBottomHeight, dwmMargins.cxRightWidth, dwmMargins.cyBottomHeight))
        Dim top As Rectangle = RectangleToScreen(New Rectangle(0, 0, Width, dwmMargins.cxLeftWidth))
        Dim cap As Rectangle = RectangleToScreen(New Rectangle(0, dwmMargins.cxLeftWidth, Width, dwmMargins.cyTopHeight - dwmMargins.cxLeftWidth))
        Dim left As Rectangle = RectangleToScreen(New Rectangle(0, 0, dwmMargins.cxLeftWidth, Height))
        Dim right As Rectangle = RectangleToScreen(New Rectangle(Width - dwmMargins.cxRightWidth, 0, dwmMargins.cxRightWidth, Height))
        Dim bottom As Rectangle = RectangleToScreen(New Rectangle(0, Height - dwmMargins.cyBottomHeight, Width, dwmMargins.cyBottomHeight))


        If topleft.Contains(p) Then Return New IntPtr(HTTOPLEFT)
        If topright.Contains(p) Then Return New IntPtr(HTTOPRIGHT)
        If botleft.Contains(p) Then Return New IntPtr(HTBOTTOMLEFT)
        If botright.Contains(p) Then Return New IntPtr(HTBOTTOMRIGHT)
        If top.Contains(p) Then Return New IntPtr(HTTOP)
        If cap.Contains(p) Then Return New IntPtr(HTCAPTION)
        If left.Contains(p) Then Return New IntPtr(HTLEFT)
        If right.Contains(p) Then Return New IntPtr(HTRIGHT)
        If bottom.Contains(p) Then Return New IntPtr(HTBOTTOM)

        Return New IntPtr(HTCLIENT)
    End Function
    Private Const BorderWidth As Integer = 6

#End Region


    Protected Overrides Sub OnPaintBackground(e As System.Windows.Forms.PaintEventArgs)
    End Sub
    Protected Overrides Sub OnPaint(e As PaintEventArgs)
        MyBase.OnPaint(e)
        e.Graphics.InterpolationMode = 7
        e.Graphics.SmoothingMode = 2
    End Sub


    Public Shadows Sub UpdateBounds()
        SetWindowPos(Handle, ParentForm.Handle, ParentForm.Left - 20, ParentForm.Top - 20, Width, Height, SetWindowPosFlags.FrameChanged)
    End Sub

    Protected Overrides ReadOnly Property CreateParams() As CreateParams
        Get
            Dim cParms As CreateParams = MyBase.CreateParams
            'cParms.ExStyle = cParms.ExStyle Or WindowStyles.WS_EX_LAYERED ' Or
            'WindowStyles.WS_EX_TOOLWINDOW 'Or
            'WindowStyles.WS_DISABLED Or
            'WindowStyles.WS_EX_TRANSPARENT
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

