using System.Text;

namespace CommandPattern;

public class RemoteControl
{
    private ICommand[] OnCommands { get; set; }
    private ICommand[] OffCommands { get; set; }

    public RemoteControl()
    {
        OnCommands = new ICommand[7];
        OffCommands = new ICommand[7];

        ICommand noCommand = new NoCommand();
        for (int i = 0; i < 7; i++)
        {
            OnCommands[i] = noCommand;
            OffCommands[i] = noCommand;
        }
    }

    public void SetCommand(int slot, ICommand onCommand, ICommand offCommand)
    {
        OnCommands[slot] = onCommand;
        OffCommands[slot] = offCommand;
    }

    public void OnButtonWasPushed(int slot)
    {
        if (OnCommands[slot] != null)
        {
            OnCommands[slot].Execute();
        }

    }

    public void OffButtonWasPushed(int slot)
    {
        OffCommands[slot].Execute();
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        sb.Append($"\n----------Remote Control----------\n");
        for (int i = 0; i < OnCommands.Length; i++)
        {
            sb.Append($"Slot {i}, {OnCommands[i].ToString()}     {OffCommands[i].ToString()}\n");
        }

        return sb.ToString();
    }
}