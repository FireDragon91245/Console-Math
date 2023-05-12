using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neo.IronLua;

namespace Console_Math
{
    [Serializable]
    internal class FormulasData
    {

        public Dictionary<string, Dictionary<string, string>> UnitConversions = new();

        public List<Formula> Formulas = new();


        public void AddUnitConversion(string unitA, string unitB, string formula, bool upUnit)
        {
            if (!UnitConversions.ContainsKey(unitA))
            {
                UnitConversions.Add(unitA, new Dictionary<string, string>());
            }
            UnitConversions[unitA].Add(unitB, formula);
            Console.WriteLine($"Added {unitA} -> {unitB}");
        }
    }

    [Serializable]
    internal class Formula
    {
        public (string symbol, string unit) Result
        {
            get;
            init;
        }

        public (string symbol, string unit)[] Inputs
        {
            get;
            init;
        }

        public string FormulaString
        {
            get;
            init;
        }
    }

    
}
