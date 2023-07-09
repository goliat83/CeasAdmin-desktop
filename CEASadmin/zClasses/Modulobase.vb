Imports MySql.Data
Imports MySql.Data.MySqlClient
Imports System.Net.Mail

Module Modulobase
    'Public database_string As String = My.Computer.FileSystem.ReadAllText("c:\ceas_admin\setup\auto_data_base.txt", System.Text.Encoding.UTF32)
    'Public user_string As String = My.Computer.FileSystem.ReadAllText("c:\ceas_admin\setup\auto_data_user.txt", System.Text.Encoding.UTF32)
    'Public pass_string As String = My.Computer.FileSystem.ReadAllText("c:\ceas_admin\setup\auto_data_pass.txt", System.Text.Encoding.UTF32)

    'Public conex As New MySqlConnection("data source=localhost; user id=root; password='Radiobemba';database=ceas_continental; port=3306; sslmode=none")
    Public ind As Integer
    Public conex As New MySqlConnection("")
    Public conex_miclick As New MySqlConnection("data source=75.102.22.98; user id=bhhurjmu_root; password='Radiob3mba@_';database=bhhurjmu_infovictimas; port=3306; Sslmode=none;")
    Public conex_conti As New MySqlConnection("data source=75.102.22.98; user id=bhhurjmu_contiroot; password='_rWas663';database=bhhurjmu_contibd; port=3306; Sslmode=none;")
    Public conex_contisas As New MySqlConnection("data source=75.102.22.98; user id=bhhurjmu_contiroot; password='_rWas663';database=bhhurjmu_contibd_sas; port=3306; Sslmode=none;")
    Public conex_conti_dev As New MySqlConnection("data source=75.102.22.98; user id=bhhurjmu_contiroot; password='_rWas663';database=bhhurjmu_contibd_dev; port=3306; Sslmode=none;")
    Public conex_local As New MySqlConnection("data source=localhost; user id=root; password='Radiobemba';database=ceas_continental; port=3306; sslmode=none")
    Public conex_temp As New MySqlConnection("data source=75.102.22.98; user id=bhhurjmu_contiroot; password='_rWas663';database=bhhurjmu_contibd_dev; port=3306; Sslmode=none;")

    'net conex
    'Public conex As New MySqlConnection("data source=192.168.0.185; user id=goliat; password='goliat';database=ceas_continental; port=3306; sslmode=none")




    'Public conex As New MySqlConnection("data source=PRINCIPAL; user id=goliat; password='goliat';database=ceas_continental; port=3306; sslmode=none")


    Public da As MySqlDataAdapter
    Public da_COMBO_INFORMEDIARIO As MySqlDataAdapter
    Public da_GRIDDIARIOINGRESOS As MySqlDataAdapter
    Public da_GRIDDIARIOEGRESOS As MySqlDataAdapter



    Public dt As DataTable
    Public dt_COMBO_INFORMEDIARIO As DataTable
    Public dt__GRIDDIARIOINGRESOS As DataTable
    Public dt__GRIDDIARIOEGRESOS As DataTable

    Public sql As String
    Public ds As DataSet
    Public dr As MySqlDataReader
    Public cmd As New MySqlCommand

    Public ESTADO_CONEXION_LOCAL As String = ""
    Public ESTADO_CONEXION_REMOTA As String = ""



    Public Sub VERIFICAR_CONEXION_LOCAL()
        Try
            conex.Open()
            ESTADO_CONEXION_LOCAL = "OK"
        Catch ex As Exception
            ESTADO_CONEXION_LOCAL = "NO"
            'MsgBox(ex.ToString)
        End Try
    End Sub


    Public Sub LOADDATAPARK()
        ' load datos del parqueadero
        'Try
        '    sql = "SELECT * FROM parametros WHERE COD = '001'"
        '    da = New MySqlDataAdapter(sql, conex)
        '    dt = New DataTable
        '    da.Fill(dt)

        '    For Each row As DataRow In dt.Rows
        '        SQLSOURCE = row.Item("datasource")
        '        aca_nom = row.Item("nombre")
        '        aca_nom2 = row.Item("nombre2")
        '        aca_dirs = row.Item("direccion")
        '        aca_nit = row.Item("identificacion")
        '        aca_tels = row.Item("telefono")
        '        aca_cels = row.Item("celular")
        '        aca_regimen = row.Item("regimen")
        '        aca_prop = row.Item("PROPIETARIO")
        '        aca_mail = row.Item("mail")
        '        aca_lic_min = row.Item("lic_min")
        '        aca_lic_sec = row.Item("lic_sec")
        '        aca_web = row.Item("web")
        '        dian_res = row.Item("dian_res")
        '        dian_fecha = row.Item("dian_fecha")
        '        dian_rango = row.Item("dian_rango")
        '        aca_logoname = row.Item("logo")
        '    Next

        'Catch ex As Exception
        '    MsgBox(ex.Message)
        '    ' If ex.ToString.Contains("Duplicate entry") Then MsgBox("Ya existe una Mensualidad para esa PALCA.", vbInformation)
        'End Try

        'conex.Close()
        'da.Dispose()
        'dt.Dispose()



        Dim miconex = New miConex()
        If miconex.ConexTest Then
            Dim dt As DataTable = miconex.Buscar("SELECT * FROM parametros WHERE COD = '001'")
            For Each row As DataRow In dt.Rows
                SQLSOURCE = row.Item("datasource")
                aca_nom = row.Item("nombre")
                aca_nom2 = row.Item("nombre2")
                aca_dirs = row.Item("direccion")
                aca_nit = row.Item("identificacion")
                aca_tels = row.Item("telefono")
                aca_cels = row.Item("celular")
                aca_regimen = row.Item("regimen")
                aca_prop = row.Item("propietario")
                aca_mail = row.Item("mail")
                aca_lic_min = row.Item("lic_min")
                aca_lic_sec = row.Item("lic_sec")
                aca_web = row.Item("web")
                dian_res = row.Item("dian_res")
                dian_fecha = row.Item("dian_fecha")
                dian_rango = row.Item("dian_rango")
                aca_logoname = row.Item("logo")
            Next
            dt.Dispose()
        End If


    End Sub
    Public Sub llenagridalumnos()
        sql = "SELECT * FROM alumnos"
        da = New MySqlDataAdapter(sql, conex)
        dt = New DataTable
        da.Fill(dt)
        Formalumnos.DataGridView1.DataSource = dt
        da.Dispose()
        dt.Dispose()
        conex.Close()
    End Sub


    Public Sub llenagridcursos(ByVal estado As String)

        '' consulto filtrando segun el estado sino consulta todo
        'sql = "SELECT num, fecha, alumno, categoria, estado, recomendado FROM cursos WHERE estado='" & estado & "' ORDER BY num"
        'If estado = "todos" Then sql = "SELECT num, fecha, alumno, categoria, estado, recomendado FROM cursos ORDER BY num"

        'da = New MySqlDataAdapter(sql, conex)
        'dt = New DataTable
        'da.Fill(dt)

        'Formcursos.DataGridView1.DataSource = dt

        'da.Dispose()
        'dt.Dispose()
        'conex.Close()
    End Sub
    Public Sub curso_log(ByVal curso As Long, ByVal fecha As String, ByVal user As String, ByVal log As String)
        'guardamos cursos_financiero
        sql = "INSERT INTO cursos_logs (curso, fecha, usuario, log)" & _
                  " VALUES (" & CLng(curso) & ",'" & fecha & "','" & user & "','" & log & "')"
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

    'Public Sub enviarmail()
    '    Try
    '        sql = "SELECT * FROM concejales"
    '        da = New MySqlDataAdapter(sql, conex)
    '        dt = New DataTable
    '        da.Fill(dt)

    '        For Each row As DataRow In dt.Rows
    '            Dim message As New MailMessage
    '            Dim smtp As New SmtpClient

    '            message.From = New MailAddress("ventanillaunica@concejodeibague.gov.co")

    '            message.To.Add(row.Item("mail"))
    '            message.Body = ("Buen día, Esta es una notificación del Concejo Municipal De Ibagué " + Chr(13) + _
    '                            "Radicado No. " + mrad + Chr(13) + _
    '                            "Fecha " + mfecha + Chr(13) + _
    '                            "Remitente " + msolicita + Chr(13) + _
    '                            "Asunto: " + masunto + Chr(13))
    '            message.Subject = ("Concejo Municipal de Ibagué.")
    '            message.Attachments.Add(New Attachment(files))

    '            message.Priority = MailPriority.Normal

    '            smtp.EnableSsl = True
    '            smtp.Port = "587"
    '            smtp.Host = "smtp.gmail.com"

    '            smtp.Credentials = New Net.NetworkCredential("ventanillaunica@concejodeibague.gov.co", "Colombia2015")
    '            smtp.Send(message)
    '        Next

    '    Catch ex As Exception
    '        MsgBox(ex.Message)

    '    End Try

    '    conex.Close()
    '    da.Dispose()
    '    dt.Dispose()
    'End Sub
End Module
