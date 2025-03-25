namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/19/
[TestClass()]
public class Puzzle19Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle19(File.ReadAllText("19-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("2024-04-09T17:49:00+00:00", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle19(File.ReadAllText("19-input.txt"));
        var answer = solver.Solve();
        //Assert.AreEqual("", answer);
        Assert.Inconclusive(answer);
    }
}
