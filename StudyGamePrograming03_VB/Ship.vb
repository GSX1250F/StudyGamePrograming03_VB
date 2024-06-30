Imports System.Buffers
Imports System.Numerics

Public Class Ship
    Inherits Actor

    Private mLaserCooldown As Double
    Private crashPos As Vector2     '衝突したときの位置
    Private crashRot As Double      '衝突したときの向き
    Private crash As Boolean        '衝突検知
    Private mCrashCooldown As Double     '衝突演出時間
    Private mShipCooldown As Double  '衝突演出後、リセットされるまでスプライトを消す時間
    Private mAsteroidCooldown As Double      '小惑星増殖までの待機時間

    Private mCircle As CircleComponent      '衝突チェックのためのアクセスポインタ。他のオブジェクトから参照するため。
    Private mASC As AnimSpriteComponent
    Private mInput As InputComponent      '衝突チェックのためのアクセスポインタ。他のオブジェクトから参照するため。

    Sub New(ByRef game As Game)
        MyBase.New(game)
        mLaserCooldown = 0.0
        mCrashCooldown = 0.0
        mShipCooldown = 0.0
        mAsteroidCooldown = 3.0
        crashPos.X = 0.0
        crashPos.Y = 0.0
        crash = False
        SetScale(0.8)

        'アニメーションのスプライトコンポーネント作成、テクスチャ設定
        mASC = New AnimSpriteComponent(Me, 30)
        Dim anims = New List(Of Image) From {
            game.GetTexture("\Assets\Ship01.png"),
            game.GetTexture("\Assets\Ship02.png"),
            game.GetTexture("\Assets\Ship03.png"),
            game.GetTexture("\Assets\Ship04.png"),
            game.GetTexture("\Assets\Ship05.png")}
        mASC.SetAnimTextures(anims, 1, 1, True)


        'InputComponent作成.MoveComponentの子
        mInput = New InputComponent(Me, 10)
        mInput.SetMaxForwardVelocity(200.0)
        mInput.SetMaxRotSpeed(5.0)
        'mInput.SetMaxForwardForce(300.0)
        'mInput.SetMaxRotForce(150.0)
        'mInput.SetMoveResist(30.0)
        'mInput.SetRotResist(15.0)
        'mInput.SetMass(1.0)
        mInput.SetForwardKey(Keys.Up)
        mInput.SetBackwardKey(Keys.Down)
        mInput.SetCrockwardKey(Keys.Right)
        mInput.SetCounterCrockwardKey(Keys.Left)

        'CircleComponent作成
        mCircle = New CircleComponent(Me, 10)

        Init()
    End Sub

    Public Overrides Sub UpdateActor(ByVal deltaTime As Double)
        mLaserCooldown -= deltaTime     'レーザーを次に撃てるまでの時間
        mAsteroidCooldown -= deltaTime

        '小惑星を一定時間ごとに増やす。小惑星の数が０になったらゲームクリア画面をアクティブにする。
        Dim numAsteroids As Integer = GetGame().GetAsteroids().Count()
        If (mAsteroidCooldown < 0.0 And numAsteroids > 0) Then
            GetGame().AddAsteroid()
            mAsteroidCooldown = 5.0
        End If
        If (numAsteroids = 0) Then
            GetGame().GetClearPict().SetState(State.EActive)
        End If

        If (crash = False) Then
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

            '小惑星と衝突したかを判定
            For Each ast In GetGame().GetAsteroids()
                If Intersect(mCircle, ast.GetCircle()) And ast.GetState() = State.EActive Then
                    '小惑星と衝突
                    crashPos = GetPosition()
                    crashRot = GetRotation()
                    crash = True
                    mCrashCooldown = 2.0
                    mShipCooldown = 2.0

                    'ゲーム自体を終了する場合
                    'GetGame()->SetRunning(false);
                    Exit For
                End If
            Next
        Else
            '小惑星と衝突したとき
            If (GetState() = State.EPaused) Then
                '状態がEPausedのとき、リスポーンするまでの時間を計算
                mShipCooldown -= deltaTime
                'リスポーンするまでの時間になったら、初期位置・角度にリスポーン
                If (mShipCooldown <= 0.0) Then
                    Init()
                    SetState(State.EActive)
                    mShipCooldown = 0.0F
                    crash = False
                End If
            Else
                '衝突演出中
                SetPosition(crashPos)       'MoveComponentが更新されても衝突したときの位置に置きなおし
                crashRot -= 3.0 * 2.0 * Math.PI * deltaTime
                SetRotation(crashRot)       'MoveComponentが更新されても衝突してからの回転角度に置きなおし
                mCrashCooldown -= deltaTime
                If (mCrashCooldown <= 0.0F) Then
                    SetState(State.EPaused)
                    mCrashCooldown = 0.0
                End If
            End If
        End If
    End Sub

    Public Overrides Sub ActorInput(ByVal keyState As Boolean())
        MyBase.ActorInput(keyState)

        If crash = False Then
            If keyState(Keys.Left) = True Then
                mASC.SetAnimNum(2, 2, False)
            ElseIf keyState(Keys.Right) = True Then
                mASC.SetAnimNum(3, 3, False)
            ElseIf keyState(Keys.Up) = True Then
                mASC.SetAnimNum(4, 4, False)
            ElseIf keyState(Keys.Down) = True Then
                mASC.SetAnimNum(5, 5, False)
            End If


            If (keyState(Keys.Space) = True) And (mLaserCooldown <= 0.0) Then
                ' レーザーオブジェクトを作成、位置と回転角を宇宙船とあわせる。
                Dim laser As New Laser(GetGame())
                laser.SetPosition(GetPosition() + GetRadius() * GetForward())
                laser.SetRotation(GetRotation())
                laser.Shot()
                ' レーザー冷却期間リセット
                mLaserCooldown = 0.7
            End If
            If mASC.mIsAnimating = False Then
                ' アニメーション中が終わっていたら元のループに戻る。
                mASC.SetAnimNum(1, 1, True)
            End If

        End If

    End Sub

    Public Sub Init()
        Dim v As Vector2
        v.X = GetGame().mWindowWidth / 2
        v.Y = GetGame().mWindowHeight / 2
        SetPosition(v)
        Dim random As New Random()
        Dim rot As Double = 2.0 * random.NextDouble() * Math.PI
        'Dim rot As Double = 0.0
        SetRotation(rot)
        mInput.SetVelocity(Vector2.Zero)
        mInput.SetRotSpeed(0.0)
    End Sub
End Class
