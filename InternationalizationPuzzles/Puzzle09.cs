using System.Diagnostics;
using System.Globalization;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/9/
// Puzzle 9: Nine Eleven
public class Puzzle09(string input) : IPuzzle
{
    private static readonly DateOnly NineEleven = new(2001, 9, 11);
    private static readonly string[] DateFormats =
    {
        "dd-MM-yy", // Margot
        "MM-dd-yy", // Peter
        "yy-MM-dd", // Frank
        "yy-dd-MM", // Elise (?!)
        //"MM-yy-dd", // Hypothetical
        //"dd-yy-MM", // Hypothetical
    };

    public string Solve()
    {
        // Parse
        string[] lines = input.TrimEnd().ReplaceLineEndings("\n").Split('\n');
        Dictionary<string, List<string>> entriesByName = new();
        foreach (string line in lines)
        {
            string[] parts = line.Split(": ");
            string dateStamp = parts[0];
            string[] names = parts[1].Split(", ");
            foreach (string name in names)
            {
                entriesByName.TryAdd(name, new List<string>());
                entriesByName[name].Add(dateStamp);
            }
        }
        // Test formats
        List<string> matches = new();
        foreach (var (name, dateStamps) in entriesByName)
        {
            HashSet<DateOnly> dates = [];
            foreach (string format in DateFormats)
            {
                List<DateOnly> parsed = new();
                foreach (string dateStamp in dateStamps)
                {
                    if (DateOnly.TryParseExact(dateStamp, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
                    {
                        parsed.Add(date);
                    }
                    else
                    {
                        break;
                    }
                }
                if (parsed.Count == dateStamps.Count)
                {
                    Debug.Assert(dates.Count == 0, "Dates are ambiguous");
                    dates = parsed.ToHashSet();
                }
            }
            Debug.Assert(dates.Count == dateStamps.Count);
            if (dates.Contains(NineEleven))
            {
                matches.Add(name);
            }
        }
        string answer = string.Join(" ", matches.Order());
        return answer;
    }
}
