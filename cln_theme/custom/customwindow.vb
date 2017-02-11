Imports System.Runtime.InteropServices
Imports System.Drawing.Drawing2D


Public Class CustomWindow : Inherits Form : Implements AnimatedObject


#Region "DECLARE"
    Private Const BorderWidth As Integer = 6
    Dim tp As Pen : Dim tb As SolidBrush = Brushes.Black : Dim tgb As LinearGradientBrush
    Dim lb(4), db(4) As Bitmap
    Dim ot% = 0
    Dim fxt% = 0
    Dim cbx% = 0
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


#Region "VISUALS"
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
        mp(col(25, 255), tp)
        g.DrawLine(tp, 1, 23 + 3, Width - 2, 23 + 3)
        g.DrawLine(tp, 1, 1, Width - 2, 1)

        mp(col(60, 0), tp)
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
        g.DrawString("0", f, mb(Color.Fuchsia), pt(Width - 60, 6))


    End Sub
    Sub drawlight(ByRef g As Graphics)

    End Sub

#End Region

#Region "Events"
    Protected Overrides Sub OnMouseMove(e As MouseEventArgs)
        ms = 1
        MyBase.OnMouseMove(e)
        For Each c As Control In Controls
            'On Error Resume Next : DirectCast(c, customControl).leavemouse(e)
        Next
        Dim r = Width - 99 - BorderWidth / 2
        If e.X > r And e.X < Width - BorderWidth / 2 And e.Y < 34 And e.Y > 0 Then
            If e.X > r And e.X < r + 34 Then cbx = r
            If e.X > r + 33 And e.X < r + 67 Then cbx = r + 33
            If e.X > r + 65 Then cbx = r + 66
        Else : cbx = 0
        End If
        Invalidate()
        mmove()
    End Sub
    Protected Overrides Sub OnResize(e As EventArgs)
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
    Public Shadows Sub updatebounds()
        'SetWindowPos(Handle, ParentForm.Handle, ParentForm.Left - ShadowSize, ParentForm.Top - ShadowSize, ParentForm.Width + ShadowSize * 2, ParentForm.Height + ShadowSize * 2, SetWindowPosFlags.FrameChanged)
        SetWindowPos(Handle, 0, Left, Top, Shadow.Width - 40, Shadow.Height - 40, SetWindowPosFlags.FrameChanged)
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
            '.SetClip(rct(s + 5, s + 5, Width, Height), CombineMode.Exclude)
            '.DrawRectangle(Pens.Black, s + 4, s + 4, Width + 1, Height + 1)
            'mp(col(100, 0)) : G.DrawRectangle(tp, s - 1, s - 1, Width + 11, Height + 11)
            mb(col(100, 255), tb)
            .FillRectangle(tb, rct(Width + s + 5 - 70, s + 7, 70 - 2, 23 - 4))
            .ResetClip()

            .SmoothingMode = 2 ': .PixelOffsetMode = 2
            .SetClip(rct(s, s, Width - 2 * s, Height - 2 * s), CombineMode.Exclude)
            For i = 0 To 13
                Dim pth As GraphicsPath = DM.CreateRoundRectangle(rct(1 + i, 1 + i, Width + 37 - 2 * i, Height + 37 - 2 * i), 13 - i, 1, 1, 1, 1)
                mp(col(ip.GetValue(0, 200, i + 1, 14, Type.EaseIn, EasingMethods.Exponent, 1), 0), tp)
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














































'#Region "CONTROL BOX"

'#Region "Functions"
'	Sub mp(c As Byte)
'		tp = New Pen(col(c))
'	End Sub
'	Sub mp(c As Color)
'		tp = New Pen(c)
'	End Sub
'	Sub mp(a As Byte, c As Color)
'		tp = New Pen(col(a, c))
'	End Sub
'	Sub px(c As Color, x!, y!, g As Graphics)
'		tb = New SolidBrush(c)
'		g.FillRectangle(tb, x, y, 1, 1)
'		tb.Dispose()
'	End Sub
'	Function gri(b As Bitmap) As Graphics
'		Return Graphics.FromImage(b)
'	End Function
'#End Region

