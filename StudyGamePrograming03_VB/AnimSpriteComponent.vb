Public Class AnimSpriteComponent
	Inherits SpriteComponent

	' アニメーションをループさせるか
	Public mLoopFlag As Boolean

	' アニメーション中かどうか
	Public mIsAnimating As Boolean

	' アニメーションでのすべてのテクスチャ
	Public mAnimTextures As New List(Of Image)

	' 現在表示中のアニメーションテクスチャのための番号
	Public mAnimNumBeg As Integer
	Public mAnimNumLast As Integer
	' 現在表示中のフレーム
	Public mCurrFrame As Single
	' アニメーションのフレームレート
	Public mAnimFPS As Single

	Sub New(ByRef owner As Actor, ByVal drawOrder As Integer)
		MyBase.New(owner, drawOrder)
		mCurrFrame = 0.0
		mAnimFPS = 12
		mAnimNumBeg = 0
		mAnimNumLast = 0
	End Sub
	' フレームごとにアニメーションを更新する(componentからオーバーライド)
	Public Overrides Sub Update(deltaTime As Single)
		If (mAnimTextures.Count() > 0) Then
			'フレームレートとデルタタイムに基づいて
			'カレントフレームを更新する
			mCurrFrame += mAnimFPS * deltaTime

			'ループさせないアニメーションが終わったら止める。
			If (mLoopFlag = False) Then
				If (mCurrFrame >= mAnimNumLast - (mAnimNumBeg - 1)) Then
					mIsAnimating = False    ' アニメーションが止まった
					mCurrFrame = (mAnimNumLast - mAnimNumBeg) + 0.99    '次のUpdate時に同じくifが成立するようにしておく。
				Else mIsAnimating = True    ' アニメーション中
					'if (mLoopFlag == false)std:cout << static_cast < Int() > (mCurrFrame) << "\n";
				End If
			Else
				mIsAnimating = True     ' ループアニメはアニメーション中とする。
				' 必要に応じてカレントフレームを巻き戻す
				Do While (mCurrFrame >= mAnimNumLast - (mAnimNumBeg - 1))
					mCurrFrame -= (mAnimNumLast - (mAnimNumBeg - 1))
				Loop
			End If

			' 現時点でのテクスチャを設定する
			SetTexture(mAnimTextures(Fix(mCurrFrame) + (mAnimNumBeg - 1)))
		End If
	End Sub

	' アニメーションに使うテクスチャを設定する
	Public Sub SetAnimTextures(ByRef textures As List(Of Image), ByVal beg As Integer, ByVal last As Integer, loop_flag As Boolean)
		SetAnimNum(beg, last, loop_flag)
		mAnimTextures = textures
		' アクティブなテクスチャを最初のフレームに設定する
		If mAnimTextures.Count > 0 Then
			SetTexture(mAnimTextures(0))
		End If
	End Sub

	' アニメーションに使うテクスチャの範囲を設定する
	Public Sub SetAnimNum(ByVal beg As Integer, ByVal last As Integer, ByVal loop_flag As Integer)
		mCurrFrame = 0.0
		mAnimNumBeg = beg
		mAnimNumLast = last
		' ループするか否かの判定
		mLoopFlag = loop_flag
	End Sub


End Class
