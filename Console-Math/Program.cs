using System.Text;
using NCalc;

namespace Console_Math
{
    internal class Program
    {
        static void Main (string[] args)
        {
            StringBuilder sb = new StringBuilder();
            foreach (var arg in args)
            {
                sb.Append(arg);
                sb.Append(' ');
            }
            var expr = new Expression(sb.ToString());
            expr.EvaluateFunction += new EvalFunctions().Call;
            Console.WriteLine(expr.Evaluate());
        }
    }
}