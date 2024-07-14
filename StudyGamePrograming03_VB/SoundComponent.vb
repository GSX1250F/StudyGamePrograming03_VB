Public Class SoundComponent
    Inherits Component

    Private mPlayable As Boolean
    Private mAliasNames As New List(Of String)

    Sub New(ByRef owner As Actor, ByVal updateOrder As Integer)
        MyBase.New(owner, updateOrder)
        mPlayable = True
        mOwner.GetGame().GetSoundPlayer().AddSndCmpnt(Me)
    End Sub
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposed Then
            If disposing Then
                '*** アンマネージリソースの開放
            End If
            '*** マネージドリソースの開放
            mOwner.GetGame().GetSoundPlayer().RemoveSndCmpnt(Me)
        End If
        MyBase.Dispose(disposing)
    End Sub

    Public Sub SetAliasName(ByVal aliasName As String)
        mAliasNames.Add(aliasName)
    End Sub
    Public Sub SetControl(ByVal id As Integer, ByVal control As String)
        Dim aliasName As String = mAliasNames(id)
        mOwner.GetGame().GetSoundPlayer().SetSoundControl(aliasName, control)
    End Sub
    Public Sub SetPlayable(ByVal playable As Boolean)
        mPlayable = playable
    End Sub
    Public Function GetPlayable() As Boolean
        Return mPlayable
    End Function

End Class