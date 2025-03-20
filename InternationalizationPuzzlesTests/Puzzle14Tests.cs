namespace InternationalizationPuzzles.Tests;

// https://i18n-puzzles.com/puzzle/14/
// Puzzle 14: Metrification in Japan
[TestClass()]
public class Puzzle14Tests
{
    [TestMethod()]
    public void ParseNumberTest()
    {
        Assert.AreEqual(300, Puzzle14.ParseNumber("三百"));
        Assert.AreEqual(321, Puzzle14.ParseNumber("三百二十一"));
        Assert.AreEqual(4_000, Puzzle14.ParseNumber("四千"));
        Assert.AreEqual(50_000, Puzzle14.ParseNumber("五万"));
        Assert.AreEqual(99_999, Puzzle14.ParseNumber("九万九千九百九十九"));
        Assert.AreEqual(420_042, Puzzle14.ParseNumber("四十二万四十二"));
        Assert.AreEqual(987_654_321, Puzzle14.ParseNumber("九億八千七百六十五万四千三百二十一"));
    }

    [TestMethod()]
    public void TestInputTest()
    {
        var solver = new Puzzle14(File.ReadAllText("14-test-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("2177741195", answer);
    }

    [TestMethod()]
    public void InputTest()
    {
        var solver = new Puzzle14(File.ReadAllText("14-input.txt"));
        var answer = solver.Solve();
        Assert.AreEqual("130675442686", answer);
    }
}
