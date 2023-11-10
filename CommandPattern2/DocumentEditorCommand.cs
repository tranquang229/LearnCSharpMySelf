namespace CommandPattern2;

public class DocumentEditorCommand : ICommand
{
    private Document document;
    private string text;

    public DocumentEditorCommand(Document document, string text)
    {
        this.document = document;
        this.text = text;
        this.document.Write(text);
    }
    public void Undo()
    {
        document.EraseLast();
    }

    public void Redo()
    {
        document.Write(text);
    }
}