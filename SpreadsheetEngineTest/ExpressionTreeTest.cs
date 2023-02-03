using System.Reflection;

#pragma warning disable CS8603 // Possible null reference return.

namespace SpreadsheetEngine.Test
{
    public class ExpressionTreeTest
    {
        private ExpressionTree objectUnderTest = new ExpressionTree("A+B*C");
        private MethodInfo GetMethod(string methodName)
        {
            if (string.IsNullOrWhiteSpace(methodName))
                Assert.Fail("methodName cannot be null or whitespace");
            var method = this.objectUnderTest.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);
            if (method == null)
                Assert.Fail(string.Format("{0} method not found", methodName));
            return method;
        }

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        public void DefaultVariableTest()
        {
            ExpressionTree testTree = new ExpressionTree("A");
            Assert.That(testTree.Evaluate(), Is.EqualTo(0));
        }

        [Test]
        public void SetVariableBaseTest()
        {
            ExpressionTree testTree = new ExpressionTree("A");
            testTree.SetVariable("A", 1);
            Assert.That(testTree.Evaluate(), Is.EqualTo(1));
        }

        [Test]
        public void AddComplexTest()
        {
            ExpressionTree testTree = new ExpressionTree("A+1+1");
            testTree.SetVariable("A", 1);
            Assert.That(testTree.Evaluate(), Is.EqualTo(3));
        }

        [Test]
        public void SubtractComplexTest()
        {
            ExpressionTree testTree = new ExpressionTree("A-1-1");
            testTree.SetVariable("A", 3);
            Assert.That(testTree.Evaluate(), Is.EqualTo(1));
        }

        [Test]
        public void MultiplyComplexTest()
        {
            ExpressionTree testTree = new ExpressionTree("A*2*2");
            testTree.SetVariable("A", 1);
            Assert.That(testTree.Evaluate(), Is.EqualTo(4));
        }

        [Test]
        public void DivideComplexTest()
        {
            ExpressionTree testTree = new ExpressionTree("A/2/1");
            testTree.SetVariable("A", 8);
            Assert.That(testTree.Evaluate(), Is.EqualTo(4));
        }

        [Test]
        public void AddTest()
        {
            AddOpNode testNode = new AddOpNode(new NumNode(7), new NumNode(3));
            Assert.That(testNode.Evaluate(7, 3), Is.EqualTo(10));
        }

        [Test]
        public void SubtractTest()
        {
            SubtractOpNode testNode = new SubtractOpNode(new NumNode(7), new NumNode(3));
            Assert.That(testNode.Evaluate(7, 3), Is.EqualTo(4));
        }

        [Test]
        public void MultiplyTest()
        {
            MultiplyOpNode testNode = new MultiplyOpNode(new NumNode(7), new NumNode(3));
            Assert.That(testNode.Evaluate(7, 3), Is.EqualTo(21));
        }

        [Test]
        public void DivideTest()
        {
            DivideOpNode testNode = new DivideOpNode(new NumNode(8), new NumNode(2));
            Assert.That(testNode.Evaluate(8, 2), Is.EqualTo(4));
        }

        [Test]
        public void ComplexOperatorTest()
        {
            ExpressionTree testTree = new ExpressionTree("1+1*2");
            Assert.That(testTree.Evaluate(), Is.EqualTo(3));
        }

        [Test]
        public void TurnPostfixTest()
        {
            MethodInfo methodInfo = this.GetMethod("TurnPostfix");
            Assert.That(methodInfo.Invoke(objectUnderTest, new object[] { "A*B+C" }), Is.EqualTo("A B * C + "));
        }

        [Test]
        public void TurnPostfixParenthesisTest()
        {
            MethodInfo methodInfo = this.GetMethod("TurnPostfix");
            Assert.That(methodInfo.Invoke(objectUnderTest, new object[] { "A*(B+C)" }), Is.EqualTo("A B C + * "));
        }

        [Test]
        public void CheckPrecedenceTrueTest()
        {
            Assert.That(ExpressionTree.CheckPrecedence('+', '*'), Is.EqualTo(true));
        }

        [Test]
        public void CheckPrecedenceFalseTest()
        {
            Assert.That(ExpressionTree.CheckPrecedence('*', '+'), Is.EqualTo(false));
        }
    }
}
