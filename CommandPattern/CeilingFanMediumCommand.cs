namespace CommandPattern;

public class CeilingFanMediumCommand : ICommand
{
    private CeilingFan _ceilingFan;
    private int previousSpeed;

    public CeilingFanMediumCommand(CeilingFan ceilingFan)
    {
        _ceilingFan = ceilingFan;
    }

    public void Execute()
    {
        previousSpeed = _ceilingFan.GetSpeed();
        _ceilingFan.Medium();
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