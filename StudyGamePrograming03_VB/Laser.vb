Imports System.Numerics

Public Class Laser
	Inherits Actor

	Private mCircle As CircleComponent
	Private mDeathTime As Single
	Private mLaserSpeed As Single

	Sub New(ByRef game As Game)
		MyBase.New(game)
		'スプライトコンポーネント作成、テクスチャ設定
		mDeathTime = 2.0
		mLaserSpeed = 900.0
		Dim sc As New SpriteComponent(Me, 50)
		sc.SetTexture(game.GetTexture("\Assets\Laser.png"))
	End Sub

	Public Overrides Sub UpdateActor(deltaTime As Single)
		'DeathTimeが0になったら消去する。
		mDeathTime -= deltaTime
		If (mDeathTime <= 0.0 Or
			GetPosition().X < 0.0 Or
			GetPosition().X > GetGame().mWindowWidth Or
			GetPosition().Y < 0.0 Or
			GetPosition().Y > GetGame().mWindowHeight) _
			Then
			SetState(State.EDead)
		Else
			'小惑星に当たったとき
			For Each ast In GetGame().GetAsteroids()
				If Intersect(mCircle, ast.GetCircle) Then
					'レーザーも消去するなら次を実行
					SetState(State.EDead)
					'小惑星を消去
					ast.SetState(State.EDead)
					Exit For
				End If
			Next
		End If
	End Sub

	Public Sub Shot()
		'MoveComponent作成
		Dim mc As New MoveComponent(Me, 10)
		Dim v As Vector2
		v.X = mLaserSpeed * Math.Cos(GetRotation())
		v.Y = -mLaserSpeed * Math.Sin(GetRotation())
		mc.SetVelocity(v)

		'CircleComponent作成
		mCircle = New CircleComponent(Me, 10)
	End Sub



End Class
