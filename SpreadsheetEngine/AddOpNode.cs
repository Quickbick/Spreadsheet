namespace SpreadsheetEngine
{
    public class AddOpNode : OpNode
    {
        public AddOpNode(ExpressionNode left, ExpressionNode right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Adds two values together
        /// </summary>
        /// <param name="lhs"> value of left node </param>
        /// <param name="rhs"> value of right node </param>
        /// <returns> sum as double </returns>
        public override double Evaluate(double lhs, double rhs)
        {
            return lhs + rhs;
        }
    }
}
