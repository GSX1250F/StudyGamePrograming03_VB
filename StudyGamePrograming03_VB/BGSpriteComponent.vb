Imports System.Numerics
Imports Windows.Win32.System

Public Class BGSpriteComponent
    Inherits SpriteComponent

    Private mBGSpritePos As Vector2
    Private mScrollSpeed As Vector2

    Sub New(ByRef owner As Actor, ByVal drawOrder As Integer)
        MyBase.New(owner, drawOrder)
        mScrollSpeed = Vector2.Zero
        mBGSpritePos = Vector2.Zero
    End Sub

    Public Overrides Sub Update(ByVal deltaTime As Double)
        '背景の位置を更新する。
        mBGSpritePos += mScrollSpeed * deltaTime
        'ラッピング処理
        If (mBGSpritePos.X < -GetTexWidth() * mOwner.GetScale() * 0.5) Then
            mBGSpritePos.X += GetTexWidth() * 2.0 * mOwner.GetScale()
        ElseIf (mBGSpritePos.X > GetTexWidth() * mOwner.GetScale() * 1.5) Then
            mBGSpritePos.X -= GetTexWidth() * mOwner.GetScale() * 2.0
        End If
        If (mBGSpritePos.Y < -GetTexHeight() * mOwner.GetScale() * 0.5) Then
            mBGSpritePos.Y = GetTexHeight() * mOwner.GetScale() * 1.5
        ElseIf (mBGSpritePos.Y > GetTexHeight() * mOwner.GetScale() * 1.5) Then
            mBGSpritePos.Y = -GetTexHeight() * mOwner.GetScale() * 0.5
        End If
    End Sub

    Public Overrides Sub Draw(ByRef mRenderer As Object)
        ' 各背景テクスチャを描画します
        ' SpriteComponentのDrawをOverride。背景の位置で描画する。
        Dim w = CInt(GetTexWidth() * mOwner.GetScale())
        Dim h = CInt(GetTexHeight() * mOwner.GetScale())
        Dim x = CInt(mBGSpritePos.X - w / 2)
        Dim y = CInt(mBGSpritePos.Y - h / 2)

        mRenderer.DrawImage(GetTexture(), x, y, w, h)
    End Sub

    Public Sub SetBGSpritePos(ByVal pos As Vector2)
        mBGSpritePos = pos
    End Sub

    Public Function GetBGSpritePos() As Vector2
        Return mBGSpritePos
    End Function

    Public Sub SetScrollSpeed(ByVal speed As Vector2)
        mScrollSpeed = speed
    End Sub

    Public Function GetScrollSpeed() As Vector2
        Return mScrollSpeed
    End Function

End Class