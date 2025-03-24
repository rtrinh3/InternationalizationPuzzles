namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/17/
// Puzzle 17: ╳ marks the spot
[TestClass()]
public class Puzzle17Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle17(File.ReadAllText("17-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("132", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle17(File.ReadAllText("17-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("7912", answer);
    }
}
