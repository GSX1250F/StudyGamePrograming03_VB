Imports System.Windows.Forms.Design
Imports System.Windows.Forms.VisualStyles.VisualStyleElement
Imports Windows.Win32

Public Class SoundPlayer
    Implements IDisposable      '明示的にクラスを開放するために必要

#If Win64 Then
    Private Declare PtrSafe Function mciSendString Lib "winmm.dll" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String,     ByVal uReturnLength As Long, ByVal hwndCallback As Long) As Long
#Else
    Private Declare Function mciSendString Lib "winmm" Alias "mciSendStringA" (ByVal lpstrCommand As String, ByVal lpstrReturnString As String, ByVal uReturnLength As Long, ByVal hwndCallback As Long) As Long
#End If

    Structure SoundControl
        Dim AliasName As String
        Dim Control As String
    End Structure

    Private mSounds As New List(Of String)    'エイリアスの配列。
    Private mSoundControls As New List(Of SoundControl)     'SoundControlをまとめた配列。
    Private mDeleteSounds As New List(Of String)           '削除待ちエイリアスの配列
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
    Public Sub Shutdown()
        Me.Dispose()
    End Sub
    Public Sub UnloadData()
        For Each aliasname In mSounds
            Dim cmd As String = "close " & aliasname
        Next
    End Sub
    Public Function AddSound(ByRef filenames As List(Of String)) As List(Of String)
        If filenames.Count > 0 Then
            Dim aliasnames As New List(Of String)
            For i As Integer = 0 To filenames.Count - 1
                'ファイル名の音声ファイルを空いているAliasNameで開き、mSoundsに加え、AliasNameの配列を返す
                Dim cmd As String = "open """ & filenames(i) & """ alias " & "AliasName" & mSounds.Count
                If mciSendString(cmd, Nothing, 0, 0) = 0 Then
                    aliasnames.Add("AliasName" & mSounds.Count)
                    mSounds.Add("AliasName" & mSounds.Count)
                Else
                    'ファイルオープンに失敗したら、返す配列に空白を追加する。
                    aliasnames.Add(Nothing)
                End If
            Next
            Return aliasnames
        Else
            Return Nothing
        End If
    End Function
    Public Sub SetSoundControl(ByVal aliasname As String, ByVal control As String)
        'SoundControlsに追加する。
        Dim scl As SoundControl
        scl.AliasName = aliasname
        scl.Control = control
        mSoundControls.Add(scl)
    End Sub

    Public Sub Play()
        For Each scl In mSoundControls
            If scl.Control = "play" Then
                ControlPlay(scl.AliasName)
            ElseIf scl.Control = "replay" Then
                ControlReplay(scl.AliasName)
            ElseIf scl.Control = "stop" Then
                ControlStop(scl.AliasName)
            ElseIf scl.Control = "pause" Then
                ControlPause(scl.AliasName)
            ElseIf scl.Control = "delete" Then
                mDeleteSounds.Add(scl.AliasName)
            End If
        Next
        mSoundControls.Clear()

        '削除待ちのエイリアスが再生中でなければ、mSoundsから削除し、エイリアスを閉じる
        If mDeleteSounds.Count > 0 Then
            Dim i As Integer = 0
            Dim flag As Boolean = False
            Do While flag = False
                Dim status As String = ControlGetStatus(mDeleteSounds(i))
                If Strings.Left(status, 7) = "playing" Then
                    '何もしない
                Else
                    ControlClose(mDeleteSounds(i))
                    mSounds.Remove(mDeleteSounds(i))
                    mDeleteSounds.Remove(mDeleteSounds(i))
                    i -= 1
                End If
                i += 1
                If i >= mDeleteSounds.Count - 1 Then
                    flag = True
                End If
            Loop
        End If
    End Sub
    Public Function ControlGetStatus(ByRef aliasname As String) As String
        Dim cmd As String = "status " & aliasname & " mode"
        Dim status As String = "          "
        mciSendString(cmd, status, status.Length(), 0)      '状態取得
        Return status
    End Function
    Public Sub ControlPlay(ByRef aliasname As String)
        Dim cmd As String
        Dim status As String = ControlGetStatus(aliasname)
        If Strings.Left(status, 7) = "playing" Then
            '何もしない
        ElseIf Strings.Left(status, 5) = "pause" Then
            cmd = "resume " & aliasname
            mciSendString(cmd, Nothing, 0, 0)
        Else
            cmd = "play " & aliasname & " from 0"
            mciSendString(cmd, Nothing, 0, 0)
        End If
    End Sub
    Public Sub ControlReplay(ByRef aliasname As String)
        Dim cmd As String = "stop " & aliasname
        mciSendString(cmd, Nothing, 0, 0)
        cmd = "play " & aliasname & " from 0"
        mciSendString(cmd, Nothing, 0, 0)
    End Sub
    Public Sub ControlPause(ByRef aliasname As String)
        Dim cmd As String = "pause " & aliasname
        mciSendString(cmd, Nothing, 0, 0)
    End Sub
    Public Sub ControlStop(ByRef aliasname As String)
        Dim cmd As String = "stop " & aliasname
        mciSendString(cmd, Nothing, 0, 0)
    End Sub
    Public Sub ControlResume(ByRef aliasname As String)
        Dim cmd As String = "resume " & aliasname
        mciSendString(cmd, Nothing, 0, 0)
    End Sub
    Public Sub ControlClose(ByVal aliasname As String)
        Dim cmd As String = "close " & aliasname
        mciSendString(cmd, Nothing, 0, 0)
    End Sub
End Class
