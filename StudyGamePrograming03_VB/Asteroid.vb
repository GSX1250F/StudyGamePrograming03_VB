Imports System.Numerics

Public Class Asteroid
	Inherits Actor

	Public mCircle As CircleComponent

	Sub New(ByRef game As Game)
		MyBase.New(game)

		'ランダムな位置と向きと大きさと初速で初期化
		Dim randPos As Vector2
		Dim Random As New Random()     ' Randomオブジェクトの作成 
		randPos.X = mGame.mWindowW / 2.0
		randPos.Y = mGame.mWindowH / 2.0
		'画面の中央1/4区画以外を初期位置とする。
		While (randPos.X > mGame.mWindowW * 0.25) And
			  (randPos.X < mGame.mWindowH * 0.75) And
			  (randPos.Y > mGame.mWindowH * 0.25) And
			  (randPos.Y < mGame.mWindowH * 0.75F)

			randPos.X = Random.Next(0, mGame.mWindowW)
			randPos.Y = Random.Next(0, mGame.mWindowH)
		End While
		mPosition = randPos

		mRotation = 0   '初期回転は 0とする
		mScale = 0.1 * Random.Next(5, 15)   '拡大率 0.5～1.5
		mRotSpeed = 2 * Math.PI * Random.NextSingle() - Math.PI     '回転速度 -π～π
		Dim randSpeed As Integer = Random.Next(50, 200)     '速度 50～200
		Dim randAngle As Single = Math.PI * Random.Next(20, 70) / 180   '速度の方向角度　20度～70度
		Dim PorN_X As Integer = 2 * Random.Next(0, 2) - 1   'X方向速度の正負
		Dim PorN_Y As Integer = 2 * Random.Next(0, 2) - 1   'X方向速度の正負
		mVelocity.X = PorN_X * randSpeed * Math.Cos(randAngle)
		mVelocity.Y = PorN_Y * randSpeed * Math.Sin(randAngle)

		'スプライトコンポーネント作成、テクスチャ設定
		Dim sc As SpriteComponent = New SpriteComponent(Me, 40)
		sc.SetTexture(game.GetTexture("../../../Assets/Asteroid.png"))

		'MoveComponent作成　※力は働かないでただ動かすだけなら不要。
		Dim mc As MoveComponent = New MoveComponent(Me, 10)

		'CircleComponent作成
		mCircle = New CircleComponent(Me, 10)

		mGame.AddAsteroid(Me)

	End Sub

	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		If Not Me.disposed Then
			If disposing Then
				' Insert code to free managed resources.
			End If
			' Insert code to free unmanaged resources.
			mGame.RemoveAsteroid(Me)
		End If
		MyBase.Dispose(disposing)
	End Sub

	Public Overrides Sub UpdateActor(detaTime As Single)
		'画面外にでたら反対の位置に移動（ラッピング処理）
		If (mPosition.X < 0.0 - 2 * mRadius) Or (mPosition.X > mGame.mWindowW + 2 * mRadius) Then
			mPosition.X = mGame.mWindowW - mPosition.X
		End If
		If (mPosition.Y < 0.0 - 2 * mRadius) Or (mPosition.Y > mGame.mWindowH + 2 * mRadius) Then
			mPosition.Y = mGame.mWindowH - mPosition.Y
		End If
	End Sub


End Class
