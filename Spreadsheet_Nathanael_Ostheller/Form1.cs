// Written by Nathanael Ostheller
// 011717168

#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8622 // Nullability of reference types in type of parameter doesn't match the target delegate (possibly because of nullability attributes).
#pragma warning disable SA1601 // Partial elements should be documented
#pragma warning disable SA1600 // Elements should be documented

namespace Spreadsheet_Nathanael_Ostheller
{
    using System.ComponentModel;
    using System.Text;
    using SpreadsheetEngine;

    public partial class Form1 : Form
    {
        private SpreadsheetEngine.Spreadsheet spreadsheet;

        public Form1()
        {
            this.spreadsheet = new SpreadsheetEngine.Spreadsheet(50, 26);
            this.InitializeComponent();
            this.InitializeDataGrid();
            this.spreadsheet.CellPropertyChangedEv += this.SheetPropertyChanged;
            this.UpdateUndoRedoMenu();
        }

        private void InitializeDataGrid()
        {
            this.dataGridView1.Columns.Clear();

            // adds rows A to Z
            for (char c = 'A'; c <= 'Z'; c++)
            {
                this.dataGridView1.Columns.Add(c.ToString(), c.ToString());
            }

            this.dataGridView1.Rows.Clear();

            // adds and labels rows 1 to 50
            this.dataGridView1.Rows.Add(50);
            for (int i = 0; i < 50; i++)
            {
                this.dataGridView1.Rows[i].HeaderCell.Value = (i + 1).ToString();
            }
        }

        private void SheetPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Cell changedCell = (Cell)sender;
            this.dataGridView1.Rows[changedCell.RowIndex].Cells[changedCell.ColumnIndex].Value = changedCell.Value;
            this.dataGridView1.Rows[changedCell.RowIndex].Cells[changedCell.ColumnIndex].Style.BackColor = System.Drawing.Color.FromArgb((int)changedCell.BGColor);
            this.UpdateUndoRedoMenu();
        }

        private void Spreadsheet_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text;
        }

        private void Spreadsheet_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            BasicCommand command = new TextChangeCommand((string)this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value, this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex), ref this.spreadsheet);
            this.spreadsheet.AddUndo(command);
            this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Text = (string)this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value;
            this.dataGridView1.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = this.spreadsheet.GetCell(e.RowIndex, e.ColumnIndex).Value;
        }

        private void DemoButton_Click(object sender, EventArgs e)
        {
            Random rnd = new Random();
            for (int i = 0; i < 50; i++)
            {
                int col = rnd.Next(0, 26);
                int row = rnd.Next(0, 50);
                this.spreadsheet.GetCell(row, col).Text = "Random Cell";
            }

            for (int i = 0; i < 50; i++)
            {
                this.spreadsheet.GetCell(i, 1).Text = "This is cell B" + (i + 1).ToString();
            }

            for (int i = 0; i < 50; i++)
            {
                this.spreadsheet.GetCell(i, 0).Text = "=B" + (i + 1).ToString();
            }
        }

        private void Button2_Click(object sender, EventArgs e)
        {
            if (this.colorDialog1.ShowDialog() == DialogResult.OK)
            {
                Color newColor = this.colorDialog1.Color;
                List<Cell> cells = new List<Cell>();
                foreach (DataGridViewCell cell in this.dataGridView1.SelectedCells)
                {
                    int row = cell.RowIndex;
                    int col = cell.ColumnIndex;
                    cells.Add(this.spreadsheet.GetCell(row, col));
                }

                BasicCommand command = new CellColorChangeCommand((uint)newColor.ToArgb(), cells, ref this.spreadsheet);
                this.spreadsheet.AddUndo(command);

                foreach (DataGridViewCell cell in this.dataGridView1.SelectedCells)
                {
                    int row = cell.RowIndex;
                    int col = cell.ColumnIndex;
                    this.spreadsheet.GetCell(row, col).BGColor = (uint)newColor.ToArgb();
                }
            }
        }

        private void UndoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Undo();
        }

        private void RedoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.spreadsheet.Redo();
        }

        /// <summary>
        /// Updates the Menu Tool Strips for Undo and Redo
        /// </summary>
        private void UpdateUndoRedoMenu()
        {
            if (this.spreadsheet.UndoTitle == string.Empty)
            {
                this.undoToolStripMenuItem.Enabled = false;
                this.undoToolStripMenuItem.Text = "Nothing to Undo";
            }
            else
            {
                this.undoToolStripMenuItem.Enabled = true;
                this.undoToolStripMenuItem.Text = "Undo" + this.spreadsheet.UndoTitle;
            }

            if (this.spreadsheet.RedoTitle == string.Empty)
            {
                this.redoToolStripMenuItem.Enabled = false;
                this.redoToolStripMenuItem.Text = "Nothing to Redo";
            }
            else
            {
                this.redoToolStripMenuItem.Enabled = true;
                this.redoToolStripMenuItem.Text = "Redo" + this.spreadsheet.RedoTitle;
            }
        }

        /// <summary>
        /// Saves spreadsheet into a file using system file windows. No user inputted parameters.
        /// </summary>
        private void SaveToFile(object sender, EventArgs e)
        {
            Stream saveStream;

            this.saveFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            this.saveFileDialog1.FilterIndex = 2;
            this.saveFileDialog1.RestoreDirectory = true;

            if (this.saveFileDialog1.ShowDialog() == DialogResult.OK)
            {
                if ((saveStream = this.saveFileDialog1.OpenFile()) != null)
                {
                    this.spreadsheet.SaveToFile(saveStream);
                    saveStream.Close();
                }
            }
        }

        /// <summary>
        /// Reads in text from a file into the spreadsheet. No user inputted parameters.
        /// </summary>
        private void LoadFromFile(object sender, EventArgs e)
        {
            Stream loadStream;

            this.openFileDialog1.InitialDirectory = "c:\\";
            this.openFileDialog1.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
            this.openFileDialog1.FilterIndex = 2;
            this.openFileDialog1.RestoreDirectory = true;

            if (this.openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                // Get the path of specified file
                string filePath = this.openFileDialog1.FileName;

                // Read the contents of the file into a stream
                loadStream = this.openFileDialog1.OpenFile();
                this.spreadsheet.LoadFromFile(loadStream);
            }
        }
    }
}