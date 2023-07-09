﻿Public Class Form_factura_egreso

    Dim concepvr, subttl, ttl As Long


    Private Sub Form_factura_egreso_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        'Me.PictureBox1.Image = imagenlogocea
        Me.Label7.Text = aca_nom
        Me.Label8.Text = aca_nom2
        Me.Label9.Text = "Resoculción No." & aca_lic_min & " Ministerio de Transporte"
        Me.Label10.Text = "Lic. No. " & aca_lic_min & " Secretaría de Educación Municipal"
        Me.Label11.Text = "Dirección: " & aca_dirs & ". Tels:" & aca_tels & " " & aca_cels & "Ibagué Tolima"


        concepvr = CLng(imp_egre_concepvr)
        subttl = CLng(imp_egre_subttl)
        ttl = CLng(imp_egre_ttl)

        Me.Label17.Text = imp_egre_cliedoc
        Me.Label16.Text = imp_egre_clienom
        Me.Label19.Text = imp_egre_cliedir
        Me.Label18.Text = imp_egre_clietel
        Me.Label22.Text = imp_egre_concep
        Me.Label27.Text = subttl.ToString("###,###,##0")
        Me.Label29.Text = ttl.ToString("###,###,##0")
        Me.Label31.Text = "Elaboró: " & imp_egre_empleado
        Me.Label30.Text = Num2Text(CLng(imp_egre_concepvr))
        Me.Label30.Text = "SON: " & Me.Label30.Text & " PESOS M/CTE"
        Me.Label1.Text = imp_egre_num
        Me.Label2.Text = imp_egre_fecha
        Me.Label23.Text = concepvr.ToString("###,###,##0")
        Me.Label36.Text = "Forma de Pago: " & imp_mediopago



    End Sub
    Private Function Num2Text(ByVal value As Double) As String
        Select Case value
            Case 0 : Num2Text = "CERO"
            Case 1 : Num2Text = "UN"
            Case 2 : Num2Text = "DOS"
            Case 3 : Num2Text = "TRES"
            Case 4 : Num2Text = "CUATRO"
            Case 5 : Num2Text = "CINCO"
            Case 6 : Num2Text = "SEIS"
            Case 7 : Num2Text = "SIETE"
            Case 8 : Num2Text = "OCHO"
            Case 9 : Num2Text = "NUEVE"
            Case 10 : Num2Text = "DIEZ"
            Case 11 : Num2Text = "ONCE"
            Case 12 : Num2Text = "DOCE"
            Case 13 : Num2Text = "TRECE"
            Case 14 : Num2Text = "CATORCE"
            Case 15 : Num2Text = "QUINCE"
            Case Is < 20 : Num2Text = "DIECI" & Num2Text(value - 10)
            Case 20 : Num2Text = "VEINTE"
            Case Is < 30 : Num2Text = "VEINTI" & Num2Text(value - 20)
            Case 30 : Num2Text = "TREINTA"
            Case 40 : Num2Text = "CUARENTA"
            Case 50 : Num2Text = "CINCUENTA"
            Case 60 : Num2Text = "SESENTA"
            Case 70 : Num2Text = "SETENTA"
            Case 80 : Num2Text = "OCHENTA"
            Case 90 : Num2Text = "NOVENTA"
            Case Is < 100 : Num2Text = Num2Text(Int(value \ 10) * 10) & " Y " & Num2Text(value Mod 10)
            Case 100 : Num2Text = "CIEN"
            Case Is < 200 : Num2Text = "CIENTO " & Num2Text(value - 100)
            Case 200, 300, 400, 600, 800 : Num2Text = Num2Text(Int(value \ 100)) & "CIENTOS"
            Case 500 : Num2Text = "QUINIENTOS"
            Case 700 : Num2Text = "SETECIENTOS"
            Case 900 : Num2Text = "NOVECIENTOS"
            Case Is < 1000 : Num2Text = Num2Text(Int(value \ 100) * 100) & " " & Num2Text(value Mod 100)
            Case 1000 : Num2Text = "MIL"
            Case Is < 2000 : Num2Text = "MIL " & Num2Text(value Mod 1000)
            Case Is < 1000000 : Num2Text = Num2Text(Int(value \ 1000)) & " MIL"
                If value Mod 1000 Then Num2Text = Num2Text & " " & Num2Text(value Mod 1000)
            Case 1000000 : Num2Text = "UN MILLON"
            Case Is < 2000000 : Num2Text = "UN MILLON " & Num2Text(value Mod 1000000)
            Case Is < 1000000000000.0# : Num2Text = Num2Text(Int(value / 1000000)) & " MILLONES "
                If (value - Int(value / 1000000) * 1000000) Then Num2Text = Num2Text & " " & Num2Text(value - Int(value / 1000000) * 1000000)
            Case 1000000000000.0# : Num2Text = "UN BILLON"
            Case Is < 2000000000000.0# : Num2Text = "UN BILLON " & Num2Text(value - Int(value / 1000000000000.0#) * 1000000000000.0#)
            Case Else : Num2Text = Num2Text(Int(value / 1000000000000.0#)) & " BILLONES"
                If (value - Int(value / 1000000000000.0#) * 1000000000000.0#) Then Num2Text = Num2Text & " " & Num2Text(value - Int(value / 1000000000000.0#) * 1000000000000.0#)
        End Select
    End Function

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Cursor = Cursors.WaitCursor
        Me.Button1.Dispose()
        Me.Button1.Visible = False
        System.Threading.Thread.Sleep(2500)
        Me.Cursor = Cursors.Default

        Me.PrintForm1.PrintAction = Printing.PrintAction.PrintToPreview
        Me.PrintForm1.PrinterSettings.DefaultPageSettings.Margins.Top = 7
        Me.PrintForm1.PrinterSettings.DefaultPageSettings.Margins.Left = 5
        Me.PrintForm1.PrinterSettings.DefaultPageSettings.Margins.Right = 5
        Me.PrintForm1.Print()
        Me.Close()
    End Sub

    Private Sub Label24_Click(sender As Object, e As EventArgs) Handles Label24.Click

    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        'Me.Button1.Visible = False
        'Timer1.Enabled = False
        'Dim msgrta As Integer = 0
        'Me.Button1.Visible = False

        'msgrta = MsgBox("Desea Imprimir la Factura Ahora.", MsgBoxStyle.YesNo + vbQuestion)
        'If msgrta = 6 Then
        '    Button1_Click(Nothing, Nothing)
        '    Form_egreso.Close()
        '    Me.Close()
        'End If
        'If msgrta <> 6 Then

        'End If
    End Sub

    Private Sub Label36_Click(sender As Object, e As EventArgs) Handles Label36.Click

    End Sub
End Class