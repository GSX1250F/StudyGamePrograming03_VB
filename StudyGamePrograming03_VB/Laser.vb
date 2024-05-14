Public Class Laser
	Inherits Actor

	Sub New(ByRef game As Game)
		MyBase.New(game)
		mDeathTime = 2.0
		'スプライトコンポーネント作成、テクスチャ設定
		Dim sc As SpriteComponent = New SpriteComponent(Me, 50)
		sc.SetTexture(game.GetTexture("\Assets\Laser.png"))

		'初期位置,速度,角度はShipで設定

		'CircleComponent作成
		mCircle = New CircleComponent(Me, 10)
	End Sub

	Public Overrides Sub UpdateActor(deltaTime As Single)
		'位置はMoveComponentで更新される。
		'画面外にでるか、DeathTimeが0になったら消去する。
		mDeathTime -= deltaTime
		If mDeathTime <= 0.0 Or
		   mPosition.X < 0.0 Or
		   mPosition.X > mGame.mWindowW Or
		   mPosition.Y < 0.0 Or
		   mPosition.Y > mGame.mWindowH Then
			mState = State.EDead
		Else
			'小惑星に当たったとき
			For Each ast In mGame.mAsteroids
				If mCircle.Intersect(mCircle, ast.mCircle) Then
					'レーザーも消去するなら次を実行
					mState = State.EDead
					'小惑星を消去
					ast.mState = State.EDead
					Exit For
				End If
			Next
		End If
	End Sub

    Public mCircle As CircleComponent
    Public mDeathTime As Single

End Class
