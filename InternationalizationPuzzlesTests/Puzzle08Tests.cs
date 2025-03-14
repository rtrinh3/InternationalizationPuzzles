namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/8/
// Day 8: Unicode passwords redux
[TestClass()]
public class Puzzle08Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle08(File.ReadAllText("08-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("2", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle08(File.ReadAllText("08-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("809", answer);
    }
}