'#Region "Draw"
'	Sub drawmin(gd As Graphics, gl As Graphics)

'		gl.InterpolationMode = 7


'		mp(70)
'		gl.DrawLine(tp, 11, 20, 21, 20)
'		mp(97)
'		gl.DrawLine(tp, 11, 21, 21, 21)


'		mp(col(70, Color.Black))
'		gl.DrawLine(tp, 12, 22, 20, 22)
'		mp(col(40, Color.Black))
'		gl.DrawLine(tp, 12, 19, 20, 19)
'		px(col(40, Color.Black), 10, 20, gl)
'		px(col(35, Color.Black), 10, 21, gl)
'		px(col(40, Color.Black), 22, 20, gl)
'		px(col(35, Color.Black), 22, 21, gl) '									light version

'		gd.InterpolationMode = 7
'		mp(70)
'		gd.DrawLine(tp, 11, 20, 21, 20)
'		mp(97)
'		gd.DrawLine(tp, 11, 21, 21, 21)

'		If rescol(BackColor) = Color.White Then
'			mp(col(51, Color.Black))
'			gd.DrawLine(tp, 12, 20, 20, 20)
'		Else
'			mp(col(50, Color.White))
'			gd.DrawLine(tp, 12, 22, 20, 22)
'			mp(col(40, Color.White))
'			gd.DrawLine(tp, 12, 19, 20, 19)
'			px(col(40, Color.White), 10, 20, gd)
'			px(col(35, Color.White), 10, 21, gd)
'			px(col(40, Color.White), 22, 20, gd)
'			px(col(35, Color.White), 22, 21, gd)
'		End If '									dark version

'	End Sub
'	Sub drawmax(gd As Graphics, gl As Graphics)

'		gl.InterpolationMode = 7


'		gl.SetClip(New Rectangle(48, 13, 7, 7), CombineMode.Exclude)
'		tgb = New LinearGradientBrush(New Rectangle(46, 12, 11, 11), col(85), col(100), 90.0F)
'		gl.FillRectangle(tgb, New Rectangle(46, 11, 11, 11))
'		mp(col(22, Color.Black))
'		gl.DrawRectangle(tp, 46, 12, 10, 9)
'		mp(col(3, Color.White))
'		gl.DrawLine(tp, 46, 13, 56, 13)
'		mp(60)
'		gl.DrawLine(tp, 46, 11, 56, 11)
'		mp(51)
'		gl.DrawLine(tp, 48, 20, 54, 20)



'		gl.ResetClip()
'		mp(col(50, Color.Black))
'		gl.DrawLine(tp, 48, 13, 54, 13)
'		mp(col(24, Color.Black))
'		gl.DrawLine(tp, 48, 13, 48, 19)
'		mp(col(24, Color.Black))
'		gl.DrawLine(tp, 54, 13, 54, 19)
'		mp(col(12, Color.Black))
'		gl.DrawLine(tp, 48, 19, 54, 19)

'		mp(col(16, Color.Black))
'		gl.DrawLine(tp, 46, 10, 56, 10)
'		mp(col(38, Color.Black))
'		gl.DrawLine(tp, 46, 22, 56, 22)
'		mp(col(24, Color.Black))
'		gl.DrawLine(tp, 45, 11, 45, 21)
'		mp(col(24, Color.Black))
'		gl.DrawLine(tp, 57, 11, 57, 21)

'		px(col(2, Color.Black), 45, 10, gl)
'		px(col(2, Color.Black), 57, 10, gl)
'		px(col(8, Color.Black), 45, 22, gl)
'		px(col(2, Color.Black), 57, 22, gl)	'									light version

'		gd.InterpolationMode = 7

'		gd.SetClip(New Rectangle(48, 13, 7, 7), CombineMode.Exclude)
'		tgb = New LinearGradientBrush(New Rectangle(46, 12, 11, 11), col(85), col(100), 90.0F)
'		gd.FillRectangle(tgb, New Rectangle(46, 11, 11, 11))

