namespace InternationalizationPuzzles.Tests;

[TestClass()]
public class Puzzle15Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle15(File.ReadAllText("15-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("3030", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle15(File.ReadAllText("15-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("38850", answer);
    }
}
