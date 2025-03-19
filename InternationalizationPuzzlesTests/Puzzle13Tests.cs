namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/13/
// Puzzle 13: Gulliver's puzzle dictionary
[TestClass()]
public class Puzzle13Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle13(File.ReadAllText("13-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("47", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle13(File.ReadAllText("13-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("18927", answer);
    }
}
