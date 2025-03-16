namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/10/
// Puzzle 10: Unicode passwords strike back!
[TestClass()]
public class Puzzle10Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle10(File.ReadAllText("10-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("4", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle10(File.ReadAllText("10-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("2232", answer);
    }
}
