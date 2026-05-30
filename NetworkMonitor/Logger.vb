Imports System.Text

Public Class Logger

    Private Shared ReadOnly logPath As String = Application.StartupPath & "\errorLog.txt"

    Shared Sub Log(message As String, method As String)
        Try
            Dim sb As New StringBuilder
            sb.AppendLine("**********Start*********")
            sb.AppendLine("")
            sb.AppendLine("Date: " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss"))
            sb.AppendLine("")
            sb.AppendLine("Message: " & message)
            sb.AppendLine("")
            sb.AppendLine("Method: " & method)
            sb.AppendLine("")
            sb.AppendLine("***********End***********")
            sb.AppendLine("")

            My.Computer.FileSystem.WriteAllText(logPath, sb.ToString(), True)
        Catch
        End Try
    End Sub

    Shared Sub Log(ex As Exception, method As String, Optional message As String = "")
        Try

            Dim sb As New StringBuilder
            sb.AppendLine("**********Start*********")
            sb.AppendLine("")
            sb.AppendLine("Date: " & Date.Now.ToString("dd/MM/yyyy HH:mm:ss"))
            sb.AppendLine("")
            If Not String.IsNullOrEmpty(message) Then
                sb.AppendLine("Message: " & message)
                sb.AppendLine("")
            End If
            sb.AppendLine("Method: " & method)
            sb.AppendLine("")
            sb.AppendLine("Exception: " & ex.Message)
            sb.AppendLine("")
            If ex.InnerException IsNot Nothing Then
                sb.AppendLine("Inner Exception: " & ex.InnerException.Message)
                sb.AppendLine("")
            End If
            sb.AppendLine("Stack Trace:")
            sb.AppendLine(ex.StackTrace)
            sb.AppendLine("")
            sb.AppendLine("***********End***********")
            sb.AppendLine("")

            My.Computer.FileSystem.WriteAllText(logPath, sb.ToString(), True)
        Catch
        End Try
    End Sub

End Class
