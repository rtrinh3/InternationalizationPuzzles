namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/4/
// A trip around the world
[TestClass()]
public class Puzzle04Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle04(File.ReadAllText("04-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("3143", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle04(File.ReadAllText("04-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("16451", answer);
    }
}
