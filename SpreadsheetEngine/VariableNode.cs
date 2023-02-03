namespace SpreadsheetEngine
{
    internal class VariableNode : ExpressionNode
    {
        private string varName;

        public VariableNode(string name)
        {
            this.varName = name;
        }

        public string? VarName
        {
            get => this.varName;
        }
    }
}
