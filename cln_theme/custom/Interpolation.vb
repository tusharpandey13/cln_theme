Class Interpolation
#Region "Declare"
    Private Property ivar As Calc = Variants.Regular
    Private Delegate Function Calc(x#, pow#) As Single

#End Region

#Region "  FUNCTS"

    Private NotInheritable Class Variants

        Public Shared ReadOnly Regular = New Calc(Function(x#, pow#) Math.Pow(x, pow))
        Public Shared ReadOnly Circular = New Calc(Function(x#, pow#) 1 - Math.Sin(Math.Acos(x)))
        Public Shared ReadOnly Jumpback = New Calc(Function(x#, pow#) (x ^ pow) * (2.5 * x - 1.5))
        Public Shared ReadOnly Elastic = New Calc(Function(x#, pow#) Math.Pow(2, 10 * (x - 1)) * Math.Cos(20 * Math.PI * 1.5 / 3 * x))
        Public Shared ReadOnly Bounce = New Calc(Function(x#, pow#)
                                                     Dim a As Double = 0, b As Double = 1
                                                     Do
                                                         If x >= (7 - 4 * a) / 11 Then Return -Math.Pow((11 - 6 * a - 11 * x) / 4, 2) + Math.Pow(b, 2)
                                                         a += b : b /= 2
                                                     Loop
                                                 End Function)
        Public Shared ReadOnly Sine = New Calc(Function(x#, pow#) 1 - Math.Cos(x * (Math.PI / 2)))
        Public Shared ReadOnly Exponent = New Calc(Function(x#, pow#) 2 ^ (10 * ((x ^ pow) - 1)))
        Public Shared ReadOnly Critical_Damping = New Calc(Function(x#, pow#) (1 - Math.Exp(((-1 * x) * 5))) / 0.993262053)

    End Class

    Private Function cmr(t#, p0#, p1#, p2#, p3#)
        Return 0.5F * (
                      (2 * p1) +
                      (-p0 + p2) * t +
                      (2 * p0 - 5 * p1 + 4 * p2 - p3) * t * t +
                      (-p0 + 3 * p1 - 3 * p2 + p3) * t * t * t
                      )
    End Function
    Private Function smtstp(x#) As Double
        Return 3 * (x ^ 2) - 2 * (x ^ 3)
        'Return (x * x * (3 - 2 * x))
    End Function
    Private Function smtrstp(x#) As Double
        Return 6 * (x ^ 5) - 15 * (x ^ 4) + 10 * (x ^ 3)
    End Function

#End Region



    ''' <summary>
    ''' Gets the calculated value of the Interpolation.
    ''' </summary>
    Public Function GetValue(StartV#, EndV#, Time#, Duration#, IType As Type, Method As EasingMethods, Power#, Q#, R#) As Single

        Dim p# = Time / Duration
        Dim rev As Boolean
        Dim c# = EndV - StartV
        If c < 0.0# Then rev = True
        c = Math.Abs(c)

        Select Case Method
            Case EasingMethods.Bounce
                ivar = Variants.Bounce
            Case EasingMethods.Circular
                ivar = Variants.Circular
            Case EasingMethods.Elastic
                ivar = Variants.Elastic
            Case EasingMethods.Exponent
                ivar = Variants.Exponent
            Case EasingMethods.Jumpback
                ivar = Variants.Jumpback
            Case EasingMethods.Regular
                ivar = Variants.Regular
            Case EasingMethods.Sine
                ivar = Variants.Sine
            Case EasingMethods.Critical_Damping
                ivar = Variants.Critical_Damping
        End Select



        Dim rt#
        Select Case IType
            Case Type.Linear
            Case Type.CatmullRom
                p = cmr(p, Q, 0, 1, R)
            Case Type.EaseIn
                p = ivar.Invoke(p, Power)
            Case Type.EaseOut
                p = 1 - ivar.Invoke(1 - p, Power)
            Case Type.EaseInOut
                p = If(p <= 0.5, ivar.Invoke(2 * p, Power) / 2, (2 - ivar.Invoke(2 * (1 - p), Power)) / 2)
            Case Type.SmoothStep
                For i = 1 To CInt(Power)
                    p = smtstp(p)
                Next
            Case Type.Smootherstep
                For i = 1 To CInt(Power)
                    p = smtrstp(p)
                Next
        End Select

        If rev Then p = 1 - p

        rt = p * c
        Return CSng(rt + Math.Min(StartV, EndV))
    End Function
    Public Function GetValue(StartV#, EndV#, Time#, Duration#, Q#, R#) As Single
        Return GetValue(StartV, EndV, Time, Duration, Type.CatmullRom, EasingMethods.Regular, 1, Q, R)
    End Function
    Public Function GetValue(StartV#, EndV#, Time#, Duration#, IType As Type, Power#) As Single
        Return GetValue(StartV, EndV, Time, Duration, IType, EasingMethods.Regular, Power, 0, 0)
    End Function
    Public Function GetValue(StartV#, EndV#, Time#, Duration#, IType As Type, Method As EasingMethods, Power#) As Single
        Return GetValue(StartV, EndV, Time, Duration, IType, Method, Power, 0, 0)
    End Function

End Class
