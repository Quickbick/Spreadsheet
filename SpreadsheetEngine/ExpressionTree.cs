// Written by Nathanael Ostheller
// 011717168

#pragma warning disable CS8604 // assignment will not be null

namespace SpreadsheetEngine
{
    using System.Collections.Generic;
    using System.Text;

    public class ExpressionTree
    {
        private ExpressionNode? treeHead;
        private Dictionary<char, int> operatorDictionary = new Dictionary<char, int>();
        private Dictionary<string, double> variableDictionary = new Dictionary<string, double>();

        public ExpressionTree(string expression)
        {
            // initalizes dictionary
            this.operatorDictionary.Add('+', 0);
            this.operatorDictionary.Add('-', 1);
            this.operatorDictionary.Add('*', 2);
            this.operatorDictionary.Add('/', 3);

            // changes expression to postfix
            string postfixExpression = this.TurnPostfix(expression);

            // initalizes variables for use in construction
            int expressionPlaceholder = 0;
            int expressionPartEnd;
            Stack<ExpressionNode> nodeStack = new Stack<ExpressionNode>();

            // constructs from expression string
            while (expressionPlaceholder < postfixExpression.Length)
            {
                // if value is space from postfix
                if (postfixExpression[expressionPlaceholder] == ' ')
                {
                    expressionPlaceholder++;
                }

                // if value is operator
                else if (this.operatorDictionary.ContainsKey(postfixExpression[expressionPlaceholder]))
                {
                    ExpressionNode right = nodeStack.Pop();
                    ExpressionNode left = nodeStack.Pop();
                    ExpressionNode newNode = OperatorNodeFactory.CreateOperatorNode(left, right, postfixExpression[expressionPlaceholder]);
                    nodeStack.Push(newNode);
                    expressionPlaceholder++;
                }

                // if value is variable
                else if (char.IsLetter(postfixExpression[expressionPlaceholder]) == true)
                {
                    // finds length of the number
                    expressionPartEnd = expressionPlaceholder;
                    while (expressionPartEnd < postfixExpression.Length && postfixExpression[expressionPartEnd] != ' ')
                    {
                        expressionPartEnd++;
                    }

                    // creates new variable node
                    VariableNode newNode = new VariableNode(postfixExpression.Substring(expressionPlaceholder, expressionPartEnd - expressionPlaceholder));
                    this.variableDictionary.Add(newNode.VarName, 0);
                    nodeStack.Push(newNode);
                    expressionPlaceholder = expressionPartEnd;
                }

                // if value is number
                else if (char.IsDigit(postfixExpression[expressionPlaceholder]) == true)
                {
                    // finds length of the number
                    expressionPartEnd = expressionPlaceholder;
                    while (expressionPartEnd < postfixExpression.Length && postfixExpression[expressionPartEnd] != ' ')
                    {
                        expressionPartEnd++;
                    }

                    // creates new number node and saves to left hold and updates placeholder
                    NumNode newNode = new NumNode(double.Parse(postfixExpression.Substring(expressionPlaceholder, expressionPartEnd - expressionPlaceholder)));
                    nodeStack.Push(newNode);

                    expressionPlaceholder = expressionPartEnd;
                }
            }

            this.treeHead = nodeStack.Pop();
        }

