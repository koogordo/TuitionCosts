using System;
using TuitionCosts;
namespace TuitionDataTest
{
    class Program
    {
        static void Main(string[] args)
        {
            TuitionData data = new TuitionData(@"C:\Users\kgordon\source\repos\TuitionCosts\TuitionCosts\college_costs.csv");
            var result = data.getTuitionInState("arizona state university", true);
            if (result.success)
            {
                Console.WriteLine(result.totalCost);
            }
            else
            {
                Console.WriteLine(result.errorMessage);
            }
            var x = Console.Read();
        }
    }
}
