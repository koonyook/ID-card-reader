Public Class Form1

    Private Sub Button1_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Button1.Click

        Dim image As New Bitmap("a.jpg")
        Dim ocr As New tessnet2.Tesseract()
        ocr.SetVariable("tessedit_char_whitelist", "0123456789")
        ocr.Init(Nothing, "eng", True)
        Dim result As List(Of tessnet2.Word)
        result = ocr.doOCR(image, Rectangle.Empty)
        For Each word As tessnet2.Word In result
            TextBox1.AppendText(word.Text + " ")
        Next

    End Sub
End Class