'		mp(col(22, Color.Black))
'		gd.DrawRectangle(tp, 46, 12, 10, 9)
'		mp(col(3, Color.White))
'		gd.DrawLine(tp, 46, 13, 56, 13)
'		mp(60)
'		gd.DrawLine(tp, 46, 11, 56, 11)
'		mp(51)
'		gd.DrawLine(tp, 48, 20, 54, 20)
'		gd.ResetClip()
'		mp(col(50, Color.White))
'		gd.DrawLine(tp, 48, 13, 54, 13)
'		mp(col(24, Color.White))
'		gd.DrawLine(tp, 48, 13, 48, 19)
'		mp(col(24, Color.White))
'		gd.DrawLine(tp, 54, 13, 54, 19)
'		mp(col(12, Color.White))
'		gd.DrawLine(tp, 48, 19, 54, 19)

'		mp(col(16, Color.White))
'		gd.DrawLine(tp, 46, 10, 56, 10)
'		mp(col(38, Color.White))
'		gd.DrawLine(tp, 46, 22, 56, 22)
'		mp(col(24, Color.White))
'		gd.DrawLine(tp, 45, 11, 45, 21)
'		mp(col(24, Color.White))
'		gd.DrawLine(tp, 57, 11, 57, 21)

'		px(col(2, Color.White), 45, 10, gd)
'		px(col(2, Color.White), 57, 10, gd)
'		px(col(8, Color.White), 45, 22, gd)
'		px(col(2, Color.White), 57, 22, gd)	'									dark version

'	End Sub
'	Sub drawrestore(gd As Graphics, gl As Graphics)

'		gl.InterpolationMode = 7

'		mp(64)
'		gl.DrawLine(tp, 46, 15, 48, 15)
'		gl.DrawLine(tp, 50, 11, 56, 11)
'		mp(56)
'		gl.DrawLine(tp, 48, 20, 50, 20)
'		gl.DrawLine(tp, 51, 19, 52, 19)
'		gl.DrawLine(tp, 52, 16, 54, 16)

'		tgb = New LinearGradientBrush(New Rectangle(50, 12, 7, 6), col(105), col(89), 90.0F)
'		gl.SetClip(New Rectangle(52, 13, 3, 4), CombineMode.Exclude)
'		gl.FillRectangle(tgb, tgb.Rectangle)
'		gl.ResetClip()

'		mp(105)
'		gl.DrawLine(tp, 46, 16, 48, 16)
'		mp(102)
'		gl.DrawLine(tp, 46, 17, 47, 17)
'		mp(99)
'		gl.DrawLine(tp, 46, 18, 47, 18)
'		mp(95)
'		gl.DrawLine(tp, 46, 19, 47, 19)
'		mp(92)
'		gl.DrawLine(tp, 51, 20, 52, 20)
'		gl.DrawLine(tp, 46, 20, 47, 20)
'		mp(89)
'		gl.DrawLine(tp, 46, 21, 52, 21)


'		mp(col(50, Color.Black))
'		px(col(50, Color.Black), 48, 17, gl)
'		gl.DrawLine(tp, 46, 22, 52, 22)
'		gl.DrawLine(tp, 50, 18, 56, 18)
'		gl.DrawLine(tp, 52, 13, 54, 13)	'									light version

'		gd.InterpolationMode = 7
'		mp(64)
'		gd.DrawLine(tp, 46, 15, 48, 15)
'		gd.DrawLine(tp, 50, 11, 56, 11)
'		mp(56)
'		gd.DrawLine(tp, 48, 20, 50, 20)
'		gd.DrawLine(tp, 51, 19, 52, 19)
'		gd.DrawLine(tp, 52, 16, 54, 16)

'		tgb = New LinearGradientBrush(New Rectangle(50, 12, 7, 6), col(105), col(89), 90.0F)
'		gd.SetClip(New Rectangle(52, 13, 3, 4), CombineMode.Exclude)
'		gd.FillRectangle(tgb, tgb.Rectangle)
'		gd.ResetClip()

'		mp(105)
'		gd.DrawLine(tp, 46, 16, 48, 16)
'		mp(102)
'		gd.DrawLine(tp, 46, 17, 47, 17)
'		mp(99)
'		gd.DrawLine(tp, 46, 18, 47, 18)
'		mp(95)
'		gd.DrawLine(tp, 46, 19, 47, 19)
'		mp(92)
'		gd.DrawLine(tp, 51, 20, 52, 20)
'		gd.DrawLine(tp, 46, 20, 47, 20)
'		mp(89)
'		gd.DrawLine(tp, 46, 21, 52, 21)

