Public Class SomeSpriteComponent
	Inherits SpriteComponent

	' すべてのテクスチャ
	Private mSomeTextures As New List(Of Image)

	Sub New(ByRef owner As Actor, ByVal drawOrder As Integer)
		MyBase.New(owner, drawOrder)

	End Sub

	Public Sub SetSomeTextures(ByRef textures As List(Of Image))
		mSomeTextures = textures
	End Sub

	Public Sub SetTextureFromId(ByVal id As Integer)
		SetTexture(mSomeTextures(id))
	End Sub
End Class