Imports System.Numerics

Public Class Ship
    Inherits Actor
    Sub New(ByRef game As Game)
        MyBase.New(game)
        mPosition.X = 1024 / 2
        mPosition.Y = 768 / 2
        mRotation = 30 / 180 * Math.PI
        mScale = 1.5

        'アニメーションのスプライトコンポーネント作成、テクスチャ設定
        Dim asc As New AnimSpriteComponent(Me, 10)
        Dim anims As New List(Of Image)(New Image() {
            game.GetTexture("../../../Assets/Ship01.png"),
            game.GetTexture("../../../Assets/Ship02.png"),
            game.GetTexture("../../../Assets/Ship03.png"),
            game.GetTexture("../../../Assets/Ship04.png")})
        asc.SetAnimTextures(anims, True)



	End Sub

    Public Overrides Sub ActorInput(keyState As KeyEventArgs)
        MyBase.ActorInput(keyState)

        mc.mForwardSpeed = 0
        If Not keyState Is Nothing Then
            If keyState.KeyCode = Keys.Down Then
                mRotation = -Math.PI / 2.0
                mc.mForwardSpeed = 300
            ElseIf keyState.KeyCode = Keys.Up Then
                mRotation = (Math.PI / 2.0)
                mc.mForwardSpeed = 300
            ElseIf keyState.KeyCode = Keys.Right Then
                mRotation = 0.0
                mc.mForwardSpeed = 300
            ElseIf keyState.KeyCode = Keys.Left Then
                mRotation = Math.PI
                mc.mForwardSpeed = 300
            Else
                mc.mForwardSpeed = 0
            End If
        End If
    End Sub

    Public mc As New MoveComponent(Me, 20)

End Class
