namespace CommandPattern;

public class TVOnCommand : ICommand
{
    TV light;

    public TVOnCommand(TV light)
    {
        this.light = light;
    }
    public void Execute()
    {
        light.On();
    }

    public void Undo()
    {
        light.Off();
    }
}

public class TVOffCommand : ICommand
{
    TV _tv;

    public TVOffCommand(TV tv)
    {
        this._tv = tv;
    }
    public void Execute()
    {
        _tv.Off();
    }

    public void Undo()
    {
        _tv.On();
    }
}

public class TV
{
    private string _name;
    public TV(string name)
    {
        _name = name;
    }

    public void On()
    {
        Console.WriteLine($"TV {_name} in on");
    }

    public void Off()
    {
        Console.WriteLine($"TV {_name} in off");
    }
}

