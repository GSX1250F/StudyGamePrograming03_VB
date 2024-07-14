Imports System.Numerics

Public Class BackGround
	Inherits Actor

	Private mBGsprites As New List(Of BGSpriteComponent)
	Sub New(ByRef game As Game)
		MyBase.New(game)

		LoadData()
	End Sub
	Public Overrides Sub UpdateActor(ByVal deltaTime As Double)
		' 各BGSpriteComponentを更新
		For Each sprite In mBGsprites
			sprite.Update(deltaTime)
		Next
	End Sub
	Private Sub LoadData()
		'BGSpriteComponentを作成し、配列に追加する。
		'背景1つ目
		Dim bgsc As New BGSpriteComponent(Me, 5)
		Dim v As Vector2
		v = New Vector2(GetGame().mWindowWidth * 0.5, GetGame().mWindowHeight * 0.5)
		bgsc.SetTexture(GetGame().GetRenderer().GetTexture("Assets/Farback01.png"))
		bgsc.SetBGSpritePos(v)
		v = New Vector2(-10.0, 0.0)
		bgsc.SetScrollSpeed(v)
		mBGsprites.Add(bgsc)

		'背景2つ目
		bgsc = New BGSpriteComponent(Me, 5)
		bgsc.SetTexture(GetGame().GetRenderer().GetTexture("Assets/Farback02.png"))
		v = New Vector2(GetGame().mWindowWidth * 0.5 + bgsc.GetTexWidth(), GetGame().mWindowHeight * 0.5)
		bgsc.SetBGSpritePos(v)
		v = New Vector2(-10.0, 0.0)
		bgsc.SetScrollSpeed(v)
		mBGsprites.Add(bgsc)

		'背景3つ目
		bgsc = New BGSpriteComponent(Me, 10)
		bgsc.SetTexture(GetGame().GetRenderer().GetTexture("Assets/Stars.png"))
		v = New Vector2(GetGame().mWindowWidth * 0.5, GetGame().mWindowHeight * 0.5)
		bgsc.SetBGSpritePos(v)
		v = New Vector2(-20.0, 0.0)
		bgsc.SetScrollSpeed(v)
		mBGsprites.Add(bgsc)

		'背景4つ目
		bgsc = New BGSpriteComponent(Me, 10)
		bgsc.SetTexture(GetGame().GetRenderer().GetTexture("Assets/Stars.png"))
		v = New Vector2(GetGame().mWindowWidth * 0.5 + bgsc.GetTexWidth(), GetGame().mWindowHeight * 0.5)
		bgsc.SetBGSpritePos(v)
		v = New Vector2(-20.0, 0.0)
		bgsc.SetScrollSpeed(v)
		mBGsprites.Add(bgsc)
	End Sub

End Class