Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Windows.Win32

Public Class SoundPlayer
    Implements IDisposable      '明示的にクラスを開放するために必要
    Private disposedValue As Boolean
#If Win64 Then
    Private Declare PtrSafe Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String,     ByVal uReturnLength As Long, ByVal hwndCallback As Long) As Long
#Else
    Private Declare Function mciSendString Lib "winmm" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Long, ByVal hwndCallback As Long) As Long
#End If

    Structure SoundControl
        Dim AliasName As String
        Dim Control As String
    End Structure
    Private mSoundControls As New List(Of SoundControl)     'AliasControlを集めた配列。


    Private mGame As Game
    Sub New(ByRef game As Game)
        mGame = game
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
    Public Function Initialize() As Boolean
        Return True
    End Function
    Public Sub Shutdown()
        Me.Dispose()
    End Sub
    Public Sub UnloadData()
    End Sub

    Public Function AddAliasControl(ByVal filename As String) As String
        'ファイル名の音声ファイルを空いているAliasNameで開き、mAliasControlsに加える。そのAliasNameを返す。
        Dim id As Integer = mSoundControls.Count
        Dim scl As New SoundControl
        scl.AliasName = "AliasName" & id
        scl.Control = ""
        'ファイルオープン
        Dim cmd As String = "open """ & filename & """ alias " & scl.AliasName
        If mciSendString(cmd, Nothing, 0, 0) <> 0 Then
            Return Nothing
        Else
            mSoundControls.Add(scl)
            Return scl.AliasName
        End If
    End Function
    Public Sub SetControl(ByVal aliasname As String, ByVal control As String)
        'aliasnameをmAliasControlsから検索し、そのControlにcontrolを追記する。
        If mSoundControls.Count > 0 Then
            For i As Integer = 0 To mSoundControls.Count - 1
                If mSoundControls(i).AliasName = aliasname Then
                    Dim scl As New SoundControl
                    scl.AliasName = aliasname
                    scl.Control = mSoundControls(i).Control & control
                    mSoundControls(i) = scl
                    Exit For
                End If
            Next
        End If
    End Sub

    Public Sub Play()
        Dim i As Integer
        Dim scl As New SoundControl
        If mSoundControls.Count > 0 Then
            For i = 0 To mSoundControls.Count - 1
                If Strings.Left(mSoundControls(i).Control, 4) = "play" Then
                    ControlPlay(mSoundControls(i))
                ElseIf Strings.Left(mSoundControls(i).Control, 6) = "replay" Then
                    ControlReplay(mSoundControls(i))
                ElseIf Strings.Left(mSoundControls(i).Control, 4) = "stop" Then
                    ControlStop(mSoundControls(i))
                ElseIf Strings.Left(mSoundControls(i).Control, 5) = "pause" Then
                    ControlPause(mSoundControls(i))
                End If
            Next
            Dim flag As Boolean = False
            i = 0
            Do While flag = False
                If Strings.Left(mSoundControls(i).Control, 6) = "delete" Then
                    Dim status As String = ControlGetStatus(mSoundControls(i))
                    If Strings.Left(status, 7) = "playing" Then
                        '何もしない
                    Else
                        ControlClose(mSoundControls(i).AliasName)
                        mSoundControls.RemoveAt(i)
                        i -= 1
                    End If
                End If
                i += 1
                If i >= mSoundControls.Count - 1 Then
                    flag = True
                End If
            Loop
        End If
    End Sub
    Public Function ControlGetStatus(ByRef scl As SoundControl) As String
        Dim cmd As String = "status " & scl.AliasName & " mode"
        Dim status As String = "          "
        mciSendString(cmd, status, status.Length(), 0)      '状態取得
        Return status
    End Function
    Public Sub ControlPlay(ByRef scl As SoundControl)
        Dim cmd As String
        Dim status As String = ControlGetStatus(scl)
        If Strings.Left(status, 7) = "playing" Then
            '何もしない
        ElseIf Strings.Left(status, 5) = "pause" Then
            cmd = "resume " & scl.AliasName
            mciSendString(cmd, Nothing, 0, 0)
        Else
            cmd = "play " & scl.AliasName & " from 0"
            mciSendString(cmd, Nothing, 0, 0)
        End If
        Dim control As String = scl.Control.Remove(0, 4)
        scl.Control = control
    End Sub
    Public Sub ControlReplay(ByRef scl As SoundControl)
        Dim cmd As String = "stop " + scl.AliasName
        mciSendString(cmd, Nothing, 0, 0)
        cmd = "play " & scl.AliasName & " from 0"
        mciSendString(cmd, Nothing, 0, 0)
        Dim control As String = scl.Control.Remove(0, 6)
        scl.Control = control
    End Sub
    Public Sub ControlPause(ByRef scl As SoundControl)
        Dim cmd As String = "pause " & scl.AliasName
        mciSendString(cmd, Nothing, 0, 0)
        Dim control As String = scl.Control.Remove(0, 5)
        scl.Control = control
    End Sub
    Public Sub ControlStop(ByRef scl As SoundControl)
        Dim cmd As String = "stop " & scl.AliasName
        mciSendString(cmd, Nothing, 0, 0)
        Dim control As String = scl.Control.Remove(0, 4)
        scl.Control = control
    End Sub
    Public Sub ControlResume(ByRef scl As SoundControl)
        Dim cmd As String = "resume " & scl.AliasName
        mciSendString(cmd, Nothing, 0, 0)
        Dim control As String = scl.Control.Remove(0, 6)
        scl.Control = control
    End Sub
    Public Sub ControlClose(ByVal aliasname As String)
        Dim cmd As String = "close " & aliasname
        mciSendString(cmd, Nothing, 0, 0)
    End Sub


End Class
