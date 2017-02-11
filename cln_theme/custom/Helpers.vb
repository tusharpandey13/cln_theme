#Region "Imports"
Imports System.Drawing.Drawing2D
Imports System.Drawing.Imaging
Imports System.Runtime.InteropServices
#End Region
Module Helpers
#Region " Functions"


#Region "Color"
    Friend Function col(a!, r!, g!, b!, sat!) As Color
        Return Color.FromArgb(CByte(CInt(a)), CByte(Math.Min(255, Math.Max(0, CInt(r) * sat))), CByte(Math.Min(255, Math.Max(0, CInt(g) * sat))), CByte(Math.Min(255, Math.Max(0, CInt(b) * sat))))
    End Function
    Friend Function col(a!, r!, g!, b!) As Color
        Return col(a, r, g, b, 1)
    End Function
    Friend Function col(r As Byte, g As Byte, b As Byte) As Color
        Return col(255, r, g, b)
    End Function
    Friend Function col(c As Color, sat!) As Color
        Return col(c.A, c.R, c.G, c.B, sat)
    End Function
    Friend Function col(br As SolidBrush) As Color
        Return br.Color
    End Function
    Friend Function col(a As Byte, br As SolidBrush) As Color
        Return Color.FromArgb(CByte(CInt(a)), br.Color)
    End Function
    Friend Function col(c As Color, sat!, limit As Color, l As LimitingType) As Color
        Dim floor, ceil As Color
        If l = LimitingType.NotLessThan Then
            floor = limit
            ceil = col(255)
        Else
            ceil = limit
            floor = col(0)
        End If
        Return col(c.A, CByte(Math.Min(CInt(ceil.R), Math.Max(CInt(floor.R), CInt(c.R) * sat))), CByte(Math.Min(CInt(ceil.R), Math.Max(CInt(floor.G), CInt(c.G) * sat))), CByte(Math.Min(CInt(ceil.R), Math.Max(CInt(floor.B), CInt(c.B) * sat))))
    End Function
    Friend Function col(n As Byte) As Color
        Return col(n, n, n)
    End Function
    Friend Function col(a As Byte, n As Byte) As Color
        Return col(a, n, n, n)
    End Function
    Friend Function col(a As Byte, c As Color) As Color
        Return col(a, c.R, c.G, c.B, 1)
    End Function

    Friend Function lc(c As Color, n As Integer) As Color
        Return col(CByte(Math.Abs(CInt(c.A))), CByte(Math.Abs(CInt(c.R) - n)), CByte(Math.Abs(CInt(c.G) - n)), CByte(Math.Abs(CInt(c.B) - n)))
    End Function
    Friend Function lc(c As Color, r%, g%, b%) As Color
        Return col(CByte(Math.Abs(CInt(c.A))), CByte(Math.Abs(CInt(c.R) - r)), CByte(Math.Abs(CInt(c.G) - g)), CByte(Math.Abs(CInt(c.B) - b)))
    End Function

    Friend Function ic(c As Color, n As Integer) As Color
        Return col(CByte(Math.Min(255, CInt(c.A))), CByte(Math.Min(255, CInt(c.R) + n)), CByte(Math.Min(255, CInt(c.G) + n)), CByte(Math.Min(255, CInt(c.B) + n)))
    End Function
    Friend Function ic(c As Color, r%, g%, b%) As Color
        Return col(CByte(Math.Min(255, CInt(c.A))), CByte(Math.Min(255, CInt(c.R) + r)), CByte(Math.Min(255, CInt(c.G) + g)), CByte(Math.Min(255, CInt(c.B) + b)))
    End Function
    Friend Function rescol(c As Color) As Color
        'Dim t = (Val(CStr(c.R)) + Val(CStr(c.G)) + Val(CStr(c.B))) / 3 '				stupid bug
        Dim t = (CInt(CByte(c.R)) + CInt(CByte(c.G)) + CInt(CByte(c.B))) / 3
        If t < 128 Then Return Color.White Else Return Color.Black
    End Function

    'lum = Y=0.3RED+0.59GREEN+0.11Blue   rgb(y,y,y)
    Friend Function invert(c As Color) As Color
        Return col(CByte(CInt(c.A)), CByte(CInt(255 - c.R)), CByte(CInt(255 - c.G)), CByte(CInt(255 - c.B)))
    End Function

    Friend Function bw(c As Color) As Color
        Dim y = CByte(CInt(c.R) * 0.3) + CByte(CInt(c.G) * 0.59) + CByte(CInt(c.B) * 0.11)
        Return col(CByte(CInt(c.A)), y)
    End Function
    Friend Function bw(a As Byte, c As Color) As Color
        Dim y = CByte(CInt(c.R) * 0.3) + CByte(CInt(c.G) * 0.59) + CByte(CInt(c.B) * 0.11)
        Return col(a, y)
    End Function
    Friend Function bw(r As Byte, g As Byte, b As Byte) As Color
        Dim y = r * 0.3 + g * 0.59 + b * 0.11
        Return col(y)
    End Function
    Friend Function bw(a As Byte, r As Byte, g As Byte, b As Byte) As Color
        Dim y = r * 0.3 + g * 0.59 + b * 0.11
        Return col(a, y)
    End Function
    Friend Function blendcol(c1 As Color, c2 As Color, Optional r! = 0.5) As Color
        Return col(CByte(CInt(c1.R) * r + CInt(c2.R) * (1 - r)), CByte(CInt(c1.G) * r + CInt(c2.G) * (1 - r)), CByte(CInt(c1.B) * r + CInt(c2.B) * (1 - r)))
    End Function
    Friend Function hsltorgb(h!, s!, l!) As Color
        Dim c!, k!, m!
        c = (1 - Math.Abs(2 * l - 1)) * s
        k = c * (1 - Math.Abs(((h / 60) Mod 2) - 1))
        m = l - c / 2
        Return cyltorgb(c, k, m, h)
    End Function
    Friend Function hsvtorgb(h!, s!, v!) As Color
        Dim c!, k!, m!
        c = v * s
        k = c * (1 - Math.Abs(((h / 60) Mod 2) - 1))
        m = v - c
        Return cyltorgb(c, k, m, h)
    End Function
    Private Function cyltorgb(c!, k!, m!, h!) As Color
        Dim rd() = {c, k, 0, 0, k, c}
        Dim gd() = {k, c, c, k, 0, 0}
        Dim bd() = {0, 0, k, c, c, k}
        Dim t = Math.Ceiling(h / 60)
        If t <= 0 Then t = 0 Else t -= 1
        Return col((rd(t) + m) * 255, (gd(t) + m) * 255, (bd(t) + m) * 255)
    End Function

    Friend Function rgbtohsv(r!, g!, b!) As Single()
        Dim cmax!, cmin!, rd!, gd!, bd!, del!
        Dim h!, s!, v!

        rd = r / 255 : gd = g / 255 : bd = b / 255
        cmax = Math.Max(Math.Max(rd, gd), bd)
        cmin = Math.Min(Math.Min(rd, gd), bd)
        del = cmax - cmin

        If Not del = 0 Then
            If cmax = rd Then h = 60 * (((gd - bd) / del) Mod 6)
            If cmax = gd Then h = 60 * ((bd - rd) / del + 2)
            If cmax = bd Then h = 60 * ((rd - gd) / del + 4)
        Else
            h = 0
        End If
        If cmax = 0 Then s = 0 Else s = del / cmax

        v = cmax

        Return {h, s, v}
    End Function
