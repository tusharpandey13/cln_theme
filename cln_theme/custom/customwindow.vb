Imports System.Runtime.InteropServices
Imports System.Drawing.Drawing2D


Public Class CustomWindow : Inherits Form : Implements AnimatedObject


#Region "DECLARE"
    Private Const BorderWidth As Integer = 6
    Dim tp As Pen = Pens.Black : Dim tb As SolidBrush = Brushes.Black : Dim tgb As LinearGradientBrush
    Dim lb(4), db(4) As Bitmap
    Dim ot% = 0
    Dim fxt% = 0
    Dim _cbx% : Private Property cbx%
        Get
            Return _cbx
        End Get
        Set(value%)
            If Not _cbx = value Then
                _cbx = value
                Invalidate()
                DrawShadow()
            End If
        End Set
    End Property
    Dim ms% = 0
    Public Enum style
        Dark
        Light
    End Enum
    Public Property animating As Boolean = 0 Implements AnimatedObject.animating
    Public ReadOnly Property designing As Boolean Implements AnimatedObject.designing
        Get
            Return DesignMode
        End Get
    End Property
    Dim _cp As Boolean = True : Public Property custompaint As Boolean
        Get
            Return _cp
        End Get
        Set(value As Boolean)
            _cp = value
            Invalidate()
        End Set
    End Property
    Public Property fordesign As Boolean
    Private _style As style : Public Property windowstyle As style
        Get
            Return _style
        End Get
        Set(value As style)
            _style = value
            Invalidate()
        End Set
    End Property
#End Region

#Region "DWM"
    Private dwmMargins As MARGINS
    Private _marginOk As Boolean
#Region "Ctor"
    Public Sub New()
        Opacity = 0
        SetStyle(ControlStyles.OptimizedDoubleBuffer Or ControlStyles.ResizeRedraw Or ControlStyles.AllPaintingInWmPaint Or ControlStyles.UserPaint, True)
        DoubleBuffered = True
        FormBorderStyle = FormBorderStyle.None
    End Sub

#End Region
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


        ElseIf m.Msg = WindowsMessages.WmNchitTest AndAlso CInt(m.Result) = 0 AndAlso custompaint Then
            m.Result = HitTestNCA(m.HWnd, m.WParam, m.LParam)

        ElseIf m.Msg = WindowsMessages.WmSetFocus Then
            gotfocus_()
            MyBase.WndProc(m)

        ElseIf m.Msg = WindowsMessages.WmKillFocus Then
            lostfocus_()
            MyBase.WndProc(m)

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

    Protected Overrides Sub OnMouseDown(ByVal e As MouseEventArgs)
        If DesignMode And Not fordesign Then Exit Sub
        ms = 2
        If Me.Width - BorderWidth > e.Location.X AndAlso
                    e.Location.X > BorderWidth AndAlso e.Location.Y > BorderWidth Then
            MoveControl(Me.Handle)
        End If
        MyBase.OnMouseDown(e)
        mdown()
    End Sub
    Private Sub MoveControl(ByVal hWnd As IntPtr)
        If DesignMode And Not fordesign Then Exit Sub
        ReleaseCapture()
        SendMessage(hWnd, WindowsMessages.WmNcLButtonDown, HTCAPTION, 0)
    End Sub
    Protected Overrides Sub SetBoundsCore(x As Integer, y As Integer, width As Integer, height As Integer, specified As BoundsSpecified)
        If DesignMode And Not fordesign Then MyBase.SetBoundsCore(x, y, width, height, specified)
    End Sub

#End Region

#Region "VISUALS"
    Dim hr() As Rectangle
    Protected Overrides Sub OnPaintBackground(e As PaintEventArgs)
        Dim g = e.Graphics

        g.Clear(Color.Black)

        If windowstyle = style.Dark Then drawdark(g)
        If windowstyle = style.Light Then drawlight(g)

        tp.Dispose() : tb.Dispose()

        paint_(e)
    End Sub

    Sub drawdark(ByRef g As Graphics)
        g.Clear(col(30, 30, 32))
        g.FillRectangle(Brushes.Fuchsia, rct(0, 23, Width, 2))
        g.DrawRectangle(Pens.Black, rct(0, 0, Width - 1, 22))
        g.DrawRectangle(Pens.Black, rct(0, 23 + 2, Width - 1, Height - 29 + 3))

        g.SmoothingMode = 2
        mp(col(25, 255))
        g.DrawLine(tp, 1, 23 + 3, Width - 2, 23 + 3)
        g.DrawLine(tp, 1, 1, Width - 2, 1)

        mp(col(60, 0))
        g.DrawLine(tp, 0, 21, Width, 21)
        g.DrawLine(tp, 0, Height - 2, Width, Height - 2)

        g.TextRenderingHint = 5
        g.DrawString(Text, Font, mb(col(0)), pt(25, 6))
        g.DrawString(Text, Font, mb(col(90, 255)), pt(25, 5))
        g.DrawIcon(Icon, rct(4, 4, 15, 15))

        g.PixelOffsetMode = 2
        Dim f = New Font("Marlett", 9, FontStyle.Regular)
        g.DrawString("r", f, mb(Color.Fuchsia), pt(Width - 20, 6))
        f = New Font("Marlett", 10, FontStyle.Regular)
        If WindowState = FormWindowState.Maximized Then
            g.DrawString("2", f, mb(Color.Fuchsia), pt(Width - 40, 6))
        Else
            g.DrawString("1", f, mb(Color.Fuchsia), pt(Width - 40, 6))
        End If
        g.DrawString("0", f, mb(Color.Fuchsia), pt(Width - 58, 6))

    End Sub
    Sub drawlight(ByRef g As Graphics)

    End Sub
