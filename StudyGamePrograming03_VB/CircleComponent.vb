Imports System.Numerics

Public Class CircleComponent
    Inherits Component
    Sub New(ByRef owner As Actor, ByVal updateOrder As Integer)
        MyBase.New(owner, updateOrder)
        '半径はActorの半径で初期化
        mRadius = mOwner.mRadius        'Actorの半径= scale * radius
        'Actorの慣性モーメントを設定。一様の円板とする。(I=1/2*mR^2)
        mOwner.mImoment = mOwner.mScale * mOwner.mScale * mRadius * mRadius / 2
    End Sub
    Public Function GetCenter() As Vector2
        '中心はActorの位置
        Return mOwner.mPosition
    End Function

    Public mRadius As Single

    Public Function Intersect(ByRef a As CircleComponent, ByRef b As CircleComponent) As Boolean
        ' ２つのCircleComponentの中心間距離を計算
        Dim diff As Vector2 = a.GetCenter() - b.GetCenter()
        Dim dist As Single = diff.Length()

        ' ２つのCircleComponentの半径の和を計算 
        Dim sumRadius As Single = a.mRadius + b.mRadius

        ' 中心間距離 <= 半径の和 のとき、衝突したと判定
        If dist <= sumRadius Then
            Return True
        Else
            Return False
        End If
    End Function


End Class
