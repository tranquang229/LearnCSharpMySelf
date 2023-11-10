namespace CommandPattern;

public class MarcoCommand : ICommand
{
    ICommand[] _commands;

    public MarcoCommand(ICommand[] commands)
    {
        _commands = commands;
    }


    public void Execute()
    {
        foreach (var t in _commands)
        {
            t.Execute();
        }
    }

    public void Undo()
    {
        foreach (var t in _commands)
        {
            t.Undo();
        }
    }
}