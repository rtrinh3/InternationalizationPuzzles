namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/7/
// Puzzle 7: The audit trail fixer
[TestClass()]
public class Puzzle07Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle07(File.ReadAllText("07-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("866", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle07(File.ReadAllText("07-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("32152346", answer);
    }
}
