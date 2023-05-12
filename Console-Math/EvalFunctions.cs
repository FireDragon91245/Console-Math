using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using NCalc;
using Neo.IronLua;

namespace Console_Math
{
    internal class EvalFunctions
    {
        [IgnoreMember]
        private Dictionary<string, List<MethodInfo>> methods = new();


        public EvalFunctions ()
        {
            var members = this.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            foreach (var member in members)
            {
                if (member.GetCustomAttribute<IgnoreMemberAttribute>() is null)
                {
                    methods.GetOrCreate(member.Name).Add(member);
                }
            }
            sharGlobal = SharedLua.CreateEnvironment();
        }

        [IgnoreMember]
        public void Call (string name, FunctionArgs args)
        {
            if (this.methods.TryGetValue(name, out var PosibleMethods))
            {
                var methodArgs = args.Parameters.Select(p => p.Evaluate()).ToArray();
                foreach (var method in PosibleMethods)
                {
                    if (method.IsCallableWith(methodArgs.Select(m => m.GetType()).ToArray()))
                    {
                        try
                        {
                            args.Result = method.Invoke(this, methodArgs);
                            return;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                        try
                        {
                            args.Result = method.Invoke(null, methodArgs);
                            return;
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e);
                        }
                    }
                }
            }
            args.Result = null;
        }

        public double Pow (double a, double b) => Math.Pow(a, b);

        public double Pow (double a) => Math.Pow(a, 2);

        public double Sqrt (double a) => Math.Sqrt(a);

        public double Sqrt (double a, double b) => Math.Pow(a, 1d / b);

        public double Log10 (double a) => Math.Log10(a);

        public double Log (double a) => Math.Log(a);

        public double Log (double a, double b) => Math.Log(a, b);

        public double Sin (double a) => Math.Sin(a);

        public double Cos (double a) => Math.Cos(a);

        public double Tan (double a) => Math.Tan(a);

        public double Asin (double a) => Math.Asin(a);

        public double Acos (double a) => Math.Acos(a);

        public double Atan (double a) => Math.Atan(a);

        public double Abs (double a) => Math.Abs(a);

        public double Exp (double a) => Math.Exp(a);

        public double Ceil (double a) => Math.Ceiling(a);

        public double Floor (double a) => Math.Floor(a);

        public double Round (double a) => Math.Round(a);

        public double Round (double a, int b) => Math.Round(a, b);

        public double Min (double a, double b) => Math.Min(a, b);

        public double Max (double a, double b) => Math.Max(a, b);

        public double Clamp (double a, double b, double c) => Math.Clamp(a, b, c);

        public double Sign (double a) => Math.Sign(a);

        public int Int (double a) => (int) a;

        [IgnoreMember]
        private Lua SharedLua = new();

        [IgnoreMember]
        private LuaCompileOptions shareCompileOptions = new();

        [IgnoreMember]
        private LuaGlobal sharGlobal;

        public int Func (string func, string name)
        {
            var chunk = SharedLua.CompileChunk(func, name, shareCompileOptions);

            //Create lambda with args types from chunk.Method <= MethodInfo
            var lambda = (chunk.Method.Args) =>
            {
                chunk.Run(sharGlobal, chunk.Method.Args);
            };

            var newMethodInfo = lambda.GetMethodInfoSomehow();

            /*
             * using System;
using System.Linq.Expressions;
using System.Reflection;

// Assuming you have a valid `chunk` object obtained from SharedLua.CompileChunk

// Get the parameter types of the method
Type[] parameterTypes = chunk.Method.GetParameters().Select(p => p.ParameterType).ToArray();

// Create the lambda expression with the appropriate parameters
ParameterExpression[] parameterExpressions = parameterTypes
    .Select((type, index) => Expression.Parameter(type, $"arg{index}"))
    .ToArray();

// Create the method call expression using the lambda parameters
Expression methodCallExpression = Expression.Call(
    Expression.Constant(chunk), // Pass the chunk as a constant
    "Run",
    null,
    Expression.Constant(sharGlobal), // Pass sharGlobal as a constant
    Expression.NewArrayInit(typeof(object), parameterExpressions) // Create an object array from the lambda parameters
);

// Create the lambda expression
LambdaExpression lambdaExpression = Expression.Lambda(methodCallExpression, parameterExpressions);

// Compile the lambda expression to obtain a delegate
Delegate compiledLambda = lambdaExpression.Compile();

// Get the MethodInfo of the compiled lambda
MethodInfo newMethodInfo = compiledLambda.Method;

             */
        }




    }
}
