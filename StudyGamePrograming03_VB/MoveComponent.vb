Imports System.Numerics

Public Class MoveComponent
    Inherits Component

    '単純移動パラメータ
    Private mVelocity As Vector2     '重心移動速度
    Private mRotSpeed As Single      '回転速度

    '古典物理パラメータ
    Private mMass As Single      '質量
    Private mMoveForce As Vector2        '重心にかかる力
    Private mRotForce As Single          '回転方向の力F　 トルク=RotForce * Radius = Imoment * RotAccel
    Private mTorque As Single        'トルク=回転方向の力 * 半径 = 慣性モーメント * 回転加速度
    Private mMoveAccel As Vector2        '重心加速度
    Private mRotAccel As Single      '回転加速度
    Private mImoment As Single       '慣性モーメント
    Private mMoveResist As Single        '重心速度抵抗率(%)
    Private mRotResist As Single     '回転速度抵抗率(%)

    Sub New(ByRef owner As Actor, ByVal updateOrder As Integer)
        MyBase.New(owner, updateOrder)
        mVelocity.X = 0.0
        mVelocity.Y = 0.0
        mRotSpeed = 0.0
        mMass = 1.0
        mMoveForce.X = 0.0
        mMoveForce.Y = 0.0
        mMoveAccel.X = 0.0
        mMoveAccel.Y = 0.0
        mRotForce = 0.0
        mRotAccel = 0.0
        mTorque = 0.0
        mImoment = 0.0
        mMoveResist = 0.0
        mRotResist = 0.0
    End Sub
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
    End Sub

    Public Overrides Sub Update(deltaTime As Single)
        ' Actorの位置を更新 x=x0+vt
        Dim v As New Vector2
        v.X = mOwner.GetPosition().X + mVelocity.X * deltaTime
        v.Y = mOwner.GetPosition().Y + mVelocity.Y * deltaTime
        mOwner.SetPosition(v)
        ' Actorの方向を更新 θ=θ0+ωt
        mOwner.SetRotation(mOwner.GetRotation() + mRotSpeed * deltaTime)

        ' Actorの重心速度を更新
        If mMass <> 0 Then
            mMoveAccel = mMoveForce * (1 / mMass)    '重心加速度の計算　F=ma  a=F*(1/m)
            '抵抗力 = 速さ*抵抗係数    減速 = -速さ*抵抗係数/質量
            Dim movedecel As Vector2 = mVelocity * mMoveResist * 0.01 * (1 / mMass)
            mMoveAccel -= movedecel
        Else
            mMoveAccel.X = 0.0
            mMoveAccel.Y = 0.0
        End If

        ' 方向を更新
        ' 慣性モーメント計算　※※2次元においては、一様密度の円板とする。 I=0.5*質量*半径^2
        mImoment = 0.5 * mMass * mOwner.GetRadius() * mOwner.GetRadius()
        If (mImoment <> 0) Then
            'トルク計算　トルク=回転方向の力 * 半径
            mTorque = mRotForce * mOwner.GetRadius()
            '回転加速度の計算　回転加速度 = トルク / 慣性モーメント
            mRotAccel = mTorque / mImoment    '回転加速度の計算 Fr=Ia  a=Fr/I
            '抵抗力 = 速さ*抵抗係数    減速 = -速さ*抵抗係数*半径/慣性モーメント
            Dim rotDecel As Single = mRotSpeed * mOwner.GetRadius() * mRotResist / mImoment
            mRotAccel -= rotDecel
        Else
            mRotAccel = 0
        End If
        mVelocity += mMoveAccel * deltaTime     'v = v0 + at
        mRotSpeed += mRotAccel * deltaTime     'ω = ωo + bt
    End Sub

    Public Function GetVelocity() As Vector2
        Return mVelocity
    End Function
    Public Sub SetVelocity(ByVal vel)
        mVelocity = vel
    End Sub
    Public Function GetRotSpeed() As Single
        Return mRotSpeed
    End Function
    Public Sub SetRotSpeed(ByVal rotspeed As Single)
        mRotSpeed = rotspeed
    End Sub
    Public Function GetMoveForce() As Vector2
        Return mMoveForce
    End Function
    Public Sub SetMoveForce(ByVal force As Vector2)
        mMoveForce = force
    End Sub
    Public Function GetRotForce() As Single
        Return mRotForce
    End Function
    Public Sub SetRotForce(ByVal force As Single)
        mRotForce = force
    End Sub
    Public Function GetMoveResist() As Single
        Return mMoveResist
    End Function
    Public Sub SetMoveResist(ByVal resist As Single)
        mMoveResist = resist
    End Sub
    Public Function GetRotResist() As Single
        Return mRotResist
    End Function
    Public Sub SetRotResist(ByVal resist As Single)
        mRotResist = resist
    End Sub
    Public Function GetMass() As Single
        Return mMass
    End Function
    Public Sub SetMass(ByVal mass As Single)
        mMass = mass
    End Sub
    Public Function GetTorque() As Single
        Return mTorque
    End Function
    Public Sub SetTorque(ByVal torque As Single)
        mTorque = torque
    End Sub
    Public Function GetImoment() As Single
        Return mImoment
    End Function
    Public Sub SetImoment(ByVal imoment As Single)
        mImoment = imoment
    End Sub
End Class
