namespace SpreadsheetEngine
{
    public class SubtractOpNode : OpNode
    {
        public SubtractOpNode(ExpressionNode left, ExpressionNode right)
        {
            this.Left = left;
            this.Right = right;
        }

        /// <summary>
        /// Subtracts right from left
        /// </summary>
        /// <param name="lhs"> value of left node </param>
        /// <param name="rhs"> value of right node </param>
        /// <returns> difference as double </returns>
        public override double Evaluate(double lhs, double rhs)
        {
            return lhs - rhs;
        }
    }
}
