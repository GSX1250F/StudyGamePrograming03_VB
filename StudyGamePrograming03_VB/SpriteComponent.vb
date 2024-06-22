Imports Windows.Win32.UI.Input

Public Class SpriteComponent
    Inherits Component

    Private mTexture As Image
    Private mDrawOrder As Integer
    Private mTexWidth As Integer
    Private mTexHeight As Integer

    Sub New(ByRef owner As Actor, ByVal drawOrder As Integer)
        MyBase.New(owner, drawOrder)
        mTexture = Nothing
        mDrawOrder = drawOrder
        mTexWidth = 0
        mTexHeight = 0
        mOwner.GetGame().AddSprite(Me)
    End Sub
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposed Then
            If disposing Then
                ' Insert code to free managed resources.
            End If
            ' Insert code to free unmanaged resources.
            mOwner.GetGame().RemoveSprite(Me)
        End If
        MyBase.Dispose(disposing)
    End Sub

    Public Overridable Sub Draw(ByRef mRenderer As Object)
        If (mTexture IsNot Nothing) And (mOwner.GetState() <> Actor.State.EPaused) Then
            Dim w = CInt(mTexWidth * mOwner.GetScale())
            Dim h = CInt(mTexHeight * mOwner.GetScale())
            'Dim x = CInt(mOwner.mPosition.X - w / 2)
            'Dim y = CInt(mOwner.mPosition.Y - h / 2)
            Dim img As New Bitmap(w, h)
            img = mTexture

            '新しい座標位置を計算する
            Dim x(3) As Single
            Dim y(3) As Single
            Dim x2(3) As Single
            Dim y2(3) As Single
            x(0) = mOwner.GetPosition().X - w / 2
            y(0) = mOwner.GetPosition().Y - h / 2
            x(1) = mOwner.GetPosition().X + w / 2
            y(1) = mOwner.GetPosition().Y - h / 2
            x(2) = mOwner.GetPosition().X - w / 2
            y(2) = mOwner.GetPosition().Y + h / 2
            For i = 0 To 2
                x2(i) = (x(i) - mOwner.GetPosition().X) * Math.Cos(mOwner.GetRotation()) + (y(i) - mOwner.GetPosition().Y) * Math.Sin(mOwner.GetRotation()) + mOwner.GetPosition().X
                y2(i) = -(x(i) - mOwner.GetPosition().X) * Math.Sin(mOwner.GetRotation()) + (y(i) - mOwner.GetPosition().Y) * Math.Cos(mOwner.GetRotation()) + mOwner.GetPosition().Y
            Next

            'Point配列を作成
            Dim destinationPoints() As Point = {
                New Point(CInt(x2(0)), CInt(y2(0))),
                New Point(CInt(x2(1)), CInt(y2(1))),
                New Point(CInt(x2(2)), CInt(y2(2)))}

            mRenderer.DrawImage(img, destinationPoints)
        End If

    End Sub
    Public Sub SetTexture(ByRef tex As Image)
        mTexture = tex
        mTexWidth = tex.Width
        mTexHeight = tex.Height
        ' 高さと幅の平均をActorの直径とする。
        mOwner.SetRadius((mTexWidth + mTexHeight) / 4)
    End Sub

    Public Function GetDrawOrder() As Integer
        Return mDrawOrder
    End Function

    Public Function GetTexWidth() As Integer
        Return mTexWidth
    End Function
    Public Function GetTexHeight() As Integer
        Return mTexHeight
    End Function

    Public Function GetTexture() As Image
        Return mTexture
    End Function


End Class
