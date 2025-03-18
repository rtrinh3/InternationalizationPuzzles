namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/12/
// Puzzle 12: Sorting it out
[TestClass()]
public class Puzzle12Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle12(File.ReadAllText("12-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("1885816494308838", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle12(File.ReadAllText("12-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("3965840697411880", answer);
    }
}
