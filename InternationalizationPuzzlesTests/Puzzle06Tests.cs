namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/6/
// Day 6: Mojibake puzzle dictionary
[TestClass()]
public class Puzzle06Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle06(File.ReadAllText("06-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("50", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle06(File.ReadAllText("06-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("11252", answer);
    }
}
