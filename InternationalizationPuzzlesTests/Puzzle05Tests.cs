namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/5/
// Puzzle 5: Don't step in it...
[TestClass()]
public class Puzzle05Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle05(File.ReadAllText("05-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("2", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle05(File.ReadAllText("05-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("74", answer);
    }
}
