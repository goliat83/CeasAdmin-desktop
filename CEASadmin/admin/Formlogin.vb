Imports MySql.Data.MySqlClient
Imports System.IO

Public Class Formlogin
    Dim user As String
    Dim pass As String
    Dim dataBase As miConex = New miConex()

    Dim DT_LOGIN As DataTable

    Public da_up As MySqlDataAdapter
    Public dt_up As DataTable
    Dim nuevaversion As String
    Dim VersionActual As String

    Dim LinkActualizacion As String

    Private Sub ComboBox1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ComboBox1.SelectedIndexChanged
 
        Me.TextBox1.Focus()
    End Sub

    Private Sub Formlogin_Activated(sender As Object, e As EventArgs) Handles Me.Activated
        
        
    End Sub

   


    Private Sub TextBox1_KeyDown(sender As Object, e As KeyEventArgs) Handles TextBox1.KeyDown
        If Me.TextBox1.Text = "" Then Exit Sub
        If e.KeyCode = 13 Then 'ENTRER
            Try
                sql = "SELECT * FROM usuarios WHERE password = '" & Me.TextBox1.Text.ToString & "' AND nombre='" & Me.ComboBox1.Text.ToString & "'"
                da = New MySqlDataAdapter(sql, conex)
                dt = New DataTable
                da.Fill(dt)
                For Each row As DataRow In dt.Rows
                    usrusr = row.Item("doc")
                    usrdoc = row.Item("doc")
                    usrusr = row.Item("usuario")
                    usrnom = row.Item("nombre")
                    usrpass = row.Item("password")
                    usrtipo = row.Item("tipo")
                Next
            Catch ex As Exception
                MsgBox(ex.Message)
            End Try
            conex.Close()
            da.Dispose()
            dt.Dispose()
            If Me.TextBox1.Text = usrpass Then
                'Form_main.Show()
                Formprincipal.Show()
                Me.Cursor = Cursors.Default
                Me.Close()
            Else
                MsgBox("Esa no es su contraseña.  Revise e Intente de Nuevo.", vbExclamation)
                Me.TextBox1.Text = ""
                Me.TextBox1.Focus()
                Me.Cursor = Cursors.Default
            End If
        End If
    End Sub


  


    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click


        If LabelVA.Text <> LabelVN.Text Then
            MsgBox("debe actualizar el sistema")
            Exit Sub
        End If



        Dim user As String = ComboBox1.Text

        If Me.TextBox1.Text = Nothing Then MsgBox("Digite una contraseña.", vbInformation) : Exit Sub
        Button1.Enabled = False
        Try
            sql = "SELECT * FROM usuarios WHERE password = '" & TextBox1.Text & "' AND nombre='" & user & "';"
            da = New MySqlDataAdapter(sql, conex)
            dt = New DataTable
            da.Fill(dt)
            For Each row As DataRow In dt.Rows
                usrdoc = row.Item("doc")
                usrusr = row.Item("usuario")
                usrnom = row.Item("nombre")
                usrpass = row.Item("password")
                usrtipo = row.Item("tipo")
            Next
        Catch ex As Exception
            MsgBox(ex.Message)
        End Try
        conex.Close()
        da.Dispose()
        dt.Dispose()






        If Me.TextBox1.Text = usrpass Then
            'Form_main.Show()
            Formprincipal.Show()
            Me.Cursor = Cursors.Default
            Me.Close()
        Else
            MsgBox("Esa no es su contraseña.  Revise e Intente de Nuevo.", vbExclamation)
            Me.TextBox1.Text = ""
            Me.TextBox1.Focus()
            Me.Cursor = Cursors.Default
        End If
    End Sub


    Private Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Me.TextBox1.Text = "PRINCIPAL"
        Me.TextBox1.Focus()
        SendKeys.Send("{ENTER}")


    End Sub


    Private Sub Button4_Click(sender As Object, e As EventArgs) Handles Button4.Click
        sql = "UPDATE cursos SET medico='SI' WHERE medico='001 - SI - PENDIENTE'"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        Try
            da.Fill(dt)
            MsgBox("Listo, Hecho  :).", vbInformation)
        Catch ex As Exception
            MsgBox(ex.ToString)
        End Try
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub

    Private Sub Timer1_Tick(sender As Object, e As EventArgs) Handles Timer1.Tick
        Timer1.Enabled = False

        LOADDATAPARK()

        'cargar logo y datos del park
        'Dim exists As Boolean = False
        'exists = System.IO.File.Exists("C:\CEASadmin\" & aca_logoname)
        'If exists = True Then
        '    imagenlogocea = Image.FromFile("C:\CEASadmin\" & aca_logoname)
        '    Me.PictureBox1.Image = imagenlogocea
        'Else
        '    imagenlogocea = Image.FromFile("C:\CEASadmin\logoaca.jpg")
        'End If
        'Me.PictureBox1.Visible = True


        'cargamos combo Usuarios
        'Try
        '    sql = "SELECT usuario, nombre FROM usuarios"
        '    da = New MySqlDataAdapter(sql, conex)
        '    dt = New DataTable
        '    da.Fill(dt)
        '    Dim comboitem As DataRow
        '    For Each comboitem In dt.Rows
        '        Me.ComboBox1.Items.Add(comboitem.Item("nombre"))
        '    Next
        '    Me.ComboBox1.Enabled = True
        'Catch ex As Exception
        '    MsgBox(ex.ToString)
        '    da.Dispose()
        '    dt.Dispose()
        '    conex.Close()
        'End Try
        'da.Dispose()
        'dt.Dispose()
        'conex.Close()



        Dim ConnRC As miConex = New miConex()

        Try
            DT_LOGIN = ConnRC.Buscar("SELECT usuario, nombre FROM usuarios")
            ComboBox1.DataSource = DT_LOGIN.DefaultView
            ComboBox1.DisplayMember = "nombre"
            ComboBox1.ValueMember = "usuario"
        Catch ex As Exception
            MsgBox("error listando cuentas de caja.", ex.ToString)
            Exit Sub
        End Try


        Try
            sql = "SELECT * FROM actualizaciones where software='CEASADMIN' and documento='" & "12345" & "'"
            da_up = New MySqlDataAdapter(sql, conex_miclick)
            dt_up = New DataTable
            da_up.Fill(dt_up)
            For Each row As DataRow In dt_up.Rows
                nuevaversion = Strings.Replace((row.Item("version").ToString), ".", "")
                LinkActualizacion = row.Item("link")

            Next
        Catch ex As Exception
            'If ex.ToString.Contains("timeout") = True Then MsgBox("el servidor esta fuera de linea")
            da_up.Dispose()
            dt_up.Dispose()
            conex_miclick.Close()
        End Try
        da_up.Dispose()
        dt_up.Dispose()
        conex_miclick.Close()
        LabelNuevaVersion.Text = nuevaversion


        Dim VersionActual As String
        VersionActual = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString



        Dim NV, VA As String

        VA = VersionActual
        NV = nuevaversion


        LabelNuevaVersion.Text = "Versión Nueva: " + NV.Replace(".", "")
        LabelVersionActual.Text = "Version Actual: " + VA.Replace(".", "")

        LabelVA.Text = VA.Replace(".", "")
        LabelVN.Text = NV.Replace(".", "")

        LabelNuevaVersion.Visible = True
        LabelVersionActual.Visible = True
        Me.Cursor = Cursors.Default

    End Sub

    Private Sub Formlogin_Shown(sender As Object, e As EventArgs) Handles Me.Shown

        Dim disco As New System.Management.ManagementObject("Win32_PhysicalMedia='\\.\PHYSICALDRIVE0'")
        Dim SERIAL_DD As String
        Try
            'MessageBox.Show(disco.Properties("SerialNumber").Value.ToString, "Número de Serie", MessageBoxButtons.OK, MessageBoxIcon.Information)
            SERIAL_DD = disco.Properties("SerialNumber").Value.ToString
            SERIAL_DD = Trim(SERIAL_DD)
            Label3.Text = SERIAL_DD
        Catch ex As Exception
            MessageBox.Show(ex.Message)
        End Try

        If SERIAL_DD = "0000_0000_0100_0000_4CE0_0018_DD8C_9084." Or SERIAL_DD = "E823_8FA6_BF53_0001_001B_448B_4827_DF5D." Then
            conex = conex_conti_dev
            Label1.Visible = True
            Labeltiposas.Visible = False
        End If


        VERIFICAR_CONEXION_LOCAL()

        If ESTADO_CONEXION_LOCAL = "OK" Then
            Me.Timer1.Enabled = True
        Else
            MsgBox("HAY UN ERROR EN LA COMUNICACION CON LA BASE DE DATOS LOCAL, POR FAVOR COMUNIQUESE CON SOPORTE TECNICO.", vbExclamation)
        End If



    End Sub

    Private Sub Formlogin_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        If File.Exists("c:\ceasadmin\pc.txt") = True Then

            If My.Computer.FileSystem.ReadAllText("c:\ceasadmin\pc.txt") = "c" Then
                conex = conex_conti
                Labeltiposas.Visible = False
            End If

            If My.Computer.FileSystem.ReadAllText("c:\ceasadmin\pc.txt") = "s" Then
                conex = conex_contisas
                Labeltiposas.Visible = True
            End If
        End If

    End Sub

    Private Sub PictureBox2_Click(sender As Object, e As EventArgs)

    End Sub

    Private Sub TextBox1_TextChanged(sender As Object, e As EventArgs) Handles TextBox1.TextChanged

    End Sub

    Private Sub LinkLabel3_LinkClicked(sender As Object, e As LinkLabelLinkClickedEventArgs) Handles LinkLabel3.LinkClicked
        Form_update.Show()

    End Sub

    Private Sub ComboBox1_SelectionChangeCommitted(sender As Object, e As EventArgs) Handles ComboBox1.SelectionChangeCommitted

    End Sub

    Private Sub CheckBox1_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBox1.CheckedChanged
        If CheckBox1.Checked = True Then
            conex = conex_conti_dev
            VERIFICAR_CONEXION_LOCAL()

            If ESTADO_CONEXION_LOCAL = "OK" Then
                Me.Timer1.Enabled = True
            Else
                MsgBox("HAY UN ERROR EN LA COMUNICACION CON LA BASE DE DATOS LOCAL, POR FAVOR COMUNIQUESE CON SOPORTE TECNICO.", vbExclamation)
            End If
        End If
    End Sub
End Class