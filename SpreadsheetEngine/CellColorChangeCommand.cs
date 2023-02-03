#pragma warning disable CS8602 // Dereference of a possibly null reference.

namespace SpreadsheetEngine
{
    public class CellColorChangeCommand : BasicCommand
    {
        private List<uint>? oldColor = new List<uint>();
        private uint newColor;
        private List<int>? rows = new List<int>();
        private List<int>? columns = new List<int>();
        private Spreadsheet refSheet;

        public CellColorChangeCommand(uint color, List<Cell> cells, ref Spreadsheet reciever)
        {
            this.Title = "Color Change";
            this.newColor = color;
            foreach (Cell cell in cells)
            {
                this.rows.Add(cell.RowIndex);
                this.columns.Add(cell.ColumnIndex);
                this.oldColor.Add(cell.BGColor);
            }

            this.refSheet = reciever;
        }

        public override void Execute()
        {
            for (int i = 0; i < this.rows.Count; i++)
            {
                this.refSheet.GetCell(this.rows[i], this.columns[i]).BGColor = this.newColor;
            }
        }

        public override void Undo()
        {
            for (int i = 0; i < this.rows.Count; i++)
            {
                this.refSheet.GetCell(this.rows[i], this.columns[i]).BGColor = this.oldColor[i];
            }
        }
    }
}
