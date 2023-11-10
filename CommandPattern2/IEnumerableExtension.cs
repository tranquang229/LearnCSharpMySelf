namespace CommandPattern2;

public static class IEnumerableExtension
{
    public static bool IsEmpty<T>(this IEnumerable<T>? arr)
    {
        return arr == null || !arr.Any();
    }
}