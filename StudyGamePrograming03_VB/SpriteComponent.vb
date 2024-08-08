Public Class SpriteComponent
    Inherits Component

    Private mTexture As Image
    Private mDrawOrder As Integer
    Private mTexWidth As Integer
    Private mTexHeight As Integer
    Private mVisible As Boolean

    Sub New(ByRef owner As Actor, ByVal drawOrder As Integer)
        MyBase.New(owner, drawOrder)
        mTexture = Nothing
        mDrawOrder = drawOrder
        mTexWidth = 0
        mTexHeight = 0
        mVisible = True
        mOwner.GetGame().GetRenderer().AddSprite(Me)
    End Sub
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposed Then
            If disposing Then
                '*** アンマネージリソースの開放
            End If
            '*** マネージドリソースの開放
            mOwner.GetGame().GetRenderer().RemoveSprite(Me)
        End If
        MyBase.Dispose(disposing)
    End Sub

    Public Overridable Sub Draw(ByRef mGraphics As Object)
        If (mTexture IsNot Nothing) And (mVisible = True) Then
            Dim w As Double = mTexWidth * mOwner.GetScale()
            Dim h As Double = mTexHeight * mOwner.GetScale()
            Dim x0 As Double = mOwner.GetPosition().X
            Dim y0 As Double = mOwner.GetPosition().Y

            '画像を回転して表示
            Dim angle As Double = mOwner.GetRotation()

            Dim x1 As Integer = CInt((-w / 2) * Math.Cos(angle) + (-h / 2) * Math.Sin(angle) + x0)
            Dim y1 As Integer = CInt(-(-w / 2) * Math.Sin(angle) + (-h / 2) * Math.Cos(angle) + y0)
            Dim x2 As Integer = CInt(w / 2 * Math.Cos(angle) + (-h / 2) * Math.Sin(angle) + x0)
            Dim y2 As Integer = CInt(-w / 2 * Math.Sin(angle) + (-h / 2) * Math.Cos(angle) + y0)
            Dim x3 As Integer = CInt((-w / 2) * Math.Cos(angle) + h / 2 * Math.Sin(angle) + x0)
            Dim y3 As Integer = CInt(-(-w / 2) * Math.Sin(angle) + h / 2 * Math.Cos(angle) + y0)
            'PointF配列を作成
            Dim destinationPoints() As PointF = {New PointF(x1, y1), New PointF(x2, y2), New PointF(x3, y3)}

            mGraphics.DrawImage(mTexture, destinationPoints)
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
    Public Sub SetVisible(ByVal visible As Boolean)
        mVisible = visible
    End Sub
    Public Function GetVisible() As Boolean
        Return mVisible
    End Function
End Class
