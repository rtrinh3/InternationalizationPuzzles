namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/11/
// Puzzle 11: Homer's cipher
[TestClass()]
public class Puzzle11Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle11(File.ReadAllText("11-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("19", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle11(File.ReadAllText("11-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("162", answer);
    }
}
