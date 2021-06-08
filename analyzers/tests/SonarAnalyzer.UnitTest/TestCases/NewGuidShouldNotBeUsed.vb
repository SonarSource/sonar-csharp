Imports System

Public Class NewGuid

    Public Sub DefaultInitalization()
        Dim asDefault As Guid = Nothing ' Noncompliant
    End Sub

    Public Sub WithoutArguments()

        Dim id As Guid = New Guid() ' Noncompliant {{Use 'Guid.NewGuid()' or 'Guid.Empty' or add arguments to this Guid instantiation.}}
        '                ^^^^^^^^^^
        Dim noBrackets As Guid = New Guid ' Noncompliant

    End Sub

    Public Sub WithArguments()
        Dim asGuid As Guid = New Guid(New Byte() {}) ' Compliant
        Dim asNewGuid As New Guid(New Byte() {}) ' Compliant
    End Sub

    Public Sub Other()
        Dim empty As Guid = Guid.Empty ' Compliant
        Dim rnd As Guid = Guid.NewGuid() ' Compliant
    End Sub

End Class
