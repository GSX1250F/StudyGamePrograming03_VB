Public Class ClearPict
    Inherits Actor
    Sub New(ByRef game As Game)
        MyBase.New(game)
        mState = State.EPaused
        'スプライトコンポーネント作成、テクスチャ設定
        Dim sc As SpriteComponent = New SpriteComponent(Me, 100)
        sc.SetTexture(game.GetTexture("\Assets\ClearPict.png"))
    End Sub
End Class
