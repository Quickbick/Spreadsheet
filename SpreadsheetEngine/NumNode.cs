namespace SpreadsheetEngine
{
    public class NumNode : ExpressionNode
    {
        private double dataValue;

        public NumNode(double dataValue)
        {
            this.dataValue = dataValue;
        }

        public double DataValue
        {
            get { return this.dataValue; }
        }
    }
}