#End Region

#Region "Pens"
#Region "ByRef"
    Friend Sub mp(c As Color, ByRef p As Pen)
        p = New Pen(c)
    End Sub
    Friend Sub mp(br As Brush, ByRef p As Pen)
        p = New Pen(br)
    End Sub
    Friend Sub mp(c As Color, w!, ByRef p As Pen)
        p = New Pen(c, w)
    End Sub
    Friend Sub mp(br As Brush, w!, ByRef p As Pen)
        p = New Pen(br, w)
    End Sub
#End Region

    Friend Function mp(br As Brush, w!) As Pen
        Return New Pen(br, w)
    End Function
    Friend Function mp(c As Color) As Pen
        Return New Pen(c)
    End Function
    Friend Function mp(br As Brush) As Pen
        Return New Pen(br)
    End Function
    Friend Function mp(c As Color, w!) As Pen
        Return New Pen(c, w)
    End Function
    Friend Sub mb(c As Color, ByRef b As SolidBrush)
        b = New SolidBrush(c)
    End Sub
    Friend Function mb(c As Color) As SolidBrush
        Return New SolidBrush(c)
    End Function
#End Region

#Region "Rectangle"
    Function rct(Sender As Object) As Rectangle
        Return rct(0, 0, Sender.width, Sender.height)
    End Function
    Function rct(Sender As Object, offset!) As Rectangle
        Return rct(offset, offset, Sender.width - 2 * offset, Sender.height - 2 * offset)
    End Function
    Function rct(w!, h!) As Rectangle
        Return rct(0, 0, w, h)
    End Function
    Function rct(r As Rectangle, Optional xshift! = 0, Optional yshift! = 0, Optional WidthShift! = 0, Optional HeightShift! = 0) As Rectangle
        Return rct(r.X + xshift, r.Y + yshift, r.Width + WidthShift, r.Height + HeightShift)
    End Function
    Function rct(lgb As LinearGradientBrush) As Rectangle
        Dim r = lgb.Rectangle
        Return rct(r.X, r.Y, r.Width, r.Height)
    End Function
    Function rct(lgb As LinearGradientBrush, Optional xshift! = 0, Optional yshift! = 0, Optional WidthShift! = 0, Optional HeightShift! = 0) As Rectangle
        Dim r = lgb.Rectangle
        Return rct(r.X + xshift, r.Y + yshift, r.Width + WidthShift, r.Height + HeightShift)
    End Function
    Function rct(x!, y!, w!, h!) As Rectangle
        Return New Rectangle(x, y, w, h)
    End Function