'		mp(col(50, Color.White))
'		px(col(50, Color.White), 48, 17, gd)
'		gd.DrawLine(tp, 46, 22, 52, 22)
'		gd.DrawLine(tp, 50, 18, 56, 18)
'		gd.DrawLine(tp, 52, 13, 54, 13)	'									dark version

'	End Sub
'	Sub drawclose(gd As Graphics, gl As Graphics)
'		Dim g As Graphics
'		Static im As New Bitmap(13, 13)
'		im = New Bitmap(Image.FromStream(New System.IO.MemoryStream(Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAA0AAAANCAYAAABy6+R8AAAABGdBTUEAALGPC/xhBQAAAAlwSFlzAAAOwAAADsABataJCQAAABp0RVh0U29mdHdhcmUAUGFpbnQuTkVUIHYzLjUuMTAw9HKhAAAAxUlEQVQoU5WPQQqDMBREvZOiC1u1e8GiAau1YhEt3v8A00zgpzFSShcvPzPvBzQA8Df2UhQXPY4LguvNkecFuq4DpwgX31uxbRva9oYsy4wQmNnTS2clWdcXmkbhdDrriICTmb3skN0jsiwLlFJI09RMZn9nF4R5nlHXNTh9Rw5FkiSYpqeF2d/ZhTiOzT+M46gjAk5m9rJD7CWKIvNJw/DQ8bPAzJ5eOnOEYYiquqLv71a4sKfnHrMVZVma4huuP8jfIHgDP+3Ac95JZCsAAAAASUVORK5CYII="))))

'		gl.InterpolationMode = 7

'		Static cgl As New Bitmap(103 - 80, 33 - 10)
'		g = Graphics.FromImage(cgl)
'		g.InterpolationMode = 7
'		g.CompositingQuality = 2

'		px(col(16, Color.Black), 2, 0, g)
'		px(col(16, Color.Black), 1, 1, g)
'		px(col(16, Color.Black), 9, 1, g)
'		px(col(16, Color.Black), 10, 0, g)
'		px(col(16, Color.Black), 11, 1, g)

'		px(col(14, Color.Black), 4, 2, g)
'		px(col(14, Color.Black), 5, 3, g)
'		px(col(14, Color.Black), 6, 4, g)
'		px(col(14, Color.Black), 7, 3, g)
'		px(col(14, Color.Black), 8, 2, g)
'		px(col(14, Color.Black), 3, 10, g)
'		px(col(14, Color.Black), 9, 10, g)

'		px(col(12, Color.Black), 2, 8, g)
'		px(col(12, Color.Black), 1, 9, g)
'		px(col(12, Color.Black), 0, 10, g)
'		px(col(12, Color.Black), 10, 8, g)
'		px(col(12, Color.Black), 11, 9, g)
'		px(col(12, Color.Black), 12, 10, g)

'		px(col(38, Color.Black), 1, 11, g)
'		px(col(38, Color.Black), 2, 12, g)
'		px(col(38, Color.Black), 3, 11, g)
'		px(col(38, Color.Black), 9, 11, g)
'		px(col(38, Color.Black), 10, 12, g)
'		px(col(38, Color.Black), 11, 11, g)

'		px(col(40, Color.Black), 6, 8, g)
'		px(col(40, Color.Black), 5, 9, g)
'		px(col(40, Color.Black), 4, 10, g)
'		px(col(40, Color.Black), 7, 9, g)
'		px(col(40, Color.Black), 8, 10, g)

'		px(col(46, Color.Black), 0, 2, g)
'		px(col(46, Color.Black), 1, 3, g)
'		px(col(46, Color.Black), 12, 2, g)
'		px(col(46, Color.Black), 11, 3, g)

'		px(col(44, Color.Black), 2, 4, g)
'		px(col(44, Color.Black), 3, 5, g)
'		px(col(44, Color.Black), 10, 4, g)
'		px(col(44, Color.Black), 9, 5, g)

