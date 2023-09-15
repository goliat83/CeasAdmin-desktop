
Imports MySql.Data.MySqlClient
Imports System.IO
Imports System.Data.SqlClient
Imports System.Data.Odbc
Imports iTextSharp.text.pdf
Imports iTextSharp.text

Public Class Formcursos_ver
    Dim itemCartera As String
    Dim fini, ffin As Date
    Dim timetotal As String
    Dim sumadias As Integer
    Dim elimino_pdftmp As Boolean = False
    Dim filtro_tipohraca As String = "PRACTICA"
    Dim filtro_tipohrarunt As String = "PRACTICA"
    Dim tipohora As String
    Dim doc_adjunto As Byte
    Dim totalhrs As Integer
    Dim ultimocontrato As Long
    Dim cursoanterior As String
    Dim prmt_vrservicio, prmt_vrmedico, prmt_costomedico, prmt_paq As Long

    Dim prmt_hpract, prmt_hteor, prmt_htaller As Integer

    Public Permisos As New Permisos()

    Dim id_clase As Long = 0
    Dim fecha_clase, hora_clase, estado_clase As String

    Dim DT_SERVS As DataTable
    Dim DA_SERVS As MySqlDataAdapter

    Dim DT_financiero As DataTable
    Dim DA_financiero As MySqlDataAdapter

    Dim DT_financiero2 As DataTable
    Dim DA_financiero2 As MySqlDataAdapter

    Dim DT_sicop As DataTable
    Dim DA_sicop As MySqlDataAdapter

    Dim DT_runt As DataTable
    Dim DA_runt As MySqlDataAdapter

    Dim DT_PAQ As DataTable
    Dim DA_PAQ As MySqlDataAdapter

    Dim DT_ASESORES As DataTable
    Dim DA_ASESORES As MySqlDataAdapter

    Dim DT_INSTRUCTORES As DataTable
    Dim DA_INSTRUCTORES As MySqlDataAdapter

    Dim DT_INSTRUCTORES2 As DataTable
    Dim DA_INSTRUCTORES2 As MySqlDataAdapter

    Dim DA_ConvMeds As MySqlDataAdapter
    Dim DT_ConvMeds As DataTable


    Public losAlumnos = New Alumnos

    Private Sub Formcursos_ver_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        actualiza_clases()

        If CInt(Me.LabelCurso.Text) < 2525 Then
            actualiza_financiero_old()
        Else
            actualiza_financiero()
        End If


        actualiza_docs()
    End Sub

    Private Sub Formcursos_ver_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing

        'Me.PictureBox1.Image.Dispose()
        Me.PictureBox1.Image = Nothing
        Application.DoEvents()
        If PictureBox1.ImageLocation <> Nothing Then
            Kill(PictureBox1.ImageLocation)
        End If
        My.Computer.FileSystem.CreateDirectory("C:\CEASadmin\pdf_tmp\")
        My.Computer.FileSystem.CreateDirectory("C:\CEASadmin\fotos_tmp\")
        If elimino_pdftmp = True Then
            For Each foundFile As String In My.Computer.FileSystem.GetFiles("C:\CEASadmin\pdf_tmp\", FileIO.SearchOption.SearchAllSubDirectories, "*.*")
                My.Computer.FileSystem.DeleteFile(foundFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            Next
            For Each foundFile As String In My.Computer.FileSystem.GetFiles("C:\CEASadmin\fotos_tmp\", FileIO.SearchOption.SearchAllSubDirectories, "*.*")
                My.Computer.FileSystem.DeleteFile(foundFile, FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently)
            Next
        End If
        Formprincipal.WindowState = FormWindowState.Maximized
    End Sub

    Private Sub Formcursos_ver_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.Cursor = Cursors.WaitCursor


        sql = "SELECT cod, servicio, cat FROM servicios WHERE  activo='SI'"

        DA_SERVS = New MySqlDataAdapter(sql, conex)
        DT_SERVS = New DataTable
        DA_SERVS.Fill(DT_SERVS)
        Me.ComboBoxServicio.DataSource = DT_SERVS.DefaultView
        Me.ComboBoxServicio.DisplayMember = "servicio"
        Me.ComboBoxServicio.ValueMember = "cod"


        Me.ComboBoxCat.DataSource = DT_SERVS.DefaultView
        Me.ComboBoxCat.DisplayMember = "cat"
        Me.ComboBoxCat.ValueMember = "cod"

        Dim topRow2 As DataRow = DT_SERVS.NewRow()
        topRow2("servicio") = ""
        DT_SERVS.Rows.InsertAt(topRow2, 0)

        DA_SERVS.Dispose()
        DT_SERVS.Dispose()
        conex.Close()
        Me.ComboBoxServicio.SelectedIndex = -1



        'cargamos combo paquetes clases

        sql = "SELECT parametros_paquetesclases.*, concat(clases,' CLASES') as lasclases FROM parametros_paquetesclases"

        DA_PAQ = New MySqlDataAdapter(sql, conex)
        DT_PAQ = New DataTable
        DA_PAQ.Fill(DT_PAQ)
        Me.ComboBoxPaquetes.DataSource = DT_PAQ.DefaultView
        Me.ComboBoxPaquetes.DisplayMember = "lasclases"
        Me.ComboBoxPaquetes.ValueMember = "id"

        Dim topRow3 As DataRow = DT_PAQ.NewRow()
        topRow3("lasclases") = "NO"
        DT_PAQ.Rows.InsertAt(topRow3, 0)

        DA_PAQ.Dispose()
        DT_PAQ.Dispose()
        conex.Close()
        Me.ComboBoxPaquetes.SelectedIndex = -1


        'cargamos combo MEDICOS 

        sql = "SELECT cod, CONCAT(convenio,'  $',valor) AS convenio, valor FROM convenios_medicos"

        DA_ConvMeds = New MySqlDataAdapter(sql, conex)
        DT_ConvMeds = New DataTable
        DA_ConvMeds.Fill(DT_ConvMeds)
        ComboBoxMedico.DataSource = DT_ConvMeds.DefaultView
        ComboBoxMedico.DisplayMember = "convenio"
        ComboBoxMedico.ValueMember = "cod"

        Dim topRow4 As DataRow = DT_ConvMeds.NewRow()
        topRow4("convenio") = "NO"
        DT_ConvMeds.Rows.InsertAt(topRow4, 0)

        DA_ConvMeds.Dispose()
        DT_ConvMeds.Dispose()
        conex.Close()



        'cargamos combo ASESORES 


        sql = "SELECT cod, nombre FROM asesores"

        DA_ASESORES = New MySqlDataAdapter(sql, conex)
        DT_ASESORES = New DataTable
        DA_ASESORES.Fill(DT_ASESORES)
        ComboBoxAsesor.DataSource = DT_ASESORES.DefaultView
        ComboBoxAsesor.DisplayMember = "nombre"
        ComboBoxAsesor.ValueMember = "cod"
        ComboBoxAsesor.AutoCompleteSource = AutoCompleteSource.ListItems
        ComboBoxAsesor.AutoCompleteMode = AutoCompleteMode.Append

        Dim topRow5 As DataRow = DT_ASESORES.NewRow()
        topRow5("nombre") = ""
        DT_ASESORES.Rows.InsertAt(topRow5, 0)

        DA_ASESORES.Dispose()
        DT_ASESORES.Dispose()
        conex.Close()


        'cargamos combo INSTRUCTORES 


        sql = "SELECT cod, nombre FROM instructores"

        DA_INSTRUCTORES = New MySqlDataAdapter(sql, conex)
        DT_INSTRUCTORES = New DataTable
        DA_INSTRUCTORES.Fill(DT_INSTRUCTORES)
        ComboBox_instructores.DataSource = DT_INSTRUCTORES.DefaultView
        ComboBox_instructores.DisplayMember = "nombre"
        ComboBox_instructores.ValueMember = "cod"
        ComboBox_instructores.AutoCompleteSource = AutoCompleteSource.ListItems
        ComboBox_instructores.AutoCompleteMode = AutoCompleteMode.Append

        Dim topRow6 As DataRow = DT_INSTRUCTORES.NewRow()
        topRow6("nombre") = ""
        topRow6("cod") = "0"
        DT_INSTRUCTORES.Rows.InsertAt(topRow6, 0)

        DA_INSTRUCTORES.Dispose()
        DT_INSTRUCTORES.Dispose()
        conex.Close()

        ComboBox_instructores.Text = ""




        'ComboBox_instructores cicop
        sql = "SELECT cod, nombre FROM instructores"

        DA_INSTRUCTORES2 = New MySqlDataAdapter(sql, conex)
        DT_INSTRUCTORES2 = New DataTable
        DA_INSTRUCTORES2.Fill(DT_INSTRUCTORES2)
        ComboBoxinstructorReporte.DataSource = DT_INSTRUCTORES2.DefaultView
        ComboBoxinstructorReporte.DisplayMember = "nombre"
        ComboBoxinstructorReporte.ValueMember = "cod"
        ComboBoxinstructorReporte.AutoCompleteSource = AutoCompleteSource.ListItems
        ComboBoxinstructorReporte.AutoCompleteMode = AutoCompleteMode.Append

        Dim topRow7 As DataRow = DT_INSTRUCTORES2.NewRow()
        topRow7("nombre") = ""
        topRow7("cod") = "0"
        DT_INSTRUCTORES2.Rows.InsertAt(topRow7, 0)

        DA_INSTRUCTORES2.Dispose()
        DT_INSTRUCTORES2.Dispose()
        conex.Close()

        ComboBoxinstructorReporte.Text = ""




        'cargamos datos del curso
        sql = "SELECT cr.*, concat(al.nombre1,' ',al.nombre2,' ',al.apellido1,' ',al.apellido2) as fullname 
               FROM cursos cr 
               LEFT JOIN alumnos al ON cr.alumno_doc = al.documento 
               WHERE cr.num='" & mycurso & "'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        For Each lin As DataRow In dt.Rows
            Me.LabelCurso.Text = lin.Item("num")
            Me.TextBox1.Text = lin.Item("alumno_doc")
            Me.ComboBox6.Text = lin.Item("alumno_tipodoc")
            Me.TextBox2.Text = lin.Item("fullname")
            Me.TextBox3.Text = lin.Item("estado")
            Me.ComboBoxServicio.SelectedValue = lin.Item("idServ")
            Me.TextBox10.Text = lin.Item("fecha")
            Me.TextBox13.Text = lin.Item("fechafin")
            Me.ComboBoxCat.Text = lin.Item("categoria")
            Me.ComboBoxMedico.SelectedValue = lin.Item("idmedico")
            Me.ComboBox5.Text = lin.Item("dertrans")
            Me.ComboBoxAsesor.SelectedValue = lin.Item("asesor")

            Me.TextBox_F_D_T.Text = lin.Item("fecha_dertran")

            Me.ComboBox_instructores.SelectedValue = lin.Item("instructor")
            Me.ComboBoxinstructorReporte.SelectedValue = lin.Item("instructor")

            Me.TextBox4.Text = lin.Item("observacion")
            Me.ComboBox7.Text = lin.Item("prioridad")
            Me.ComboBoxPaquetes.SelectedValue = lin.Item("idpaquete")
            Me.TextBox14.Text = lin.Item("alumno_cel")
            Me.TextBox15.Text = lin.Item("reg_runt") : If Me.TextBox15.Text = "" Then Me.TextBox15.Enabled = True
            Me.TextBox19.Text = lin.Item("f_reg_runt") : If Me.TextBox19.Text = "" Then Me.TextBox19.Enabled = True : Me.Button12.Visible = True
            Me.TextBox16.Text = lin.Item("cert_runt") : If Me.TextBox16.Text = "" Then Me.TextBox16.Enabled = True : Me.Button13.Visible = True
            Me.TextBox20.Text = lin.Item("f_cert_runt") : If Me.TextBox20.Text = "" Then Me.TextBox20.Enabled = True
            Me.TextBox17.Text = lin.Item("num_contrato")
            Me.ComboBoxGrupo.Text = lin.Item("grupo")
            Me.TextBox21.Text = lin.Item("autorizacionmedico")

            Me.TextBoxComision.Text = lin.Item("asesorcomision")

            'Me.TextBoxUtilidad.Text = lin.Item("utilidad")

            Me.TextBox11.Text = lin.Item("alerta1_titulo")
            Me.TextBox8.Text = lin.Item("alerta1_txt")
            Me.TextBox12.Text = lin.Item("alerta2_titulo")
            Me.TextBox9.Text = lin.Item("alerta2_txt")

            Me.TextBox22.Text = lin.Item("fechacomision")


            If lin.Item("estadosicov") <> "" Then
                Me.Button_Finish_sicop.Visible = False
            End If
            If lin.Item("estadorunt") <> "" Then
                Me.Button_Finish_runt.Visible = False
            End If
            If lin.Item("estadofinanciero") <> "" Then
                Me.Button7.Visible = False
            End If

        Next
        'Me.DataGridView1.DataSource = dt
        da.Dispose()
        dt.Dispose()
        conex.Close()

        Me.Cursor = Cursors.Default

    End Sub

    Private Sub Formcursos_ver_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        Me.Cursor = Cursors.WaitCursor


        actualiza_clases()
        'actualiza_horas()
        'actualiza_docs()

        If CInt(Me.LabelCurso.Text) < 2525 Then
            actualiza_financiero_old()
        Else
            actualiza_financiero()
        End If





        'SI YA TIENE ABONOS NO DEJA CAMBIAR UTILIDAD Y COMISION
        'If TextBox6.Text > 0 Then
        '    TextBoxComision.Enabled = False
        '    TextBoxUtilidad.Enabled = False
        'End If

        If Me.TextBox10.Text <> "" Then
            fini = Me.TextBox10.Text
            ffin = DateTime.Now().ToShortDateString
            sumadias = (DateDiff("s", fini, ffin) \ 86400) Mod 365
            timetotal = sumadias
            Me.Label12.Text = "Este alumno se matriculó hace  " & timetotal & "  días..."
            Me.Label12.Parent = PictureBox3
            If Me.TextBox13.Text <> "" Then Me.Label12.Visible = False
        End If


        If Me.TextBox3.Text = "GRADUADO" Then  'calculo tiempo que duro el tramite
            fini = Me.TextBox10.Text
            ffin = Me.TextBox13.Text
            sumadias = (DateDiff("s", fini, ffin) \ 86400) Mod 365
            timetotal = sumadias
            Me.Label12.Text = "El trámite de este alumno tardó  " & timetotal & "  días..."
            Me.Label12.Parent = PictureBox3
            If Me.TextBox13.Text <> "" Then Me.Label12.Visible = False
        End If
        Me.Cursor = Cursors.Default



        sql = "SELECT * FROM servicios_parametros WHERE cod='" & ComboBoxServicio.SelectedValue & "'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        For Each lin As DataRow In dt.Rows
            prmt_hpract = CInt(lin.Item("hpractica"))
            prmt_hteor = CInt(lin.Item("hteoria"))
            prmt_htaller = CInt(lin.Item("htaller"))

        Next
        da.Dispose()
        dt.Dispose()
        conex.Close()




        BunifuMaterialTextbox3.Text = prmt_hpract
        BunifuMaterialTextbox2.Text = prmt_hteor
        BunifuMaterialTextbox1.Text = prmt_hpract


        BunifuMaterialTextbox6.Text = prmt_hpract
        BunifuMaterialTextbox5.Text = prmt_hteor
        BunifuMaterialTextbox4.Text = prmt_hpract

        actualiza_sicop(Me.LabelCurso.Text)
        actualiza_runt(Me.LabelCurso.Text)


        ''BUSCO IMAGEN
        'Dim clavecurso_doc As Long
        'Dim archivo_ext As String
        'clavecurso_doc = CLng(Me.TextBox1.Text)
        ''archivo_ext = CStr(row.Cells("ext").Value)
        'Try
        '    Using conex
        '        ' recuperamos el documento de la base de datos y lo pasamos a un fichero
        '        Dim drDocumentos As MySqlDataReader
        '        Dim aBytDocumento() As Byte = Nothing
        '        Dim oFileStream As FileStream
        '        Dim loFila As DataGridViewRow = Me.DataGridView1.CurrentRow()
        '        Dim lsQuery As String = "Select IMAGEN, ext From ALUMNOS_FOTO Where DOCUMENTO=" & CStr(clavecurso_doc) & ""
        '        Using loComando As New MySqlCommand(lsQuery, conex)
        '            conex.Open()
        '            drDocumentos = loComando.ExecuteReader(CommandBehavior.SingleRow)
        '        End Using
        '        If drDocumentos.Read() Then
        '            aBytDocumento = CType(drDocumentos("IMAGEN"), Byte())
        '            archivo_ext = drDocumentos("ext")
        '            'End If
        '            drDocumentos.Close()
        '            oFileStream = New FileStream("C:\ceasadmin\fotos_tmp\" & "FOTO_" & CStr(clavecurso_doc) & archivo_ext, FileMode.Create, FileAccess.Write)
        '            oFileStream.Write(aBytDocumento, 0, aBytDocumento.Length)
        '            oFileStream.Close()
        '            Me.PictureBox1.Image = Image.FromFile("C:\ceasadmin\fotos_tmp\" & "FOTO_" & CStr(clavecurso_doc) & archivo_ext)
        '            elimino_pdftmp = True
        '            'MessageBox.Show("Documento generado con éxito", "Generar Documentos", MessageBoxButtons.OK, MessageBoxIcon.Information)
        '        End If
        '    End Using
        'Catch Exp As Exception
        '    MessageBox.Show(Exp.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information)
        'End Try
        Me.Cursor = Cursors.Default


        If losAlumnos.BuscarxDoc(TextBox1.Text) Then
            TextBox18.Text = losAlumnos.email

        End If

    End Sub

    Private Sub actualiza_financiero()

        ''cargamos datos financieros del curso
        sql = "SELECT id, concepto, descripcion, valor, estado
               From cartera WHERE curso='" & LabelCurso.Text & "'"
        DA_financiero = New MySqlDataAdapter(sql, conex)
        DT_financiero = New DataTable
        DA_financiero.Fill(DT_financiero)
        For Each lin As DataRow In DT_financiero.Rows
            'Me.Label3.Text = lin.Item("num")
            'Me.TextBox1.Text = lin.Item("alumno_doc")
        Next
        Me.DataGridView1.DataSource = DT_financiero
        Me.DataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView1.Columns(0).Visible = False
        Me.DataGridView1.Columns(1).HeaderText = "Concepto"
        Me.DataGridView1.Columns(1).Width = 100
        Me.DataGridView1.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView1.Columns(2).HeaderText = "Descripción"
        Me.DataGridView1.Columns(2).Width = 230
        Me.DataGridView1.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Me.DataGridView1.Columns(3).HeaderText = "Valor"
        Me.DataGridView1.Columns(3).Width = 110
        Me.DataGridView1.Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        Me.DataGridView1.Columns(4).HeaderText = "Estado"
        Me.DataGridView1.Columns(4).Width = 110
        Me.DataGridView1.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        Me.DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue
        Me.DataGridView1.DefaultCellStyle.BackColor = Color.AliceBlue
        Me.DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        Me.DataGridView1.EnableHeadersVisualStyles = False
        Me.DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

        DA_financiero.Dispose()
        DT_financiero.Dispose()
        conex.Close()


        Dim derecho_tr As String

        Dim valorinicial, valor, abono, descuento, saldo, comision As Long
        Try
            For i As Integer = 0 To DataGridView1.RowCount - 1
                If DataGridView1.Item("concepto", i).Value = "DERECHOS TRANSITO" Then
                    derecho_tr = CLng(DataGridView1.Item("valor", i).Value)
                End If

                If DataGridView1.Item("concepto", i).Value = "CERTIFICADO" Then
                    valorinicial = CLng(DataGridView1.Item("valor", i).Value)
                End If

                If DataGridView1.Item("concepto", i).Value = "COMISION" Then
                    comision = CLng(DataGridView1.Item("valor", i).Value)
                End If

                If DataGridView1.Item("concepto", i).Value <> "CERTIFICADO" And DataGridView1.Item("concepto", i).Value <> "COMISION" And DataGridView1.Item("concepto", i).Value <> "DESCUENTO" Then
                    valor = valor + CLng(DataGridView1.Item("valor", i).Value)
                End If
                'abono = abono + CLng(DataGridView1.Item("abono", i).Value)
                If DataGridView1.Item("concepto", i).Value = "DESCUENTO" Then
                    descuento = descuento + CLng(DataGridView1.Item("valor", i).Value)
                End If
                'saldo = CLng(DataGridView1.Item("saldo", i).Value)
            Next
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try



        sql = "Select SUM(valor) As abonos from recibos_caja where curso='" & LabelCurso.Text & "' AND estado<>'ANULADO'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        For Each lin As DataRow In dt.Rows
            If Not IsDBNull(lin.Item("abonos")) Then
                abono = lin.Item("abonos")
            End If
        Next
        da.Dispose()
        dt.Dispose()
        conex.Close()



        sql = "SELECT valor FROM servicios WHERE cod='" & ComboBoxServicio.SelectedValue & "'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        For Each lin As DataRow In dt.Rows
            Me.TextBoxSaldoInicial.Text = CInt(lin.Item("valor"))
        Next
        da.Dispose()
        dt.Dispose()
        conex.Close()


        If derecho_tr = "NO" Or derecho_tr = "" Then
            Me.ComboBox1.Text = "NO"
        Else
            Me.ComboBox1.Text = derecho_tr
        End If


        Me.TextBox_gastosop.Text = valor

        Me.TextBox_gastosop.Text = valor

        Me.TextBox6.Text = abono
        TextBoxDescuento.Text = descuento

        saldo = CInt(Me.TextBoxSaldoInicial.Text) - CInt(abono) - CInt(descuento) - CInt(comision)
        Me.TextBox7.Text = saldo

        TextBoxUtilidad.Text = CInt(Me.TextBoxSaldoInicial.Text) - CInt(valor) - CInt(descuento) - CInt(comision)

        ''cargamos datos financieros del curso
        sql = "SELECT * from recibos_caja WHERE curso='" & LabelCurso.Text & "'"
        DA_financiero2 = New MySqlDataAdapter(sql, conex)
        DT_financiero2 = New DataTable
        DA_financiero2.Fill(DT_financiero2)
        For Each lin As DataRow In DT_financiero2.Rows
            'Me.Label3.Text = lin.Item("num")
            'Me.TextBox1.Text = lin.Item("alumno_doc")
        Next
        Me.DataGridView6.DataSource = DT_financiero2
        Me.DataGridView6.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView6.Columns(2).Visible = False
        Me.DataGridView6.Columns(3).Visible = False
        Me.DataGridView6.Columns(4).Visible = False
        Me.DataGridView6.Columns(5).Visible = False
        Me.DataGridView6.Columns(6).Visible = False
        Me.DataGridView6.Columns(12).Visible = False
        Me.DataGridView6.Columns(14).Visible = False



        Me.DataGridView6.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue
        Me.DataGridView6.DefaultCellStyle.BackColor = Color.AliceBlue
        Me.DataGridView6.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        Me.DataGridView6.EnableHeadersVisualStyles = False
        Me.DataGridView6.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

        DA_financiero2.Dispose()
        DT_financiero2.Dispose()
        conex.Close()
    End Sub

    Private Sub actualiza_financiero_old()

        ''cargamos datos financieros del curso
        sql = "SELECT id, concepto, descripcion, valor, estado
               From cartera WHERE curso='" & LabelCurso.Text & "'"
        DA_financiero = New MySqlDataAdapter(sql, conex)
        DT_financiero = New DataTable
        DA_financiero.Fill(DT_financiero)
        For Each lin As DataRow In DT_financiero.Rows
            'Me.Label3.Text = lin.Item("num")
            'Me.TextBox1.Text = lin.Item("alumno_doc")
        Next
        Me.DataGridView1.DataSource = DT_financiero
        Me.DataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView1.Columns(0).Visible = False
        Me.DataGridView1.Columns(1).HeaderText = "Concepto"
        Me.DataGridView1.Columns(1).Width = 100
        Me.DataGridView1.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView1.Columns(2).HeaderText = "Descripción"
        Me.DataGridView1.Columns(2).Width = 230
        Me.DataGridView1.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Me.DataGridView1.Columns(3).HeaderText = "Valor"
        Me.DataGridView1.Columns(3).Width = 110
        Me.DataGridView1.Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        Me.DataGridView1.Columns(4).HeaderText = "Estado"
        Me.DataGridView1.Columns(4).Width = 110
        Me.DataGridView1.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        Me.DataGridView1.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue
        Me.DataGridView1.DefaultCellStyle.BackColor = Color.AliceBlue
        Me.DataGridView1.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        Me.DataGridView1.EnableHeadersVisualStyles = False
        Me.DataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

        DA_financiero.Dispose()
        DT_financiero.Dispose()
        conex.Close()

        Dim valor, abono, descuento, saldo As Long
        Try
            For i As Integer = 0 To DataGridView1.RowCount - 1
                valor = valor + CLng(DataGridView1.Item("valor", i).Value)
                'abono = abono + CLng(DataGridView1.Item("abono", i).Value)
                If DataGridView1.Item("concepto", i).Value = "DESCUENTO" Then
                    descuento = descuento + CLng(DataGridView1.Item("valor", i).Value)
                End If
                'saldo = CLng(DataGridView1.Item("saldo", i).Value)
            Next
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try



        sql = "Select SUM(valor) As abonos from recibos_caja where curso='" & LabelCurso.Text & "' and estado<>'ANULADO'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        For Each lin As DataRow In dt.Rows
            If Not IsDBNull(lin.Item("abonos")) Then
                abono = lin.Item("abonos")
            End If
        Next
        da.Dispose()
        dt.Dispose()
        conex.Close()


        Me.TextBoxSaldoInicial.Text = valor
        Me.TextBox6.Text = abono
        TextBoxDescuento.Text = descuento

        saldo = CInt(valor) - CInt(abono) - CInt(descuento)
        Me.TextBox7.Text = saldo





        ''cargamos datos financieros del curso
        sql = "SELECT * from recibos_caja WHERE curso='" & LabelCurso.Text & "'"
        DA_financiero2 = New MySqlDataAdapter(sql, conex)
        DT_financiero2 = New DataTable
        DA_financiero2.Fill(DT_financiero2)
        For Each lin As DataRow In DT_financiero2.Rows
            'Me.Label3.Text = lin.Item("num")
            'Me.TextBox1.Text = lin.Item("alumno_doc")
        Next
        Me.DataGridView6.DataSource = DT_financiero2
        Me.DataGridView6.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView6.Columns(2).Visible = False
        Me.DataGridView6.Columns(3).Visible = False
        Me.DataGridView6.Columns(4).Visible = False
        Me.DataGridView6.Columns(5).Visible = False
        Me.DataGridView6.Columns(6).Visible = False
        Me.DataGridView6.Columns(12).Visible = False
        Me.DataGridView6.Columns(14).Visible = False

        'Me.DataGridView5.Columns(1).HeaderText = "Concepto"
        'Me.DataGridView5.Columns(1).Width = 100
        'Me.DataGridView5.Columns(1).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        'Me.DataGridView5.Columns(2).HeaderText = "Descripción"
        'Me.DataGridView5.Columns(2).Width = 230
        'Me.DataGridView5.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        'Me.DataGridView5.Columns(3).HeaderText = "Valor"
        'Me.DataGridView5.Columns(3).Width = 110
        'Me.DataGridView5.Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight
        'Me.DataGridView5.Columns(4).HeaderText = "Estado"
        'Me.DataGridView5.Columns(4).Width = 110
        'Me.DataGridView5.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight

        Me.DataGridView6.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue
        Me.DataGridView6.DefaultCellStyle.BackColor = Color.AliceBlue
        Me.DataGridView6.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
        Me.DataGridView6.EnableHeadersVisualStyles = False
        Me.DataGridView6.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells

        DA_financiero2.Dispose()
        DT_financiero2.Dispose()
        conex.Close()
    End Sub

    Private Sub actualiza_sicop(curso)
        ''cargamos datos financieros del curso
        sql = "SELECT 
    cicop.*, a.* , SUM(cicop.hteoria) as thteoria, SUM(cicop.htaller) as thtaller, SUM(cicop.hpractica) as thpractica
    FROM horas_cicop cicop
    inner join alumnos a 
        on cicop.alumno=a.documento 
    inner join cursos c
        on cicop.curso=c.num
    inner join cursos cur 
        on cicop.curso = cur.num
    where cicop.curso = " & curso & "
    group by cicop.id"
        DA_sicop = New MySqlDataAdapter(sql, conex)
        DT_sicop = New DataTable
        DA_sicop.Fill(DT_sicop)
        For Each lin As DataRow In DT_sicop.Rows
            'Me.Label3.Text = lin.Item("num")
            'Me.TextBox1.Text = lin.Item("alumno_doc")
        Next
        Me.DataGridView5.DataSource = DT_sicop

        DA_sicop.Dispose()
        DT_sicop.Dispose()
        conex.Close()



        Me.DataGridView5.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView5.Columns(0).Visible = False
            Me.DataGridView5.Columns(1).Visible = False
            Me.DataGridView5.Columns(2).Visible = False
            Me.DataGridView5.Columns(3).Visible = False
            Me.DataGridView5.Columns(4).Visible = False

            'Me.DataGridView5.Columns(5).Width = 100
            Me.DataGridView5.Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView5.Columns(5).HeaderText = "FECHA"

            Me.DataGridView5.Columns(6).Visible = False
            Me.DataGridView5.Columns(7).Visible = False
            Me.DataGridView5.Columns(8).Visible = False
            'Me.DataGridView5.Columns(9).Width = 100
            Me.DataGridView5.Columns(9).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView5.Columns(9).HeaderText = "TEORIA"
            'Me.DataGridView5.Columns(10).Width = 100
            Me.DataGridView5.Columns(10).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView5.Columns(10).HeaderText = "TALLER"
            'Me.DataGridView5.Columns(11).Width = 100
            Me.DataGridView5.Columns(11).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView5.Columns(11).HeaderText = "PRACTICA"
            Me.DataGridView5.Columns(12).Visible = False
            Me.DataGridView5.Columns(13).Visible = False
            Me.DataGridView5.Columns(14).Visible = False
            Me.DataGridView5.Columns(15).Visible = False
            Me.DataGridView5.Columns(16).Visible = False
            Me.DataGridView5.Columns(17).Visible = False
            Me.DataGridView5.Columns(18).Visible = False
            Me.DataGridView5.Columns(19).Visible = False
            Me.DataGridView5.Columns(20).Visible = False
            Me.DataGridView5.Columns(21).Visible = False
            Me.DataGridView5.Columns(22).Visible = False
            Me.DataGridView5.Columns(23).Visible = False
            Me.DataGridView5.Columns(24).Visible = False
            Me.DataGridView5.Columns(25).Visible = False
            Me.DataGridView5.Columns(26).Visible = False
            Me.DataGridView5.Columns(27).Visible = False
            Me.DataGridView5.Columns(28).Visible = False
            Me.DataGridView5.Columns(29).Visible = False
            Me.DataGridView5.Columns(30).Visible = False
            Me.DataGridView5.Columns(31).Visible = False
            Me.DataGridView5.Columns(32).Visible = False
            Me.DataGridView5.Columns(33).Visible = False
            Me.DataGridView5.Columns(34).Visible = False
            Me.DataGridView5.Columns(35).Visible = False
            Me.DataGridView5.Columns(36).Visible = False
            Me.DataGridView5.Columns(37).Visible = False
            Me.DataGridView5.Columns(38).Visible = False
            Me.DataGridView5.Columns(39).Visible = False
            Me.DataGridView5.Columns(40).Visible = False

            Me.DataGridView5.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue
            Me.DataGridView5.DefaultCellStyle.BackColor = Color.AliceBlue
            Me.DataGridView5.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            Me.DataGridView5.EnableHeadersVisualStyles = False
            Me.DataGridView5.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells





            Dim HTERORIA As Long = 0, HTALLER As Long = 0, HPRACTICA As Long = 0

        Textbox_htaller.Text = 0
        Textbox_hteoria.Text = 0
        Textbox_hpractica.Text = 0


        Try
            For i As Integer = 0 To DataGridView5.RowCount - 1

                HTERORIA = HTERORIA + CLng(DataGridView5.Item("hteoria", i).Value)
                HTALLER = HTALLER + CLng(DataGridView5.Item("htaller", i).Value)
                HPRACTICA = HPRACTICA + CLng(DataGridView5.Item("hpractica", i).Value)

            Next
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try


        Textbox_htaller.Text = HTALLER
        Textbox_hteoria.Text = HTERORIA
        Textbox_hpractica.Text = HPRACTICA

        Textbox_hpractica.BackColor = Color.Orange
        Textbox_hteoria.BackColor = Color.Orange
        Textbox_htaller.BackColor = Color.Orange

        If prmt_hpract = Textbox_hpractica.Text Then Textbox_hpractica.BackColor = Color.LightGreen
        If prmt_hteor = Textbox_hteoria.Text Then Textbox_hteoria.BackColor = Color.LightGreen
        If prmt_htaller = Textbox_htaller.Text Then Textbox_htaller.BackColor = Color.LightGreen

    End Sub

    Private Sub actualiza_runt(curso)
        ''cargamos datos financieros del curso
        sql = "SELECT 
    runt.*, a.* , SUM(runt.hteoria) as thteoria, SUM(runt.htaller) as thtaller, SUM(runt.hpractica) as thpractica
    FROM horas_runt runt
    inner join alumnos a 
        on runt.alumno=a.documento 
    inner join cursos c
        on runt.curso=c.num
    inner join cursos cur 
        on runt.curso = cur.num
    where runt.curso = " & curso & "
    group by runt.id"
        DA_runt = New MySqlDataAdapter(sql, conex)
        DT_runt = New DataTable
        DA_runt.Fill(DT_runt)
        For Each lin As DataRow In DT_runt.Rows
            'Me.Label3.Text = lin.Item("num")
            'Me.TextBox1.Text = lin.Item("alumno_doc")
        Next

        Me.DataGridView2.DataSource = DT_runt
        DA_runt.Dispose()
        DT_runt.Dispose()
        conex.Close()


        Me.DataGridView2.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView2.Columns(0).Visible = False
            Me.DataGridView2.Columns(1).Visible = False
            Me.DataGridView2.Columns(2).Visible = False
            Me.DataGridView2.Columns(3).Visible = False
            Me.DataGridView2.Columns(4).Visible = False

            'Me.DataGridView2.Columns(5).Width = 100
            Me.DataGridView2.Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView2.Columns(5).HeaderText = "FECHA"

            Me.DataGridView2.Columns(6).Visible = False
            Me.DataGridView2.Columns(7).Visible = False
            Me.DataGridView2.Columns(8).Visible = False
            'Me.DataGridView2.Columns(9).Width = 100
            Me.DataGridView2.Columns(9).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView2.Columns(9).HeaderText = "TEORIA"
            'Me.DataGridView2.Columns(10).Width = 100
            Me.DataGridView2.Columns(10).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView2.Columns(10).HeaderText = "TALLER"
            'Me.DataGridView2.Columns(11).Width = 100
            Me.DataGridView2.Columns(11).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView2.Columns(11).HeaderText = "PRACTICA"
            Me.DataGridView2.Columns(12).Visible = False
            Me.DataGridView2.Columns(13).Visible = False
            Me.DataGridView2.Columns(14).Visible = False
            Me.DataGridView2.Columns(15).Visible = False
            Me.DataGridView2.Columns(16).Visible = False
            Me.DataGridView2.Columns(17).Visible = False
            Me.DataGridView2.Columns(18).Visible = False
            Me.DataGridView2.Columns(19).Visible = False
            Me.DataGridView2.Columns(20).Visible = False
            Me.DataGridView2.Columns(21).Visible = False
            Me.DataGridView2.Columns(22).Visible = False
            Me.DataGridView2.Columns(23).Visible = False
            Me.DataGridView2.Columns(24).Visible = False
            Me.DataGridView2.Columns(25).Visible = False
            Me.DataGridView2.Columns(26).Visible = False
            Me.DataGridView2.Columns(27).Visible = False
            Me.DataGridView2.Columns(28).Visible = False
            Me.DataGridView2.Columns(29).Visible = False
            Me.DataGridView2.Columns(30).Visible = False
            Me.DataGridView2.Columns(31).Visible = False
            Me.DataGridView2.Columns(32).Visible = False
            Me.DataGridView2.Columns(33).Visible = False
            Me.DataGridView2.Columns(34).Visible = False
            Me.DataGridView2.Columns(35).Visible = False
            Me.DataGridView2.Columns(36).Visible = False
            Me.DataGridView2.Columns(37).Visible = False
            Me.DataGridView2.Columns(38).Visible = False
            Me.DataGridView2.Columns(39).Visible = False
            Me.DataGridView2.Columns(40).Visible = False

            Me.DataGridView2.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue
            Me.DataGridView2.DefaultCellStyle.BackColor = Color.AliceBlue
            Me.DataGridView2.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            Me.DataGridView2.EnableHeadersVisualStyles = False
            Me.DataGridView2.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells






        Dim HTERORIA As Long = 0, HTALLER As Long = 0, HPRACTICA As Long = 0

        Textbox_htallerR.Text = 0
        Textbox_hteoriaR.Text = 0
        Textbox_hpracticaR.Text = 0


        Try
            For i As Integer = 0 To DataGridView2.RowCount - 1

                HTERORIA = HTERORIA + CLng(DataGridView2.Item("hteoria", i).Value)
                HTALLER = HTALLER + CLng(DataGridView2.Item("htaller", i).Value)
                HPRACTICA = HPRACTICA + CLng(DataGridView2.Item("hpractica", i).Value)

            Next
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try


        Textbox_htallerR.Text = HTALLER
        Textbox_hteoriaR.Text = HTERORIA
        Textbox_hpracticaR.Text = HPRACTICA

        Textbox_hpracticaR.BackColor = Color.Orange
        Textbox_hteoriaR.BackColor = Color.Orange
        Textbox_htallerR.BackColor = Color.Orange

        If prmt_hpract = Textbox_hpracticaR.Text Then Textbox_hpracticaR.BackColor = Color.LightGreen
        If prmt_hteor = Textbox_hteoriaR.Text Then Textbox_hteoriaR.BackColor = Color.LightGreen
        If prmt_htaller = Textbox_htallerR.Text Then Textbox_htallerR.BackColor = Color.LightGreen

    End Sub

    Private Sub actualiza_clases()
        Dim clases_req As Long
        Dim clases_acum As Long = 0

        sql = "SELECT * FROM parametros_paquetesclases WHERE id='" & Strings.Left(Me.ComboBoxPaquetes.Text, 3) & "'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        'Me.DataGridView3.DataSource = dt

        For Each lin As DataRow In dt.Rows
            clases_req = lin.Item("clases")

        Next
        da.Dispose()
        dt.Dispose()
        conex.Close()

        mycurso = CLng(Me.LabelCurso.Text)


        sql = "SELECT * FROM cursos_clases WHERE curso=" & mycurso & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        Me.DataGridView3.DataSource = dt

        da.Dispose()
        dt.Dispose()
        conex.Close()

        Try

            DataGridView3.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            DataGridView3.Columns(0).Visible = False
            DataGridView3.Columns(1).Visible = False
            DataGridView3.Columns(2).HeaderText = "FECHA"
            DataGridView3.Columns(2).Width = 80
            Me.DataGridView3.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView3.Columns(3).HeaderText = "HORARIO"
            Me.DataGridView3.Columns(3).Width = 80
            Me.DataGridView3.Columns(3).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
            Me.DataGridView3.Columns(4).Visible = False
            Me.DataGridView3.Columns(5).HeaderText = "INSTRUCTOR"
            Me.DataGridView3.Columns(5).Width = 300
            Me.DataGridView3.Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            Me.DataGridView3.Columns(6).HeaderText = "VEHICULO"
            Me.DataGridView3.Columns(6).Width = 200
            Me.DataGridView3.Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
            Me.DataGridView3.Columns(7).HeaderText = "ESTADO"
            Me.DataGridView3.Columns(7).Width = 100
            Me.DataGridView3.Columns(7).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter

            Me.DataGridView3.ColumnHeadersDefaultCellStyle.BackColor = Color.SteelBlue
            Me.DataGridView3.DefaultCellStyle.BackColor = Color.AliceBlue
            Me.DataGridView3.ColumnHeadersDefaultCellStyle.ForeColor = Color.White
            Me.DataGridView3.EnableHeadersVisualStyles = False
            TextboxClasesCumplidas.Text = 0
            Try
                For i As Integer = 0 To DataGridView3.RowCount - 1

                    If DataGridView3.Item("estado", i).Value = "CUMPLIDA" Then
                        clases_acum = clases_acum + 1
                    End If
                Next
            Catch ex As Exception
                MsgBox(ex.Message.ToString)
            End Try
        Catch ex As Exception

        End Try

        TextboxClasesCumplidas.Text = clases_acum


        DataGridView3.ClearSelection()

    End Sub
    Private Sub actualiza_horas()
        'sql = "SELECT * FROM cargues_runt WHERE curso=" & CLng(Me.Label3.Text) & ""
        'da = New MySqlDataAdapter(sql, conex)
        'dt = New DataTable
        'da.Fill(dt)
        'Me.DataGridView2.DataSource = dt

        'Try
        '    For i As Integer = 0 To DataGridView2.RowCount - 1

        '        totalhrs = totalhrs + DataGridView2.Item("total", i).Value
        '    Next

        '    Me.TextBox9.Text = CStr(totalhrs)

        'Catch ex As Exception
        '    MsgBox(ex.Message.ToString)
        'End Try

        'da.Dispose()
        'dt.Dispose()
        'conex.Close()
    End Sub
    Private Sub actualiza_docs()
        sql = "SELECT * FROM cursos_docs where curso=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)

        Me.DataGridView4.DataSource = dt

        Me.DataGridView4.ColumnHeadersVisible = False
        Me.DataGridView4.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView4.Columns(0).Visible = False
        Me.DataGridView4.Columns(1).Visible = False
        Me.DataGridView4.Columns(2).Width = 590
        Me.DataGridView4.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView4.Columns(3).Visible = False
        Me.DataGridView4.Columns(4).Visible = False
        Me.DataGridView4.Columns(5).Visible = False

        da.Dispose()
        dt.Dispose()
        conex.Close()

    End Sub


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click

        Formcursos_verprogclase.Show()

    End Sub



    Private Sub Label30_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBox3_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox3.KeyPress
        e.KeyChar = ""
    End Sub



    Private Sub TextBox3_TextChanged(sender As Object, e As EventArgs) Handles TextBox3.TextChanged
        If Me.TextBox3.Text = "ANULADO" Then Me.TextBox3.ForeColor = Color.Red
    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        Formcursos_veradjuntar.Show()

    End Sub

    Private Sub DataGridView4_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView4.CellClick
        Dim row As DataGridViewRow = DataGridView4.CurrentRow
        'Me.ComboBox1.Text = CStr(row.Cells("tipo").Value)

    End Sub
    Private Sub DataGridView4_CellDoubleClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView4.CellDoubleClick
        Dim row As DataGridViewRow = DataGridView4.CurrentRow
        Dim clavecurso_doc As Long
        Dim tipocurso, archivo_ext As String
        clavecurso_doc = CLng(row.Cells("id").Value)
        tipocurso = CStr(row.Cells("tipo").Value)
        archivo_ext = CStr(row.Cells("ext").Value)
        Try
            Using conex
                ' recuperamos el documento de la base de datos y lo pasamos a un fichero
                Dim drDocumentos As MySqlDataReader
                Dim aBytDocumento() As Byte = Nothing
                Dim oFileStream As FileStream
                Dim loFila As DataGridViewRow = Me.DataGridView4.CurrentRow()
                Dim lsQuery As String = "Select archivo, ext From cursos_docs Where id=" & clavecurso_doc & ""
                Using loComando As New MySqlCommand(lsQuery, conex)
                    conex.Open()
                    drDocumentos = loComando.ExecuteReader(CommandBehavior.SingleRow)
                End Using
                If drDocumentos.Read() Then
                    aBytDocumento = CType(drDocumentos("archivo"), Byte())
                    archivo_ext = drDocumentos("ext")
                    'End If
                    drDocumentos.Close()
                    oFileStream = New FileStream("C:\CEASadmin\pdf_tmp\" & tipocurso & "_" & Me.TextBox1.Text & archivo_ext, FileMode.CreateNew, FileAccess.Write)
                    oFileStream.Write(aBytDocumento, 0, aBytDocumento.Length)
                    oFileStream.Close()
                    'MessageBox.Show("Documento generado con éxito", "Generar Documentos", MessageBoxButtons.OK, MessageBoxIcon.Information)
                End If
            End Using
        Catch Exp As Exception
            MessageBox.Show(Exp.Message, "", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End Try
        Process.Start("C:\CEASadmin\pdf_tmp\" & tipocurso & "_" & Me.TextBox1.Text & archivo_ext)
        elimino_pdftmp = True
    End Sub

    Private Sub DataGridView4_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView4.CellContentClick

    End Sub




    Private Sub Label3_Click(sender As Object, e As EventArgs) Handles LabelCurso.Click

    End Sub

    Private Sub DataGridView3_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView3.CellClick
        Dim row As DataGridViewRow = DataGridView3.CurrentRow
        id_clase = CLng(row.Cells("id").Value)
        fecha_clase = CStr(row.Cells("fecha").Value)
        hora_clase = CStr(row.Cells("hora").Value)
        estado_clase = CStr(row.Cells("estado").Value)

    End Sub




    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        If id_clase = 0 Then MsgBox("Seleccione una clase para cancelar.", vbInformation) : Exit Sub
        If estado_clase <> "PROGRAMADA" Then MsgBox("El estado de la clase no permite cancelar la clase", vbExclamation) : Exit Sub


        Dim msgrta As Integer = 0
        msgrta = MsgBox("Está seguro que desea cancelar la siguiente clase" & Chr(13) & "Dia: " & fecha_clase & "  en el horario de:" & hora_clase & "", MsgBoxStyle.YesNo + vbCritical)
        If msgrta = 6 Then
            sql = "UPDATE cursos_clases SET estado='" & "CANCELADA" & "', instructor='" & "" & "', placa='" & "" & "' WHERE id=" & id_clase & ""
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
                MsgBox("Se Cancelo la clase.", vbInformation)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        End If

        'guardamos horario general
        sql = "UPDATE horario_general SET estado='" & "CANCELADA" & "', instructor='" & "" & "', vehiculo='" & "" & "' WHERE curso='" & Me.LabelCurso.Text & "' AND hora='" & hora_clase & "' AND fecha='" & fecha_clase & "'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()

        curso_log(CLng(Me.LabelCurso.Text), DateTime.Now().ToString, usrnom.ToString, "Se canceló la clase: " & fecha_clase & " " & hora_clase)


    End Sub

    Private Sub Button6_Click(sender As Object, e As EventArgs) Handles Button6.Click
        If id_clase = 0 Then MsgBox("Seleccione una clase para reportar la asitencia.", vbInformation) : Exit Sub
        If estado_clase <> "PROGRAMADA" Then MsgBox("El estado de la clase no permite reportar aistencia", vbExclamation) : Exit Sub

        Dim msgrta As Integer = 0
        msgrta = MsgBox("Está seguro que desea reportar la asistencia a la siguiente clase" & Chr(13) & "Dia: " & fecha_clase & "  en el horario de:" & hora_clase & "", MsgBoxStyle.YesNo + vbCritical)
        If msgrta = 6 Then
            sql = "UPDATE cursos_clases SET estado='" & "CUMPLIDA" & "' WHERE id=" & id_clase & ""
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
                MsgBox("Se reportó la sistencia a la clase.", vbInformation)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()

            'guardamos horario general
            sql = "UPDATE horario_general SET estado='" & "CUMPLIDA" & "' WHERE curso='" & Me.LabelCurso.Text & "' AND hora='" & hora_clase & "' AND fecha='" & fecha_clase & "'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()

            curso_log(CLng(Me.LabelCurso.Text), DateTime.Now().ToString, usrnom.ToString, "Se reportó asistencia a clase: " & fecha_clase & " " & hora_clase)

        End If
    End Sub

    Private Sub TextBox1_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox1.KeyPress
        e.KeyChar = ""
    End Sub

    Private Sub TextBox2_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox2.KeyPress
        e.KeyChar = ""
    End Sub
    Private Sub TextBox14_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox14.KeyPress
        e.KeyChar = ""
    End Sub

    Private Sub Button9_Click(sender As Object, e As EventArgs) Handles Button9.Click
        Try
            Permisos.getPermiso("3", usrdoc)
            If Permisos.idpermiso = "" Then
                MsgBox("Acceso no disponible. consulte al Admistrador")
                Exit Sub
            End If

        Catch ex As Exception

        End Try



        Dim VR_CONTRATO As String = ""
        Select Case ComboBoxCat.Text
            Case "A1"
                VR_CONTRATO = "$ 370.000"
            Case "A2"
                VR_CONTRATO = "$ 470.000"
            Case "B1"
                VR_CONTRATO = "$ 660.000"
            Case "B2"
                VR_CONTRATO = "$ 1.000.000"
            Case "C1"
                VR_CONTRATO = "$ 860.000"
            Case "C2"
                VR_CONTRATO = "$ 1.250.000"
            Case "RC1"
                VR_CONTRATO = "$ 470.000"
        End Select




        Dim msgrta As Integer = 0
        If TextBox17.Text = "" Then msgrta = MsgBox("CONFIRMA QUE DESEA GENERAR EL CONTRATO.", MsgBoxStyle.YesNo + vbQuestion)
        If TextBox17.Text <> "" Then msgrta = MsgBox("CONFIRMA QUE VER  EL CONTRATO.", MsgBoxStyle.YesNo + vbQuestion)
        If msgrta = 6 Then
            Me.Cursor = Cursors.WaitCursor


            ' SI NO HAY CONTRATO LO GENERA NUEVO
            If TextBox17.Text = "" Then
                ' calculo numero de contrato
                cmd.Connection = conex
                cmd.CommandType = CommandType.Text
                cmd.CommandText = "select id from contratos ORDER BY id DESC"

                conex.Open()
                Try
                    dr = cmd.ExecuteReader()
                    dr.Read()
                    ultimocontrato = CLng(dr(0))
                Catch ex As Exception
                    ultimocontrato = 0
                End Try
                conex.Close()

                Me.TextBox17.Text = ultimocontrato + 1

                'guardo contrato en contratos
                sql = "INSERT INTO contratos (fecha, curso, alumnodoc, alumnonom)" &
                      "VALUES ('" & Me.TextBox10.Text & "','" & Me.LabelCurso.Text.ToString & "','" & Me.TextBox1.Text.ToString & "','" & Me.TextBox2.Text.ToString & "')"
                da = New MySqlDataAdapter(sql, conex)
                dt = New DataTable
                Try
                    da.Fill(dt)

                    ' MsgBox("Se Guardaron los datos del Cliente. ")
                Catch ex As Exception
                    If ex.ToString.Contains("Duplicate entry") Then MsgBox("Ya existe Intructor registrado con ese código, verifique los datos.", vbExclamation)
                    MsgBox(ex.ToString)
                End Try
                da.Dispose()
                dt.Dispose()
                conex.Close()
            End If

            ' Y SI HAY CONTRATO GENERA E IMPRIME CON EL MISMO NUMERO
            If TextBox17.Text <> "" Then
                ultimocontrato = Me.TextBox17.Text
            End If

            'guarda el numero del contrato en la tabla cursos
            Button4_Click(Nothing, Nothing)





            generarcontrato()
            'Form_contrato2.Show()
            elimino_pdftmp = True
            Me.Cursor = Cursors.Default


            Try
                'Intentar generar el documento.
                Dim pgSize = New iTextSharp.text.Rectangle(201, 350)
                Dim doc As New Document(PageSize.A4, 20, 20, 20, 20)

                'Path que guarda el reporte en el escritorio de windows (Desktop).
                Dim filename As String = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\ceasadmin\CONTRATO-" & TextBox17.Text & ".pdf"
                Dim file As New FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
                PdfWriter.GetInstance(doc, file)
                doc.Open()
                ExportarContrato(doc)
                doc.Close()
                'Process.Start(filename)
            Catch ex As Exception
                Me.Cursor = Cursors.Default
                MessageBox.Show("error al imprimir CONTRATO", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End Try
            Dim instance As New Printing.PrinterSettings
            Dim impresosaPredt As String = instance.PrinterName
            'Process.Start("C:\Program Files\Tracker Software\PDF Viewer\pdfxcview.exe", " /print:printer=""" & impresosaPredt & """ """ & Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\ceasadmin\Recibo de Caja " & Label_consecutivo.Text & ".pdf" & """")
            Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\ceasadmin\CONTRATO-" & TextBox17.Text & ".pdf")
        End If

    End Sub
    Private Sub Button11_Click_1(sender As Object, e As EventArgs) Handles Button11.Click
        Try
            Permisos.getPermiso("3", usrdoc)
            If Permisos.idpermiso = "" Then
                MsgBox("Acceso no disponible. consulte al Admistrador")
                Exit Sub
            End If

        Catch ex As Exception

        End Try


        Try
            'Intentar generar el documento.
            Dim pgSize = New iTextSharp.text.Rectangle(201, 350)
            Dim doc As New Document(PageSize.A4, 20, 20, 20, 20)

            'Path que guarda el reporte en el escritorio de windows (Desktop).
            Dim filename As String = Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\ceasadmin\REGISTROCLASES-" & TextBox17.Text & ".pdf"
            Dim file As New FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.ReadWrite)
            PdfWriter.GetInstance(doc, file)
            doc.Open()
            ExportarRegistroClasesPracticas(doc)
            doc.Close()
            'Process.Start(filename)
        Catch ex As Exception
            Me.Cursor = Cursors.Default
            MessageBox.Show("error al imprimir CONTRATO", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
        Dim instance As New Printing.PrinterSettings
        Dim impresosaPredt As String = instance.PrinterName
        'Process.Start("C:\Program Files\Tracker Software\PDF Viewer\pdfxcview.exe", " /print:printer=""" & impresosaPredt & """ """ & Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\\ceasadmin\Recibo de Caja " & Label_consecutivo.Text & ".pdf" & """")
        Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "\ceasadmin\REGISTROCLASES-" & TextBox17.Text & ".pdf")

    End Sub


    Public Sub ExportarContrato(ByVal document As Document)
        'Dim datatable As New PdfPTable(METROGRID_PDF.ColumnCount)
        ''Se asignan algunas propiedades para el diseño del PDF.
        'datatable.DefaultCell.Padding = 3
        'Dim headerwidths As Single() = GetColumnasSize(METROGRID_PDF)

        'datatable.SetWidths({80, 200, 120})
        'datatable.WidthPercentage = 100
        'datatable.DefaultCell.BorderWidth = 2
        'datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER

        Dim ArialBlack20b = iTextSharp.text.FontFactory.GetFont("Arial Black", 20, iTextSharp.text.Font.BOLD)
        Dim ArialBlack15b = iTextSharp.text.FontFactory.GetFont("Arial Black", 15, iTextSharp.text.Font.BOLD)
        Dim ArialBlack12b = iTextSharp.text.FontFactory.GetFont("Arial Black", 12, iTextSharp.text.Font.BOLD)
        Dim ArialBlack10b = iTextSharp.text.FontFactory.GetFont("Arial Black", 10, iTextSharp.text.Font.BOLD)
        Dim ArialBlack10 = iTextSharp.text.FontFactory.GetFont("Arial Black", 10, iTextSharp.text.Font.NORMAL)
        Dim ArialBlack9b = iTextSharp.text.FontFactory.GetFont("Arial Black", 9, iTextSharp.text.Font.BOLD)
        Dim ArialBlack9 = iTextSharp.text.FontFactory.GetFont("Arial Black", 9, iTextSharp.text.Font.NORMAL)
        Dim ArialBlack8b = iTextSharp.text.FontFactory.GetFont("Arial Black", 8, iTextSharp.text.Font.BOLD)




        Dim tablahome As New PdfPTable(3)
        'tablahome.SetWidths({80, 200, 80})
        tablahome.WidthPercentage = 100
        tablahome.DefaultCell.Padding = 0
        tablahome.DefaultCell.BorderWidth = 0
        tablahome.SpacingBefore = 0
        tablahome.HorizontalAlignment = 0

        Dim cellhome As New PdfPCell
        cellhome.BorderWidth = 0

        'LOGO
        Dim imagelogogopdf As iTextSharp.text.Image 'Declaracion de una imagen

        If "C:\CeasAdmin\logo_conti.jpg" <> "" And File.Exists("C:\CeasAdmin\logo_conti.jpg") = True Then

            imagelogogopdf = iTextSharp.text.Image.GetInstance("C:\CeaSAdmin\logo_conti.jpg") 'Dirreccion a la imagen que se hace referencia
            imagelogogopdf.Alignment = Element.ALIGN_CENTER
            imagelogogopdf.SetAbsolutePosition(10, 750) 'Posicion en el eje cartesiano
            imagelogogopdf.ScaleAbsoluteWidth(160) 'Ancho de la imagen
            imagelogogopdf.ScaleAbsoluteHeight(60) 'Altura de la imagen
        End If

        'LOGO
        Dim firmaimg As iTextSharp.text.Image 'Declaracion de una imagen

        If "C:\CeasAdmin\firma.png" <> "" And File.Exists("C:\CeasAdmin\firma.png") = True Then

            firmaimg = iTextSharp.text.Image.GetInstance("C:\CeasAdmin\firma.png") 'Dirreccion a la imagen que se hace referencia
            firmaimg.Alignment = Element.ALIGN_LEFT
            firmaimg.ScaleAbsoluteWidth(10) 'Ancho de la imagen
            firmaimg.ScaleAbsoluteHeight(10) 'Altura de la imagen
        End If

        Dim VR_CONTRATO As String = ""
        Select Case ComboBoxCat.Text
            Case "A1"
                VR_CONTRATO = "$ 370.000"
            Case "A2"
                VR_CONTRATO = "$ 470.000"
            Case "B1"
                VR_CONTRATO = "$ 660.000"
            Case "B2"
                VR_CONTRATO = "$ 1.000.000"
            Case "C1"
                VR_CONTRATO = "$ 860.000"
            Case "C2"
                VR_CONTRATO = "$ 1.250.000"
            Case "RC1"
                VR_CONTRATO = "$ 470.000"
        End Select

        cmd.Connection = conex
        cmd.CommandType = CommandType.Text
        cmd.CommandText = "select valor from servicios where cat='" & ComboBoxCat.Text & "'"

        conex.Open()
        Try
            dr = cmd.ExecuteReader()
            dr.Read()
            VR_CONTRATO = CLng(dr(0))
        Catch ex As Exception
            ultimocontrato = 0
        End Try
        conex.Close()

        cellhome.BorderWidth = 2

        cellhome.Phrase = New Phrase("", ArialBlack20b)
        cellhome.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome.BackgroundColor = BaseColor.WHITE
        cellhome.AddElement(imagelogogopdf)
        tablahome.AddCell(cellhome)

        cellhome.Phrase = New Phrase(Chr(13) & "CONTRATO DE ENSEÑANZA", ArialBlack15b)
        cellhome.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        cellhome.Phrase = New Phrase("VERSION 01" & Chr(13) & Chr(13) & "24-07-2022", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.BorderWidth = 0
        cellhome.Colspan = 3

        cellhome.Phrase = New Phrase(Chr(13) & "CONTRATO No." & TextBox17.Text & "    CATEGORIA " & ComboBoxCat.Text, ArialBlack15b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase("LUGAR  ________________________" & "FECHA: " & TextBox10.Text, ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase("NOMBRE DEL ALUMNO: " & TextBox2.Text & " " & ComboBox6.Text & " " & TextBox1.Text, ArialBlack12b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()


        cellhome.Phrase = New Phrase(Chr(13) & "Celebrado entre el Alumno  y/o acudiente (menores de edad) y " & aca_nom & " con NIT " & aca_nit & ".    Regido por las siguientes clausulas.", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase(Chr(13) & "El curso consta de:", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()


        If ComboBoxCat.Text = "RC1" Then
            cellhome.BorderWidth = 0
            cellhome.Colspan = 0
            cellhome.Phrase = New Phrase("", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase("NUMERO DE HORAS", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase("TEMA", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            tablahome.CompleteRow()

            cellhome.Phrase = New Phrase("", ArialBlack20b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase(prmt_hteor, ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase("Clases Teóricas de Conducción", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            tablahome.CompleteRow()


            cellhome.Phrase = New Phrase("", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase(prmt_hpract, ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase("Clases Prácticas Conducción", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            tablahome.CompleteRow()

        Else
            cellhome.BorderWidth = 0
            cellhome.Colspan = 0
            cellhome.Phrase = New Phrase("", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase("NUMERO DE HORAS", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase("TEMA", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            tablahome.CompleteRow()

            cellhome.Phrase = New Phrase("", ArialBlack20b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase(prmt_hteor, ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase("Clases Teóricas de Conducción", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            tablahome.CompleteRow()

            cellhome.Phrase = New Phrase("", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase(prmt_htaller, ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase("Clases Taller de Conducción", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            tablahome.CompleteRow()

            cellhome.Phrase = New Phrase("", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase(prmt_hpract, ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            cellhome.Phrase = New Phrase("Clases Prácticas Conducción", ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            tablahome.CompleteRow()
        End If


        cellhome.BorderWidth = 0
        cellhome.Colspan = 3
        cellhome.Phrase = New Phrase("", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase(Chr(13) & "2. INTENSIDAD HORARIA. El presente contrato tendrá una intensidad horaria de " & CInt(prmt_hpract + prmt_htaller + prmt_hteor) & " horas, estipuladas de acuerdo al PEI (PROYECTO EDUCATIVO INSTITUCIONAL); aprobado por la Secretaria de Educación del Tolima. Cada clase tendrá una duración de 45 minutos. " & aca_nom & " con NIT " & aca_nit & " con previa programación con el alumno se compromete a dictar clases entre el día Lunes y el día Sábado de acuerdo con la disponibilidad del CEA y de ud. como alumno; no se impartirá clase en casos de fuerza mayor tales como: Lluvias intensas, Presentación del alumno a clases en condiciones no aptas para la clase (Sustancias psicoactivas o en estado de embriaguez), Desperfectos o daños inesperados en el vehículo.", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase(Chr(13) & "3. Costo. El valor del presente contrato es de " & VR_CONTRATO & " M/L. que corresponde exclusivamente  al servicio de formacion otorgado por el CEA de acuerdo a lo establecido por el Ministerio de Transporte.  Después de firmado el presente contrato no se hará devolución del dinero.", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase(Chr(13) & "4. Terminación.  El presente contrato se dará por finalizado cuando se terminen de dictar la totalidad del curso; se podrá dar por terminado cuando no se asista a tres clases sin previo aviso; o por no culminación del curso dentro de los 90 días autorizados para este fin. Se reembolsará dinero solo en aquellos casos en los que la anulación del contrato se deba a incumplimiento de la academia.", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase(Chr(13) & "5. Certificación. Una vez finalizado y aprobado el curso, de acuerdo a lo establecido en el PEI y la legislación del Ministerio de Transporte vigente, el alumno recibirá un certificado que acredita el cumplimiento de los requisitos y objetivos del curso, para que pueda continuar con los trámites de la licencia de conducción, la cual será expedida por las autoridades de tránsito competentes a voluntad del alumno; según la normatividad vigente y la documentación exigida para esta.", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase(Chr(13) & "6. Clases de Apoyo; si el alumno o La Escuela determina que se necesitan más horas prácticas, estas se negociaran por aparte y no se encuentran estipuladas dentro de este contrato.", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase(Chr(13) & "7. Interacción con la Plataforma de Seguridad:  es de conocimiento del alumno que es obligatorio interactuar con la plataforma de seguridad definida por la Súper Intendencia de Puertos y Transportes, con la que se controlará en tiempo real el desarrollo del curso y evaluaciones; en caso de falla de esta, el alumno está dispuesto a esperar y cooperar con el CEA, ya que esta plataforma no es de dominio del CEA y se depende de la repuesta de este operador; así como acepta  que sus datos se encuentren guardados en esta.", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()


        cellhome.Phrase = New Phrase(Chr(13) & "8. Certifico que conozco y acepto los deberes y derechos del alumno; y que la información dada es estricto apego a la verdad.", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()


        cellhome.Phrase = New Phrase(Chr(13) & "El valor del presente contrato no incluye valores adicionales tales como:  PIN de recaudo, sistema de control y vigilancia, sobre tasa de ANSV, RUNT y otros (exámenes médicos y derechos de transito para la expedición de la licencia), los cuales se deben cancelar en forma independiente a estos organismos.", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase(Chr(13) & "10. Autorizo a " & aca_nom & "  con NIT " & aca_nit & " para que empleen las imágenes y videos en las cuales aparezca y que se realicen durante el desarrollo de la capacitación objeto del presente contrato para ser publicadas dentro de las redes sociales de la academia.", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_JUSTIFIED
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()



        Dim mes As String = ""
        Select Case CDate(TextBox10.Text).Month
            Case 1
                mes = "Enero"
            Case 2
                mes = "Febrero"
            Case 3
                mes = "Marzo"
            Case 4
                mes = "Abril"
            Case 5
                mes = "Mayo"
            Case 6
                mes = "Junio"
            Case 7
                mes = "Julio"
            Case 8
                mes = "Agosto"
            Case 9
                mes = "Septiembre"
            Case 10
                mes = "Octubre"
            Case 11
                mes = "Noviembre"
            Case 12
                mes = "Diciembre"
        End Select



        cellhome.Phrase = New Phrase(Chr(13) & "Este contrato se firma en el Municipio de Ibagué - Tolima, a los " & CDate(TextBox10.Text).Day & " días del mes de " & mes & " de " & DateTime.Now.Year() & "  es intransferible." & Chr(13) & Chr(13), ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()


        cellhome.Colspan = 0
        cellhome.Phrase = New Phrase("_______________________", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        cellhome.Phrase = New Phrase("               ", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        cellhome.Phrase = New Phrase("_______________________", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)


        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase("FIRMA ALUMNO", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        cellhome.Phrase = New Phrase("               ", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        cellhome.Phrase = New Phrase("FIRMA DIRECTOR", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        document.Add(tablahome)
        document.NewPage()


        Dim tablahome2 As New PdfPTable(3)
        'tablahome.SetWidths({80, 200, 80})
        tablahome2.WidthPercentage = 100
        tablahome2.DefaultCell.Padding = 0
        tablahome2.DefaultCell.BorderWidth = 0
        tablahome2.SpacingBefore = 0
        tablahome2.HorizontalAlignment = 0

        Dim cellhome2 As New PdfPCell
        cellhome2.BorderWidth = 0

        cellhome2.BorderWidth = 2
        cellhome2.Colspan = 0
        cellhome2.Phrase = New Phrase("", ArialBlack20b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.WHITE
        cellhome2.AddElement(imagelogogopdf)
        tablahome2.AddCell(cellhome2)

        cellhome2.Phrase = New Phrase(Chr(13) & "REGISTRO DEL ALUMNO", ArialBlack12b)
        cellhome2.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome2.BackgroundColor = BaseColor.WHITE
        tablahome2.AddCell(cellhome2)

        cellhome2.Phrase = New Phrase("VERSION 01" & Chr(13) & Chr(13) & "24 de Julio de 2022", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome2.BackgroundColor = BaseColor.WHITE
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()


        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 3
        cellhome2.Phrase = New Phrase(" ", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.WHITE
        tablahome2.AddCell(cellhome2)




        tablahome2.CompleteRow()



        cellhome2.Colspan = 3
        cellhome2.BorderWidth = 0
        cellhome2.BorderWidthBottom = 0
        cellhome2.BorderWidthTop = 0
        cellhome2.Phrase = New Phrase("CONTRATO  " & TextBox17.Text, ArialBlack12b)
        cellhome2.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()

        losAlumnos.BuscarxDoc(TextBox1.Text)


        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 2
        cellhome2.Phrase = New Phrase("NOMBRE DEL ALUMNO: " & TextBox2.Text, ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)


        cellhome2.BorderWidth = 1
        cellhome2.BorderWidthBottom = 0
        cellhome2.BorderWidthTop = 1
        cellhome2.Phrase = New Phrase("", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()

        cellhome2.BorderWidthTop = 0
        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 2
        cellhome2.Phrase = New Phrase("DOCUMENTO:  " & ComboBox6.Text & "  " & TextBox1.Text & " DE " & losAlumnos.expediciondoc, ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        cellhome2.BorderWidth = 1
        cellhome2.BorderWidthBottom = 0
        cellhome2.BorderWidthTop = 0
        cellhome2.Phrase = New Phrase("", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()

        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 2
        cellhome2.Phrase = New Phrase("DIRECCION: " & losAlumnos.dir & " CEL: " & losAlumnos.cel, ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        cellhome2.BorderWidth = 1
        cellhome2.BorderWidthBottom = 0
        cellhome2.BorderWidthTop = 0
        cellhome2.Phrase = New Phrase("", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()

        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 2
        cellhome2.Phrase = New Phrase("ESTADO CIVIL: " & losAlumnos.ecivil & " ESTUDIOS: " & losAlumnos.educa, ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        cellhome2.BorderWidth = 1
        cellhome2.BorderWidthBottom = 0
        cellhome2.BorderWidthTop = 0
        cellhome2.Phrase = New Phrase("", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()

        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 2
        cellhome2.Phrase = New Phrase("FECHA DE NACIMIENTO: " & losAlumnos.fechan & " GENERO: " & losAlumnos.genero, ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        cellhome2.BorderWidth = 1
        cellhome2.BorderWidthBottom = 0
        cellhome2.BorderWidthTop = 0
        cellhome2.Phrase = New Phrase("", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()

        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 2
        cellhome2.Phrase = New Phrase("ESTRATO: " & losAlumnos.estrato & " OCUPACION: " & losAlumnos.ocupa, ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        cellhome2.BorderWidth = 1
        cellhome2.BorderWidthBottom = 0
        cellhome2.BorderWidthTop = 0
        cellhome2.Phrase = New Phrase("", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()

        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 2
        cellhome2.Phrase = New Phrase("CATEGORIA: " & ComboBoxCat.Text & " INSCRIPCION RUNT: " & losAlumnos.runt, ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        cellhome2.BorderWidth = 1
        cellhome2.BorderWidthBottom = 0
        cellhome2.BorderWidthTop = 0
        cellhome2.Phrase = New Phrase("", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()

        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 2
        cellhome2.Phrase = New Phrase("CERTIFICADO RUNT _____________" & " E-MAIL: " & losAlumnos.email, ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        cellhome2.BorderWidth = 1
        cellhome2.BorderWidthBottom = 0
        cellhome2.BorderWidthTop = 0
        cellhome2.Phrase = New Phrase("", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()


        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 2
        cellhome2.Phrase = New Phrase("FECHA INSCRIPCION: " & TextBox10.Text, ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        cellhome2.Rowspan = 9
        cellhome2.BorderWidth = 1
        cellhome2.BorderWidthBottom = 1
        cellhome2.BorderWidthTop = 0
        cellhome2.Phrase = New Phrase("", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()


        cellhome2.BorderWidthBottom = 0
        cellhome2.BorderWidth = 0
        cellhome2.Colspan = 2
        cellhome2.Phrase = New Phrase(" ", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.WHITE
        tablahome2.AddCell(cellhome2)


        cellhome2.BorderWidth = 0
        cellhome2.Phrase = New Phrase("", ArialBlack10b)
        cellhome2.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome2.BackgroundColor = BaseColor.WHITE
        tablahome2.AddCell(cellhome2)

        tablahome2.CompleteRow()




        Dim tablahome3 As New PdfPTable(9)
        tablahome3.SetWidths({50, 80, 20, 80, 50, 80, 20, 50, 80})
        tablahome3.WidthPercentage = 100
        tablahome3.DefaultCell.Padding = 0
        tablahome3.DefaultCell.BorderWidth = 0
        tablahome3.SpacingBefore = 0
        tablahome3.HorizontalAlignment = 0

        Dim cellhome3 As New PdfPCell

        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 1
        cellhome3.Colspan = 4
        cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)


        cellhome3.Colspan = 5
        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 1
        cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        tablahome3.CompleteRow()

        cellhome3.BorderWidth = 1
        cellhome3.Colspan = 4
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 0
        cellhome3.Phrase = New Phrase("REGISTRO CLASES TEORICAS Y DE TALLER", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)


        cellhome3.Colspan = 5
        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 0
        cellhome3.Phrase = New Phrase("REGISTRO CLASES PRACTICAS", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        tablahome3.CompleteRow()

        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthTop = 0
        cellhome3.BorderWidthBottom = 1
        cellhome3.Colspan = 4
        cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)


        cellhome3.Colspan = 5
        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthTop = 0
        cellhome3.BorderWidthBottom = 1
        cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        tablahome3.CompleteRow()

        cellhome3.Colspan = 0
        cellhome3.FixedHeight = 20
        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthTop = 1
        cellhome3.BorderWidthBottom = 1
        cellhome3.Phrase = New Phrase("FECHA", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        cellhome3.Phrase = New Phrase("FIRMA ALUMNO", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        cellhome3.Phrase = New Phrase("No.", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        cellhome3.Phrase = New Phrase("FIRMA DOCENTE ", ArialBlack9b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        cellhome3.Phrase = New Phrase("FECHA", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        cellhome3.Phrase = New Phrase("FIRMA ALUMNO", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        cellhome3.Phrase = New Phrase("No.", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        cellhome3.Phrase = New Phrase("CALIF", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        cellhome3.Phrase = New Phrase("FIRMA INSTRUCTOR", ArialBlack8b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        tablahome3.CompleteRow()



        If ComboBoxCat.Text = "A1" Then


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)

            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)

            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 4
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("TALLER", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
            tablahome3.AddCell(cellhome3)


            cellhome3.Colspan = 0
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            cellhome3.FixedHeight = -1
            cellhome3.Colspan = 9
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("OBSERVACIONES " & Chr(13) & Chr(13) & Chr(13) & Chr(13) & Chr(13) & Chr(13), ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

        End If

        If ComboBoxCat.Text = "A2" Then

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()



            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()



            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)

            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 4
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("TALLER", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
            tablahome3.AddCell(cellhome3)


            cellhome3.Colspan = 0
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("1 ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            cellhome3.FixedHeight = -1
            cellhome3.Colspan = 9
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("OBSERVACIONES " & Chr(13) & Chr(13) & Chr(13) & Chr(13) & Chr(13) & Chr(13), ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

        End If

        If ComboBoxCat.Text = "B1" Then

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()



            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()



            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 4
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("TALLER", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
            tablahome3.AddCell(cellhome3)


            cellhome3.Colspan = 0
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("1 ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            cellhome3.FixedHeight = -1
            cellhome3.Colspan = 9
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("OBSERVACIONES " & Chr(13) & Chr(13) & Chr(13) & Chr(13) & Chr(13) & Chr(13), ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

        End If

        If ComboBoxCat.Text = "B2" Then

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()



            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()



            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 4
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("TALLER", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
            tablahome3.AddCell(cellhome3)


            cellhome3.Colspan = 0
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("1 ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            cellhome3.FixedHeight = -1
            cellhome3.Colspan = 9
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("OBSERVACIONES " & Chr(13) & Chr(13) & Chr(13) & Chr(13) & Chr(13) & Chr(13), ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

        End If

        If ComboBoxCat.Text = "C1" Then

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()



            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 4
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("TALLER", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
            tablahome3.AddCell(cellhome3)


            cellhome3.Colspan = 0
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1
            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()




            cellhome3.FixedHeight = -1
            cellhome3.Colspan = 9
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("OBSERVACIONES " & Chr(13) & Chr(13) & Chr(13) & Chr(13), ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

        End If

        If ComboBoxCat.Text = "C2" Then

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()



            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 4
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("TALLER", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
            tablahome3.AddCell(cellhome3)


            cellhome3.Colspan = 0
            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1
            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.AddElement(firmaimg)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()




            cellhome3.FixedHeight = -1
            cellhome3.Colspan = 9
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("OBSERVACIONES " & Chr(13) & Chr(13) & Chr(13) & Chr(13), ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

        End If

        If ComboBoxCat.Text = "RC1" Then

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthTop = 0
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.BorderWidthBottom = 0
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

            'CELDA SIN BUTTON WIDTH
            cellhome3.Colspan = 0
            cellhome3.BorderWidth = 1
            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("1", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase("X", ArialBlack12b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)


            cellhome3.Phrase = New Phrase("2", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()


            cellhome3.FixedHeight = -1
            cellhome3.Colspan = 9
            cellhome3.BorderWidth = 1
            cellhome3.BorderWidthBottom = 1
            cellhome3.BorderWidthTop = 1
            cellhome3.Phrase = New Phrase("OBSERVACIONES " & Chr(13) & Chr(13) & Chr(13) & Chr(13), ArialBlack10b)
            cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
            cellhome3.BackgroundColor = BaseColor.WHITE
            tablahome3.AddCell(cellhome3)

            tablahome3.CompleteRow()

        End If

        cellhome3.Colspan = 9
        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthBottom = 1
        cellhome3.BorderWidthTop = 1
        cellhome3.Phrase = New Phrase(Chr(13) & "CERTIFICO QUE CONOZCO Y ACEPTO LOS DEBERES Y DERECHOS DEL ALUMNO, Y LA INFORMACION DADA EN EL PROCESO ES EN APEGO A LA VERDAD. " & Chr(13) & Chr(13), ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.WHITE
        tablahome3.AddCell(cellhome3)

        tablahome3.CompleteRow()


        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 1
        cellhome3.Colspan = 4
        cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)


        cellhome3.Colspan = 5
        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 1
        cellhome3.Phrase = New Phrase(" ", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        tablahome3.CompleteRow()

        cellhome3.BorderWidth = 1
        cellhome3.Colspan = 4
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 0
        cellhome3.Phrase = New Phrase("", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)


        cellhome3.Colspan = 5
        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 0
        cellhome3.Phrase = New Phrase("", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        tablahome3.CompleteRow()

        cellhome3.BorderWidth = 1
        cellhome3.Colspan = 4
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 0
        cellhome3.Phrase = New Phrase("", ArialBlack10b)
        cellhome3.AddElement(firmaimg)
        cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)


        cellhome3.Colspan = 5
        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 0
        cellhome3.Phrase = New Phrase("", ArialBlack10b)
        cellhome3.AddElement(firmaimg)
        cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        tablahome3.CompleteRow()

        cellhome3.BorderWidth = 1
        cellhome3.Colspan = 4
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 0
        cellhome3.Phrase = New Phrase("", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)


        cellhome3.Colspan = 5
        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthBottom = 0
        cellhome3.BorderWidthTop = 0
        cellhome3.Phrase = New Phrase("", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        tablahome3.CompleteRow()

        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthTop = 0
        cellhome3.BorderWidthBottom = 1
        cellhome3.Colspan = 4
        cellhome3.Phrase = New Phrase("(X) FIRMA ALUMNO", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)


        cellhome3.Colspan = 5
        cellhome3.BorderWidth = 1
        cellhome3.BorderWidthTop = 0
        cellhome3.BorderWidthBottom = 1
        cellhome3.Phrase = New Phrase("(X) FIRMA DIRECTOR", ArialBlack10b)
        cellhome3.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome3.BackgroundColor = BaseColor.LIGHT_GRAY
        tablahome3.AddCell(cellhome3)

        tablahome3.CompleteRow()



        tablahome3.CompleteRow()


        tablahome2.SpacingAfter = 3
        document.Add(tablahome2)

        document.Add(tablahome3)

    End Sub


    Public Sub ExportarRegistroClasesPracticas(ByVal document As Document)
        'Dim datatable As New PdfPTable(METROGRID_PDF.ColumnCount)
        ''Se asignan algunas propiedades para el diseño del PDF.
        'datatable.DefaultCell.Padding = 3
        'Dim headerwidths As Single() = GetColumnasSize(METROGRID_PDF)

        'datatable.SetWidths({80, 200, 120})
        'datatable.WidthPercentage = 100
        'datatable.DefaultCell.BorderWidth = 2
        'datatable.DefaultCell.HorizontalAlignment = Element.ALIGN_CENTER

        Dim ArialBlack20b = iTextSharp.text.FontFactory.GetFont("Arial Black", 20, iTextSharp.text.Font.BOLD)
        Dim ArialBlack15b = iTextSharp.text.FontFactory.GetFont("Arial Black", 15, iTextSharp.text.Font.BOLD)
        Dim ArialBlack12b = iTextSharp.text.FontFactory.GetFont("Arial Black", 12, iTextSharp.text.Font.BOLD)
        Dim ArialBlack10b = iTextSharp.text.FontFactory.GetFont("Arial Black", 10, iTextSharp.text.Font.BOLD)
        Dim ArialBlack10 = iTextSharp.text.FontFactory.GetFont("Arial Black", 10, iTextSharp.text.Font.NORMAL)
        Dim ArialBlack9b = iTextSharp.text.FontFactory.GetFont("Arial Black", 9, iTextSharp.text.Font.BOLD)
        Dim ArialBlack9 = iTextSharp.text.FontFactory.GetFont("Arial Black", 9, iTextSharp.text.Font.NORMAL)
        Dim ArialBlack8b = iTextSharp.text.FontFactory.GetFont("Arial Black", 8, iTextSharp.text.Font.BOLD)



        Dim tablahome As New PdfPTable(3)
        'tablahome.SetWidths({80, 200, 80})
        tablahome.WidthPercentage = 100
        tablahome.DefaultCell.Padding = 0
        tablahome.DefaultCell.BorderWidth = 0
        tablahome.SpacingBefore = 0
        tablahome.HorizontalAlignment = 0

        Dim cellhome As New PdfPCell
        cellhome.BorderWidth = 0

        'LOGO
        Dim imagelogogopdf As iTextSharp.text.Image 'Declaracion de una imagen

        If "C:\CeasAdmin\logo_conti.jpg" <> "" And File.Exists("C:\CeasAdmin\logo_conti.jpg") = True Then

            imagelogogopdf = iTextSharp.text.Image.GetInstance("C:\CeaSAdmin\logo_conti.jpg") 'Dirreccion a la imagen que se hace referencia
            imagelogogopdf.Alignment = Element.ALIGN_RIGHT
            imagelogogopdf.SetAbsolutePosition(10, 750) 'Posicion en el eje cartesiano
            imagelogogopdf.ScaleAbsoluteWidth(160) 'Ancho de la imagen
            imagelogogopdf.ScaleAbsoluteHeight(60) 'Altura de la imagen
        End If

        'document.Add(imagelogogopdf) ' Agrega la imagen al documento



        Dim firmaimg As iTextSharp.text.Image 'Declaracion de una imagen

        If "C:\CeasAdmin\firma.png" <> "" And File.Exists("C:\CeasAdmin\firma.png") = True Then

            firmaimg = iTextSharp.text.Image.GetInstance("C:\CeasAdmin\firma.png") 'Dirreccion a la imagen que se hace referencia
            firmaimg.Alignment = Element.ALIGN_LEFT
            firmaimg.ScaleAbsoluteWidth(10) 'Ancho de la imagen
            firmaimg.ScaleAbsoluteHeight(10) 'Altura de la imagen
        End If


        cellhome.BorderWidth = 2

        cellhome.Phrase = New Phrase("", ArialBlack20b)
        cellhome.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome.BackgroundColor = BaseColor.WHITE
        cellhome.AddElement(imagelogogopdf)
        tablahome.AddCell(cellhome)

        cellhome.Phrase = New Phrase("FORMATO" & Chr(13) & "REGISTRO DE CLASES PRACTICAS", ArialBlack12b)
        cellhome.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        cellhome.Phrase = New Phrase("CODIGO" & Chr(13) & Chr(13) & "F-PS-05" & Chr(13) & Chr(13) & "VERSION 1  |  24/07/2022", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_CENTER
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()


        cellhome.Colspan = 3

        cellhome.BorderWidth = 0
        cellhome.BorderWidthLeft = 2
        cellhome.BorderWidthRight = 2


        cellhome.Phrase = New Phrase(" " & TextBox2.Text, ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()


        cellhome.Phrase = New Phrase("Alumno:" & TextBox2.Text, ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Phrase = New Phrase("Documento: " & ComboBox6.Text & " " & TextBox1.Text & "  RH: " & losAlumnos.RH, ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Colspan = 3
        cellhome.BorderWidth = 0
        cellhome.BorderWidthLeft = 2
        cellhome.BorderWidthRight = 2

        cellhome.Phrase = New Phrase("Direccion:  " & losAlumnos.dir, ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Colspan = 3

        cellhome.Phrase = New Phrase("Teléfono: " & TextBox14.Text, ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Colspan = 3

        cellhome.Phrase = New Phrase("Fecha Inicial: _______________", ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()


        cellhome.Colspan = 3

        cellhome.Phrase = New Phrase("Instructor: " & ComboBox_instructores.Text, ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.Colspan = 3

        cellhome.Phrase = New Phrase(" " & ComboBox_instructores.Text, ArialBlack10b)
        cellhome.HorizontalAlignment = Element.ALIGN_LEFT
        cellhome.BackgroundColor = BaseColor.WHITE
        tablahome.AddCell(cellhome)

        tablahome.CompleteRow()

        cellhome.BorderWidth = 2
        cellhome.Colspan = 3

        If ComboBoxCat.Text = "" Then
            cellhome.Phrase = New Phrase(Chr(13) & "PARTICULAR" & Chr(13), ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            tablahome.CompleteRow()
        Else
            cellhome.Phrase = New Phrase("CONTRATO: " & TextBox17.Text & "  CATEGORIA: " & ComboBoxCat.Text, ArialBlack10b)
            cellhome.HorizontalAlignment = Element.ALIGN_CENTER
            cellhome.BackgroundColor = BaseColor.WHITE
            tablahome.AddCell(cellhome)

            tablahome.CompleteRow()
        End If




        Dim tablaFirma As New PdfPTable(4)
        tablaFirma.SetWidths({80, 100, 395, 395})
        tablaFirma.WidthPercentage = 100
        tablaFirma.DefaultCell.Padding = 0
        tablaFirma.DefaultCell.BorderWidth = 0
        tablaFirma.SpacingBefore = 0
        tablaFirma.HorizontalAlignment = 0

        Dim cellfirma As New PdfPCell


        ''FILA1
        'cellfirma.BorderWidth = 2
        'cellfirma.Colspan = 4

        'cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
        'cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
        'cellfirma.BackgroundColor = BaseColor.WHITE
        'tablaFirma.AddCell(cellfirma)

        'tablaFirma.CompleteRow()
        ''FILA1

        'FILA2

        cellfirma.Colspan = 0
        cellfirma.BorderWidth = 2
        cellfirma.Phrase = New Phrase("HORA", ArialBlack10b)
        cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
        cellfirma.BackgroundColor = BaseColor.LIGHT_GRAY
        tablaFirma.AddCell(cellfirma)


        cellfirma.Phrase = New Phrase("FECHA", ArialBlack10b)
        cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
        cellfirma.BackgroundColor = BaseColor.LIGHT_GRAY
        tablaFirma.AddCell(cellfirma)


        cellfirma.Phrase = New Phrase("TEMA", ArialBlack10b)
        cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
        cellfirma.BackgroundColor = BaseColor.LIGHT_GRAY
        tablaFirma.AddCell(cellfirma)


        cellfirma.Phrase = New Phrase("FIRMA", ArialBlack10b)
        cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
        cellfirma.BackgroundColor = BaseColor.LIGHT_GRAY
        tablaFirma.AddCell(cellfirma)

        tablaFirma.CompleteRow()
        'FILA2



        If ComboBoxCat.Text = "A2" Then

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("1", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conformación del tablero de instrumentos, reconocimiento de pedales, frenos, cambios y demás accesorios. Puesta en marcha de la moto.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("2", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Paradas y arranques, dominio de la arrancada y paradas. Estabilidad, adaptación viso espacial.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("3", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Mecanismos de control; embrague, freno, acelerador, cambios, dirección y sus funciones. Coordinación, aceleración, freno y embrague.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("4", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Ejercicios con la dirección, aceleración y desaceleración, control de cambios.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("5", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Giros abiertos y cerrados de izquierda y derecha repetitivamente.  Entrada y salida en curvas.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("6", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Cambios ascendentes y descendentes (1, 2, 3, 4).", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()



            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("7", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización de señales lumínicas, corporales y acústicas.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("8", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Maniobras de cruces y adelantamientos. Repaso sobre los ejercicios de pedales, dirección, izquierda y derecha.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("9", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Arranque en pendiente en repeticiones constantes. Referencia de la nomenclatura urbana y nacional.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("10", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en zona urbana, parada en pendiente y utilización de carriles.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("11", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Perímetro urbano, reconocimiento de carriles derecho e izquierdo. Afrontar y utilizar glorietas e intersecciones. Respeto de las marcas viales y señales de tránsito.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("12", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Manejo de espejos, direccionales, control sobre la aceleración y manipulación de cambios de velocidad, 3, 4, 5. Distancia de reacción Y Distancia de frenado.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("13", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Visualización y entendimiento de las señales de tránsito (preventivas, reglamentarias e informativas)", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("14", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Reconocimiento sobre los posibles riesgos en la vía, y el respeto por el peatón, conduccion de la moto en via urbana y aplicación de tecnicas aprendidas", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("EXAMEN PRACTICO SICOV", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

        End If

        If ComboBoxCat.Text = "B1" Then

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("1", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Alistamiento Preoperacional del Vehículo", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("2", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Adaptación viso espacial", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("3", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización elementos de seguridad", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("4", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Puesta en marcha del motor", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("5", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Selector de velocidades", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("6", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Puesta en marcha del vehículo", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()



            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("7", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Coordinación. Aceleración-freno-embrague", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("8", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Aceleración y desaceleración ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("9", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Control de cambios", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("10", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en vía urbana", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("11", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en carretera", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("12", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en terreno plano", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("13", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en pendiente", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("14", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Maniobras de cruces y adelantamientos", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("15", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización de señales lumínicas, corporales y acústicas", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("16", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización de calzadas", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("17", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Distancia de frenado", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("18", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Maniobras de adelantamiento (derecha e izquierda)", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("19", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Ejercicio de Conducción Dinámico", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("EXAMEN PRACTICO SICOV", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

        End If

        If ComboBoxCat.Text = "C1" Then

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("1", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Ajuste de asiento", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("2", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Adaptación viso espacial", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("3", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización elementos de seguridad", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("4", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Puesta en marcha del motor", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("5", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Selector de velocidades", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("6", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Puesta en marcha del vehículo", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()



            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("7", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Coordinación. Aceleración-freno-embrague", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("8", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Aceleración y desaceleración ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("9", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Control de cambios", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("10", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en vía urbana", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("11", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en carretera", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("12", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en terreno plano", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("13", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en pendiente", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("14", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Maniobras de cruces y adelantamientos", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("15", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización de señales lumínicas, corporales y acústicas", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("16", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización de calzadas", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("17", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización de carriles", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("18", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Afronta y utilizar glorietas", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("19", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Afrontar intersecciones", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("20", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Respeto de las marcas viales y señales de transito", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("21", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Distancia de reacción", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("22", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización de carriles", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("23", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Maniobras de adelantamiento (derecha e izquierda)", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("24", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Maniobras con el cambio en reversa", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("25", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Entrada y salida en curva", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("26", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Parque y estacionamiento frontal y en reversa", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("27", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización del equipo de seguridad", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("28", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Nomenclatura urbana y nacional", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("29", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Ejercicio de Conducción Dinámico", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("EXAMEN PRACTICO SICOV", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()
        End If

        If ComboBoxCat.Text = "C2" Then

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("1", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Inspección Pre operacional", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("2", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en vía urbana.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("3", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en carretera.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("4", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en terreno plano.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("5", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en pendiente", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("6", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Maniobras de cruces y adelantamientos", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()



            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("7", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización de calzadas.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("8", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Distancia de reacción.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("9", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Distancia de frenado.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("10", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Maniobras de adelantamiento (derecha e izquierda).", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("11", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Ejercicio de Conducción Dinámico.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("12", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Cargue y Descargue de Mercancías.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("13", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Normas de seguridad en el aseguramiento de la carga", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("14", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Uso de salida de Emergencia", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("EXAMEN PRACTICO SICOV", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

        End If

        If ComboBoxCat.Text = "RC1" Then

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("1", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Ajuste de asiento.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("2", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Adaptación viso espacial.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("3", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Utilización elementos de seguridad.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("4", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Puesta en marcha del motor.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("5", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Selector de velocidades.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("6", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Puesta en marcha del vehículo.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()



            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("7", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Coordinación. Aceleración-freno-embrague.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("8", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Aceleración, desaceleración y control de cambios .", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()


            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase("9", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("Conducción del vehículo en vía urbana.", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()




            cellfirma.Colspan = 0
            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase(" ", ArialBlack10b)
            cellfirma.HorizontalAlignment = Element.ALIGN_CENTER
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("EXAMEN PRACTICO SICOV", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            cellfirma.Phrase = New Phrase("X", ArialBlack12b)
            cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
            cellfirma.BackgroundColor = BaseColor.WHITE
            tablaFirma.AddCell(cellfirma)

            tablaFirma.CompleteRow()

        End If

        cellfirma.Colspan = 4
        cellfirma.Phrase = New Phrase("OBSERVACIONES: " & Chr(13) & Chr(13) & Chr(13), ArialBlack10b)
        cellfirma.HorizontalAlignment = Element.ALIGN_LEFT
        cellfirma.BackgroundColor = BaseColor.WHITE
        tablaFirma.AddCell(cellfirma)



        tablaFirma.CompleteRow()




        document.Add(tablahome)
        document.Add(tablaFirma)


    End Sub

    Private Sub TextBox15_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox15.KeyPress
        If InStr(1, "0123456789" & Chr(8) & Chr(13), e.KeyChar) = 0 Then e.KeyChar = ""

    End Sub

    Private Sub TextBox15_TextChanged(sender As Object, e As EventArgs) Handles TextBox15.TextChanged

    End Sub

    Private Sub Button12_Click(sender As Object, e As EventArgs) Handles Button12.Click
        If Me.TextBox19.Text = "" Then
            MsgBox("No escribio una fecha se establecerá la fecha actual:  " & DateTime.Now().ToShortDateString())
            Me.TextBox19.Text = DateTime.Now().ToShortDateString()
        End If

        If Me.TextBox15.Enabled = False Then Exit Sub
        sql = "UPDATE cursos SET reg_runt='" & Me.TextBox15.Text & "', f_reg_runt='" & Me.TextBox19.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            MsgBox("Listo, Guardado  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxServicio.SelectedIndexChanged


    End Sub
    Private Sub parametros_servicios_load(ByVal serv_cod As String)

        sql = "SELECT * FROM servicios WHERE cod='" & serv_cod & "'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        For Each lin As DataRow In dt.Rows
            prmt_vrservicio = CLng(lin.Item("valor"))
        Next
        da.Dispose()
        dt.Dispose()
        conex.Close()


    End Sub

    Private Sub Button13_Click(sender As Object, e As EventArgs) Handles Button13.Click

        If Me.TextBox20.Text = "" Then
            MsgBox("No escribio una fecha se establecerá la fecha actual:  " & DateTime.Now().ToShortDateString())
            Me.TextBox20.Text = DateTime.Now().ToShortDateString()
        End If
        If Me.TextBox16.Enabled = False Then Exit Sub


        sql = "UPDATE cursos SET cert_runt='" & Me.TextBox16.Text & "', f_cert_runt='" & Me.TextBox20.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            MsgBox("Listo, Guardado  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub


    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        Try
            Permisos.getPermiso("3", usrdoc)
            If Permisos.idpermiso = "" Then
                MsgBox("Acceso no disponible. consulte al Admistrador")
                Exit Sub
            End If

        Catch ex As Exception

        End Try


        sql = "UPDATE cursos SET num_contrato='" & Me.TextBox17.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub TextBox17_KeyPress(sender As Object, e As KeyPressEventArgs) Handles TextBox17.KeyPress
        e.KeyChar = ""

    End Sub

    Private Sub TextBox17_TextChanged(sender As Object, e As EventArgs) Handles TextBox17.TextChanged

    End Sub

    Private Sub TabPage3_Click(sender As Object, e As EventArgs) Handles TabPage3.Click

    End Sub

    Private Sub MenuStrip1_ItemClicked(sender As Object, e As ToolStripItemClickedEventArgs) Handles MenuStrip1.ItemClicked

    End Sub

    Private Sub AgregarObservaciónToolStripMenuItem_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub Button5_Click(sender As Object, e As EventArgs) Handles Button5.Click
        Me.Cursor = Cursors.WaitCursor
        Me.Button5.Visible = False
        Me.TextBox4.BackColor = Color.White
        Me.TextBox4.Enabled = False
        sql = "UPDATE cursos SET observacion='" & Me.TextBox4.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
        Me.Cursor = Cursors.Default

    End Sub





    Private Sub AnularCursoToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles AnularCursoToolStripMenuItem.Click
        Try
            Permisos.getPermiso("3", usrdoc)
            If Permisos.idpermiso = "" Then
                MsgBox("Acceso no disponible. consulte al Admistrador")
                Exit Sub
            End If

        Catch ex As Exception

        End Try


        Try
            Permisos.getPermiso("4", usrdoc)
            If Permisos.idpermiso = "" Then
                MsgBox("Acceso no disponible. consulte al Admistrador")
                Exit Sub
            End If

        Catch ex As Exception

        End Try


        Dim msgrta As Integer = 0
        msgrta = MsgBox("Esta seguro que desea anular este curso." & Chr(13) & Chr(13) &
                        "Si se ANULA un curso se eliminan también todos los datos vinculados a él y no se podrán consultar en el futuro.", MsgBoxStyle.YesNo + vbCritical)

        If msgrta = 6 Then


            If msgrta = 6 Then

                sql = "UPDATE cursos SET estado='" & "ANULADO" & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
                da = New MySqlDataAdapter(sql, conex)
                dt = New DataTable
                Try
                    da.Fill(dt)
                    ' MsgBox("Listo, Anulado  :).", vbInformation)
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
                da.Dispose()
                dt.Dispose()
                conex.Close()

                'sql = "DELETE from cursos_abonos WHERE CURSO=" & CLng(Me.LabelCurso.Text) & ""
                'da = New MySqlDataAdapter(sql, conex)
                'dt = New DataTable
                'Try
                '    da.Fill(dt)
                '    ' MsgBox("Listo, Anulado  :).", vbInformation)
                'Catch ex As Exception
                '    MsgBox(ex.ToString)
                'End Try
                'da.Dispose()
                'dt.Dispose()
                'conex.Close()


                'sql = "DELETE from recibos_asesores WHERE CURSO='" & Me.LabelCurso.Text & "'"
                'da = New MySqlDataAdapter(sql, conex)
                'dt = New DataTable
                'Try
                '    da.Fill(dt)
                '    ' MsgBox("Listo, Anulado  :).", vbInformation)
                'Catch ex As Exception
                '    MsgBox(ex.ToString)
                'End Try
                'da.Dispose()
                'dt.Dispose()
                'conex.Close()

                'sql = "DELETE from recibos_caja WHERE CURSO='" & Me.LabelCurso.Text & "'"
                'da = New MySqlDataAdapter(sql, conex)
                'dt = New DataTable
                'Try
                '    da.Fill(dt)
                '    ' MsgBox("Listo, Anulado  :).", vbInformation)
                'Catch ex As Exception
                '    MsgBox(ex.ToString)
                'End Try
                'da.Dispose()
                'dt.Dispose()
                'conex.Close()

                'sql = "DELETE from recibos_especiales WHERE CURSO='" & Me.LabelCurso.Text & "'"
                'da = New MySqlDataAdapter(sql, conex)
                'dt = New DataTable
                'Try
                '    da.Fill(dt)
                '    ' MsgBox("Listo, Anulado  :).", vbInformation)
                'Catch ex As Exception
                '    MsgBox(ex.ToString)
                'End Try
                'da.Dispose()
                'dt.Dispose()
                'conex.Close()

                'sql = "DELETE from recibos_medicos WHERE CURSO='" & Me.LabelCurso.Text & "'"
                'da = New MySqlDataAdapter(sql, conex)
                'dt = New DataTable
                'Try
                '    da.Fill(dt)
                '    ' MsgBox("Listo, Anulado  :).", vbInformation)
                'Catch ex As Exception
                '    MsgBox(ex.ToString)
                'End Try
                'da.Dispose()
                'dt.Dispose()
                'conex.Close()

                'sql = "DELETE from empleados_caja WHERE curso='" & Me.LabelCurso.Text & "'"
                'da = New MySqlDataAdapter(sql, conex)
                'dt = New DataTable
                'Try
                '    da.Fill(dt)

                'Catch ex As Exception
                '    MsgBox(ex.ToString)
                'End Try
                'da.Dispose()
                'dt.Dispose()
                'conex.Close()


                'sql = "DELETE from horario_general WHERE curso=" & CLng(Me.LabelCurso.Text) & ""
                'da = New MySqlDataAdapter(sql, conex)
                'dt = New DataTable
                'Try
                '    da.Fill(dt)

                'Catch ex As Exception
                '    MsgBox(ex.ToString)
                'End Try
                'da.Dispose()
                'dt.Dispose()
                'conex.Close()


                MsgBox("Listo, Se anuló el curso y todos los datos vinculados  :).", vbInformation)
                Formcursos_ver_Load(Nothing, Nothing)
            End If
        End If
    End Sub



    Private Sub ObservacionesDelCursoToolStripMenuItem_Click(sender As Object, e As EventArgs)
        MsgBox("Ya Puede Agregar observaciones, no olvide guardar en el diskette   :). ", vbInformation)
        Me.Button5.Visible = True
        Me.TextBox4.Enabled = True
        Me.TextBox4.BackColor = Color.Yellow
        Me.TextBox4.Focus()
    End Sub

    Private Sub TextBox18_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub DescuentosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DescuentosToolStripMenuItem.Click
        Try
            Permisos.getPermiso("3", usrdoc)
            If Permisos.idpermiso = "" Then
                MsgBox("Acceso no disponible. consulte al Admistrador")
                Exit Sub
            End If

        Catch ex As Exception

        End Try


        VIENEDE_dto = "VER"
        Formcursos_descuento.Show()

    End Sub





    Private Sub EnaviarEmailToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles EnaviarEmailToolStripMenuItem.Click
        email_vienede = "Alumnos"
        Form_mail.Show()
        Form_mail.ComboBox1.Text = "Alumno"
        Form_mail.ComboBox2.Text = Me.TextBox2.Text
        Form_mail.ComboBox1.Enabled = False
        Form_mail.ComboBox2.Enabled = False
    End Sub




    Private Sub PruebaDeConocimientosToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles PruebaDeConocimientosToolStripMenuItem.Click
        Me.Cursor = Cursors.WaitCursor
        generarexamen()
        elimino_pdftmp = True
        Me.Cursor = Cursors.Default
    End Sub






    Private Sub Button8_Click(sender As Object, e As EventArgs) Handles Button8.Click

        Try
            Permisos.getPermiso("3", usrdoc)
            If Permisos.idpermiso = "" Then
                MsgBox("Acceso no disponible. consulte al Admistrador")
                Exit Sub
            End If

        Catch ex As Exception

        End Try

        If Me.TextBox16.Text = "" Then
            MsgBox("El alumno aún no tiene el certificado del RUNT,  no se puede graduar sin certificado RUNT.") : Exit Sub
            'If Me.Label23.Text = "PAGOS PENDIENTES" Then
            '    MsgBox("El curso tien un saldo pendiente verifique el estado financiero del curso.") : Exit Sub
            'End If
        Else

            MsgBox("Elija la Fecha de grado y haga clic en guardar.", vbInformation)
            Me.DateTimePicker2.Enabled = True
            Me.Button14.Visible = True
            Me.Button8.Visible = False

        End If
    End Sub

    Private Sub Button14_Click(sender As Object, e As EventArgs) Handles Button14.Click
        If Me.TextBox13.Text = "" Then MsgBox("No elijió fecha de grado", vbInformation) : Exit Sub

        sql = "UPDATE cursos SET estado='GRADUADO' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
        Me.Button14.Visible = False
        Me.DateTimePicker2.Enabled = False
        Me.TextBox3.Text = "GRADUADO"
    End Sub

    Private Sub DateTimePicker2_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker2.ValueChanged
        Me.TextBox13.Text = DateTimePicker2.Value.ToShortDateString
    End Sub

    Private Sub Button8_MouseEnter(sender As Object, e As EventArgs) Handles Button8.MouseEnter
        Me.ToolTip1.SetToolTip(Me.Button8, "Esta opción permite graduar el alumno, lo que quiere decir que ha finalizado su tramite por completo.")
        Me.ToolTip1.ToolTipTitle = "Consultar Cursos y Trámites."

    End Sub

    Private Sub Button11_Click(sender As Object, e As EventArgs)
    End Sub



    Private Sub ComboBox_instructores_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox_instructores.SelectedIndexChanged

    End Sub

    Private Sub Button15_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBox21_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBox21_TextChanged_1(sender As Object, e As EventArgs) Handles TextBox21.TextChanged

    End Sub

    Private Sub DataGridView1_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellContentClick

    End Sub


    Private Sub Button10_Click(sender As Object, e As EventArgs) Handles Button10.Click
        Try
            Permisos.getPermiso("3", usrdoc)
            If Permisos.idpermiso = "" Then
                MsgBox("Acceso no disponible. consulte al Admistrador")
                Exit Sub
            End If

        Catch ex As Exception



        End Try
    End Sub

    Private Sub TextBox22_TextChanged(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBoxComision_TextChanged(sender As Object, e As EventArgs) Handles TextBoxComision.TextChanged

    End Sub

    Private Sub TextBox4_TextChanged(sender As Object, e As EventArgs) Handles TextBox4.TextChanged

    End Sub

    Private Sub TextBoxUtilidad_TextChanged(sender As Object, e As EventArgs) Handles TextBoxUtilidad.TextChanged

    End Sub

    Private Sub DataGridView2_CellClick(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub GroupBox1_Enter(sender As Object, e As EventArgs) Handles GroupBox1.Enter

    End Sub

    Private Sub DataGridView2_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub PictureBox5_Click(sender As Object, e As EventArgs) Handles PictureBox5.Click

        Try
            Permisos.getPermiso("3", usrdoc)
            If Permisos.idpermiso = "" Then
                MsgBox("Acceso no disponible. consulte al Admistrador")
                Exit Sub
            End If

        Catch ex As Exception

        End Try



        ComboBoxServicio.Enabled = True
        ComboBoxPaquetes.Enabled = True
        ComboBox5.Enabled = True
        ComboBoxMedico.Enabled = True
        ComboBoxAsesor.Enabled = True
        ComboBox_instructores.Enabled = True
        ComboBox7.Enabled = True
        ComboBoxGrupo.Enabled = True
        PictureBox5.Visible = False
        PictureBoxOkEdit.Visible = True
        DateTimePicker3.Enabled = True

    End Sub

    Private Sub ComboBoxPaquetes_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxPaquetes.SelectedIndexChanged

    End Sub

    Private Sub DataGridView3_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView3.CellContentClick

    End Sub

    Private Sub ComboBox5_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox5.SelectedIndexChanged

    End Sub

    Private Sub GroupBox2_Enter(sender As Object, e As EventArgs) Handles GroupBox2.Enter

    End Sub

    Private Sub ComboBox7_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox7.SelectedIndexChanged

    End Sub

    Private Sub ComboBox_instructores_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBox_instructores.SelectionChangeCommitted
        sql = "UPDATE cursos SET instructor='" & ComboBox_instructores.SelectedValue & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub

    Private Sub ComboBoxGrupo_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxGrupo.SelectedIndexChanged

    End Sub

    Private Sub TextBox21_LostFocus(sender As Object, e As EventArgs) Handles TextBox21.LostFocus
        sql = "UPDATE cursos SET autorizacionmedico='" & TextBox21.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub

    Private Sub ComboBoxMedico_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBoxMedico.SelectedIndexChanged

    End Sub

    Private Sub TextBox4_LostFocus(sender As Object, e As EventArgs) Handles TextBox4.LostFocus
        sql = "UPDATE cursos SET observacion='" & TextBox4.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub

    Private Sub PictureBoxOkEdit_Click(sender As Object, e As EventArgs) Handles PictureBoxOkEdit.Click
        ComboBoxServicio.Enabled = False
        ComboBoxPaquetes.Enabled = False
        ComboBox5.Enabled = False
        ComboBoxMedico.Enabled = False
        ComboBoxAsesor.Enabled = False
        ComboBox_instructores.Enabled = False
        ComboBox7.Enabled = False
        ComboBoxGrupo.Enabled = False
        PictureBox5.Visible = True
        PictureBoxOkEdit.Visible = False
        DateTimePicker3.Enabled = False



        sql = "UPDATE cursos SET fecha_dertran='" & TextBox_F_D_T.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub

    Private Sub LinkLabel1_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel1.LinkClicked
        Try
            Permisos.getPermiso("3", usrdoc)
            If Permisos.idpermiso = "" Then
                MsgBox("Acceso no disponible. consulte al Admistrador")
                Exit Sub
            End If

        Catch ex As Exception

        End Try


        If usrdoc = "2" Or usrdoc = "3" Or usrdoc = "4" Then
            If itemCartera <> "" Then
                sql = "DELETE FROM cartera where id='" & itemCartera & "'"
                da = New MySqlDataAdapter(sql, conex)
                dt = New DataTable
                Try
                    da.Fill(dt)
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
                da.Dispose()
                dt.Dispose()
                conex.Close()
                itemCartera = ""
                DataGridView1.ClearSelection()


                If CInt(Me.LabelCurso.Text) < 2525 Then
                    actualiza_financiero_old()
                Else
                    actualiza_financiero()
                End If



                MsgBox("Se Eliminó el item seleccionado de la cartera")
            End If
        End If


    End Sub

    Private Sub TextBoxComision_LostFocus(sender As Object, e As EventArgs) Handles TextBoxComision.LostFocus
        sql = "UPDATE cursos SET asesorcomision='" & TextBoxComision.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()

        If Me.TextBoxComision.Text > 0 Then
            Dim comi As Integer = 0
            sql = "Select valor from cartera where curso='" & LabelCurso.Text & "' and concepto='COMISION'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            da.Fill(dt)
            For Each lin As DataRow In dt.Rows
                If Not IsDBNull(lin.Item("valor")) Then
                    comi = lin.Item("valor")
                End If
            Next
            da.Dispose()
            dt.Dispose()
            conex.Close()

            If comi < 0 Then
                TextBoxComision.Text = TextBoxComision.Text
                sql = "update cartera set valor='" & TextBoxComision.Text & "' where curso='" & LabelCurso.Text & "' and concepto='COMISION'"
                da = New MySqlDataAdapter(sql, conex)
                dt = New DataTable
                Try
                    da.Fill(dt)
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
                da.Dispose()
                dt.Dispose()
                conex.Close()
            End If
            If comi = 0 Then
                sql = "insert into cartera (fecha,curso,idservicio,concepto,descripcion,valor,estado) 
                        values ('" & TextBox10.Text & "','" & LabelCurso.Text & "','" & 0 & "','COMISION','COMISION','" & TextBoxComision.Text & "','PENDIENTE')"
                da = New MySqlDataAdapter(sql, conex)
                dt = New DataTable
                Try
                    da.Fill(dt)
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
                da.Dispose()
                dt.Dispose()
                conex.Close()
            End If
        End If


        If Me.TextBoxComision.Text = 0 Then
            sql = "DELETE FROM cartera where curso='" & LabelCurso.Text & "' and concepto='COMISION'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        End If

        If CInt(Me.LabelCurso.Text) < 2525 Then
            actualiza_financiero_old()
        Else
            actualiza_financiero()
        End If

    End Sub

    Private Sub ButtonReporteCicop_Click(sender As Object, e As EventArgs) Handles ButtonReporteCicop.Click
        Exit Sub

        'guardo reporte cicop
        sql = "INSERT INTO horas_cicop (curso,alumno,instructor,vehiculo,fecha,hora,fecha_r,hora_r,ReportadoPor,estado)" &
              "VALUES ('" & Me.TextBox10.Text & "')"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)

            ' MsgBox("Se Guardaron los datos del Cliente. ")
        Catch ex As Exception
            If ex.ToString.Contains("Duplicate entry") Then MsgBox("Ya existe Intructor registrado con ese código, verifique los datos.", vbExclamation)
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub

    Private Sub ToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles ToolStripMenuItem1.Click

    End Sub

    Private Sub PictureBox6_Click(sender As Object, e As EventArgs) Handles PictureBox6.Click

    End Sub

    Private Sub TextBox11_TextChanged(sender As Object, e As EventArgs) Handles TextBox11.TextChanged

    End Sub

    Private Sub TextBoxUtilidad_LostFocus(sender As Object, e As EventArgs) Handles TextBoxUtilidad.LostFocus
        'sql = "UPDATE cursos SET utilidad='" & TextBoxUtilidad.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        'da = New MySqlDataAdapter(sql, conex)
        'dt = New DataTable
        'Try
        '    da.Fill(dt)
        '    'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        'Catch ex As Exception
        '    MsgBox(ex.ToString)
        'End Try
        'da.Dispose()
        'dt.Dispose()
        'conex.Close()




        'If Me.TextBoxUtilidad.Text > 0 Then
        '    Dim comi As Integer = 0
        '    sql = "Select valor from cartera where curso='" & LabelCurso.Text & "' and concepto='UTILIDAD'"
        '    da = New MySqlDataAdapter(sql, conex)
        '    dt = New DataTable
        '    da.Fill(dt)
        '    For Each lin As DataRow In dt.Rows
        '        If Not IsDBNull(lin.Item("valor")) Then
        '            comi = lin.Item("valor")
        '        End If
        '    Next
        '    da.Dispose()
        '    dt.Dispose()
        '    conex.Close()

        '    If comi > 0 Then
        '        sql = "update cartera set valor='" & TextBoxUtilidad.Text & "' where curso='" & LabelCurso.Text & "' and concepto='UTILIDAD'"
        '        da = New MySqlDataAdapter(sql, conex)
        '        dt = New DataTable
        '        Try
        '            da.Fill(dt)
        '        Catch ex As Exception
        '            MsgBox(ex.ToString)
        '        End Try
        '        da.Dispose()
        '        dt.Dispose()
        '        conex.Close()
        '    End If
        '    If comi = 0 Then
        '        sql = "insert into cartera (fecha,curso,idservicio,concepto,descripcion,valor,estado) 
        '                values ('" & TextBox10.Text & "','" & LabelCurso.Text & "','" & 0 & "','UTILIDAD','UTILIDAD','" & TextBoxUtilidad.Text & "','PENDIENTE')"
        '        da = New MySqlDataAdapter(sql, conex)
        '        dt = New DataTable
        '        Try
        '            da.Fill(dt)
        '        Catch ex As Exception
        '            MsgBox(ex.ToString)
        '        End Try
        '        da.Dispose()
        '        dt.Dispose()
        '        conex.Close()
        '    End If
        'End If


        'If Me.TextBoxUtilidad.Text = 0 Then
        '    sql = "DELETE FROM cartera where curso='" & LabelCurso.Text & "' and concepto='UTILIDAD'"
        '    da = New MySqlDataAdapter(sql, conex)
        '    dt = New DataTable
        '    Try
        '        da.Fill(dt)
        '    Catch ex As Exception
        '        MsgBox(ex.ToString)
        '    End Try
        '    da.Dispose()
        '    dt.Dispose()
        '    conex.Close()
        'End If
        'actualiza_financiero()
    End Sub

    Private Sub TextBox8_TextChanged(sender As Object, e As EventArgs) Handles TextBox8.TextChanged

    End Sub

    Private Sub ComboBoxServicio_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBoxServicio.SelectionChangeCommitted

        Dim servval As Integer = 0
        sql = "Select valor from cartera where curso='" & LabelCurso.Text & "' and concepto='CERTIFICADO'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        For Each lin As DataRow In dt.Rows
            If Not IsDBNull(lin.Item("valor")) Then
                servval = lin.Item("valor")
            End If
        Next
        da.Dispose()
        dt.Dispose()
        conex.Close()


        Dim ServicioNuevo As New Servicios()

        ServicioNuevo.cosultar(ComboBoxServicio.SelectedValue)


        sql = "UPDATE cursos SET idserv='" & ServicioNuevo.cod & "', tipotramite='" & ServicioNuevo.Servicio & " " & ServicioNuevo.Cat & " $" & ServicioNuevo.Valor & "', categoria='" & ServicioNuevo.Cat & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()


        If servval = 0 Then
            sql = "insert into cartera(fecha,curso,idservicio,concepto,descripcion,valor,estado) 
                                values('" & TextBox10.Text & "','" & LabelCurso.Text & "','" & ComboBoxServicio.SelectedValue & "','CERTIFICADO','CERTIFICADO DE CONDUCCION','" & ServicioNuevo.Valor & "','PENDIENTE')"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        Else
            sql = "update cartera set idservicio='" & ServicioNuevo.cod & "', valor='" & ServicioNuevo.Valor & "', descripcion='" & ServicioNuevo.Servicio & "' where curso='" & LabelCurso.Text & "' and concepto='CERTIFICADO'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        End If



        ComboBoxServicio.Enabled = False

        If CInt(Me.LabelCurso.Text) < 2525 Then
            actualiza_financiero_old()
        Else
            actualiza_financiero()
        End If

    End Sub

    Private Sub TextBox12_TextChanged(sender As Object, e As EventArgs) Handles TextBox12.TextChanged

    End Sub

    Private Sub ComboBoxPaquetes_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBoxPaquetes.SelectionChangeCommitted
        Dim PAQval As Integer = 0
        sql = "Select valor from cartera where curso='" & LabelCurso.Text & "' and concepto='CLASES'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        For Each lin As DataRow In dt.Rows
            If Not IsDBNull(lin.Item("valor")) Then
                PAQval = lin.Item("valor")
            End If
        Next
        da.Dispose()
        dt.Dispose()
        conex.Close()



        Dim PaqueteNuevo As New Paquetes()

        prmt_paq = 0
        'consulto el valor del paquete
        If Me.ComboBoxPaquetes.Text <> "NO" Then
            If PaqueteNuevo.cosultar(Me.ComboBoxPaquetes.SelectedValue.ToString) Then
                prmt_paq = PaqueteNuevo.valor
            End If
        End If


        sql = "UPDATE cursos SET idpaquete='" & PaqueteNuevo.cod & "', paq_clases='" & Me.ComboBoxPaquetes.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()

        If PAQval = 0 Then
            sql = "insert into cartera(fecha,curso,idservicio,concepto,descripcion,valor,estado) 
                                values('" & TextBox10.Text & "','" & LabelCurso.Text & "','" & ComboBoxServicio.SelectedValue & "','CLASES','" & PaqueteNuevo.categoria & " " & PaqueteNuevo.clases & " CLASES" & "','" & prmt_paq & "','PENDIENTE')"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        Else

            sql = "update cartera set idservicio='" & PaqueteNuevo.cod & "', valor='" & prmt_paq & "', descripcion='" & PaqueteNuevo.categoria & " " & PaqueteNuevo.clases & " CLASES" & "' where curso='" & LabelCurso.Text & "' and concepto='CLASES'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        End If




        If Me.ComboBoxPaquetes.Text = "NO" Then
            sql = "DELETE From cartera where curso='" & LabelCurso.Text & "' and concepto='CLASES'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        End If


        ComboBoxPaquetes.Enabled = False

        If CInt(Me.LabelCurso.Text) < 2525 Then
            actualiza_financiero_old()
        Else
            actualiza_financiero()
        End If



    End Sub

    Private Sub TextBox9_TextChanged(sender As Object, e As EventArgs) Handles TextBox9.TextChanged

    End Sub

    Private Sub ComboBox5_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBox5.SelectionChangeCommitted
        Dim PINval As Integer = 0
        sql = "Select valor from cartera where curso='" & LabelCurso.Text & "' and concepto='PIN'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        For Each lin As DataRow In dt.Rows
            If Not IsDBNull(lin.Item("valor")) Then
                PINval = lin.Item("valor")
            End If
        Next
        da.Dispose()
        dt.Dispose()
        conex.Close()




        Me.Cursor = Cursors.WaitCursor
        Dim prmt_vrderechos = 0

        'consulto el valor del tramite de derechos de transito
        If Me.ComboBox5.Text = "NO APLICA" Then
            prmt_vrderechos = 0
        End If
        If Me.ComboBox5.Text = "SI" Then
            sql = "SELECT vr_dertran FROM parametros WHERE cod='" & "001" & "'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            da.Fill(dt)
            For Each lin As DataRow In dt.Rows
                prmt_vrderechos = CLng(lin.Item("vr_dertran"))
            Next
            da.Dispose()
            dt.Dispose()
            conex.Close()
        End If



        sql = "UPDATE cursos SET dertrans='" & Me.ComboBox5.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()

        If PINval = 0 Then
            sql = "insert into cartera(fecha,curso,idservicio,concepto,descripcion,valor,estado) 
                                values('" & TextBox10.Text & "','" & LabelCurso.Text & "','0','PIN','PIN','" & prmt_vrderechos & "','PENDIENTE')"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        Else
            sql = "update cartera set idservicio='0', valor='" & prmt_vrderechos & "', where curso='" & LabelCurso.Text & "' and concepto='PIN'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        End If


        If Me.ComboBox5.Text = "NO" Then
            sql = "DELETE From cartera where curso='" & LabelCurso.Text & "' and concepto='PIN'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        End If


        ComboBox5.Enabled = False

        If CInt(Me.LabelCurso.Text) < 2525 Then
            actualiza_financiero_old()
        Else
            actualiza_financiero()
        End If

    End Sub

    Private Sub TextBox22_TextChanged_1(sender As Object, e As EventArgs) Handles TextBox22.TextChanged

    End Sub

    Private Sub ComboBox7_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBox7.SelectionChangeCommitted
        sql = "UPDATE cursos SET prioridad='" & Me.ComboBox7.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()



        ComboBox7.Enabled = False

    End Sub

    Private Sub Button7_Click(sender As Object, e As EventArgs) Handles Button7.Click
        sql = "UPDATE cursos SET estadofinanciero='" & DateTime.Now.ToShortDateString & "'  WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()

        Button7.Visible = False
    End Sub

    Private Sub Button_Finish_sicop_Click(sender As Object, e As EventArgs) Handles Button_Finish_sicop.Click
        sql = "UPDATE cursos SET estadosicov='" & DateTime.Now.ToShortDateString & "'  WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()

        Button_Finish_sicop.Visible = False
    End Sub

    Private Sub Button_Finish_runt_Click(sender As Object, e As EventArgs) Handles Button_Finish_runt.Click



        sql = "UPDATE cursos SET estadorunt='" & DateTime.Now.ToShortDateString & "'  WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()

        Button_Finish_runt.Visible = False
    End Sub


    Private Sub TextBoxDescuento_TextChanged(sender As Object, e As EventArgs) Handles TextBoxDescuento.TextChanged

    End Sub


    Private Sub Label14_Click(sender As Object, e As EventArgs) Handles Label14.Click

    End Sub

    Private Sub DateTimePicker3_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker3.ValueChanged
        Me.TextBox_F_D_T.Text = DateTimePicker3.Value.ToShortDateString
    End Sub

    Private Sub ComboBox1_SelectedIndexChanged_1(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged

    End Sub

    Private Sub ComboBoxGrupo_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBoxGrupo.SelectionChangeCommitted
        sql = "UPDATE cursos SET grupo='" & Me.ComboBoxGrupo.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()



    End Sub

    Private Sub DataGridView6_CellContentClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView6.CellContentClick

    End Sub

    Private Sub TextBox6_TextChanged(sender As Object, e As EventArgs) Handles TextBox6.TextChanged

    End Sub

    Private Sub ComboBoxMedico_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBoxMedico.SelectionChangeCommitted

        Dim MEDval As Integer = 0
        sql = "Select valor from cartera where curso='" & LabelCurso.Text & "' and concepto='CERTIFICADO MEDICO'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        For Each lin As DataRow In dt.Rows
            If Not IsDBNull(lin.Item("valor")) Then
                MEDval = lin.Item("valor")
            End If
        Next
        da.Dispose()
        dt.Dispose()
        conex.Close()




        Dim MedicoNuevo As New Medicos()


        prmt_vrmedico = 0
        If ComboBoxMedico.Text <> "NO" Then
            If MedicoNuevo.cosultar(ComboBoxMedico.SelectedValue) Then
                prmt_vrmedico = MedicoNuevo.Valor
            End If
        End If

        sql = "UPDATE cursos SET idmedico='" & ComboBoxMedico.SelectedValue & "', medico='" & Me.ComboBoxMedico.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()


        If MEDval = 0 Then
            sql = "insert into cartera(fecha,curso,idservicio,concepto,descripcion,valor,estado) 
                                values('" & TextBox10.Text & "','" & LabelCurso.Text & "','" & ComboBoxMedico.SelectedValue & "','CERTIFICADO MEDICO','" & ComboBoxMedico.Text & "','" & prmt_vrmedico & "','PENDIENTE')"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        Else
            sql = "update cartera set idservicio='" & ComboBoxMedico.SelectedValue & "', valor='" & prmt_vrmedico & "', descripcion='" & ComboBoxMedico.Text & "' where curso='" & LabelCurso.Text & "' and concepto='CERTIFICADO MEDICO'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        End If



        If Me.ComboBoxMedico.Text = "NO" Then
            sql = "DELETE From cartera where curso='" & LabelCurso.Text & "' and concepto='CERTIFICADO MEDICO'"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            Try
                da.Fill(dt)
            Catch ex As Exception
                MsgBox(ex.ToString)
            End Try
            da.Dispose()
            dt.Dispose()
            conex.Close()
        End If


        ComboBoxMedico.Enabled = False

        If CInt(Me.LabelCurso.Text) < 2525 Then
            actualiza_financiero_old()
        Else
            actualiza_financiero()
        End If
    End Sub

    Private Sub DataGridView1_CellClick(sender As Object, e As DataGridViewCellEventArgs) Handles DataGridView1.CellClick
        Dim row As DataGridViewRow = DataGridView1.CurrentRow
        itemCartera = CStr(row.Cells("id").Value)


    End Sub

    Private Sub TextBox11_LostFocus(sender As Object, e As EventArgs) Handles TextBox11.LostFocus
        sql = "UPDATE cursos 
                SET alerta1_titulo='" & TextBox11.Text & "' 
                WHERE num=" & LabelCurso.Text & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub

    Private Sub TextBox8_LostFocus(sender As Object, e As EventArgs) Handles TextBox8.LostFocus
        sql = "UPDATE cursos 
                SET alerta1_txt='" & TextBox8.Text & "' 
                WHERE num=" & LabelCurso.Text & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub


    Private Sub TextBox12_LostFocus(sender As Object, e As EventArgs) Handles TextBox12.LostFocus
        sql = "UPDATE cursos 
                SET alerta2_titulo='" & TextBox12.Text & "' 
                WHERE num=" & LabelCurso.Text & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub


    Private Sub TextBox9_LostFocus(sender As Object, e As EventArgs) Handles TextBox9.LostFocus
        sql = "UPDATE cursos 
                SET alerta2_txt='" & TextBox9.Text & "' 
                WHERE num=" & LabelCurso.Text & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub


    Private Sub TextBox22_LostFocus(sender As Object, e As EventArgs) Handles TextBox22.LostFocus
        sql = "UPDATE cursos SET fechacomision='" & TextBox22.Text & "' WHERE num=" & CLng(Me.LabelCurso.Text) & ""
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            'MsgBox("Se actualizó el número del contrato en los datos del curso. Ahora se cargara e contrato.  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub

    Private Sub ComboBox1_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBox1.SelectionChangeCommitted

    End Sub
End Class