#End Region

#Region "Point"

    Friend Function pt(x!, y!) As Point
        Return New Point(x, y)
    End Function
    Friend Function pt(r As Rectangle) As Point
        Return New Point(r.Location.X, r.Location.Y)
    End Function
    Friend Function pt(o As Object) As Point
        Return New Point(o.Location.X, o.Location.Y)
    End Function
    Friend Function pt(n!) As Point
        Return New Point(n, n)
    End Function
#End Region

#Region "Rounded Rectangle"
    <DllImport("gdi32.dll")>
    Friend Function CreateRoundRectRgn(ByVal x1 As Integer, ByVal y1 As Integer, ByVal x2 As Integer, ByVal y2 As Integer, ByVal cx As Integer, ByVal cy As Integer) As IntPtr
    End Function
    Friend Function DrawRoundRectangle1(rct As Rectangle, r As Single, MSAA As Integer, FillColor As Color, BorderColor As Color, Optional BorderWidth As Single = 1) As Bitmap
        If MSAA <= 0 Then MSAA = 1
        If r <= 0 Then r = 1
        Dim b As New Bitmap(rct.Width * MSAA + MSAA, rct.Height * MSAA + MSAA)
        Dim g As Graphics = Graphics.FromImage(b)
        Dim RegH As IntPtr = CreateRoundRectRgn(rct.Left, rct.Top, (rct.Width) * MSAA, (rct.Height) * MSAA, r * MSAA, r * MSAA)
        Dim RegH1 As IntPtr = CreateRoundRectRgn(rct.Left + (MSAA * (BorderWidth + 1)), rct.Top + (MSAA * (-0.5 + BorderWidth)), (rct.Width - BorderWidth - 1) * MSAA, (rct.Height - BorderWidth - 3) * MSAA, (r - 2) * MSAA, (r - 2) * MSAA)
        Dim Reg As Region = Region.FromHrgn(RegH)
        Dim Reg1 As Region = Region.FromHrgn(RegH1)

        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.SmoothingMode = SmoothingMode.HighQuality
        g.PixelOffsetMode = PixelOffsetMode.HighQuality
        g.CompositingQuality = 2
        g.FillRegion(New SolidBrush(BorderColor), Reg)
        g.SetClip(Reg1, CombineMode.Replace)
        g.Clear(FillColor)
        Return b
    End Function
    Friend Function DrawRoundRectangle(rct As Rectangle, r!, MSAA%, FillColor As Color, BorderColor As Color, Optional BorderWidth! = 1) As Bitmap
        If MSAA <= 0 Then MSAA = 1
        If r <= 0 Then r = 1
        Dim b As New Bitmap(rct.Width * MSAA + MSAA, rct.Height * MSAA + MSAA)
        Dim g As Graphics = Graphics.FromImage(b)
        Dim RegH As IntPtr = CreateRoundRectRgn(rct.Left, rct.Top, (rct.Width) * MSAA, (rct.Height) * MSAA, r * MSAA, r * MSAA)
        Dim RegH1 As IntPtr = CreateRoundRectRgn(rct.Left + (MSAA * (BorderWidth)), rct.Top + (MSAA * (BorderWidth)), (rct.Width - BorderWidth) * MSAA, (rct.Height - BorderWidth) * MSAA, (r - 2) * MSAA, (r - 2) * MSAA)
        Dim Reg As Region = Region.FromHrgn(RegH)
        Dim Reg1 As Region = Region.FromHrgn(RegH1)

        g.InterpolationMode = InterpolationMode.HighQualityBicubic
        g.SmoothingMode = SmoothingMode.HighQuality
        g.PixelOffsetMode = 2
        g.FillRegion(New SolidBrush(BorderColor), Reg)
        g.SetClip(Reg1, CombineMode.Replace)
        g.Clear(FillColor)
        Return b
        b.Dispose()
    End Function

