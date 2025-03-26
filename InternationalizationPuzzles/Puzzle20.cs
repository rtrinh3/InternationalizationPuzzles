using System.Text;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/20/
// Puzzle 20: The future of Unicode
public class Puzzle20(string input) : IPuzzle
{
    public string Solve()
    {
        throw new NotImplementedException();
    }

    public string Decode()
    {
        var bytes = Convert.FromBase64String(input);
        Console.WriteLine(bytes.Length);
        var hex = string.Join(",", bytes.Select(x => x.ToString("x2")));
        Console.WriteLine(hex);
        var foo = Encoding.Unicode.GetString(bytes);
        Console.WriteLine(foo);
        return foo;
        // TODO
    }
}
