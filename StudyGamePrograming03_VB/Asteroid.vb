Imports System.Numerics

Public Class Asteroid
	Inherits Actor

	Private mCircle As CircleComponent
	Private mAsteroidCooldown As Single

	Sub New(ByRef game As Game)
		MyBase.New(game)

		'ランダムな位置と向きと大きさと初速で初期化
		Dim randPos As Vector2
		Dim random As New Random()     ' Randomオブジェクトの作成 
		randPos.X = GetGame().mWindowWidth / 2.0
		randPos.Y = GetGame().mWindowHeight / 2.0
		'画面の中央1/4区画以外を初期位置とする。
		While (randPos.X > GetGame().mWindowWidth * 0.25) And
			  (randPos.X < GetGame().mWindowWidth * 0.75) And
			  (randPos.Y > GetGame().mWindowHeight * 0.25) And
			  (randPos.Y < GetGame().mWindowHeight * 0.75)

			randPos.X = random.Next(0, GetGame().mWindowWidth)
			randPos.Y = random.Next(0, GetGame().mWindowHeight)
		End While
		SetPosition(randPos)
		Dim randRot As Single = random.NextSingle() * Math.PI * 2
		SetRotation(randRot)
		SetScale(0.1 * Random.Next(8, 25))   '拡大率 0.8～2.5
		Dim rotSpeed = 2 * Math.PI * random.NextSingle() - Math.PI     '回転速度 -π～π
		Dim randSpeed As Integer = random.Next(50, 200)     '速度 50～200
		Dim randAngle As Single = Math.PI * random.Next(20, 70) / 180   '速度の方向角度　20度～70度
		Dim randVel As Vector2
		randVel.X = Math.Cos(randRot) * randSpeed
		randVel.Y = -Math.Sin(randRot) * randSpeed

		'スプライトコンポーネント作成、テクスチャ設定
		Dim sc As New SpriteComponent(Me, 40)
		sc.SetTexture(game.GetTexture("\Assets\Asteroid.png"))

		'MoveComponent作成
		Dim mc As New MoveComponent(Me, 10)
		mc.SetVelocity(randVel)
		mc.SetRotSpeed(rotSpeed)

		'CircleComponent作成
		mCircle = New CircleComponent(Me, 10)


	End Sub

	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		If Not Me.disposed Then
			If disposing Then
				' Insert code to free managed resources.
			End If
			' Insert code to free unmanaged resources.
			GetGame().RemoveAsteroid(Me)
		End If
		MyBase.Dispose(disposing)
	End Sub

	Public Overrides Sub UpdateActor(detaTime As Single)
		'画面外にでたら反対の位置に移動（ラッピング処理）
		If (GetPosition().X < 0.0 - 1.0 * GetRadius() Or
			   GetPosition().X > GetGame().mWindowWidth + 1.0 * GetRadius()) _
			   Then
			Dim v As Vector2
			v.X = GetGame().mWindowWidth - GetPosition().X
			v.Y = GetPosition().Y
			SetPosition(v)
		End If

		If (GetPosition().Y < 0.0 - 1.0 * GetRadius() Or
			GetPosition().Y > GetGame().mWindowHeight + 1.0 * GetRadius()) _
			Then
			Dim v As Vector2
			v.X = GetPosition().X
			v.Y = GetGame().mWindowHeight - GetPosition().Y
			SetPosition(v)
		End If

	End Sub

	Public Function GetCircle() As CircleComponent
		Return mCircle
	End Function
End Class
