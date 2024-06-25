Public Class InputComponent
	Inherits MoveComponent

	' 前進・回転方向の力の最大値
	Private mMaxForwardForce As Single
	Private mMaxRotForce As Single
	Private disposedValue As Boolean

	Sub New(ByRef owner As Actor, ByVal updateOrder As Integer)
		MyBase.New(owner, updateOrder)

	End Sub

	Public Overrides Sub ProcessInput(ByVal keyState As Boolean())
		Dim forwardforce As Single = 0.0
		Dim rotforce As Single = 0.0
		'古典物理学でMoveComponentのための計算
		'MoveComponentには前進か回転方向の力の最大値だけを渡す
		If keyState(Keys.Up) = True Then
			forwardforce = mMaxForwardForce
		ElseIf keyState(Keys.Down) = True Then
			forwardforce = -mMaxForwardForce
		ElseIf keyState(Keys.Left) = True Then
			rotforce = mMaxRotForce
		ElseIf keyState(Keys.Right) = True Then
			rotforce = -mMaxRotForce
		End If

		SetMoveForce(forwardforce * mOwner.GetForward())
		SetRotForce(rotforce)
	End Sub

	Public Sub SetMaxForwardForce(ByVal power As Single)
		mMaxForwardForce = power
	End Sub
	Public Sub SetMaxRotForce(ByVal power As Single)
		mMaxRotForce = power
	End Sub
	Public Function GetMaxForwardForce() As Single
		Return mMaxForwardForce
	End Function
	Public Function GetMaxRotForce() As Single
		Return mMaxRotForce
	End Function



End Class