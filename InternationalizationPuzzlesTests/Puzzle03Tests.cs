namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/3/
// Unicode passwords
[TestClass()]
public class Puzzle03Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle03(File.ReadAllText("03-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("2", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle03(File.ReadAllText("03-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("509", answer);
    }
}
