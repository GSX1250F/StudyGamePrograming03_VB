Public Class InputComponent
	Inherits MoveComponent

	' 前進・回転方向の力の最大値
	Private mMaxForwardForce As Double
	Private mMaxRotForce As Double
	Private mMaxForwardVelocity As Double
	Private mMaxRotSpeed As Double
	Private mFwdKey As Integer
	Private mBwdKey As Integer
	Private mCwdKey As Integer
	Private mCCwdKey As Integer

	Sub New(ByRef owner As Actor, ByVal updateOrder As Integer)
		MyBase.New(owner, updateOrder)
		mMaxForwardVelocity = 0.0
		mMaxRotSpeed = 0.0
		mMaxForwardForce = 0.0
		mMaxRotForce = 0.0
		mFwdKey = Nothing
		mBwdKey = Nothing
		mCwdKey = Nothing
		mCCwdKey = Nothing
	End Sub

	Public Overrides Sub ProcessInput(ByVal keyState As Boolean())
		Dim fwd As Double = 0.0
		Dim rot As Double = 0.0

		If keyState(mFwdKey) = True Then
			'fwd = mMaxForwardVelocity       '単純移動の場合
			fwd = mMaxForwardForce      'ニュートン力学を使う場合
		End If
		If keyState(mBwdKey) = True Then
			'fwd = -mMaxForwardVelocity       '単純移動の場合
			fwd = -mMaxForwardForce      'ニュートン力学を使う場合
		End If
		If keyState(mCCwdKey) = True Then
			'rot = mMaxRotSpeed       '単純移動の場合
			rot = mMaxRotForce      'ニュートン力学を使う場合
		End If
		If keyState(mCwdKey) = True Then
			'rot = -mMaxRotSpeed       '単純移動の場合
			rot = -mMaxRotForce      'ニュートン力学を使う場合
		End If

		'単純移動の場合
		'SetVelocity(fwd * mOwner.GetForward())
		'SetRotSpeed(rot)

		'ニュートン力学を使う場合
		SetMoveForce(fwd * mOwner.GetForward())
		SetRotForce(rot)
	End Sub

	Public Sub SetForwardKey(ByVal key As Integer)
		mFwdKey = key
	End Sub
	Public Sub SetBackwardKey(ByVal key As Integer)
		mBwdKey = key
	End Sub
	Public Sub SetCrockwardKey(ByVal key As Integer)
		mCwdKey = key
	End Sub
	Public Sub SetCounterCrockwardKey(ByVal key As Integer)
		mCCwdKey = key
	End Sub
	Public Sub SetMaxForwardVelocity(ByVal value As Double)
		mMaxForwardVelocity = value
	End Sub
	Public Sub SetMaxRotSpeed(ByVal value As Double)
		mMaxRotSpeed = value
	End Sub

	Public Sub SetMaxForwardForce(ByVal value As Double)
		mMaxForwardForce = value
	End Sub
	Public Sub SetMaxRotForce(ByVal value As Double)
		mMaxRotForce = value
	End Sub




End Class