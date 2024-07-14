Imports System.Numerics

Public Class MoveComponent
    Inherits Component

    '単純移動パラメータ
    Private mVelocity As Vector2     '並進移動速度
    Private mRotSpeed As Double      '回転速度

    'ニュートン力学パラメータ
    Private mMass As Double      '質量
    Private mMoveForce As Vector2        '重心にかかる力
    Private mRotForce As Double          '回転方向の力F　 トルク=RotForce * Radius = Imoment * RotAccel
    Private mMoveAccel As Vector2        '重心加速度
    Private mRotAccel As Double      '回転加速度
    Private mMoveResist As Double        '重心速度抵抗率(%)
    Private mRotResist As Double     '回転速度抵抗率(%)

    Sub New(ByRef owner As Actor, ByVal updateOrder As Integer)
        MyBase.New(owner, updateOrder)
        mVelocity = Vector2.Zero
        mRotSpeed = 0.0
        mMass = 1.0
        mMoveForce = Vector2.Zero
        mMoveAccel = Vector2.Zero
        mRotForce = 0.0
        mRotAccel = 0.0
        mMoveResist = 0.0
        mRotResist = 0.0
    End Sub

    Public Overrides Sub Update(deltaTime As Double)
        mOwner.SetPosition(mOwner.GetPosition() + mVelocity * deltaTime)    ' Actorの位置を更新
        mOwner.SetRotation(mOwner.GetRotation() + mRotSpeed * deltaTime)    ' Actorの方向を更新

        mMoveAccel = GetMoveAccel()
        mRotAccel = GetRotAccel()

        mVelocity += mMoveAccel * deltaTime     'v = v0 + at
        mRotSpeed += mRotAccel * deltaTime     'ω = ωo + bt
    End Sub

    Public Function GetVelocity() As Vector2
        Return mVelocity
    End Function
    Public Sub SetVelocity(ByVal v)
        mVelocity = v
    End Sub
    Public Function GetRotSpeed() As Double
        Return mRotSpeed
    End Function
    Public Sub SetRotSpeed(ByVal v As Double)
        mRotSpeed = v
    End Sub
    Public Function GetMoveForce() As Vector2
        Return mMoveForce
    End Function
    Public Sub SetMoveForce(ByVal v As Vector2)
        mMoveForce = v
    End Sub
    Public Function GetRotForce() As Double
        Return mRotForce
    End Function
    Public Sub SetRotForce(ByVal v As Double)
        mRotForce = v
    End Sub
    Public Function GetMass() As Double
        Return mMass
    End Function
    Public Sub SetMass(ByVal v As Double)
        mMass = v
    End Sub
    Public Function GetMoveAccel() As Vector2
        If GetMass() = 0 Then
            Return Vector2.Zero
        Else
            Dim accel As Vector2 = GetMoveForce() * (1 / GetMass())    '重心加速度の計算　F=ma  a=F*(1/m)
            '抵抗力 = 速さ*抵抗係数    減速 = -速さ*抵抗係数/質量
            accel -= GetVelocity() * GetMoveResist() * 0.01 * (1 / GetMass())
            Return accel
        End If
    End Function
    Public Function GetImoment() As Double
        ' 慣性モーメント計算　※※2次元においては、一様密度の円板とする。 I=0.5*質量*半径^2
        Return 0.5 * GetMass() * mOwner.GetRadius() * mOwner.GetRadius()
    End Function
    Public Function GetTorque() As Double
        'トルク計算　トルク=回転方向の力 * 半径
        Return GetRotForce() * mOwner.GetRadius()
    End Function
    Public Function GetRotAccel() As Double
        If GetImoment() = 0 Then
            Return 0
        Else
            '回転加速度の計算　回転加速度 = トルク / 慣性モーメント
            Dim accel = GetTorque() / GetImoment()    '回転加速度の計算 Fr=Ia  a=Fr/I
            '抵抗力 = 速さ*抵抗係数    減速 = -速さ*抵抗係数*半径/慣性モーメント
            accel -= GetRotSpeed() * mOwner.GetRadius() * GetRotResist() / GetImoment()
            Return accel
        End If
    End Function
    Public Function GetMoveResist() As Double
        Return mMoveResist
    End Function
    Public Sub SetMoveResist(ByVal v As Double)
        mMoveResist = v
    End Sub
    Public Function GetRotResist() As Double
        Return mRotResist
    End Function
    Public Sub SetRotResist(ByVal v As Double)
        mRotResist = v
    End Sub
End Class