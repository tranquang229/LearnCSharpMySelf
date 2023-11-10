namespace CommandPattern;

public class CeilingFan
{
    public static int HIGH = 3;
    public static int MEDIUM = 2;
    public static int LOW = 1;
    public static int OFF = 0;

    private string _location;
    private int _speed;

    public CeilingFan(string location)
    {
        _location = location;
        _speed = OFF;
    }

    public void High()
    {
        this._speed = HIGH;
        Console.WriteLine($"Ceiling fan {_location} High");
    }

    public void Low()
    {
        this._speed = LOW;
        Console.WriteLine($"Ceiling fan {_location} Low");
    }

    public void Medium()
    {
        this._speed = MEDIUM;
        Console.WriteLine($"Ceiling fan {_location} Medium");
    }

    public void Off()
    {
        this._speed = OFF;
        Console.WriteLine($"Ceiling fan {_location} Off");
    }

    public int GetSpeed()
    {
        return _speed;
    }
}