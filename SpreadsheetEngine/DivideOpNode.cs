namespace SpreadsheetEngine
{
    public class DivideOpNode : OpNode
    {
        public DivideOpNode(ExpressionNode left, ExpressionNode right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Divides left by right
        /// </summary>
        /// <param name="lhs"> value of left node </param>
        /// <param name="rhs"> value of right node </param>
        /// <returns> quotient as double </returns>
        public override double Evaluate(double lhs, double rhs)
        {
            return lhs / rhs;
        }
    }
}
