namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/16/
// Puzzle 16: 8-bit unboxing
[TestClass()]
public class Puzzle16Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle16(File.ReadAllText("16-test-input-edit.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("34", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle16(File.ReadAllText("16-input-edit.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("1180", answer);
    }
}
