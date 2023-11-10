namespace CommandPattern;

public class CeilingFanLowCommand : ICommand
{
    private CeilingFan _ceilingFan;
    private int previousSpeed;

    public CeilingFanLowCommand(CeilingFan ceilingFan)
    {
        _ceilingFan = ceilingFan;
    }

    public void Execute()
    {
        previousSpeed = _ceilingFan.GetSpeed();
        _ceilingFan.Low();
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