'		px(col(36, Color.Black), 4, 6, g)
'		px(col(36, Color.Black), 8, 6, g) '																shadow



'		gl.DrawImageUnscaled(im, 80, 10)
'		gl.DrawImageUnscaled(cgl, 80, 10) '									light version

'		gd.InterpolationMode = 7
'		g.CompositingQuality = 2

'		Static cgd As New Bitmap(103 - 80, 33 - 10)
'		g = Graphics.FromImage(cgd)
'		g.InterpolationMode = 7

'		px(col(16, Color.White), 2, 0, g)
'		px(col(16, Color.White), 1, 1, g)
'		px(col(16, Color.White), 9, 1, g)
'		px(col(16, Color.White), 10, 0, g)
'		px(col(16, Color.White), 11, 1, g)

'		px(col(14, Color.White), 4, 2, g)
'		px(col(14, Color.White), 5, 3, g)
'		px(col(14, Color.White), 6, 4, g)
'		px(col(14, Color.White), 7, 3, g)
'		px(col(14, Color.White), 8, 2, g)
'		px(col(14, Color.White), 3, 10, g)
'		px(col(14, Color.White), 9, 10, g)

'		px(col(12, Color.White), 2, 8, g)
'		px(col(12, Color.White), 1, 9, g)
'		px(col(12, Color.White), 0, 10, g)
'		px(col(12, Color.White), 10, 8, g)
'		px(col(12, Color.White), 11, 9, g)
'		px(col(12, Color.White), 12, 10, g)

'		px(col(38, Color.White), 1, 11, g)
'		px(col(38, Color.White), 2, 12, g)
'		px(col(38, Color.White), 3, 11, g)
'		px(col(38, Color.White), 9, 11, g)
'		px(col(38, Color.White), 10, 12, g)
'		px(col(38, Color.White), 11, 11, g)

'		px(col(40, Color.White), 6, 8, g)
'		px(col(40, Color.White), 5, 9, g)
'		px(col(40, Color.White), 4, 10, g)
'		px(col(40, Color.White), 7, 9, g)
'		px(col(40, Color.White), 8, 10, g)

'		px(col(46, Color.White), 0, 2, g)
'		px(col(46, Color.White), 1, 3, g)
'		px(col(46, Color.White), 12, 2, g)
'		px(col(46, Color.White), 11, 3, g)

'		px(col(44, Color.White), 2, 4, g)
'		px(col(44, Color.White), 3, 5, g)
'		px(col(44, Color.White), 10, 4, g)
'		px(col(44, Color.White), 9, 5, g)

'		px(col(36, Color.White), 4, 6, g)
'		px(col(36, Color.White), 8, 6, g)

'		gd.DrawImageUnscaled(im, 80, 10)
'		gd.DrawImageUnscaled(cgd, 80, 10) '									dark version

'	End Sub
'#End Region

'	Protected Overrides Sub OnCreateControl()
'		MyBase.OnCreateControl()

'		For i = 0 To 3
'			lb(i) = New Bitmap(103, 33) : db(i) = New Bitmap(103, 33)
'		Next

'		drawmin(gri(db(0)), gri(lb(0)))
'		drawmax(gri(db(1)), gri(lb(1)))
'		drawrestore(gri(db(2)), gri(lb(2)))
'		drawclose(gri(db(3)), gri(lb(3)))

'		tgb.Dispose()
'	End Sub
'#End Region' old cb





'Dim g = e.Graphics
'bv1 = New LinearGradientBrush(New Rectangle(0, Height - CInt(sht), Width, CInt(sht)), Color.Transparent, Color.FromArgb(50, Color.Black), 90.0F)
'bh = New LinearGradientBrush(New Rectangle(0, Height - CInt(sht), Width, CInt(sht)), Color.FromArgb(60, Color.Black), Color.Transparent, 0.0F)
'bh1 = New LinearGradientBrush(New Rectangle(0, Height - CInt(sht), Width, CInt(sht)), Color.Transparent, Color.FromArgb(60, Color.Black), 0.0F)

