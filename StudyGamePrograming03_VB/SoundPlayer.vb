Public Class SoundPlayer
    Implements IDisposable      '明示的にクラスを開放するために必要

#If Win64 Then
    Private Declare PtrSafe Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String,     ByVal uReturnLength As Long, ByVal hwndCallback As Long) As Long
#Else
    Private Declare Function mciSendString Lib "winmm" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Long, ByVal hwndCallback As Long) As Long
#End If


    Private mAliasNames As New List(Of String)
    Private mControls As New List(Of String)     '"play","replay","resume","stop","pause","delete"
    Private mSndCmpnts As New List(Of SoundComponent)    'サウンドコンポーネントの配列

    Private disposedValue As Boolean
    Sub New(ByRef game As Game)
    End Sub
    Protected disposed = False     '開放処理が実施済みかのフラグ
    Public Overloads Sub Dispose() Implements IDisposable.Dispose
        Dispose(True)
    End Sub
    Protected Overridable Overloads Sub Dispose(ByVal disposing As Boolean)
        If Not Me.disposed Then
            If disposing Then
                '*** アンマネージリソースの開放
            End If
            '*** マネージドリソースの開放
        End If
        disposed = True
    End Sub
    Protected Overrides Sub Finalize()
        MyBase.Finalize()
        Dispose(False)
    End Sub
    Public Sub Play()
        'Controlに応じてAliasNameにMCIコントロールを行う。
        Dim i As Integer
        If mControls.Count > 0 Then
            For i = 0 To mControls.Count - 1
                If Strings.Left(mControls(i), 4) = "play" Then
                    ControlPlay(i)
                ElseIf Strings.Left(mControls(i), 6) = "replay" Then
                    ControlReplay(i)
                ElseIf Strings.Left(mControls(i), 4) = "stop" Then
                    ControlStop(i)
                ElseIf Strings.Left(mControls(i), 5) = "pause" Then
                    ControlPause(i)
                End If
            Next
            'controlがdeleteの処理
            Dim flag As Boolean = False
            i = 0
            Do While flag = False
                If Strings.Left(mControls(i), 6) = "delete" Then
                    Dim status As String = ControlGetStatus(i)
                    If Strings.Left(status, 7) = "playing" Then
                        '何もしない
                    Else
                        ControlClose(mAliasNames(i))
                        mAliasNames.RemoveAt(i)
                        mControls.RemoveAt(i)
                        i -= 1
                    End If
                End If
                i += 1
                If i >= mControls.Count - 1 Then
                    flag = True
                End If
            Loop
        End If
    End Sub

    Public Sub AddSndCmpnt(ByRef sndC As SoundComponent)
        mSndCmpnts.Add(sndC)
    End Sub
    Public Sub RemoveSndCmpnt(ByRef sndC As SoundComponent)
        Dim iter As Integer = mSndCmpnts.IndexOf(sndC)
        '見つからなかったら-1が返される。
        If iter >= 0 Then
            mSndCmpnts.RemoveAt(iter)
        End If
    End Sub

    Public Function GetAliasName(ByVal fileName As String) As String
        '音声ファイルを開き、mControlsとmAliasNamesリストに登録し、AliasNameを返す
        Dim cmd As String
        Dim i As Integer = mAliasNames.Count
        'ファイルオープン
        cmd = "open " & fileName & " alias mAliasName" & i
        If mciSendString(cmd, Nothing, 0, 0) <> 0 Then
            Return Nothing
        Else
            mAliasNames.Add("mAliasName" & i)
            mControls.Add("")
            Return mAliasNames(i)
        End If

    End Function

    Public Sub SetSoundControl(ByVal aliasName As String, ByVal control As String)
        'aliasNameと同じmAliasnamesのIDについて、mControlsにcontrolを追加する。
        If mAliasNames.Count > 0 Then
            Dim i As Integer = mAliasNames.IndexOf(aliasName)
            If i >= 0 Then
                mControls(i) = mControls(i) & control
            End If
        End If
    End Sub
    Public Function ControlGetStatus(ByVal i As Integer) As String
        Dim cmd As String = "status " & mAliasNames(i) & " mode"
        Dim status As String = "          "
        mciSendString(cmd, status, status.Length(), 0)      '状態取得
        Return status
    End Function


    Public Sub ControlPlay(ByVal i As Integer)
        'pauseなら再開、playingなら何もしない、それ以外は頭から再生
        Dim cmd As String
        Dim status As String = ControlGetStatus(i)
        If Strings.Left(status, 7) = "playing" Then
            '何もしない
        ElseIf Strings.Left(status, 5) = "pause" Then
            cmd = "resume " & mAliasNames(i)
            mciSendString(cmd, Nothing, 0, 0)
        Else
            cmd = "play " & mAliasNames(i) & " from 0"
            mciSendString(cmd, Nothing, 0, 0)
        End If
        'Controlの左からコマンドを削除
        Dim control As String = mControls(i).Remove(0, 4)
        mControls(i) = control
    End Sub
    Public Sub ControlReplay(ByVal i As Integer)
        'まず再生停止して、頭から再生
        Dim cmd As String = "stop " & mAliasNames(i)
        mciSendString(cmd, Nothing, 0, 0)
        cmd = "play " & mAliasNames(i) & " from 0"
        mciSendString(cmd, Nothing, 0, 0)
        'Controlの左からコマンドを削除
        Dim control As String = mControls(i).Remove(0, 6)
        mControls(i) = control
    End Sub
    Public Sub ControlPause(ByVal i As Integer)
        '一時停止
        Dim cmd As String = "pause " & mAliasNames(i)
        mciSendString(cmd, Nothing, 0, 0)
        'Controlの左からコマンドを削除
        Dim control As String = mControls(i).Remove(0, 5)
        mControls(i) = control
    End Sub
    Public Sub ControlStop(ByVal i As Integer)
        '停止
        Dim cmd As String = "stop " & mAliasNames(i)
        mciSendString(cmd, Nothing, 0, 0)
        'Controlの左からコマンドを削除
        Dim control As String = mControls(i).Remove(0, 4)
        mControls(i) = control
    End Sub
    Public Sub ControlResume(ByVal i As Integer)
        '再開
        Dim cmd As String = "resume " & mAliasNames(i)
        mciSendString(cmd, Nothing, 0, 0)
        'Controlの左からコマンドを削除
        Dim control As String = mControls(i).Remove(0, 6)
        mControls(i) = control
    End Sub
    Public Sub ControlClose(ByVal aliasname As String)
        'エイリアスを閉じる
        Dim cmd As String = "close " & aliasname
        mciSendString(cmd, Nothing, 0, 0)
    End Sub
    Public Sub UnloadData()
        Do While mAliasNames.Count > 0
            Dim cmd As String = "close " & mAliasNames(0)
            mAliasNames.RemoveAt(0)
        Loop
    End Sub
    Public Sub Shutdown()
        Me.Dispose()
    End Sub
End Class
