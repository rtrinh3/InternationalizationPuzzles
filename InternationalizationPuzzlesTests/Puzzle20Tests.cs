using System.Text;

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
    public void ParseExtendedUtf8_Test()
    {
        string input = "九億八千七百六十五万四千三百二十一"; // From puzzle 14
        var inputRunes = input.EnumerateRunes().Select(x => (uint)x.Value);
        var utf8 = Encoding.UTF8.GetBytes(input);
        var parsed = Puzzle20.ParseExtendedUtf8(utf8);
        Assert.IsTrue(inputRunes.SequenceEqual(parsed));
    }

    [TestMethod()]
    public void Solve_InputTest()
    {
        var solver = new Puzzle20(File.ReadAllText("20-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("4216386", answer);
    }
}
