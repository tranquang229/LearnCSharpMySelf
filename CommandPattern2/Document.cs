namespace CommandPattern2;

public class Document
{
    private Stack<string> lines = new();

    public void Write(string text)
    {
        lines.Push(text);
    }

    public void EraseLast()
    {
        if(!lines.IsEmpty())
        {
            lines.Pop();
        }
    }

    public void ReadDocument()
    {
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }
}