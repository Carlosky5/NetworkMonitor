Public Class CustomKeys

    Dim _up As Boolean = False
    Dim _down As Boolean = False

    Public Property KeyValue As Boolean = False

    Private ReadOnly Property Up As Boolean
        Get
            If Down AndAlso Not KeyValue Then
                _up = True
            End If
            Return _up
        End Get
    End Property

    Private ReadOnly Property Down As Boolean
        Get
            If KeyValue Then
                _down = True
            End If
            Return _down
        End Get
    End Property

    Public ReadOnly Property Pressed As Boolean
        Get
            Dim result As Boolean = False
            If Down And Up Then
                result = True
                _up = False
                _down = False
            End If
            Return result
        End Get
    End Property

End Class