Imports MySql.Data.MySqlClient

Public Class Formcursos_verprogclase

    Dim DT_instructores As DataTable
    Dim DA_instructores As MySqlDataAdapter

    Dim DT_vehiculos As DataTable
    Dim DA_vehiculos As MySqlDataAdapter


    Private Sub Formcursos_verprogclase_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DateTimePicker1.MinDate = Today()
        DateTimePicker1.Value = Today()
        Me.Label7.Text = Formcursos_ver.LabelCurso.Text
        Me.Label10.Text = Formcursos_ver.TextBox2.Text
        Me.Label8.Text = Formcursos_ver.TextBox1.Text
        Me.Label13.Text = Formcursos_ver.ComboBoxCat.Text



        'cargamos combo instructores

        sql = "SELECT documento, nombre from instructores"

        DA_instructores = New MySqlDataAdapter(sql, conex)
        DT_instructores = New DataTable
        DA_instructores.Fill(DT_instructores)
        Me.ComboBox3.DataSource = DT_instructores.DefaultView
        Me.ComboBox3.ValueMember = "documento"
        Me.ComboBox3.DisplayMember = "nombre"

        Dim topRow3 As DataRow = DT_instructores.NewRow()
        topRow3("nombre") = ""
        topRow3("documento") = ""
        DT_instructores.Rows.InsertAt(topRow3, 0)

        DA_instructores.Dispose()
        DT_instructores.Dispose()
        conex.Close()
        Me.ComboBox3.SelectedIndex = -1





        sql = "SELECT cod, concat(placa,': ',tipo,'  ',marca) as vehiculo  from vehiculos ORDER BY tipo"

        DA_vehiculos = New MySqlDataAdapter(sql, conex)
        DT_vehiculos = New DataTable
        DA_vehiculos.Fill(DT_vehiculos)

        Me.ComboBox2.DataSource = DT_vehiculos.DefaultView
        Me.ComboBox2.ValueMember = "cod"
        Me.ComboBox2.DisplayMember = "vehiculo"


        Dim topRow2 As DataRow = DT_vehiculos.NewRow()
        topRow2("vehiculo") = ""
        topRow2("cod") = ""
        DT_vehiculos.Rows.InsertAt(topRow2, 0)

        DA_vehiculos.Dispose()
        DT_vehiculos.Dispose()
        conex.Close()
        Me.ComboBox2.SelectedIndex = -1


    End Sub

    Private Sub DateTimePicker1_ValueChanged(sender As Object, e As EventArgs) Handles DateTimePicker1.ValueChanged
        Me.TextBox10.Text = DateTimePicker1.Value.ToShortDateString
        Me.ComboBox1.Enabled = True
        Me.ComboBox2.Enabled = True
        Me.ComboBox3.Enabled = True
        Me.ComboBox1.Text = Nothing
        Me.ComboBox2.Text = Nothing
        Me.ComboBox3.Text = Nothing
        sql = "SELECT * FROM horario_general WHERE fecha='" & Me.TextBox10.Text.ToString & "'"

        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)

        Me.DataGridView1.DataSource = dt

        Me.DataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView1.Columns(0).Visible = False
        Me.DataGridView1.Columns(1).Visible = False
        Me.DataGridView1.Columns(2).HeaderText = "HORA"
        Me.DataGridView1.Columns(2).Width = 100
        Me.DataGridView1.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView1.Columns(3).Visible = False
        Me.DataGridView1.Columns(4).HeaderText = "ALUMNO"
        Me.DataGridView1.Columns(4).Width = 250
        Me.DataGridView1.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Me.DataGridView1.Columns(5).HeaderText = "INSTRUCTOR"
        Me.DataGridView1.Columns(5).Width = 250
        Me.DataGridView1.Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Me.DataGridView1.Columns(6).HeaderText = "VEHICULO"
        Me.DataGridView1.Columns(6).Width = 200
        Me.DataGridView1.Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Me.DataGridView1.Columns(7).HeaderText = "ESTADO"
        Me.DataGridView1.Columns(7).Width = 200
        Me.DataGridView1.Columns(7).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        da.Dispose()
        dt.Dispose()
        conex.Close()
       
    End Sub

    Private Sub DataGridView4_CellContentClick(sender As Object, e As DataGridViewCellEventArgs)

    End Sub

    Private Sub TextBox10_TextChanged(sender As Object, e As EventArgs) Handles TextBox10.TextChanged

    End Sub

    Private Sub Button3_Click(sender As Object, e As EventArgs) Handles Button3.Click
        If ComboBox1.Text = Nothing Then MsgBox("Faltan Datos", vbInformation) : Exit Sub
        If ComboBox2.Text = Nothing Then MsgBox("Faltan Datos", vbInformation) : Exit Sub
        If ComboBox3.Text = Nothing Then MsgBox("Faltan Datos", vbInformation) : Exit Sub

        Dim PLACA = Split(Me.ComboBox2.Text, ":")

        'guardamos cursos_horario
        sql = "INSERT INTO cursos_clases (curso, fecha, hora, alumno, instructor, placa, estado)" &
                  " VALUES (" & CLng(Me.Label7.Text) & ",'" & Me.TextBox10.Text & "','" & Me.ComboBox1.Text & "','" & Me.Label10.Text & "','" & Me.ComboBox3.Text & "','" & PLACA(0) & "', '" & "PROGRAMADA" & "')"
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

        'guardamos horario general
        sql = "INSERT INTO horario_general (curso, fecha, hora, alumno, instructor, vehiculo, estado)" &
                  " VALUES (" & CLng(Me.Label7.Text) & ",'" & Me.TextBox10.Text & "','" & Me.ComboBox1.Text & "','" & Me.Label10.Text & "','" & Me.ComboBox3.Text & "','" & PLACA(0) & "', '" & "PROGRAMADA" & "')"
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

        sql = "SELECT * FROM horario_general WHERE fecha='" & Me.TextBox10.Text.ToString & "'"

        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)

        Me.DataGridView1.DataSource = dt

        Me.DataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView1.Columns(0).Visible = False
        Me.DataGridView1.Columns(1).Visible = False
        Me.DataGridView1.Columns(2).HeaderText = "HORA"
        Me.DataGridView1.Columns(2).Width = 100
        Me.DataGridView1.Columns(2).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        Me.DataGridView1.Columns(3).Visible = False
        Me.DataGridView1.Columns(4).HeaderText = "ALUMNO"
        Me.DataGridView1.Columns(4).Width = 250
        Me.DataGridView1.Columns(4).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Me.DataGridView1.Columns(5).HeaderText = "INSTRUCTOR"
        Me.DataGridView1.Columns(5).Width = 250
        Me.DataGridView1.Columns(5).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Me.DataGridView1.Columns(6).HeaderText = "VEHICULO"
        Me.DataGridView1.Columns(6).Width = 200
        Me.DataGridView1.Columns(6).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft
        Me.DataGridView1.Columns(7).HeaderText = "ESTADO"
        Me.DataGridView1.Columns(7).Width = 200
        Me.DataGridView1.Columns(7).DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter
        da.Dispose()
        dt.Dispose()
        conex.Close()
        Me.ComboBox1.Text = Nothing
        Me.ComboBox2.Text = Nothing
        Me.ComboBox3.Text = Nothing

        curso_log(CLng(Me.Label7.Text), DateTime.Now().ToString, usrnom.ToString, "Se programó clase: " & Me.TextBox10.Text.ToString & " " & Me.ComboBox1.Text.ToString & " " & Me.ComboBox2.Text.ToString & " " & Me.ComboBox3.Text.ToString)



    End Sub

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
      
    End Sub

    Private Sub ComboBox2_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox2.SelectedIndexChanged
        Dim valida_vehiculo, valida_hora As String
        Try
            For i As Integer = 0 To DataGridView1.RowCount - 1
                valida_vehiculo = CStr(DataGridView1.Item("vehiculo", i).Value)
                valida_hora = CStr(DataGridView1.Item("hora", i).Value)
                If valida_vehiculo = Me.ComboBox2.Text.ToString And valida_hora = Me.ComboBox1.Text.ToString Then MsgBox("Este Horario ya se ncuentra ocupado, seleccione otro.", vbExclamation) : Me.ComboBox2.Text = Nothing : Exit Sub
            Next
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try
    End Sub

    Private Sub ComboBox3_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox3.SelectedIndexChanged
        Dim valida_instructor, valida_hora As String
        Try
            For i As Integer = 0 To DataGridView1.RowCount - 1
                valida_instructor = CStr(DataGridView1.Item("instructor", i).Value)
                valida_hora = CStr(DataGridView1.Item("hora", i).Value)
                If valida_instructor = Me.ComboBox3.Text.ToString And valida_hora = Me.ComboBox1.Text.ToString Then MsgBox("Este Horario ya se ncuentra ocupado, seleccione otro.", vbExclamation) : Me.ComboBox3.Text = Nothing : Exit Sub
            Next
        Catch ex As Exception
            MsgBox(ex.Message.ToString)
        End Try
    End Sub

    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Me.Close()
    End Sub
End Class