namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/18/
// Puzzle 18
[TestClass()]
public class Puzzle18Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle18(File.ReadAllText("18-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("19282", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle18(File.ReadAllText("18-input.txt"));
        var answer = solver.Solve();
        //Assert.AreEqual("", answer);
        Assert.Inconclusive(answer);
    }
}
