Imports System.ComponentModel
Imports System.Numerics
Imports System.Security.Policy
Public Class Actor
	Public Enum State
		EActive     '稼働中
		EPaused     '休止中
		EDead       '削除対象
	End Enum
	Sub New(ByRef game As Game)
		mState = State.EActive
		mPosition = New Vector2(0, 0)
		mVelocity = New Vector2(0, 0)
		mScale = 1.0
		mRotation = 0.0
		mRotSpeed = 0.0
		mMass = 1.0
		mRadius = 0.0
		mImoment = 0.0
		mGame = game
		mGame.AddActor(Me)
	End Sub

	'ゲームから呼び出される更新関数(オーバーライド不可)
	Public Sub Update(ByVal deltaTime As Single)
		If mState = State.EActive Or mState = State.EPaused Then
			mPosition += mVelocity * deltaTime
			mRotation += mRotSpeed * deltaTime
			UpdateComponents(deltaTime)
			UpdateActor(deltaTime)
		End If
	End Sub
	'アクターが持つ全コンポーネントを更新(オーバーライド不可)
	Public Sub UpdateComponents(ByVal deltaTime As Single)
		For Each comp In mComponents
			comp.Update(deltaTime)
		Next
	End Sub
	'アクター独自の更新処理(オーバーライド可能)
	Public Overridable Sub UpdateActor(ByVal deltaTime As Single)
	End Sub

	'ゲームから呼び出されるProcess Input(オーバーライド不可)
	Public Sub ProcessInput(ByVal keyState As System.Windows.Forms.KeyEventArgs)
		If mState = State.EActive Then
			For Each comp In mComponents
				comp.ProcessInput(keyState)
			Next
		End If
		ActorInput(keyState)
	End Sub
	'アクター独自の入力処理(オーバーライド可能)
	Public Overridable Sub ActorInput(ByVal keyState As System.Windows.Forms.KeyEventArgs)
	End Sub

	'Getters/setters	とりあえず無し。すべてPublic変数とする

	Public mState As State              ' アクターの状態
	' 移動
	Public mPosition As Vector2        '位置
	Public mVelocity As Vector2        '速度x,y
	Public mScale As Single            '拡大率
	Public mRotation As Single         '回転
	Public mRotSpeed As Single         '回転速度
	Public mMass As Single             '質量
	Public mRadius As Single           '半径（拡大率は無視）
	Public mImoment As Single          '慣性モーメント
	Public mComponents As New List(Of Component)
	Public mGame As Game
	Public Function GetForward() As Vector2
		Dim v = New Vector2(Math.Cos(mRotation), -Math.Sin(mRotation))       '向きの単位ベクトル
		Return v
	End Function

	' Add/remove components
	Public Sub AddComponent(ByRef component As Component)
		'ソート済みの配列で挿入点を見つける
		Dim myOrder As Integer = component.mUpdateOrder
		Dim cnt As Integer = mComponents.Count     '配列の要素数
		Dim i As Integer = 0
		If cnt > 0 Then
			For i = 0 To mComponents.Count - 1
				If myOrder < mComponents(i).mUpdateOrder Then
					Exit For
				End If
			Next
		End If
		mComponents.Insert(i, component)
	End Sub
	Public Sub RemoveComponent(ByRef component As Component)
		Dim iter As Integer = mComponents.IndexOf(component)
		'見つからなかったら-1が返される。
		iter = mComponents.IndexOf(component)
		If iter >= 0 Then
			mComponents.RemoveAt(iter)
		End If
	End Sub
End Class

