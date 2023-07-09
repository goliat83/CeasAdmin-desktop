Imports System.ComponentModel
Imports System.IO
Imports MySql.Data.MySqlClient
Imports System.IO.Compression
Public Class Form_update
    Public da_up As MySqlDataAdapter
    Public dt_up As DataTable
    Dim nuevaversion As String
    Dim VersionActual As String

    Dim LinkActualizacion As String

    Private Sub Form_update_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        nuevaversion = "?"
        LabelVactual.Text = Strings.Replace(VersionActual, ".", "")
        LabelVnueva.Text = "?"

    End Sub
    Private Sub Form_update_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        VERIFICAR_CONEXION_REMOTA()

        If ESTADO_CONEXION_REMOTA = True Then
            Me.PictureBox_up_ok.Image = My.Resources.loading_trans
            Me.PictureBox_up_ok.Visible = True
            Label_info_update.Text = "Buscando actualizaciones..."
            Label_info_update.Visible = True
            Timer_update.Enabled = True

        End If

        If ESTADO_CONEXION_REMOTA = False Then
            Label_info_update.Visible = True

            Label_info_update.Text = "no esta conectado a internet..."
            Me.PictureBox_up_ok.Image = My.Resources.exclamation
            button_descargar.Visible = True : button_descargar.Text = "Buscar"

        End If

    End Sub


    Public Sub VERIFICAR_CONEXION_REMOTA()
        ESTADO_CONEXION_REMOTA = False

        If My.Computer.Network.IsAvailable() Then
            Try
                If My.Computer.Network.Ping("www.google.com", 1000) Then
                    ESTADO_CONEXION_REMOTA = True
                Else
                    ESTADO_CONEXION_REMOTA = False
                End If
            Catch ex As Exception
                ESTADO_CONEXION_REMOTA = False

            End Try
        Else
            ESTADO_CONEXION_REMOTA = False
        End If
    End Sub


    Private Sub Timer_update_Tick(sender As Object, e As EventArgs) Handles Timer_update.Tick
        Timer_update.Enabled = False


        If ESTADO_CONEXION_REMOTA = True Then
            ' INICIA EL BACKGROUNDWORKER D EB USCAR ACTUIALIZACION
        End If


        If Background_up.IsBusy <> True Then
            Background_up.WorkerReportsProgress = True
            Background_up.WorkerSupportsCancellation = True
            Background_up.RunWorkerAsync()
        End If
    End Sub
    Public Sub buscar_actualizaciones()
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
        LabelVnueva.Text = nuevaversion
    End Sub

    Private Sub Background_up_DoWork(sender As Object, e As DoWorkEventArgs) Handles Background_up.DoWork
        VERIFICAR_CONEXION_REMOTA()

        If ESTADO_CONEXION_REMOTA = True Then

            Try

                buscar_actualizaciones()

            Catch ex As Exception
                button_descargar.Visible = True : button_descargar.Text = "Buscar"
                Label_info_update.Text = "no se pudo conectar con el servidor ...."
                Me.PictureBox_up_ok.Image = My.Resources.exclamation

            Finally

            End Try

        End If

        If Background_up.CancellationPending Then Exit Sub

    End Sub

    Public Function VersionApp() As String

        VersionActual = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString

        Return VersionActual
    End Function

    Private Sub Background_up_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles Background_up.RunWorkerCompleted
        VersionActual = VersionApp()
        Dim NV As String = nuevaversion
        Dim VA As String = VersionActual



        NV = NV.Replace(".", "")
        VA = VA.Replace(".", "")


        LabelVactual.Text = VA
        LabelVnueva.Text = NV


        If CInt(NV) > CInt(VA) Then

            If nuevaversion = "" Then Exit Sub
            LabelVnueva.Text = nuevaversion

            'MsgBox("hay actualizacion " & Chr(13) & "*" & SERIAL_DD & "*", vbInformation)
            Label_info_update.Text = "Una nueva actualización está disponible...."
            Panel1.Visible = True
            button_descargar.Visible = True : button_descargar.Text = "Descargar"
            Me.PictureBox_up_ok.Image = My.Resources.ok_trans
            Me.PictureBox_up_ok.Visible = True

            Me.Label_info_update.Visible = True

            'Me.PictureBox_up_ok.Image = My.Resources.Informaci
            'Me.PictureBox_up_ok.Visible = True

            'elimina si hay actualizacion
            If File.Exists("c:\ceasadmin\setup.zip") Then
                My.Computer.FileSystem.DeleteFile("c:\radocsoft\setup.zip")
            End If
            If File.Exists("c:\ceasadmin\setup" & nuevaversion & ".zip") Then
                My.Computer.FileSystem.DeleteFile("c:\radocsoft\setup" & nuevaversion & ".zip")
            End If
            If File.Exists("c:\ceasadmin\setup" & VersionActual & ".zip") Then
                My.Computer.FileSystem.DeleteFile("c:\ceasadmin\setup" & VersionActual & ".zip")
            End If
            If Directory.Exists("c:\ceasadmin\setup\") Then
                Try
                    My.Computer.FileSystem.DeleteDirectory("c:\ceasadmin\setup\", FileIO.DeleteDirectoryOption.DeleteAllContents)
                Catch ex As Exception
                    MsgBox(ex.ToString)
                End Try
            End If
        Else
            Label_info_update.Text = "no se encontraron actualizaciones."
            Me.PictureBox_up_ok.Image = My.Resources.exclamation
            Me.PictureBox_up_ok.Visible = True
            LabelVactual.Text = VA
            LabelVnueva.Text = NV

            button_descargar.Text = "OK"
        End If
    End Sub



    Private Sub button_descargar_Click(sender As Object, e As EventArgs) Handles button_descargar.Click


        If button_descargar.Text = "Descargar" Then
            Label_info_update.Text = "descarganddo actualizacion, por favor espere."

            If File.Exists("c:\ceasadmin\CEAS" & Strings.Replace(VersionActual, ".", "") & ".zip") Then
                My.Computer.FileSystem.DeleteFile("c:\ceasadmin\CEAS" & VersionActual & ".zip")
            End If

            Me.Label_info_update.Text = "Descargando actualización..."
            Me.PictureBox_up_ok.Image = My.Resources.loading_trans
            Me.PictureBox_up_ok.Visible = True
            button_descargar.Visible = False
            ' INICIA EL BACKGROUNDWORKER
            BackgroundWorker_up_do.WorkerReportsProgress = True
            BackgroundWorker_up_do.WorkerSupportsCancellation = True
            BackgroundWorker_up_do.RunWorkerAsync()

        End If


        If button_descargar.Text = "Instalar" Then
            If Directory.Exists("c:\ceasadmin\setup") = True Then
                My.Computer.FileSystem.DeleteDirectory("c:\ceasadmin\setup", FileIO.UIOption.OnlyErrorDialogs, FileIO.RecycleOption.DeletePermanently, FileIO.UICancelOption.ThrowException)
            End If
            Me.Label_info_update.Text = "Instalando actualización..."

            Me.PictureBox_up_ok.Image = My.Resources.loading_trans
            Me.PictureBox_up_ok.Visible = True


            ' INICIA EL BACKGROUNDWORKER
            BackgroundWorker_install.WorkerReportsProgress = True
            BackgroundWorker_install.WorkerSupportsCancellation = True
            BackgroundWorker_install.RunWorkerAsync()

        End If


    End Sub

    Private Sub BackgroundWorker_up_do_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker_up_do.DoWork
        Try
            My.Computer.Network.DownloadFile(LinkActualizacion, "c:\ceasadmin\setup\CEAS" & nuevaversion & ".zip", False, 360000)

            If BackgroundWorker_up_do.CancellationPending Then Exit Sub
        Catch ex As Exception
            If ex.ToString.Contains("404") Then
                MsgBox("no se encontró el archivo remoto")
                Exit Sub
            End If
        End Try

    End Sub

    Private Sub BackgroundWorker_up_do_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker_up_do.RunWorkerCompleted
        If nuevaversion <> VersionActual Then
            If nuevaversion = "" Then Exit Sub
            'MsgBox("hay actualizacion " & Chr(13) & "*" & SERIAL_DD & "*", vbInformation)

            Me.Label_info_update.Visible = True
            Me.Label_info_update.Text = "se completó la descarga ahora puede instalarla."
            Me.PictureBox_up_ok.Image = My.Resources.ok_trans
            Me.PictureBox_up_ok.Visible = True
            button_descargar.Text = "Instalar"
            button_descargar.Visible = True
        End If
    End Sub

    Private Sub BackgroundWorker_install_DoWork(sender As Object, e As DoWorkEventArgs) Handles BackgroundWorker_install.DoWork
        Dim extractPath As String = "c:\ceasadmin\setup\"
        Try
            ZipFile.ExtractToDirectory("c:\ceasadmin\CEAS" & nuevaversion & ".zip", extractPath)
        Catch ex As Exception
        End Try

        If BackgroundWorker_install.CancellationPending Then Exit Sub
    End Sub

    Private Sub BackgroundWorker_install_RunWorkerCompleted(sender As Object, e As RunWorkerCompletedEventArgs) Handles BackgroundWorker_install.RunWorkerCompleted
        If File.Exists("c:\ceasadmin\CEAS" & VersionActual & ".zip") Then
            My.Computer.FileSystem.DeleteFile("c:\ceasadmin\CEAS" & VersionActual & ".zip")
        End If
        If File.Exists("c:\ceasadmin\CEAS" & nuevaversion & ".zip") Then
            My.Computer.FileSystem.DeleteFile("c:\ceasadmin\CEAS" & nuevaversion & ".zip")
        End If
        Try
            Process.Start("c:\ceasadmin\setup\setup.exe")

        Catch ex As Exception

        End Try
        End
    End Sub

    Private Sub PictureBox4_Click(sender As Object, e As EventArgs) Handles PictureBox4.Click

    End Sub
End Class