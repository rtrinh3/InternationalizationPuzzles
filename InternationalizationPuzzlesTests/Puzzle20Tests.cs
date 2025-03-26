namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/20/
// Puzzle 20: The future of Unicode
[TestClass()]
public class Puzzle20Tests
{
    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle20(File.ReadAllText("20-test-input.txt"));
        var answer = solver.Decode();
        Assert.IsTrue(answer.Contains("ꪪꪪꪪ This is a secret message. ꪪꪪꪪ Good luck decoding me! ꪪꪪꪪ"));
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle20(File.ReadAllText("20-input.txt"));
        var answer = solver.Solve();
        //Assert.AreEqual("", answer);
        Assert.Inconclusive(answer);
    }
}
