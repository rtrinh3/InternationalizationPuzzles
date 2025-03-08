namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/2/
// Detecting gravitational waves
[TestClass()]
public class Puzzle02Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle02(File.ReadAllText("02-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("2019-06-05T12:15:00+00:00", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle02(File.ReadAllText("02-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("2020-10-25T01:30:00+00:00", answer);
    }
}
