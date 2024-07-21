Imports System.Numerics
Imports System.Security.Cryptography

Public Class Ship
    Inherits Actor

    Private mLaserCooldown As Double     'レーザーを次に撃てるまでの時間
    Private mCrash As Boolean        '衝突検知
    Private mCrashingTime As Double     '衝突演出時間
    Private mCrashCooldown As Double  '衝突演出後、リセットされるまでスプライトを消す時間
    Private mCrashPos As Vector2     '衝突時の位置
    Private mCrashRot As Double      '衝突時の向き
    Private mCircle As CircleComponent
    Private mSSC As SomeSpriteComponent
    Private mIC As InputComponent
    Private mAliasNames As New List(Of String)


    Sub New(ByRef game As Game)
        MyBase.New(game)
        mSSC = New SomeSpriteComponent(Me, 30)
        Dim mtexs = New List(Of Image) From {
            game.GetRenderer().GetTexture("Assets\Ship01.png"),
            game.GetRenderer().GetTexture("Assets\Ship02.png"),
            game.GetRenderer().GetTexture("Assets\Ship03.png"),
            game.GetRenderer().GetTexture("Assets\Ship04.png"),
            game.GetRenderer().GetTexture("Assets\Ship05.png")}
        mSSC.SetSomeTextures(mtexs)

        mIC = New InputComponent(Me, 10)
        'mIC.SetMaxForwardVelocity(200.0)
        'mIC.SetMaxRotSpeed(5.0)
        mIC.SetMaxForwardForce(300.0)
        mIC.SetMaxRotForce(150.0)
        mIC.SetMoveResist(30.0)
        mIC.SetRotResist(15.0)
        mIC.SetMass(1.0)
        mIC.SetForwardKey(Keys.Up)
        mIC.SetBackwardKey(Keys.Down)
        mIC.SetClockwiseKey(Keys.Right)
        mIC.SetCounterClockwiseKey(Keys.Left)

        mCircle = New CircleComponent(Me, 10)

        '前進、後進、右回転、左回転それぞれに音声を設定
        mAliasNames.Add(game.GetSoundPlayer().AddSoundControl("Assets/thruster.mp3"))
        mAliasNames.Add(game.GetSoundPlayer().AddSoundControl("Assets/thruster.mp3"))
        mAliasNames.Add(game.GetSoundPlayer().AddSoundControl("Assets/thruster.mp3"))
        mAliasNames.Add(game.GetSoundPlayer().AddSoundControl("Assets/thruster.mp3"))
        mAliasNames.Add(game.GetSoundPlayer().AddSoundControl("Assets/beam.mp3"))
        mAliasNames.Add(game.GetSoundPlayer().AddSoundControl("Assets/explosion.mp3"))

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
                    mCrashPos = GetPosition()
                    mCrashRot = GetRotation()
                    mCrash = True
                    mCrashCooldown = 4.0
                    mCrashingTime = 2.0
                    GetGame().GetSoundPlayer().SetControl(mAliasNames(5), "play")
                    Exit For
                End If
            Next
        Else
            If mCrashingTime > 0.0 Then
                '衝突時の演出。
                SetPosition(mCrashPos)       'MoveComponentが更新されても衝突したときの位置に置きなおし
                mCrashRot -= 3.0 * 2.0 * Math.PI * deltaTime
                SetRotation(mCrashRot)       'MoveComponentが更新されても衝突してからの回転角度に置きなおし
                SetScale(GetScale() * 0.98)
            Else
                If mCrashCooldown > 0.0 Then
                    '衝突演出後、リスポーンするまで表示停止
                    SetState(State.EPaused)
                    mSSC.SetVisible(False)
                Else
                    Init()
                End If

            End If
        End If
    End Sub

    Public Overrides Sub ActorInput(ByVal keyState As Boolean())
        MyBase.ActorInput(keyState)
        If mCrash = False Then
            If keyState(mIC.GetCounterClockwiseKey()) = True Then
                mSSC.SetTextureFromId(1)
                GetGame().GetSoundPlayer().SetControl(mAliasNames(0), "play")
            ElseIf keyState(mIC.GetClockwiseKey()) = True Then
                mSSC.SetTextureFromId(2)
                GetGame().GetSoundPlayer().SetControl(mAliasNames(1), "play")
            ElseIf keyState(mIC.GetForwardKey()) = True Then
                mSSC.SetTextureFromId(3)
                GetGame().GetSoundPlayer().SetControl(mAliasNames(2), "play")
            ElseIf keyState(mIC.GetBackwardKey()) = True Then
                mSSC.SetTextureFromId(4)
                GetGame().GetSoundPlayer().SetControl(mAliasNames(3), "play")
            Else
                mSSC.SetTextureFromId(0)
            End If

            If (keyState(Keys.Space) = True) And (mLaserCooldown <= 0.0) Then
                ' レーザーオブジェクトを作成、位置と回転角を宇宙船とあわせる。
                Dim laser As New Laser(GetGame())
                laser.SetPosition(GetPosition() + GetRadius() * GetForward())
                laser.SetRotation(GetRotation())
                laser.Shot()
                ' レーザー冷却期間リセット
                mLaserCooldown = 0.7
                GetGame().GetSoundPlayer().SetControl(mAliasNames(4), "replay")
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
        mIC.SetVelocity(Vector2.Zero)
        mIC.SetRotSpeed(0.0)
        SetState(State.EActive)
        mSSC.SetVisible(True)

        mLaserCooldown = 0.0
        mCrashCooldown = 0.0
        mCrashingTime = 0.0
        mCrash = False
    End Sub
End Class