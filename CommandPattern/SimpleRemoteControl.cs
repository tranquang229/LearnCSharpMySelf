namespace CommandPattern;

public class SimpleRemoteControl
{
    private ICommand slot;

    public void SetCommand(ICommand command)
    {
        this.slot = command;
    }

    public void ButtonWasPressed()
    {
        slot.Execute();
    }
}