        /// <summary>
        /// Checks if operator has precedence over old operator
        /// </summary>
        /// <param name="oldOp"> Operator to check against </param>
        /// <param name="newOp"> Operator to check </param>
        /// <returns> True if new operator has precedence, false if it does not </returns>
        public static bool CheckPrecedence(char oldOp, char newOp)
        {
            if (newOp == '+' || newOp == '-')
            {
                if (oldOp == '*' || oldOp == '/')
                {
                    return false;
                }
            }

            if (newOp == '-' && oldOp == '-')
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Sets specified variable (selected by name) to specified value.
        /// </summary>
        /// <param name="variableName"> Name of variable to change </param>
        /// <param name="variableValue"> New variable value </param>
        public void SetVariable(string variableName, double variableValue)
        {
            this.variableDictionary[variableName] = variableValue;
        }

        /// <summary>
        /// Evaluates existing expression into a double value.
        /// </summary>
        /// <returns> result of expression </returns>
        public double Evaluate()
        {
            if (this.treeHead == null)
            {
                return double.NaN;
            }
            else if (this.treeHead.GetType() == typeof(NumNode))
            {
                return ((NumNode)this.treeHead).DataValue;
            }
            else if (this.treeHead.GetType() == typeof(VariableNode))
            {
                return this.variableDictionary[((VariableNode)this.treeHead).VarName];
            }
            else
            {
                return ((OpNode)this.treeHead).Evaluate(this.EvaluateHelper(((OpNode)this.treeHead).Left), this.EvaluateHelper(((OpNode)this.treeHead).Right));
            }
        }

        /// <summary>
        /// Returns List of variables so outside source knows what variables exist
        /// </summary>
        /// <returns> list of variables as string </returns>
        public List<string> GetAllVariables()
        {
            List<string> variables = this.variableDictionary.Keys.ToList();
            return variables;
        }

        /// <summary>
        /// Evaluates a single node in the same manner as Evaluate()
        /// </summary>
        /// <param name="node"> Node To Start Evaluation With </param>
        /// <returns> result of evaluating node as a double </returns>
        private double EvaluateHelper(ExpressionNode node)
        {
            if (node.GetType() == typeof(NumNode))
            {
                return ((NumNode)node).DataValue;
            }
            else if (node.GetType() == typeof(VariableNode))
            {
                return this.variableDictionary[((VariableNode)node).VarName];
            }
            else
            {
                return ((OpNode)node).Evaluate(this.EvaluateHelper(((OpNode)node).Left), this.EvaluateHelper(((OpNode)node).Right));
            }
        }

        /// <summary>
        /// Changes expression with no whitespace into postfix expression with whitespace in between values
        /// </summary>
        /// <param name="expression"> expression with no whitespace </param>
        /// <returns> postfix expression with whitespace in between values </returns>
        private string TurnPostfix(string expression)
        {
            int expressionPlaceholder = 0;
            int expressionPartEnd;
            Stack<char> operatorStack = new Stack<char>();
            StringBuilder postfixExpression = new StringBuilder();

            while (expressionPlaceholder < expression.Length)
            {
                // if value is operator
                if (this.operatorDictionary.ContainsKey(expression[expressionPlaceholder]))
                {
                    // if new operator has higher precedence push to stack
                    if (operatorStack.Count == 0 || CheckPrecedence(operatorStack.Peek(), expression[expressionPlaceholder]) == true)
                    {
                        operatorStack.Push(expression[expressionPlaceholder]);
                    }
                    else
                    {
                        while (operatorStack.Count > 0 && CheckPrecedence(operatorStack.Peek(), expression[expressionPlaceholder]) == false)
                        {
                            postfixExpression.Append(operatorStack.Pop() + " ");
                        }

                        operatorStack.Push(expression[expressionPlaceholder]);
                    }

                    expressionPlaceholder++;
                }

                // if value is left parenthesis
                else if (expression[expressionPlaceholder] == '(')
                {
                    operatorStack.Push(expression[expressionPlaceholder]);
                    expressionPlaceholder++;
                }

                // if value is right parenthesis
                else if (expression[expressionPlaceholder] == ')')
                {
                    char currentOp = operatorStack.Pop();
                    while (currentOp != '(')
                    {
                        postfixExpression.Append(currentOp + " ");
                        currentOp = operatorStack.Pop();
                    }

                    expressionPlaceholder++;
                }

                // if value is variable
                else if (char.IsLetter(expression[expressionPlaceholder]) == true)
                {
                    // finds length of the variable
                    expressionPartEnd = expressionPlaceholder;
                    while (expressionPartEnd < expression.Length && this.operatorDictionary.ContainsKey(expression[expressionPartEnd]) != true && expression[expressionPartEnd] != ')' && expression[expressionPartEnd] != '(')
                    {
                        expressionPartEnd++;
                    }

                    // puts variable on string
                    postfixExpression.Append(expression.Substring(expressionPlaceholder, expressionPartEnd - expressionPlaceholder) + " ");

                    expressionPlaceholder = expressionPartEnd;
                }

                // if value is number
                else if (char.IsDigit(expression[expressionPlaceholder]) == true)
                {
                    // finds length of the number
                    expressionPartEnd = expressionPlaceholder;
                    while (expressionPartEnd < expression.Length && this.operatorDictionary.ContainsKey(expression[expressionPartEnd]) != true && expression[expressionPartEnd] != ')' && expression[expressionPartEnd] != '(')
                    {
                        expressionPartEnd++;
                    }

                    // puts number on string
                    postfixExpression.Append(expression.Substring(expressionPlaceholder, expressionPartEnd - expressionPlaceholder) + " ");

                    expressionPlaceholder = expressionPartEnd;
                }
            }

            // pops remaining operators and adds to string
            while (operatorStack.Count > 0)
            {
                postfixExpression.Append(operatorStack.Pop() + " ");
            }

            return postfixExpression.ToString();
        }
    }
}
