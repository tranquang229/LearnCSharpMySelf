using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommandPattern;

public class CeilingFanHighCommand : ICommand
{
    private CeilingFan _ceilingFan;
    private int previousSpeed;
    
    public CeilingFanHighCommand(CeilingFan ceilingFan)
    {
        _ceilingFan = ceilingFan;
    }
   
    public void Execute()
    {
        previousSpeed = _ceilingFan.GetSpeed();
        _ceilingFan.High();
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