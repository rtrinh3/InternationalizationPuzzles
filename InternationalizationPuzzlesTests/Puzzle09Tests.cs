namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/9/
// Puzzle 9: Nine Eleven
[TestClass()]
public class Puzzle09Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle09(File.ReadAllText("09-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("Margot Peter", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle09(File.ReadAllText("09-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("Amelia Amoura Hugo Jack Jakob Junior Mateo", answer);
    }
}
