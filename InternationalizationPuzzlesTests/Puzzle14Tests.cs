namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/14/
// Puzzle 14: Metrification in Japan
[TestClass()]
public class Puzzle14Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle14(File.ReadAllText("14-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("2177741195", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle14(File.ReadAllText("14-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("130675442686", answer);
    }
}
