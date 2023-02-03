// Written by Nathanael Ostheller
// 011717168

// hiding was intentional
#pragma warning disable CS0108 // Member hides inherited member; missing new keyword

// fine if null
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8601 // Possible null reference assignment.

// fine to return null
#pragma warning disable CS8603 // Possible null reference return.
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.

namespace SpreadsheetEngine
{
    using System.ComponentModel;
    using System.IO;
    using System.Text;
    using System.Xml;
    using System.Xml.Linq;

    public class Spreadsheet
    {
        private Cell[,] cellSheet;
        private int colCount;
        private int rowCount;
        private Stack<BasicCommand> undoStack;
        private Stack<BasicCommand> redoStack;
        private string undoTitle;
        private string redoTitle;

        public Spreadsheet(int row, int col)
        {
            this.rowCount = row;
            this.colCount = col;
            this.cellSheet = new Cell[row, col];
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    this.cellSheet[i, j] = new SpreadsheetCell(i, j);
                    this.cellSheet[i, j].PropertyChanged += this.CellPropertyChanged;
                }
            }

            this.undoStack = new Stack<BasicCommand>();
            this.redoStack = new Stack<BasicCommand>();
            this.undoTitle = string.Empty;
            this.redoTitle = string.Empty;
        }

        public event PropertyChangedEventHandler CellPropertyChangedEv = (sender, e) => { };

        public int ColumnCount
        {
            get { return this.colCount; }
        }

        public int RowCount
        {
            get { return this.rowCount; }
        }

        public string UndoTitle
        {
            get { return this.undoTitle; }
        }

        public string RedoTitle
        {
            get { return this.redoTitle; }
        }

        /// <summary>
        /// Removes whitespace characters from a string
        /// </summary>
        /// <param name="text"> Input as a string with whitespace </param>
        /// <returns> String without whitespace </returns>
        public static string RemoveWhiteSpace(string text)
        {
            StringBuilder result = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                if (text[i] != ' ')
                {
                    result.Append(text[i]);
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Returns the cell at the location in the sheet pointed to by row and col.
        /// </summary>
        /// <param name="row">Spreadsheet row to be accessed.</param>
        /// <param name="col">Spreadsheet column to be accessed.</param>
        /// <returns>Cell if cell exists, null if cell does not exist.</returns>
        public Cell? GetCell(int row, int col)
        {
            if (row <= this.RowCount && col <= this.ColumnCount)
            {
                return this.cellSheet[row, col];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns the cell at the location implied by the name
        /// </summary>
        /// <param name="name">String of form A-0 to access cell (0,0</param>
        /// <returns>Cell if cell exists, null if cell does not exist</returns>
        public Cell? GetCellName(string name)
        {
            int col = char.Parse(name.Substring(0, 1)) - 'A';
            int row = int.Parse(name.Substring(1)) - 1;
            if (this.cellSheet[row, col] != null)
            {
                return this.cellSheet[row, col];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Adds a command to the undo stack (should be run when an action is taken)
        /// </summary>
        /// <param name="command">The command that is being run elsewhere</param>
        public void AddUndo(BasicCommand command)
        {
            this.undoStack.Push(command);
            this.undoTitle = command.Title;
        }

        /// <summary>
        /// Adds a command to the redo stack
        /// </summary>
        /// <param name="command">Command that is added to redo stack</param>
        public void AddRedo(BasicCommand command)
        {
            this.redoStack.Push(command);
            this.redoTitle = command.Title;
        }

        /// <summary>
        /// Undoes the last action on the spreadsheet
        /// </summary>
        public void Undo()
        {
            BasicCommand command = this.undoStack.Pop();
            this.AddRedo(command);
            if (this.undoStack.Count != 0)
            {
                this.undoTitle = this.undoStack.Peek().Title;
            }
            else
            {
                this.undoTitle = string.Empty;
            }

            command.Undo();
        }

        public void Redo()
        {
            BasicCommand command = this.redoStack.Pop();
            this.AddUndo(command);
            if (this.redoStack.Count != 0)
            {
                this.redoTitle = this.redoStack.Peek().Title;
            }
            else
            {
                this.redoTitle = string.Empty;
            }

            command.Execute();
        }

        /// <summary>
        /// Saves Spreadsheet as XML file to given stream
        /// </summary>
        /// <param name="outstream">stream to file the XML document will be saved as</param>
        public void SaveToFile(Stream outstream)
        {
            XDocument saveSpreadsheet = new XDocument(new XElement("Sheet"));
            for (int i = 0; i < this.rowCount; i++)
            {
                for (int o = 0; o < this.colCount; o++)
                {
                    // if cell is not default
                    if (this.GetCell(i, o).Text != null || this.GetCell(i, o).BGColor != 0xFFFFFFFF)
                    {
                        string name = Convert.ToChar(o + 'A').ToString() + (i + 1).ToString();
                        saveSpreadsheet.Root.Add(new XElement(
                            "Cell",
                            new XAttribute("Name", name),
                            new XElement("Text", this.GetCell(i, o).Text),
                            new XElement("Color", this.GetCell(i, o).BGColor)));
                    }
                }
            }

            using (StreamWriter writer = new StreamWriter(outstream, Encoding.UTF8))
            {
                writer.Write(saveSpreadsheet);
            }
        }

        /// <summary>
        /// Loads spreadsheet from XML file
        /// </summary>
        /// <param name="instream"> stream to file that sheet will be loaded from </param>
        public void LoadFromFile(Stream instream)
        {
            XmlReader reader = XmlReader.Create(instream);
            XDocument loadSpreadsheet = XDocument.Load(reader);
            this.Clear();
            IEnumerable<XElement> elements = loadSpreadsheet.Root.Elements("Cell");
            foreach (XElement element in elements)
            {
                string name = element.Attribute("Name").Value;
                string text = element.Element("Text").Value;
                uint color = uint.Parse(element.Element("Color").Value);
                this.GetCellName(name).Text = text;
                this.GetCellName(name).BGColor = color;
            }
        }

        public void Clear()
        {
            for (int i = 0; i < this.rowCount; i++)
            {
                for (int o = 0; o < this.colCount; o++)
                {
                    this.GetCell(i, o).Text = string.Empty;
                    this.GetCell(i, o).BGColor = 0xFFFFFFFF;
                }
            }

            this.undoStack.Clear();
            this.redoStack.Clear();
        }

        /// <summary>
        /// Changes cell value to match evaluated value whenever changed
        /// </summary>
        private void CellPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (((SpreadsheetCell)sender).Text != null)
            {
                string cellText = ((SpreadsheetCell)sender).Text;
                if (RemoveWhiteSpace(cellText).Length < 2 || RemoveWhiteSpace(cellText).Substring(0, 1) != "=")
                {
                    ((SpreadsheetCell)sender).Value = cellText;
                }
                else
                {
                    // create expression tree from expression
                    ExpressionTree newExpression = new ExpressionTree(RemoveWhiteSpace(cellText)[1..]);

                    // populate variables from other cell values
                    List<string> variables = newExpression.GetAllVariables();
                    if (variables.Count == 0)
                    {
                        ((SpreadsheetCell)sender).Value = newExpression.Evaluate().ToString();
                    }
                    else if (RemoveWhiteSpace(cellText) == ("=" + variables[0]))
                    {
                        int row;
                        int col = (byte)variables[0][0] - 64;
                        if (int.TryParse(variables[0].Substring(1), out row) == false)
                        {
                            throw new Exception("Expression contains invalid variable");
                        }

                        if (row < 1 || row > this.RowCount || col < 1 || col > this.ColumnCount)
                        {
                            throw new Exception("Expression contains invalid variable");
                        }

                        ((SpreadsheetCell)sender).Value = this.GetCell(row - 1, col - 1).Value;
                        this.GetCell(row - 1, col - 1).PropertyChanged += ((SpreadsheetCell)sender).CascadeUpdate;
                    }
                    else
                    {
                        for (int i = 0; i < variables.Count; i++)
                        {
                            int col;
                            int row;
                            double setValue;
                            col = (byte)variables[i][0] - 64;
                            if (int.TryParse(variables[i].Substring(1), out row) == false)
                            {
                                throw new Exception("Expression contains invalid variable");
                            }

                            if (row < 1 || row > this.RowCount || col < 1 || col > this.ColumnCount)
                            {
                                throw new Exception("Expression contains invalid variable");
                            }

                            double.TryParse(this.GetCell(row - 1, col - 1).Value, out setValue);
                            newExpression.SetVariable(variables[i], setValue);
                            this.GetCell(row - 1, col - 1).PropertyChanged += ((SpreadsheetCell)sender).CascadeUpdate;

                            // update actual cell
                            ((SpreadsheetCell)sender).Value = newExpression.Evaluate().ToString();
                        }
                    }
                }
            }

            this.CellPropertyChangedEv(sender, e);
        }

        internal class SpreadsheetCell : Cell
        {
            public SpreadsheetCell(int row, int col)
                : base(row, col)
            {
            }

            public string Value
            {
                get { return this.value; }
                set { this.value = value; }
            }
        }
    }
}