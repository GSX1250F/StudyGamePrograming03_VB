Imports System.Numerics

Public Class BGSpriteComponent
    Inherits SpriteComponent
    Sub New(ByRef owner As Actor, ByVal drawOrder As Integer)
        MyBase.New(owner, drawOrder)
        mScrollSpeed = 0.0
    End Sub
    Public Overrides Sub Update(deltaTime As Single)
        MyBase.Update(deltaTime)
        If mBGTextures.Count > 0 Then
            For i As Integer = 0 To mBGTextures.Count - 1
                Dim bg As BGTexture
                bg = mBGTextures(i)
                ' xのオフセット更新
                bg.mOffset.X += mScrollSpeed * deltaTime
                ' もし画面から完全に出たら、オフセットを最後の背景テクスチャの右にリセットする
                If (bg.mOffset.X < -mScreenSize.X) Then
                    bg.mOffset.X = (mBGTextures.Count() - 1) * mScreenSize.X - 1
                End If
                mBGTextures(i) = bg
            Next
        End If
    End Sub

    Public Overrides Sub Draw(ByRef mRenderer As Object)
        ' 各背景テクスチャを描画します
        For Each bg In mBGTextures
            Dim w = CInt(mScreenSize.X)
            Dim h = CInt(mScreenSize.Y)
            Dim x = CInt(mOwner.mPosition.X - w / 2 + bg.mOffset.X)
            Dim y = CInt(mOwner.mPosition.Y - h / 2 + bg.mOffset.Y)
            Dim img As New Bitmap(w, h)
            img = bg.mTexture
            mRenderer.DrawImage(img, x, y, w, h)
        Next


    End Sub

    Public Sub SetBGTextures(ByRef textures As List(Of Image))
        Dim Count As Integer = 0
        For Each tex In textures
            Dim temp As BGTexture
            temp.mTexture = tex
            ' それぞれのテクスチャは画面幅分のオフセットを持つ
            temp.mOffset.X = Count * mScreenSize.X
            temp.mOffset.Y = 0
            mBGTextures.Add(temp)
            Count += 1
        Next
    End Sub

    Public Structure BGTexture
        Dim mTexture As Image
        Dim mOffset As Vector2
    End Structure
    Public mBGTextures As New List(Of BGTexture)
    Public mScreenSize As Vector2
    Public mScrollSpeed As Single
End Class
