namespace CommandPattern2;

public class DocumentInvoker
{
    private Stack<ICommand> UndoCommands = new();
    private Stack<ICommand> RedoCommands = new();

    private Document document = new Document();

    public void Undo()
    {
        if (!UndoCommands.IsEmpty())
        {
            ICommand command = UndoCommands.Pop();
            command.Undo();
            RedoCommands.Push(command);
        }
        else
        {
            Console.WriteLine("Nothing to undo");
        }
    }

    public void Redo()
    {
        if (!RedoCommands.IsEmpty())
        {
            ICommand command = RedoCommands.Pop();
            command.Redo();
            UndoCommands.Push(command);
        }
        else
        {
            Console.WriteLine("Nothing to redo");
        }
    }

    public void Write(string text)
    {
        ICommand command = new DocumentEditorCommand(document, text);
        UndoCommands.Push(command);
        RedoCommands.Clear();
    }

    public void Read()
    {
        document.ReadDocument();
    }
}