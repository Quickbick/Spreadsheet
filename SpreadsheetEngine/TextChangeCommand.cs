#pragma warning disable CS8600 // Converting null literal or possible null value to non-nullable type.
#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace SpreadsheetEngine
{
    public class TextChangeCommand : BasicCommand
    {
        private string oldText;
        private string newText;
        private int row;
        private int column;
        private Spreadsheet refSheet;

        public TextChangeCommand(string text, Cell cell, ref Spreadsheet reciever)
        {
            this.Title = "Cell Value Change";
            this.oldText = cell.Text;
            this.newText = text;
            this.row = cell.RowIndex;
            this.column = cell.ColumnIndex;
            this.refSheet = reciever;
        }

        public override void Execute()
        {
            if (this.newText != null)
            {
                ((Spreadsheet.SpreadsheetCell)this.refSheet.GetCell(this.row, this.column)).Text = this.newText;
            }
            else
            {
                ((Spreadsheet.SpreadsheetCell)this.refSheet.GetCell(this.row, this.column)).Text = string.Empty;
            }
        }

        public override void Undo()
        {
            if (this.oldText != null)
            {
                ((Spreadsheet.SpreadsheetCell)this.refSheet.GetCell(this.row, this.column)).Text = this.oldText;
            }
            else
            {
                ((Spreadsheet.SpreadsheetCell)this.refSheet.GetCell(this.row, this.column)).Text = string.Empty;
            }
        }

    }
}
