// Written by Nathanael Ostheller
// 011717168

#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8604 // Possible null reference argument.

using SpreadsheetEngine;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ExpressionTree mainTree = new ExpressionTree("A1+B1+C1");
        string expression = "A1+B1+C1";
        int MenuChoice = 0;
        while (MenuChoice != 4)
        {
            Console.WriteLine("Current Expression: {0}", expression);
            MenuChoice = RunMenu();
            if (MenuChoice == 1)
            {
                Console.WriteLine("Enter A New Expression To Evaluate:");
                string newExpression = Console.ReadLine();
                expression = newExpression;
                mainTree = new ExpressionTree(newExpression);
                Console.WriteLine("Current Expression: {0}", expression);
            }
            else if (MenuChoice == 2)
            {
                Console.WriteLine("Enter The Variable Name:");
                string editVarible = Console.ReadLine();
                Console.WriteLine("Enter The New Value:");
                double newValue = double.Parse(Console.ReadLine());
                mainTree.SetVariable(editVarible, newValue);
                Console.WriteLine("{0} has been set to {1}", editVarible, newValue);
            }
            else if (MenuChoice == 3)
            {
                double result = mainTree.Evaluate();
                Console.WriteLine("This expression equals {0}", result);
            }
        }
    }

    /// <summary>
    /// Runs Menu For Text Expression Evaluator
    /// </summary>
    /// <returns> Choice Selected By User </returns>
    private static int RunMenu()
    {
        Console.WriteLine("1 - Enter a New Expression");
        Console.WriteLine("2 - Set A Variable Value");
        Console.WriteLine("3 - Evaluate Tree");
        Console.WriteLine("4 - Quit");
        Console.WriteLine("Please Select A Menu Option:");
        return int.Parse(Console.ReadLine());
    }
}