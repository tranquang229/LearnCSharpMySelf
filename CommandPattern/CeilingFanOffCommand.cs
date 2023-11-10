namespace CommandPattern;

internal class CeilingFanOffCommand : ICommand
{
    private CeilingFan _ceilingFan;
    private int previousSpeed;

    public CeilingFanOffCommand(CeilingFan ceilingFan)
    {
        _ceilingFan = ceilingFan;
    }

    public void Execute()
    {
        previousSpeed = _ceilingFan.GetSpeed();
        _ceilingFan.Off();
    }

    public void Undo()
    {
        if (previousSpeed == CeilingFan.HIGH)
        {
            _ceilingFan.High();
        }
        else if (previousSpeed == CeilingFan.MEDIUM)
        {
            _ceilingFan.Medium();
        }
        else if (previousSpeed == CeilingFan.LOW)
        {
            _ceilingFan.Low();
        }
        else
        {
            _ceilingFan.Off();
        }
    }
}