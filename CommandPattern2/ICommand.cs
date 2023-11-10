namespace CommandPattern2;

public interface ICommand
{
    public void Undo();

    public void Redo();
}