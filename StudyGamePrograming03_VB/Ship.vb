Imports System.Buffers
Imports System.Numerics

Public Class Ship
    Inherits Actor
    Sub New(ByRef game As Game)
        MyBase.New(game)
        mLaserCooldown = 0.0
        mCrashCooldown = 0.0
        mShipCooldown = 0.0
        crashPos.X = 0.0
        crashPos.Y = 0.0
        crash = False

        mScale = 0.8

        'アニメーションのスプライトコンポーネント作成、テクスチャ設定
        Dim asc As New AnimSpriteComponent(Me, 30)
        Dim anims = New List(Of Image) From {
            game.GetTexture("\Assets\Ship01.png"),
            game.GetTexture("\Assets\Ship02.png"),
            game.GetTexture("\Assets\Ship03.png"),
            game.GetTexture("\Assets\Ship04.png"),
            game.GetTexture("\Assets\Ship05.png")}
        asc.SetAnimTextures(anims, 1, 1, True)
        mAnimComponent = asc

        'InputComponent作成.MoveComponentの子
        mInput = New InputComponent(Me, 10)
        mInput.mForwardKey = Keys.Up
        mInput.mBackwardKey = Keys.Down
        mInput.mClockwiseKey = Keys.Right
        mInput.mCounterClockwiseKey = Keys.Left
        mInput.mMaxForwardForce = 300.0
        mInput.mMaxRotForce = 150.0
        mInput.mMoveResist = 30.0
        mInput.mRotResist = 30.0
        mInput.mMass = 1.0

        'CircleComponent作成
        mCircle = New CircleComponent(Me, 10)

        Init()
    End Sub

    Public Overrides Sub UpdateActor(deltaTime As Single)

        mLaserCooldown -= deltaTime     'レーザーを次に撃てるまでの時間

        If crash = False Then
            '小惑星と衝突していないとき
            '画面外にでたら反対の位置に移動（ラッピング処理）
            If mPosition.X < 0.0 - 1 * mRadius Or
               mPosition.X > mGame.mWindowW + 1 * mRadius Then
                mPosition.X = mGame.mWindowW - mPosition.X
            End If
            If mPosition.Y < 0.0 - 1 * mRadius Or
               mPosition.Y > mGame.mWindowH + 1 * mRadius Then
                mPosition.Y = mGame.mWindowH - mPosition.Y
            End If

            '小惑星と衝突したかを判定
            For Each ast In mGame.mAsteroids
                If (mCircle.Intersect(mCircle, ast.mCircle)) Then
                    '小惑星と衝突したとき
                    crashPos = mPosition
                    crashRot = mRotation
                    crash = True
                    mCrashCooldown = 2.0
                    mShipCooldown = 2.0
                    'ゲーム自体を終了する場合
                    'mGame.SetRunning(False)

                    crash = True
                    Exit For
                End If
            Next

        Else
            '小惑星と衝突したとき
            If mState = State.EPaused Then
                '状態がEPausedのとき、リスポーンするまでの時間を計算
                mShipCooldown -= deltaTime
                'リスポーンするまでの時間になったら、初期位置・角度にリスポーン
                If mShipCooldown <= 0.0 Then
                    Init()
                    mState = State.EActive
                    mShipCooldown = 0.0
                    crash = False
                End If
            Else
                '衝突演出中
                mPosition = crashPos        ' MoveComponentが更新されても衝突したときの位置に置きなおし
                crashRot -= 3.0 * 2 * Math.PI * deltaTime
                mRotation = crashRot        ' MoveComponentが更新されても衝突してからの回転角度に置きなおし
                mCrashCooldown -= deltaTime
                If mCrashCooldown <= 0.0 Then
                    mState = State.EPaused
                    mCrashCooldown = 0.0
                End If
            End If
        End If
    End Sub

    Public Overrides Sub ActorInput(keyState As KeyEventArgs)
        MyBase.ActorInput(keyState)

        If crash = False Then
            If Not keyState Is Nothing Then
                If keyState.KeyCode = Keys.Left Then
                    mAnimComponent.SetAnimNum(2, 2, False)
                ElseIf keyState.KeyCode = Keys.Right Then
                    mAnimComponent.SetAnimNum(3, 3, False)
                ElseIf keyState.KeyCode = Keys.Up Then
                    mAnimComponent.SetAnimNum(4, 4, False)
                ElseIf keyState.KeyCode = Keys.Down Then
                    mAnimComponent.SetAnimNum(5, 5, False)
                End If


                If keyState.KeyCode = Keys.Space And mLaserCooldown <= 0.0 Then
                    ' レーザーオブジェクトを作成、位置と回転角を宇宙船とあわせる。
                    Dim laser As Laser = New Laser(mGame)
                    laser.mPosition.X = mPosition.X + mRadius * Math.Cos(mRotation)
                    laser.mPosition.Y = mPosition.Y - mRadius * Math.Sin(mRotation)
                    laser.mRotation = mRotation
                    laser.Shot()
                    ' レーザー冷却期間リセット
                    mLaserCooldown = 0.5
                End If
            End If
            If mAnimComponent.mIsAnimating = False Then
                ' アニメーション中が終わっていたら元のループに戻る。
                mAnimComponent.SetAnimNum(1, 1, True)
            End If

        End If

    End Sub

    Public Sub Init()
        mPosition.X = mGame.mWindowW / 2
        mPosition.Y = mGame.mWindowH / 2

        mRotation = 0.0

        mInput.mVelocity.X = 0.0
        mInput.mVelocity.Y = 0.0
        mInput.mRotSpeed = 0.0
    End Sub

    Public mLaserCooldown As Single
    Public crashPos As Vector2     '衝突したときの位置
    Public crashRot As Single      '衝突したときの向き
    Public crash As Boolean        '衝突検知
    Public mCrashCooldown As Single     '衝突演出時間
    Public mShipCooldown As Single  '衝突演出後、リセットされるまでスプライトを消す時間

    Public mCircle As CircleComponent      '衝突チェックのためのアクセスポインタ。他のオブジェクトから参照するため。
    Public mAnimComponent As AnimSpriteComponent
    Public mInput As InputComponent      '衝突チェックのためのアクセスポインタ。他のオブジェクトから参照するため。

End Class
