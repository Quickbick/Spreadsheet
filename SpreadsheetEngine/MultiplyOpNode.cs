namespace SpreadsheetEngine
{
    public class MultiplyOpNode : OpNode
    {
        public MultiplyOpNode(ExpressionNode left, ExpressionNode right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Multiplies two values together
        /// </summary>
        /// <param name="lhs"> value of left node </param>
        /// <param name="rhs"> value of right node </param>
        /// <returns> product as double </returns>
        public override double Evaluate(double lhs, double rhs)
        {
            return lhs * rhs;
        }
    }
}