'If rescol(BackColor) = Color.White Then
'tp = New Pen(Color.FromArgb(20, rescol(BackColor)), 0)
''		g.DrawRectangle(tp, 0, 0, Width - 1, Height - 1)
'Else
'tp = New Pen(Color.FromArgb(60, rescol(BackColor)), 0)
''		g.DrawRectangle(tp, 0, 0, Width - 1, Height - 1)
'tp = New Pen(DM.Invert(rescol(BackColor)), 0)
''		g.DrawRectangle(tp, 1, 1, Width - 3, Height - 2)
'End If




'If sst Then
'tb = New SolidBrush(ForeColor)
'If sst = True Then
'g.FillRectangle(tb, New Rectangle(0, Height - CInt(sht), Width, CInt(sht)))
'g.FillRectangle(bv1, New Rectangle(0, Height - CInt(sht), Width, CInt(sht)))
'g.FillRectangle(bh, New Rectangle(0, Height - CInt(sht), CInt(Width / 2) - 1, CInt(sht)))
'g.FillRectangle(bh1, New Rectangle(CInt(Width / 2) - 1, Height - CInt(sht), CInt(Width / 2) + 2, CInt(sht)))
'End If


'Dim px As Integer() = {13, 11, 9, 7}
'Dim py As Integer() = {7, 9, 11, 13}

'For i = 0 To 3
'For j = 0 To i
'tb = New SolidBrush(Color.FromArgb(180, Color.Black))
'g.FillRectangle(tb, Width - px(i), Height - py(j), 1, 1)
'tb = New SolidBrush(Color.FromArgb(80, Color.White))
'g.FillRectangle(tb, Width - px(i) - 1, Height - py(j) - 1, 1, 1)
'Next
'Next




'tp = New Pen(Color.FromArgb(40, Color.Black))
'g.DrawLine(tp, 0, Height - CInt(sht), Width - 1, Height - CInt(sht))
'tp = New Pen(Color.FromArgb(30, Color.Black))
'g.DrawLine(tp, 0, Height - CInt(sht) + 1, Width - 1, Height - CInt(sht) + 1)


'tp = New Pen(Color.FromArgb(13, Color.White), 0)
'g.DrawLine(tp, 0, Height - 1, Width - 2, Height - 1)
'g.DrawLine(tp, 0, Height - CInt(sht), 0, Height - 2)
'g.DrawLine(tp, Width - 1, Height - CInt(sht), Width - 1, Height - 1)

'bv1.Dispose() : bh.Dispose() : bh1.Dispose()
'tp.Dispose() : tb.Dispose()

'End If




'tp = New Pen(ForeColor, 2)
'If DesignMode Then Exit Sub
'		Select Case cb.l
'Case 0
'g.DrawLine(tp, Width - 86, 1, Width - 85 + 28 - 1, 1)
'Case 1
'g.DrawLine(tp, Width - 86 + 28 + 1, 1, Width - 85 + 28 + 28, 1)
'Case 2
'g.DrawLine(tp, Width - 86 + 28 + 28, 1, Width - 85 + 28 + 28 + 28 - 1, 1)
'Case 3
'End Select
''Sub loadfx()
''    Static lf As New Interpolation
''    If Not fxt = 0 Then Exit Sub
''    If Opacity < 1 Then
''        If ot < 5000 Then
''            Opacity = lf.GetValue(0, 1, ot, 5000, Type.EaseOut, EasingMethods.Exponent, 1)
''            op = Opacity
''            ot += 1
''        Else
''            animating=false
''            Opacity = 1
''            op = 1.0#
''            fxt = -1
''            ot = 0
''        End If
''    End If
''End Sub
'Sub closefx()
'    Static lf1 As New Interpolation
'    If Not fxt = 1 Then Exit Sub
'    If Opacity > 0 Then
'        If ot < 300 Then
'            Opacity = lf1.GetValue(1, 0, ot, 300, Type.EaseOut, EasingMethods.Exponent, 1.5)
'            op = Opacity
'            ot += 1
'        Else
'            animating=false
'            Opacity = 0
'            op = 0.0#
'            fxt = -1
'            ot = 0
'            tmr.Dispose()
'            End
'        End If
'    End If
'End Sub
