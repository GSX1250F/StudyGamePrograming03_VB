Public Class InputComponent
	Inherits MoveComponent

	' 前進・回転方向の力の最大値
	Private mMaxForwardForce As Single
	Private mMaxRotForce As Single
	Private mMaxForwardVelocity As Single
	Private mMaxRotSpeed As Single
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
		Dim fwd As Single = 0.0
		Dim rot As Single = 0.0
		'古典物理学でMoveComponentのための計算
		'MoveComponentには前進か回転方向の力の最大値だけを渡す
		If keyState(mFwdKey) = True Then
			fwd = mMaxForwardVelocity
			'fwd = mMaxForwardForce
		ElseIf keyState(mBwdKey) = True Then
			fwd = -mMaxForwardVelocity
			'fwd = -mMaxForwardForce
		ElseIf keyState(mCCwdKey) = True Then
			rot = mMaxRotSpeed
			'rot = mMaxRotForce
		ElseIf keyState(mCwdKey) = True Then
			rot = -mMaxRotSpeed
			'rot = -mMaxRotForce
		End If

		SetVelocity(fwd * mOwner.GetForward())
		SetRotSpeed(rot)
		'SetMoveForce(fwd * mOwner.GetForward())
		'SetRotForce(rot)
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
	Public Sub SetMaxForwardVelocity(ByVal value As Single)
		mMaxForwardVelocity = value
	End Sub
	Public Sub SetMaxRotSpeed(ByVal value As Single)
		mMaxRotSpeed = value
	End Sub

	Public Sub SetMaxForwardForce(ByVal value As Single)
		mMaxForwardForce = value
	End Sub
	Public Sub SetMaxRotForce(ByVal value As Single)
		mMaxRotForce = value
	End Sub




End Class