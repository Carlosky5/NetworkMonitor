Imports System.Net.NetworkInformation

Public Class Network

    Public Shared Property ActiveInterface As NetworkInterface = Nothing

    Public Shared Function GetInterfaces() As List(Of NetworkInterface)
        Return NetworkInterface.GetAllNetworkInterfaces().ToList()
    End Function

    Public Shared Function GetInterfaces(ParamArray ExcludeContainingName() As String) As List(Of NetworkInterface)
        Dim returnList As New List(Of NetworkInterface)
        For Each ni As NetworkInterface In GetInterfaces()
            Dim containExclusion As Boolean = False
            For Each exclusion As String In ExcludeContainingName
                If ni.Name.ToLower().Contains(exclusion.ToLower()) Then
                    containExclusion = True
                    Exit For
                End If
            Next
            If Not containExclusion Then
                returnList.Add(ni)
            End If
        Next
        Return returnList
    End Function

    Public Shared Function GetOperationalInterfaces() As List(Of NetworkInterface)
        Dim returnList As New List(Of NetworkInterface)
        For Each ni As NetworkInterface In GetInterfaces()
            If ni.OperationalStatus = OperationalStatus.Up Then
                returnList.Add(ni)
            End If
        Next
        Return returnList
    End Function

    Public Shared Function GetOperationalInterfaces(ParamArray ExcludeContainingName() As String) As List(Of NetworkInterface) 'Common exclusions: "Loopback", "Teredo"
        Dim returnList As New List(Of NetworkInterface)
        For Each ni As NetworkInterface In GetInterfaces()
            If ni.OperationalStatus = OperationalStatus.Up Then
                Dim containExclusion As Boolean = False
                For Each exclusion As String In ExcludeContainingName
                    If ni.Name.ToLower().Contains(exclusion.ToLower()) Then
                        containExclusion = True
                        Exit For
                    End If
                Next
                If Not containExclusion Then
                    returnList.Add(ni)
                End If
            End If
        Next
        Return returnList
    End Function

    Public Shared Function GetDownloadedBytes() As Long
        If ActiveInterface IsNot Nothing Then
            Return ActiveInterface.GetIPv4Statistics().BytesReceived
        End If
        Return 0
    End Function

    Public Shared Function GetUploadedBytes() As Long
        If ActiveInterface IsNot Nothing Then
            Return ActiveInterface.GetIPv4Statistics().BytesSent
        End If
        Return 0
    End Function

End Class
