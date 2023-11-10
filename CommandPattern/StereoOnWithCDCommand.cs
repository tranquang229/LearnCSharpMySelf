namespace CommandPattern;

public class StereoOnWithCDCommand : ICommand
{
    private Stereo stereo;

    public StereoOnWithCDCommand(Stereo stereo)
    {
        this.stereo = stereo;
    }

    public void Execute()
    {
        stereo.On();
        stereo.Off();
        stereo.SetVolume(11);
    }

    public void Undo()
    {

    }
}

public class Stereo
{
    private string _location;

    public Stereo(string location)
    {
        _location = location;
    }

    public void On()
    {
        Console.WriteLine($"Stereo {_location} On");
    }

    public void Off()
    {
        Console.WriteLine($"Stereo {_location} Off");
    }

    public void SetCD()
    {
        Console.WriteLine($"Stereo {_location} SetCD");
    }

    public void SetDVD()
    {
        Console.WriteLine($"Stereo {_location} SetDVD");
    }

    public void SetRadio()
    {
        Console.WriteLine($"Stereo {_location} SetRadio");
    }

    public void SetVolume(int volume)
    {
        Console.WriteLine($"Stereo {_location} SetVolume");
    }
}