namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/2/
// Puzzle 2: Detecting gravitational waves
public class Puzzle02(string input) : IPuzzle
{
    public string Solve()
    {
        Dictionary<DateTimeOffset, int> counts = new();
        foreach (string line in input.TrimEnd().ReplaceLineEndings("\n").Split('\n'))
        {
            var parsed = DateTimeOffset.Parse(line);
            var normalized = parsed.ToUniversalTime();
            var count = counts.GetValueOrDefault(normalized, 0) + 1;
            counts[normalized] = count;
            if (count == 4)
            {
                return normalized.ToString("yyyy-MM-ddTHH:mm:sszzz");
            }
        }
        throw new Exception("No solution found");
    }
}
