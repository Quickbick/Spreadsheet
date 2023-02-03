// Written by Nathanael Ostheller
// 011717168

// place is fine to return null
#pragma warning disable CS8603 // Possible null reference return.

// code is from MSDN and should be correct way to implement
#pragma warning disable CS8612 // Nullability of reference types in type doesn't match implicitly implemented member.

// I know when I want a protected field.
#pragma warning disable SA1401 // Fields should be private
#pragma warning disable SA1600 // Elements should be documented

namespace SpreadsheetEngine
{
    using System.ComponentModel;

    public abstract class Cell : INotifyPropertyChanged
    {
        protected string? text;
        protected string? value;
        private int rowIndex;
        private int columnIndex;
        private uint bgColor;

        public Cell(int row, int col)
        {
            this.rowIndex = row;
            this.columnIndex = col;
            this.bgColor = 0xFFFFFFFF;
        }

        public event PropertyChangedEventHandler PropertyChanged = (sender, e) => { };

        public int RowIndex
        {
            get { return this.rowIndex; }
        }

        public int ColumnIndex
        {
            get { return this.columnIndex; }
        }

        public string Text
        {
            get
            {
                return this.text;
            }

            set
            {
                if (this.text != value)
                {
                    this.text = value;
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Text"));
                }
            }
        }

        public string Value
        {
            get { return this.value;  }
        }

        public uint BGColor
        {
            get
            {
                return this.bgColor;
            }

            set
            {
                if (this.bgColor != value)
                {
                    this.bgColor = value;
                    this.PropertyChanged(this, new PropertyChangedEventArgs("Color"));
                }
            }
        }

        internal void CascadeUpdate(object sender, PropertyChangedEventArgs e)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs("Text"));
        }
    }
}