namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/10/
// Puzzle 10: Unicode passwords strike back!
public class Puzzle10(string input) : IPuzzle
{
    public string Solve()
    {
        // Parse
        var paragraphs = input.TrimEnd().ReplaceLineEndings("\n").Split("\n\n");
        Dictionary<string, string> authEntries = paragraphs[0].Split('\n')
            .Select(line => line.Split(' ', 2))
            .ToDictionary(x => x[0], x => x[1]);
        int validAttempts = 0;
        foreach (var line in paragraphs[1].Split('\n'))
        {
            var parts = line.Split(' ', 2);
            string name = parts[0];
            string pass = parts[1];
            string normalizedPass = pass.Normalize(System.Text.NormalizationForm.FormC); // Composition
            var variations = GenerateDenormalizedStrings(normalizedPass);
            foreach (string variation in variations)
            {
                if (BCrypt.Net.BCrypt.Verify(variation, authEntries[name]))
                {
                    validAttempts++;
                    break;
                }
            }
        }
        return validAttempts.ToString();
    }

    private static IEnumerable<string> GenerateDenormalizedStrings(string normalInput)
    {
        if (string.IsNullOrEmpty(normalInput))
        {
            return [""];
        }
        string first = normalInput[0].ToString();
        var decomposedFirst = first.Normalize(System.Text.NormalizationForm.FormD); // Decomposition
        string tail = normalInput[1..];
        var tailVariations = GenerateDenormalizedStrings(tail);
        if (first == decomposedFirst)
        {
            return tailVariations.Select(s => first + s);
        }
        else
        {
            var result = tailVariations.Select(s => first + s).Concat(tailVariations.Select(s => decomposedFirst + s));
            return result;
        }
    }
}
