Imports System.Media
Imports System.Numerics
Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles.VisualStyleElement.TrayNotify

Public Class Game
    <DllImport("user32.dll", ExactSpelling:=True)>
    Private Shared Function GetKeyboardState(ByVal keyStates() As Byte) As Boolean
    End Function
#If Win64 Then
    Private Declare PtrSafe Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String,     ByVal uReturnLength As Long, ByVal hwndCallback As Long) As Long
#Else
    Private Declare Function mciSendString Lib "winmm" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Long, ByVal hwndCallback As Long) As Long
#End If

    '変数群
    'Public
    Public mWindowWidth As Integer      'PictureBoxの横幅
    Public mWindowHeight As Integer     'PictureBoxの縦幅

    'Private
    Private Ticks As New System.Diagnostics.Stopwatch()     '時間管理
    Private mTicksCount As Integer     'ゲーム開始時からの経過時間
    Private mIsRunning As Boolean   'ゲーム実行中
    Private mRenderer As Renderer   'レンダラー
    Private mPictureBox As PictureBox   'PictureBox
    Private mKeyBoardByte(255) As Byte      'キーボード入力検知
    Private mKeyState(255) As Boolean      'キーボード状態
    Private mSoundPlayer As SoundPlayer     'サウンドプレイヤ
    Private mUpdatingActors As Boolean      'アクター更新中
    Private mActors As New List(Of Actor)   'すべてのアクター
    Private mPendingActors As New List(Of Actor)    'すべての待ちアクター


    'game specific
    Private mShip As Ship
    Private mAsteroids As New List(Of Asteroid)

    'コンストラクタ
    Public Sub Game_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        mRenderer = Nothing
        mIsRunning = True
        mUpdatingActors = False

        Dim success = Initialize()
        If success = True Then
            mIsRunning = True
            LoadData()
        Else
            Shutdown()
        End If
    End Sub

    Public Function Initialize() As Boolean
        'ウィンドウ初期化
        mWindowWidth = 1024
        mWindowHeight = 768
        Me.SetDesktopBounds(100, 100, mWindowWidth + 26, mWindowHeight + 49)
        Me.DoubleBuffered = True
        'PictureBox初期化
        mPictureBox = PictureBox
        mPictureBox.SetBounds(5, 5, mWindowWidth, mWindowHeight)
        'レンダラー作成
        mRenderer = New Renderer(Me)
        If (mRenderer.Initialize(mWindowWidth, mWindowHeight)) = False Then
            mRenderer.Dispose()
        End If
        'サウンドプレイヤ作成
        mSoundPlayer = New SoundPlayer(Me)
        'ストップウォッチ開始
        Ticks.Start()
        'タイマー開始
        RunLoop.Interval = 16
        RunLoop.Enabled = True
        mTicksCount = Ticks.ElapsedMilliseconds

        Return True
    End Function
    Private Sub RunLoop_Tick(sender As Object, e As EventArgs) Handles RunLoop.Tick
        If mIsRunning Then
            ProcessInput()
            UpdateGame()
            GenerateOutput()
        Else
            Shutdown()
        End If
    End Sub

    Private Sub ProcessInput()
        GetKeyboardState(mKeyBoardByte)
        For i As Integer = 0 To mKeyBoardByte.Count - 1
            'キー入力状態を、ON=True, OFF=Falseに変換
            mKeyState(i) = CBool(mKeyBoardByte(i) And &H80)
        Next

        If mKeyState(Keys.Escape) = True Then
            mIsRunning = False
        End If

        mUpdatingActors = True
        For Each actor In mActors
            actor.ProcessInput(mKeyState)
        Next
        mUpdatingActors = False

    End Sub
    Private Sub UpdateGame()
        '前のフレームから16ms経つまで待つ
        While Ticks.ElapsedMilliseconds < mTicksCount + 16
        End While
        'デルタタイムの計算
        Dim deltaTime As Double = (Ticks.ElapsedMilliseconds - mTicksCount) / 1000

        'デルタタイムを最大値で制限する
        If deltaTime > 0.05 Then
            deltaTime = 0.05
        End If
        mTicksCount = Ticks.ElapsedMilliseconds

        'すべてのアクターを更新
        mUpdatingActors = True
        For Each actor In mActors
            actor.Update(deltaTime)
        Next
        mUpdatingActors = False

        '待ちアクターをmActorsに移動
        For Each pending In mPendingActors
            mActors.Add(pending)
        Next
        mPendingActors.Clear()

        '死んだアクターを一時配列に追加
        Dim deadActors As New List(Of Actor)
        For Each actor In mActors
            If actor.GetState() = Actor.State.EDead Then
                deadActors.Add(actor)
            End If
        Next

        '死んだアクターを削除
        For Each actor In deadActors
            actor.Dispose()
        Next
    End Sub
    Private Sub GenerateOutput()
        mRenderer.Draw()
        mSoundPlayer.Play()
    End Sub
    Private Sub Shutdown()
        UnloadData()
        If mRenderer IsNot Nothing Then
            mRenderer.Shutdown()
        End If
        If mSoundPlayer IsNot Nothing Then
            mSoundPlayer.Shutdown()
        End If

        Ticks.Stop()
        Me.Close()
    End Sub
    Private Sub LoadData()
        mShip = New Ship(Me)    'プレイヤーの宇宙船を作成

        '小惑星を複数生成
        Dim initialNumAsteroids = 15        '初期値
        For i As Integer = 0 To initialNumAsteroids - 1
            AddAsteroid()
        Next

        Dim astCtrl As New AsteroidControl(Me)

        '背景を作成
        Dim bg As New BackGround(Me)

        Dim clrPict As New ClearPict(Me)

    End Sub
    Private Sub UnloadData()
        While mActors.Count > 0
            mActors.First.Dispose()
        End While
        If mRenderer IsNot Nothing Then
            mRenderer.UnloadData()
        End If
        If mSoundPlayer IsNot Nothing Then
            mSoundPlayer.UnloadData()
        End If
    End Sub
    Public Sub AddActor(ByRef actor As Actor)
        If mUpdatingActors Then
            mPendingActors.Add(actor)
        Else
            mActors.Add(actor)
        End If
    End Sub

    Public Sub RemoveActor(ByRef actor As Actor)
        '待ちアクターを検索し、消去
        Dim iter As Integer = mPendingActors.IndexOf(actor)
        '見つからなかったら-1が返される。
        If iter >= 0 Then
            mPendingActors.RemoveAt(iter)
        End If
        'アクターを検索し、消去
        iter = mActors.IndexOf(actor)
        If iter >= 0 Then
            mActors.RemoveAt(iter)
        End If
    End Sub
    Public Function GetRenderer() As Renderer
        Return mRenderer
    End Function
    Public Function GetPictureBox() As PictureBox
        Return mPictureBox
    End Function
    Public Function GetSoundPlayer() As SoundPlayer
        Return mSoundPlayer
    End Function

    Public Sub SetRunning(ByVal isrunning As Boolean)
        mIsRunning = isrunning
    End Sub


    'Game Specific
    Public Function GetShip() As Ship
        Return mShip
    End Function
    Public Function GetAsteroids() As List(Of Asteroid)
        Return mAsteroids
    End Function
    Public Sub AddAsteroid()
        Dim ast As New Asteroid(Me)
        mAsteroids.Add(ast)
    End Sub

    Public Sub RemoveAsteroid(ByRef ast As Asteroid)
        Dim iter As Integer = mAsteroids.IndexOf(ast)       'Listの中になかったら-1が返される
        If iter >= 0 Then
            mAsteroids.RemoveAt(iter)
        End If
    End Sub
End Class
