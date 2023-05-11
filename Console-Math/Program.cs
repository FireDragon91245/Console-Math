using NCalc;

namespace Console_Math
{
    internal class Program
    {
        static void Main (string[] args)
        {
            Console.WriteLine(typeof(List<int>).GetGenericTypeDefinition());
        }
    }
}