#End Region

#Region "Image"
    Friend Function bmp64(s As String) As Bitmap
        s.Replace("data:image/png;base64,", "")
        Return New Bitmap(Image.FromStream(New System.IO.MemoryStream(Convert.FromBase64String(s))))
    End Function

    Declare Auto Function BitBlt Lib "GDI32.DLL" (
     ByVal hdcDest As IntPtr, ByVal nXDest As Integer, ByVal nYDest As Integer,
     ByVal nWidth As Integer, ByVal nHeight As Integer,
     ByVal hdcSrc As IntPtr, ByVal nXSrc As Integer, ByVal nYSrc As Integer,
     ByVal dwRop As Integer) As Boolean

#End Region

#Region "Gradient"
    Function grad(r As Rectangle, c1 As Color, c2 As Color, type As Type, method As EasingMethods, pow!, Optional sm% = 0, Optional im% = 7) As Bitmap
        Dim b As New Bitmap(r.Width, r.Height)
        Dim int As New Interpolation()
        Dim p As New Pen(Color.Transparent)
        Dim t(4) As Single
        With Graphics.FromImage(b)
            .Clear(Color.Transparent)
            .SmoothingMode = sm : .InterpolationMode = im
            .SetClip(r)
            For i = 0 To r.Height
                t(0) = int.GetValue(c1.A, c2.A, i, r.Height, type, method, pow)
                t(1) = int.GetValue(c1.R, c2.R, i, r.Height, type, method, pow)
                t(2) = int.GetValue(c1.G, c2.G, i, r.Height, type, method, pow)
                t(3) = int.GetValue(c1.B, c2.B, i, r.Height, type, method, pow)
                mp(col(t(0), t(1), t(2), t(3)), p)
                Call .DrawLine(p, pt(0, i), pt(r.Width, i))
            Next
        End With
        p.Dispose()
        Return b
    End Function
#End Region

