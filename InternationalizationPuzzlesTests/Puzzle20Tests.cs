namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/20/
// Puzzle 20: The future of Unicode
[TestClass()]
public class Puzzle20Tests
{
    [TestMethod()]
    public void Decode_TestInputTest()
    {
        var solver = new Puzzle20(File.ReadAllText("20-test-input.txt"));
        var answer = solver.Decode();
        Assert.AreEqual("\uAAAA\uAAAA\uAAAA This is a secret message. \uAAAA\uAAAA\uAAAA Good luck decoding me! \uAAAA\uAAAA\uAAAA", answer);
    }

    [TestMethod()]
    public void Solve_InputTest()
    {
        var solver = new Puzzle20(File.ReadAllText("20-input.txt"));
        var answer = solver.Solve();
        //Assert.AreEqual("", answer);
        Assert.Inconclusive(answer);
    }
}
