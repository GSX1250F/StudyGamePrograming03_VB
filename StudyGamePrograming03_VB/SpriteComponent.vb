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
                '*** アンマネージリソースの開放
            End If
            '*** マネージドリソースの開放
            mOwner.GetGame().RemoveSprite(Me)
        End If
        MyBase.Dispose(disposing)
    End Sub

    Public Overridable Sub Draw(ByRef mRenderer As Object)
        If (mTexture IsNot Nothing) And (mOwner.GetState() <> Actor.State.EPaused) Then
            Dim w = CInt(mTexWidth * mOwner.GetScale())
            Dim h = CInt(mTexHeight * mOwner.GetScale())
            Dim x = CInt(mOwner.GetPosition().X - w / 2)
            Dim y = CInt(mOwner.GetPosition().Y - h / 2)

            '画像を回転して表示
            Dim a As Single = mOwner.GetPosition().X
            Dim b As Single = mOwner.GetPosition().Y
            Dim t As Single = mOwner.GetRotation()

            Dim x1 As Integer = CInt(x * Math.Cos(t) + y * Math.Sin(t) - a * Math.Cos(t) - b * Math.Sin(t) + a)
            Dim y1 As Integer = CInt(-x * Math.Sin(t) + y * Math.Cos(t) + a * Math.Sin(t) - b * Math.Cos(t) + b)
            Dim x2 As Integer = CInt((x + w) * Math.Cos(t) + y * Math.Sin(t) - a * Math.Cos(t) - b * Math.Sin(t) + a)
            Dim y2 As Integer = CInt(-(x + w) * Math.Sin(t) + y * Math.Cos(t) + a * Math.Sin(t) - b * Math.Cos(t) + b)
            Dim x3 As Integer = CInt(x * Math.Cos(t) + (y + h) * Math.Sin(t) - a * Math.Cos(t) - b * Math.Sin(t) + a)
            Dim y3 As Integer = CInt(-x * Math.Sin(t) + (y + h) * Math.Cos(t) + a * Math.Sin(t) - b * Math.Cos(t) + b)
            'PointF配列を作成
            Dim destinationPoints() As PointF = {New PointF(x1, y1),
                    New PointF(x2, y2),
                    New PointF(x3, y3)}

            mRenderer.DrawImage(mTexture, destinationPoints)
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