#Region "Functions"
    Sub mp(c As Byte)
        tp = New Pen(col(c))
    End Sub
    Sub mp(c As Color)
        tp = New Pen(c)
    End Sub
    Sub mp(a As Byte, c As Color)
        tp = New Pen(col(a, c))
    End Sub
    Sub px(c As Color, x!, y!, g As Graphics)
        tb = New SolidBrush(c)
        g.FillRectangle(tb, x, y, 1, 1)
        tb.Dispose()
    End Sub
    Function gri(b As Bitmap) As Graphics
        Return Graphics.FromImage(b)
    End Function
#End Region

#Region "SHADOW"
#Region "declare"
    Private Shadow As New ShadowForm(Me, 15 + 5)
    Private Rounding!
#End Region
    Public op# = 1.0#
    Private Sub DrawShadow() Handles Me.SizeChanged
        If DesignMode Or IsHandleCreated = False Then Exit Sub
        Dim B As New Bitmap(CInt(Shadow.Size.Width), CInt(Shadow.Size.Height))
        Dim G As Graphics = Graphics.FromImage(B)
        Shadow.BackColor = Color.Black
        With G
            Dim s = Shadow.ShadowSize - 5
            Dim ip As New Interpolation


            .SetClip(rct(s - 1, s - 1, Width + 12, Height + 12))
            .Clear(col(175, 0))
            mb(col(100, 255), tb)
            .FillRectangle(tb, rct(Width + s + 5 - 70, s + 7, 70 - 2, 23 - 4))
            Dim hc() As Color = {col(160, 0, 122, 204), col(160, 255, 255, 128), col(160, 255, 25, 25), col(0, 0)}
            mb(hc(cbx), tb)
            .FillRectangle(tb, hr(cbx))
            .ResetClip()

            .SmoothingMode = 2 ': .PixelOffsetMode = 2
            .SetClip(rct(s, s, Width - 2 * s, Height - 2 * s), CombineMode.Exclude)
            For i = 0 To 13
                Dim pth As GraphicsPath = DM.CreateRoundRectangle(rct(1 + i, 1 + i, Width + 37 - 2 * i, Height + 37 - 2 * i), 13 - i, 1, 1, 1, 1)
                mp(col(ip.GetValue(0, 200, i + 1, 14, Type.EaseIn, EasingMethods.Exponent, 1), 0))
                G.DrawPath(tp, pth)
                pth.Dispose()
            Next

        End With

        G.Dispose()
        Shadow.SetBits(B)
        B.Dispose()
        tp.Dispose()
    End Sub
    'below by blackcap
    Friend Class DM

        Public Shared Function CreateRoundRectangle(ByVal rectangle As Rectangle, ByVal radius As Integer, Optional ByVal TopLeft As Boolean = True, Optional ByVal TopRigth As Boolean = True, Optional ByVal BottomRigth As Boolean = True, Optional ByVal BottomLeft As Boolean = True) As GraphicsPath
            Dim path As New GraphicsPath()
            Dim l As Integer = rectangle.Left
            Dim t As Integer = rectangle.Top
            Dim w As Integer = rectangle.Width
            Dim h As Integer = rectangle.Height
            Dim d As Integer = radius << 1

            If radius <= 0 Then
                path.AddRectangle(rectangle)
                Return path
            End If

            If TopLeft Then
                path.AddArc(l, t, d, d, 180, 90)
                If TopRigth Then path.AddLine(l + radius, t, l + w - radius, t) Else path.AddLine(l + radius, t, l + w, t)
            Else
                If TopRigth Then path.AddLine(l, t, l + w - radius, t) Else path.AddLine(l, t, l + w, t)
            End If

            If TopRigth Then
                path.AddArc(l + w - d, t, d, d, 270, 90)
                If BottomRigth Then path.AddLine(l + w, t + radius, l + w, t + h - radius) Else path.AddLine(l + w, t + radius, l + w, t + h)
            Else
                If BottomRigth Then path.AddLine(l + w, t, l + w, t + h - radius) Else path.AddLine(l + w, t, l + w, t + h)
            End If

            If BottomRigth Then
                path.AddArc(l + w - d, t + h - d, d, d, 0, 90)
                If BottomLeft Then path.AddLine(l + w - radius, t + h, l + radius, t + h) Else path.AddLine(l + w - radius, t + h, l, t + h)
            Else
                If BottomLeft Then path.AddLine(l + w, t + h, l + radius, t + h) Else path.AddLine(l + w, t + h, l, t + h)
            End If

            If BottomLeft Then
                path.AddArc(l, t + h - d, d, d, 90, 90)
                If TopLeft Then path.AddLine(l, t + h - radius, l, t + radius) Else path.AddLine(l, t + h - radius, l, t)
            Else
                If TopLeft Then path.AddLine(l, t + h, l, t + radius) Else path.AddLine(l, t + h, l, t)
            End If

            path.CloseFigure()
            Return path
        End Function
        Public Shared Function CreateRoundRectangle(x As Integer, y As Integer, w As Integer, h As Integer, radius As Integer, Optional ByVal TopLeft As Boolean = True, Optional ByVal TopRigth As Boolean = True, Optional ByVal BottomRigth As Boolean = True, Optional ByVal BottomLeft As Boolean = True) As GraphicsPath
            Return CreateRoundRectangle(New Rectangle(x, y, w, h), radius, TopLeft, TopRigth, BottomRigth, BottomLeft)
        End Function

    End Class
    'above by blackcap
