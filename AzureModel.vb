Imports System.ComponentModel
Imports System.Runtime.CompilerServices
Imports Microsoft.WindowsAzure.MobileServices

Namespace Models
    Public Class TodoItem
        Public Property Id As String
        Public Property UserId As String
        Public Property Hand As Integer
        Public Result As Integer
    End Class

    Public Class UserItem
        Public Property Id As String
        Public UserId As String
        Public UserName As String
    End Class


    Public Class AzureModel
        Implements INotifyPropertyChanged

        Private Shared MobileServiceClient As New MobileServiceClient("https://leapcloud.azure-mobile.net")

        Private Items As MobileServiceCollection(Of TodoItem, TodoItem)
        Private TodoTable As IMobileServiceTable(Of TodoItem) = MobileServiceClient.GetTable(Of TodoItem)()
        Private UserTable As IMobileServiceTable(Of UserItem) = MobileServiceClient.GetTable(Of UserItem)()

        Private _UserId As String = ""
        Public Property UserId As String
            Get
                Return Me._UserId
            End Get
            Set(value As String)
                Me._UserId = value
                OnPropertyChanged()
            End Set
        End Property

        '-1:受付中、0:対戦中、1:勝利、2:敗北
        Private _Result As Integer = -1
        Public Property Result As Integer
            Get
                Return Me._UserId
            End Get
            Set(value As Integer)
                Me._Result = value
                OnPropertyChanged()
            End Set
        End Property

        Private _ErrorString As String
        Public Property ErrorString As String
            Get
                Return Me._ErrorString
            End Get
            Set(value As String)
                Me._ErrorString = value
                OnPropertyChanged()
            End Set
        End Property

        Public Async Function SetUserId(userName As String) As Task
            Try
                Await GetUserID(userName)
                If Me.UserId.Length = 0 Then
                    Await UserTable.InsertAsync(New UserItem With {.UserName = userName})
                    Await GetUserID(userName)
                End If
            Catch ex As Exception
                Me.ErrorString = ex.Message
            End Try
        End Function

        Public Async Function GetUserID(userName As String) As Task
            Dim query = UserTable.CreateQuery()
            query.Parameters.Add("UserName", userName)
            Dim response = Await query.ToListAsync()
            If response.Count > 0 Then
                Me.UserId = response(0).UserId
            Else
                Me.UserId = ""
            End If
        End Function

        Public Async Function SetItem(hand As Integer) As Task
            Try
                Await TodoTable.InsertAsync(New TodoItem With {.UserId = Me.UserId, .Hand = hand})
                Await GetItem(hand)
            Catch ex As Exception
                Me.ErrorString = ex.Message
            End Try
        End Function

        Public Async Function GetItem(hand As Integer) As Task
            Dim query = TodoTable.CreateQuery()
            query.Parameters.Add("UserId", UserId)
            Dim response = Await query.ToListAsync()
            If response.Count > 0 Then
                Me.Result = response(0).Result
            Else
                Me.Result = -1
            End If
        End Function

        Public Event PropertyChanged(sender As Object,
                                     e As PropertyChangedEventArgs) Implements INotifyPropertyChanged.PropertyChanged
        Protected Sub OnPropertyChanged(<CallerMemberName> Optional propertyName As String = Nothing)
            RaiseEvent PropertyChanged(Me, New PropertyChangedEventArgs(propertyName))
        End Sub
    End Class
End Namespace
