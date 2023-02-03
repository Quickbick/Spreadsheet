namespace SpreadsheetEngine
{
    public abstract class BasicCommand
    {
        private string? title;

        public string? Title { get { return this.title; } set { this.title = value; } }

        public abstract void Execute();

        public abstract void Undo();

    }
}
