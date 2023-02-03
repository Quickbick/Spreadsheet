using SpreadsheetEngine;
using System.IO;

// is fine if it is null
#pragma warning disable CS8602 // Dereference of a possibly null reference.
#pragma warning disable CS8604 // Possible null reference argument.

namespace SpreadsheetEngine.Test
{
    public class SpreadSheetTest
    {
        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void GetCellBaseTest()
        {
            Spreadsheet testSheet = new Spreadsheet(10, 10);
            Assert.That(testSheet.GetCell(0, 0).RowIndex, Is.EqualTo(0));
        }

        [Test]
        public void GetCellBoundaryTest()
        {
            Spreadsheet testSheet = new Spreadsheet(10, 10);
            Assert.That(testSheet.GetCell(9, 9).ColumnIndex, Is.EqualTo(9));
        }

        [Test]
        public void EvaluateComplexCell()
        {
            Spreadsheet testSheet = new Spreadsheet(10, 10);
            testSheet.GetCell(0, 0).Text = "=5-1*2";
            Assert.That(testSheet.GetCell(0, 0).Value, Is.EqualTo("3"));
        }

        [Test]
        public void EvaluateVariableCell()
        {
            Spreadsheet testSheet = new Spreadsheet(10, 10);
            testSheet.GetCell(0, 0).Text = "E";
            testSheet.GetCell(0, 1).Text = "=A1";
            Assert.That(testSheet.GetCell(0, 1).Value, Is.EqualTo("E"));
        }

        [Test]
        public void EvaluateComplexVariableCell()
        {
            Spreadsheet testSheet = new Spreadsheet(10, 10);
            testSheet.GetCell(0, 0).Text = "1";
            testSheet.GetCell(0, 1).Text = "=5-1*A1";
            Assert.That(testSheet.GetCell(0, 1).Value, Is.EqualTo("4"));
        }

        [Test]
        public void RemoveWhiteSpaceTest()
        {
            string start = "I have a space";
            Assert.That(Spreadsheet.RemoveWhiteSpace(start), Is.EqualTo("Ihaveaspace"));
        }

        [Test]
        public void RemoveNoWhiteSpaceTest()
        {
            string start = "Ihavenospace";
            Assert.That(Spreadsheet.RemoveWhiteSpace(start), Is.EqualTo("Ihavenospace"));
        }

        [Test]
        public void CascadeUpdateTest()
        {
            Spreadsheet testSheet = new Spreadsheet(10, 10);
            testSheet.GetCell(0, 0).Text = "1";
            testSheet.GetCell(0, 1).Text = "=5-1*A1";
            testSheet.GetCell(0, 0).Text = "4";
            Assert.That(testSheet.GetCell(0, 1).Value, Is.EqualTo("1"));
        }

        [Test]
        public void UndoTest()
        {
            Spreadsheet testSheet = new Spreadsheet(1, 1);
            BasicCommand command1 = new TextChangeCommand("5", testSheet.GetCell(0, 0), ref testSheet);
            testSheet.GetCell(0, 0).Text = "5";
            testSheet.AddUndo(command1);
            BasicCommand command2 = new TextChangeCommand("7", testSheet.GetCell(0, 0), ref testSheet);
            testSheet.GetCell(0, 0).Text = "7";
            testSheet.AddUndo(command2);
            testSheet.Undo();
            Assert.That(testSheet.GetCell(0,0).Value, Is.EqualTo("5"));
        }

        [Test]
        public void RedoTest()
        {
            Spreadsheet testSheet = new Spreadsheet(1, 1);
            BasicCommand command1 = new TextChangeCommand("5", testSheet.GetCell(0, 0), ref testSheet);
            testSheet.GetCell(0, 0).Text = "5";
            testSheet.AddUndo(command1);
            BasicCommand command2 = new TextChangeCommand("7", testSheet.GetCell(0, 0), ref testSheet);
            testSheet.GetCell(0, 0).Text = "7";
            testSheet.AddUndo(command2);
            testSheet.Undo();
            testSheet.Redo();
            Assert.That(testSheet.GetCell(0, 0).Value, Is.EqualTo("7"));
        }

        [Test]
        public void SaveLoadEmptyTest()
        {
            Spreadsheet testSheet = new Spreadsheet(1, 1);
            FileStream sStream = File.Open("testsavefile.txt", FileMode.Create, FileAccess.Write);
            testSheet.SaveToFile(sStream);
            sStream.Close();
            FileStream lStream = File.Open("testsavefile.txt", FileMode.Open, FileAccess.Read);
            testSheet.LoadFromFile(lStream);
            lStream.Close();
            Assert.That(testSheet.GetCell(0, 0).Value, Is.EqualTo(string.Empty));
            Assert.That(testSheet.GetCell(0, 0).BGColor, Is.EqualTo(0xFFFFFFFF));
        }

        [Test]
        public void SaveLoadTest()
        {
            Spreadsheet testSheet = new Spreadsheet(1, 1);
            testSheet.GetCell(0, 0).Text = "Isaved";
            testSheet.GetCell(0, 0).BGColor = 000000;
            FileStream sStream = File.Open("testsavefile.txt", FileMode.Create, FileAccess.Write);
            testSheet.SaveToFile(sStream);
            sStream.Close();
            FileStream lStream = File.Open("testsavefile.txt", FileMode.Open, FileAccess.Read);
            testSheet.LoadFromFile(lStream);
            lStream.Close();
            Assert.That(testSheet.GetCell(0, 0).Value, Is.EqualTo("Isaved"));
            Assert.That(testSheet.GetCell(0, 0).BGColor, Is.EqualTo(000000));
        }

        [Test]
        public void ClearCellsTest()
        {
            Spreadsheet testSheet = new Spreadsheet(2, 2);
            testSheet.GetCell(0, 0).Text = "Isaved";
            testSheet.GetCell(0, 0).BGColor = 000000;
            testSheet.GetCell(1, 1).Text = "Isaved";
            testSheet.GetCell(1, 1).BGColor = 000000;
            testSheet.Clear();
            Assert.That(testSheet.GetCell(0, 0).Value, Is.EqualTo(string.Empty));
            Assert.That(testSheet.GetCell(0, 0).BGColor, Is.EqualTo(0xFFFFFFFF));
            Assert.That(testSheet.GetCell(1, 1).Value, Is.EqualTo(string.Empty));
            Assert.That(testSheet.GetCell(1, 1).BGColor, Is.EqualTo(0xFFFFFFFF));
        }
    }
}