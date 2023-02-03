using SpreadsheetEngine;

public class OperatorNodeFactory
{
    /// <summary>
    /// Creates the specific type of operator node needed for the expression based on the inputted operator character.
    /// </summary>
    /// <param name="left"> Left Node </param>
    /// <param name="right"> Right Node </param>
    /// <param name="op"> character used as operator </param>
    /// <returns> the created OpNode </returns>
    public static OpNode CreateOperatorNode(ExpressionNode left, ExpressionNode right, char op)
    {
        // add
        if (op == '+')
        {
            return new AddOpNode(left, right);
        }

        // subtract
        else if (op == '-')
        {
            return new SubtractOpNode(left, right);
        }

        // multiply
        else if (op == '*')
        {
            return new MultiplyOpNode(left, right);
        }

        // divide
        else
        {
            return new DivideOpNode(left, right);
        }
    }
}