#End Region


#End Region


#Region "Events"
    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        ms = 1
        MyBase.OnMouseMove(e)
        'For Each c As customControl In Controls
        '    If TypeOf c Is customControl Then DirectCast(c, customControl).leavemouse(e)
        'Next
        Dim r = Width - 65
        If e.Y > -1 And e.Y < 24 Then
            If e.X > r And e.X < r + 23 Then
                cbx = 0
            ElseIf e.X > r + 22 And e.X < r + 43 Then
                cbx = 1
            ElseIf e.X > r + 42 And e.X < Width + 1 Then
                cbx = 2
            Else
                cbx = 3
            End If
        Else
            cbx = 3
        End If
        mmove()
    End Sub
    Protected Overrides Sub OnResize(e As EventArgs)
        hr = {rct(Width - 64 + 20, 20, 22, 23),
                            rct(Width - 42 + 20, 20, 20, 23),
                            rct(Width - 22 + 20, 20, 16, 23),
                            rct(-9999, -9999, 1, 1)}
        Invalidate()
        MyBase.OnResize(e)
        resize_()
    End Sub
    Public Sub leavemouse(e As EventArgs) Implements AnimatedObject.leavemouse
        Me.OnMouseLeave(e)
    End Sub
    Private Sub custom_invalidate() Implements AnimatedObject.invalidate
        Invalidate()
    End Sub
    Protected Overrides Sub OnCreateControl()

        'If Not animatedforms.Contains(Me) Then animatedforms.Add(Me)

        DrawShadow()


        MyBase.OnCreateControl()

        create()

    End Sub
#End Region

#Region "FORM LOADING and CLOSING"
    Private Sub me_Load(sender As Object, e As EventArgs) Handles Me.Load
        Opacity = 1
        Shadow.BackColor = Color.Black
        Shadow.Visible = 1
        If Not DesignMode Then
            FormBorderStyle = FormBorderStyle.Sizable
        Else
            FormBorderStyle = 0
        End If

        MinimumSize = New Size(120, 34)

        If custompaint Then TransparencyKey = Color.Fuchsia
        StartPosition = FormStartPosition.CenterScreen
        load_()
    End Sub
    Private Sub CustomWindow_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        e.Cancel = True
        Shadow.Hide()
        Shadow.Visible = False
        Shadow.Dispose()
        For Each c As Control In Controls
            c.Dispose()
        Next
        'fxt = 1


        '_________________̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲
        'End the process  ͟___________________________________________________̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲̲ 
        '‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾																													
        Dim PR() As Process = Process.GetProcessesByName("magnify") '            
        For Each Process As Process In PR '                                                    
            On Error GoTo e  '    '                                                                
            Process.Kill() '                                                                                 
        Next '																 ̲ ̲  FUCK YOU MAGNIFYIER ̲͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟͟ ̲̲̲̲ 
        '																	  ‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾‾	 
        '̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅‾̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅‾̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅‾̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅̅ ̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅‾̅̅̅ ̅̅̅̅̅̅̅̅̅̅̅̅̅̅̅̅̅̅̅


e:
        close_()
    End Sub
    Protected Overrides Sub Dispose(disposing As Boolean)
        MyBase.Dispose(disposing)
        'animatedforms.Remove(Me)
        'If animatedforms.Count = 0 Then stopTimer()
    End Sub
#End Region

#Region "Overridables"
    Protected Overridable Sub load_()
    End Sub
    Protected Overridable Sub close_()
    End Sub
    Protected Overridable Sub paint_(e As PaintEventArgs)
    End Sub
    Protected Overridable Sub resize_()
    End Sub
    Protected Overridable Sub mdown()
    End Sub
    Protected Overridable Sub mmove()
    End Sub
    Protected Overridable Sub lostfocus_()
    End Sub
    Protected Overridable Sub gotfocus_()
    End Sub
    Protected Overridable Sub create()
    End Sub
#End Region



End Class ' DISPOSE done