#Region "Draw Methods"
    Friend Function DrawArrow(x As Integer, y As Integer, flip As Boolean) As GraphicsPath  '-- Credit: AeonHack

        Dim GP As New GraphicsPath()

        Dim W As Integer = 12
        Dim H As Integer = 6

        If flip Then
            GP.AddLine(x + 1, y, x + W + 1, y)
            GP.AddLine(x + W, y, x + H, y + H - 1)
        Else
            GP.AddLine(x, y + H, x + W, y + H)
            GP.AddLine(x + W, y + H, x + H, y)
        End If

        GP.CloseFigure()
        Return GP
    End Function
    Friend Function MeasurePath(ByVal Path As GraphicsPath) As Rectangle
        Dim x, y, w, h As Integer

        For Each p As PointF In Path.PathPoints
            If x = 0 And y = 0 And w = 0 And h = 0 Then : x = p.X : y = p.Y : Continue For : End If

            If p.X < x Then x = p.X
            If p.Y < y Then y = p.Y
            If p.X > w Then w = p.X
            If p.Y > h Then h = p.Y
        Next

        Return New Rectangle(x, y, w + -x, h + -y)
    End Function
    Friend Function MeasureString(ByVal Str As String, ByVal Font As Font) As Size
        Dim B As New Bitmap(32, 1)
        Dim Ret As Size
        Str &= "|"

        Using G As Graphics = Graphics.FromImage(B)
            Ret = G.MeasureString(Str, Font).ToSize

            G.Clear(Color.White)
            G.DrawString(Str, Font, Brushes.Black, B.Width - Math.Truncate(Ret.Width), -Font.Height / 2)
        End Using

        For x = B.Width - 1 To 0 Step -1
            If B.GetPixel(x, 0).R <> 255 Then Return Ret - New Size(B.Width - x, 0)
        Next
    End Function
    Friend Function GetVisibleRectParts(ByVal Rect1 As Rectangle, ByVal Rect2 As Rectangle) As Rectangle()
        If Not Rect1.IntersectsWith(Rect2) Then Return {Rect1}

        Dim r1, r2, r3, r4 As New Rectangle

        'top
        r1.X = Rect1.X
        r1.Y = Rect1.Y
        r1.Width = Rect1.Width
        r1.Height = Rect2.Y - Rect1.Y

        'left
        r3.X = Rect1.X
        r3.Y = Rect1.Y + r1.Height
        r3.Width = Rect2.X - Rect1.X
        r3.Height = Rect1.Height - r1.Height

        'rigth
        r4.X = Rect2.Width + (Rect2.X - Rect1.X)
        r4.Y = Rect1.Y + r1.Height
        r4.Width = (Rect1.Width - Rect2.Width) + (Rect1.X - Rect2.X)
        r4.Height = Rect1.Height - r1.Height

        'bottom
        r2.X = Rect1.X + r3.Width
        r2.Y = Rect2.Height + (Rect2.Y - Rect1.Y)
        r2.Width = Rect1.Width - (r4.Width + r3.Width)
        r2.Height = (Rect1.Height - Rect2.Height) - (Rect2.Y - Rect1.Y)

        Dim Ret As New List(Of Rectangle) From {r1, r2, r3, r4}
        For i = 3 To 0 Step -1
            If Ret(i).Width <= 0 Or Ret(i).Height <= 0 Then Ret.RemoveAt(i)
        Next

        Return Ret.ToArray
    End Function
    Friend Function PathIntersect(pth1 As GraphicsPath, pth2 As GraphicsPath) As Boolean
        If Not pth1.GetBounds.IntersectsWith(pth2.GetBounds) Then Return False

        Do
            For Each p1 As PointF In pth1.PathPoints
                Dim x, y As Boolean
                x = pth2.PathPoints(0).X >= p1.X
                y = pth2.PathPoints(0).Y >= p1.Y

                For Each p2 As PointF In pth2.PathPoints
                    If Not (p2.X >= p1.X) Or Not (p2.Y >= p1.Y) Then Continue Do
                Next

                Return True
            Next

            Return False
        Loop
    End Function

#End Region

#Region "Math!"
    Public Function clamp#(v#, l#, h#)
        Return Math.Min(h, Math.Max(l, v))
    End Function
#End Region

#End Region

End Module
