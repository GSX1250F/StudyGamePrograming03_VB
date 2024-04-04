Public Class MoveComponent
    Inherits Component
    Sub New(ByRef owner As Actor, ByVal updateOrder As Integer)
        MyBase.New(owner, updateOrder)
        mAngularSpeed = 0.0
        mForwardSpeed = 0.0
    End Sub

    Public Overrides Sub Update(deltaTime As Single)
        MyBase.Update(deltaTime)
        Dim rot As Single = mOwner.mRotation
        rot += mAngularSpeed * deltaTime
        mOwner.mRotation = rot

        mOwner.mPosition += mOwner.GetForward() * mForwardSpeed * deltaTime

    End Sub

    Public mAngularSpeed As Single
    Public mForwardSpeed As Single

End Class
