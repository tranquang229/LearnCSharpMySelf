using Newtonsoft.Json;

namespace ConsoleAppDemo
{
    public static class ExtensionDemo<T>
    {
        public static string MakeJsonSchema(T input)
        {
            //do something to make Json;
            string output = JsonConvert.SerializeObject(input);
            return output;
        }

        public static bool Compare2Object(string input1, string input2)
        {
            return true;
        }
    }
}