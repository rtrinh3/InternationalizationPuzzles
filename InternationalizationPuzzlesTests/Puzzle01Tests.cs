namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/1/
// Length limits on messaging platforms
[TestClass()]
public class Puzzle01Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle01(File.ReadAllText("01-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("31", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle01(File.ReadAllText("01-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("107989", answer);
    }
}
