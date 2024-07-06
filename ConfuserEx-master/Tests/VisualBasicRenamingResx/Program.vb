Public Class Program
	Friend Shared Function Main(args As String()) As Integer
		Console.WriteLine("START")
		Console.WriteLine(My.Resources.TestString)
		Console.WriteLine("END")
		Return 42
	End Function
End Class
