namespace CommandPattern;

public class LightOnCommand : ICommand
{
    Light light;

    public LightOnCommand(Light light)
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

public class LightOffCommand : ICommand
{
    Light light;

    public LightOffCommand(Light light)
    {
        this.light = light;
    }
    public void Execute()
    {
        light.Off();
    }

    public void Undo()
    {
        light.On();
    }
}

public class Light
{
    private string _name;
    public Light(string name)
    {
        _name = name;
    }

    public void On()
    {
        Console.WriteLine($"Light {_name} in on");
    }

    public void Off()
    {
        Console.WriteLine($"Light {_name} in off");
    }
}

