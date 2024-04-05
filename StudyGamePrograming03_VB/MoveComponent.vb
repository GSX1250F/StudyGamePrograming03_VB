Imports System.Numerics

Public Class MoveComponent
    Inherits Component
    Sub New(ByRef owner As Actor, ByVal updateOrder As Integer)
        MyBase.New(owner, updateOrder)
        mMoveForce.X = 0.0
        mMoveForce.Y = 0.0
        mMoveAccel.X = 0.0
        mMoveAccel.Y = 0.0
        mRotForce = 0.0F
        mRotAccel = 0.0F
        mMoveResist = 0.0
        mRotResist = 0.0
    End Sub

    Public Overrides Sub Update(deltaTime As Single)
        MyBase.Update(deltaTime)
        ' Actorの重心速度と回転速度を更新
        ' Actorの位置と角度はActorのUpdateで更新
        If mOwner.mMass <> 0 Then
            mMoveAccel = mMoveForce * (1 / mOwner.mMass)    '重心加速度の計算　F=ma  a=F*(1/m)
            '抵抗力 = 速さ*抵抗係数    減速 = -速さ*抵抗係数/質量
            Dim movedecel As Vector2 = mOwner.mVelocity * mMoveResist * 0.01 * (1 / mOwner.mMass)
            mMoveAccel -= movedecel
        Else
            mMoveAccel.X = 0.0
            mMoveAccel.Y = 0.0
        End If

        If (mOwner.mImoment <> 0) And (mOwner.mRadius <> 0) Then
            mRotAccel = mRotForce * mOwner.mRadius / mOwner.mImoment    '回転加速度の計算 Fr=Ia  a=Fr/I
            '抵抗力 = 速さ*抵抗係数    減速 = -速さ*抵抗係数*半径/慣性モーメント
            Dim rotDecel As Single = mOwner.mRotSpeed * mOwner.mRadius * mRotResist / mOwner.mImoment
            mRotAccel -= rotDecel
        Else
            mRotAccel = 0
        End If
        mOwner.mVelocity = mOwner.mVelocity + mMoveAccel * deltaTime    'v = vo + at
        mOwner.mRotSpeed = mOwner.mRotSpeed + mRotAccel * deltaTime     'ω = ωo + ωt
    End Sub

    '重心にかかる力
    Public mMoveForce As Vector2
    '回転方向の力F　 トルク=RotForce * Radius = Imoment * RotAccel
    Public mRotForce As Single
    '重心加速度
    Public mMoveAccel As Vector2
    '回転加速度
    Public mRotAccel As Single

    '重心速度抵抗率(%)
    Public mMoveResist As Single
    '回転速度抵抗率(%)
    Public mRotResist As Single

End Class
