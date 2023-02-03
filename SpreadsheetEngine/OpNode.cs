#pragma warning disable SA1401 // Fields are intended to be public

namespace SpreadsheetEngine
{
    public abstract class OpNode : ExpressionNode
    {
        public ExpressionNode? Left;
        public ExpressionNode? Right;

        public abstract double Evaluate(double lhs, double rhs);
    }
}
