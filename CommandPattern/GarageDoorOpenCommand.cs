namespace CommandPattern;

public class GarageDoorOpenCommand: ICommand
{

    private GarageDoor garageDoor;
    
    public GarageDoorOpenCommand(GarageDoor garageDoor)
    {
        this.garageDoor = garageDoor;
    }

    public void Execute()
    {
        garageDoor.Up();
    }

    public void Undo()
    {
        garageDoor.Down();
    }
}

public class GarageDoor
{
    public void Up()
    {
        Console.WriteLine("Garage door up");
    }

    public void Down()
    {
        Console.WriteLine("Garage door down");
    }

    public void Stop()
    {
        Console.WriteLine("Garage door stop");
    }

    public void LightOn()
    {
        Console.WriteLine("Garage door light on");
    }

    public void LightOff()
    {
        Console.WriteLine("Garage door light off");
    }
}