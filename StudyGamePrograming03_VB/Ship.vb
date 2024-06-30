Imports System.Buffers
Imports System.Numerics
Imports System.Security.Cryptography

Public Class Ship
    Inherits Actor

    Private mLaserCooldown As Double     'レーザーを次に撃てるまでの時間
    Private mCrash As Boolean        '衝突検知
    Private mCrashingTime As Double     '衝突演出時間
    Private mCrashCooldown As Double  '衝突演出後、リセットされるまでスプライトを消す時間
    Private crashPos As Vector2     '衝突時の位置
    Private crashRot As Double      '衝突時の向き
    Private mCircle As CircleComponent
    Private mASC As AnimSpriteComponent
    Private mInput As InputComponent

    Sub New(ByRef game As Game)
        MyBase.New(game)

        'アニメーションのスプライトコンポーネント作成、テクスチャ設定
        mASC = New AnimSpriteComponent(Me, 30)
        Dim anims = New List(Of Image) From {
            game.GetTexture("\Assets\Ship01.png"),
            game.GetTexture("\Assets\Ship02.png"),
            game.GetTexture("\Assets\Ship03.png"),
            game.GetTexture("\Assets\Ship04.png"),
            game.GetTexture("\Assets\Ship05.png")}
        mASC.SetAnimTextures(anims, 1, 1, True)

        mInput = New InputComponent(Me, 10)
        'mInput.SetMaxForwardVelocity(200.0)
        'mInput.SetMaxRotSpeed(5.0)
        mInput.SetMaxForwardForce(300.0)
        mInput.SetMaxRotForce(150.0)
        mInput.SetMoveResist(30.0)
        mInput.SetRotResist(15.0)
        mInput.SetMass(1.0)
        mInput.SetForwardKey(Keys.Up)
        mInput.SetBackwardKey(Keys.Down)
        mInput.SetCrockwardKey(Keys.Right)
        mInput.SetCounterCrockwardKey(Keys.Left)

        mCircle = New CircleComponent(Me, 10)

        Init()
    End Sub

    Public Overrides Sub UpdateActor(ByVal deltaTime As Double)
        mLaserCooldown -= deltaTime
        mCrashCooldown -= deltaTime
        mCrashingTime -= deltaTime

        If (mCrash = False) Then
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
                    crashPos = GetPosition()
                    crashRot = GetRotation()
                    mCrash = True
                    mCrashCooldown = 4.0
                    mCrashingTime = 2.0
                    Exit For
                End If
            Next
        Else
            If mCrashingTime > 0.0 Then
                '衝突時の演出。
                SetPosition(crashPos)       'MoveComponentが更新されても衝突したときの位置に置きなおし
                crashRot -= 3.0 * 2.0 * Math.PI * deltaTime
                SetRotation(crashRot)       'MoveComponentが更新されても衝突してからの回転角度に置きなおし
                SetScale(GetScale() * 0.98)
            Else
                If mCrashCooldown > 0.0 Then
                    '衝突演出後、リスポーンするまで表示停止
                    SetState(State.EPaused)
                Else
                    Init()
                End If

            End If
        End If
    End Sub

    Public Overrides Sub ActorInput(ByVal keyState As Boolean())
        MyBase.ActorInput(keyState)

        If mCrash = False Then
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
        SetScale(0.8)
        Dim v As Vector2
        v.X = GetGame().mWindowWidth / 2
        v.Y = GetGame().mWindowHeight / 2
        SetPosition(v)
        SetRotation(RandomNumberGenerator.GetInt32(0, 1000) * 0.01 * Math.PI * 2.0)
        'SetRotation(0.0)
        mInput.SetVelocity(Vector2.Zero)
        mInput.SetRotSpeed(0.0)
        SetState(State.EActive)

        mLaserCooldown = 0.0
        mCrashCooldown = 0.0
        mCrashingTime = 0.0
        mCrash = False
    End Sub
End Class
