Imports System.Numerics
Imports System.Security.Cryptography
Public Class Asteroid
	Inherits Actor

	Private mCircle As CircleComponent
	Private mAsteroidCooldown As Double
	Private mAliasNames As List(Of String)

	Sub New(ByRef game As Game)
		MyBase.New(game)

		'ランダムな位置と向きと大きさと初速で初期化
		Dim randPos As Vector2
		randPos.X = GetGame().mWindowWidth / 2.0
		randPos.Y = GetGame().mWindowHeight / 2.0
		'画面の中央3/5区画以外になるまで繰り返し処理
		While (randPos.X > GetGame().mWindowWidth * 0.2) And
			  (randPos.X < GetGame().mWindowWidth * 0.8) And
			  (randPos.Y > GetGame().mWindowHeight * 0.2) And
			  (randPos.Y < GetGame().mWindowHeight * 0.8)
			randPos.X = RandomNumberGenerator.GetInt32(0, GetGame().mWindowWidth)
			randPos.Y = RandomNumberGenerator.GetInt32(0, GetGame().mWindowHeight)
		End While
		SetPosition(randPos)
		SetRotation(RandomNumberGenerator.GetInt32(0, 1000) * 0.001 * Math.PI * 2)
		SetScale(0.1 * RandomNumberGenerator.GetInt32(8, 25))   '拡大率 0.8～2.5

		'スプライトコンポーネント作成、テクスチャ設定
		Dim sc As New SpriteComponent(Me, 40)
		sc.SetTexture(game.GetRenderer().GetTexture("Assets\Asteroid.png"))

		'MoveComponent作成
		Dim mc As New MoveComponent(Me, 10)
		mc.SetVelocity(GetForward() * RandomNumberGenerator.GetInt32(50, 200))
		mc.SetRotSpeed((RandomNumberGenerator.GetInt32(0, 2) * 2 - 1) * (RandomNumberGenerator.GetInt32(0, 1000) * 0.001 * Math.PI / 2))

		'CircleComponent作成
		mCircle = New CircleComponent(Me, 10)

		Dim files As New List(Of String)
		files.Add("Assets\destroy.mp3")
		mAliasNames = game.GetSoundPlayer().AddSound(files)

	End Sub

	Protected Overrides Sub Dispose(ByVal disposing As Boolean)
		If Not Me.disposed Then
			If disposing Then
				'*** アンマネージリソースの開放
			End If
			'*** マネージドリソースの開放
			GetGame().GetSoundPlayer().SetSoundControl(mAliasNames(0), "replay")
			GetGame().GetSoundPlayer().SetSoundControl(mAliasNames(0), "delete")
			GetGame().RemoveAsteroid(Me)
		End If
		MyBase.Dispose(disposing)
	End Sub

	Public Overrides Sub UpdateActor(ByVal detaTime As Double)
		'画面外にでたら反対の位置に移動（ラッピング処理）
		If (GetPosition().X < 0.0 - GetRadius() Or
			GetPosition().X > GetGame().mWindowWidth + GetRadius()) _
			   Then
			Dim v As Vector2
			v.X = GetGame().mWindowWidth - GetPosition().X
			v.Y = GetPosition().Y
			SetPosition(v)
		End If

		If (GetPosition().Y < 0.0 - GetRadius() Or
			GetPosition().Y > GetGame().mWindowHeight + GetRadius()) _
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
