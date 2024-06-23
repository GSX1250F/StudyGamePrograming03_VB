Imports System.Numerics
Imports System.Runtime.InteropServices
Imports System.Windows.Forms.VisualStyles.VisualStyleElement


Public Class Game
    <DllImport("user32.dll", ExactSpelling:=True)>
    Private Shared Function GetKeyboardState(ByVal keyStates() As Integer) As Boolean
    End Function

    '変数群
    'Public
    Public mWindowWidth As Integer      'PictureBoxの横幅
    Public mWindowHeight As Integer     'PictureBoxの縦幅

    'Private
    'テクスチャの配列
    Private mTextures As New Dictionary(Of String, Image)
    'すべてのアクター
    Private mActors As New List(Of Actor)
    'すべての待ちアクター
    Private mPendingActors As New List(Of Actor)
    'すべての描画されるスプライトコンポーネント
    Private mSprites As New List(Of SpriteComponent)

    Private mWindow As Bitmap       '描画用
    Private mRenderer As Graphics   'レンダラー
    Private Ticks As New System.Diagnostics.Stopwatch()     '時間管理
    Private mTicksCount As Integer     '時間管理
    Private mIsRunning As Boolean   '実行中
    Private mUpdatingActors As Boolean      'アクター更新中
    Private mKeyDowns As New List(Of System.Windows.Forms.KeyEventArgs)    'キー入力管理
    Private mKeyState(255) As Integer


    'game specific
    Private mShip As Ship
    Private mAsteroids As New List(Of Asteroid)
    Private mClearPict As ClearPict

    'コンストラクタ
    Public Sub Game_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        mWindow = Nothing
        mRenderer = Nothing
        mIsRunning = True
        mUpdatingActors = False
        mWindowWidth = 1024
        mWindowHeight = 768

        Dim success = Initialize()
        If success = True Then
            Ticks.Start()         'ストップウォッチ開始
            RunLoop.Interval = 5
            RunLoop.Enabled = True      'タイマー開始
        Else
            Shutdown()
        End If
    End Sub

    Public Function Initialize() As Boolean
        'ウィンドウ初期化
        Me.SetDesktopBounds(100, 100, mWindowWidth + 26, mWindowHeight + 49)
        Me.DoubleBuffered = True

        PictureBox.SetBounds(5, 5, mWindowWidth, mWindowHeight)
        'レンダラー作成
        mWindow = New Bitmap(mWindowWidth, mWindowHeight)
        mRenderer = Graphics.FromImage(mWindow)

        LoadData()

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
        GetKeyboardState(mKeyState)


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
        Dim deltaTime As Single = (Ticks.ElapsedMilliseconds - mTicksCount) / 1000

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
        '画面のクリア
        mRenderer.Clear(Color.Gray)

        'すべてのスプライトコンポーネントを描画
        For Each sprite In mSprites
            sprite.Draw(mRenderer)
        Next

        PictureBox.Image = mWindow
    End Sub
    Private Sub Shutdown()
        UnloadData()
        mRenderer = Nothing
        mWindow = Nothing
        Ticks.Stop()
        Me.Close()
    End Sub
    Private Sub LoadData()
        'プレイヤーの宇宙船を作成
        mShip = New Ship(Me)

        '小惑星を複数生成
        Dim initialNumAsteroids = 15        '初期値
        For i As Integer = 0 To initialNumAsteroids - 1
            AddAsteroid()
        Next

        '背景を作成
        Dim bg As New BackGround(Me)

        mClearPict = New ClearPict(Me)

    End Sub
    Private Sub UnloadData()
        While mActors.Count > 0
            mActors.Remove(mActors.Last)
        End While

        mTextures.Clear()
    End Sub
    Public Function GetTexture(ByRef filename As String) As Image
        Dim tex As System.Drawing.Image = Nothing
        Dim b As Boolean = mTextures.ContainsKey(filename)
        If b = True Then
            'すでに読み込み済み
            tex = mTextures(filename)
        Else
            '画像ファイルを読み込んで、Imageオブジェクトを作成し、ファイル名と紐づけする
            tex = Image.FromFile(Application.StartupPath & filename)
            mTextures.Add(filename, tex)
        End If
        Return tex
    End Function
    Public Sub AddActor(ByRef actor As Actor)
        If mUpdatingActors Then
            mPendingActors.Add(actor)
        Else
            mActors.Add(actor)
        End If
    End Sub

    Public Sub RemoveActor(actor As Actor)
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

    Public Sub AddSprite(sprite As SpriteComponent)
        Dim myDrawOrder As Integer = sprite.GetDrawOrder()
        Dim cnt As Integer = mSprites.Count     '配列の要素数
        Dim iter As Integer
        If cnt > 0 Then
            For iter = 0 To mSprites.Count - 1
                If myDrawOrder < mSprites(iter).GetDrawOrder() Then
                    Exit For
                End If
            Next
        End If
        mSprites.Insert(iter, sprite)
    End Sub
    Public Sub RemoveSprite(sprite As SpriteComponent)
        Dim iter As Integer = mSprites.IndexOf(sprite)
        '見つからなかったら-1が返される。
        iter = mSprites.IndexOf(sprite)
        If iter >= 0 Then
            mSprites.RemoveAt(iter)
        End If
    End Sub
    Public Sub SetRunning(isrunning As Boolean)
        mIsRunning = isrunning
    End Sub


    'Game Specific
    Public Function GetShip() As Ship
        Return mShip
    End Function
    Public Function GetAsteroids() As List(Of Asteroid)
        Return mAsteroids
    End Function
    Public Function GetClearPict() As ClearPict
        Return mClearPict
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


