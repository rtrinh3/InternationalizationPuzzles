using System.Collections.Concurrent;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/10/
// Puzzle 10: Unicode passwords strike back!
public class Puzzle10 : IPuzzle
{
    private readonly Dictionary<string, string> authEntries;
    private readonly (string Name, string Pass)[] loginAttempts;
    private readonly ConcurrentDictionary<string, string> confirmedPasswords;

    public Puzzle10(string input)
    {
        var paragraphs = input.TrimEnd().ReplaceLineEndings("\n").Split("\n\n");
        authEntries = paragraphs[0].Split('\n')
            .Select(line => line.Split(' ', 2))
            .ToDictionary(x => x[0], x => x[1]);
        loginAttempts = paragraphs[1].Split('\n')
            .Select(line => line.Split(' ', 2))
            .Select(x => (x[0], x[1]))
            .ToArray();
        confirmedPasswords = new();
    }

    public string Solve()
    {
        int validAttempts = 0;
        Parallel.ForEach(loginAttempts, attempt =>
        {
            if (CheckPassword(attempt.Name, attempt.Pass))
            {
                Interlocked.Increment(ref validAttempts);
            }
        });
        return validAttempts.ToString();
    }

    private bool CheckPassword(string name, string pass)
    {
        string normalizedPass = pass.Normalize(System.Text.NormalizationForm.FormC); // Composition
        if (confirmedPasswords.TryGetValue(name, out var confirmedPass))
        {
            return confirmedPass == normalizedPass;
        }
        var variations = GenerateDenormalizedStrings(normalizedPass);
        var wanted = authEntries[name];
        foreach (string variation in variations)
        {
            if (BCrypt.Net.BCrypt.Verify(variation, wanted))
            {
                confirmedPasswords[name] = normalizedPass;
                return true;
            }
        }
        return false;
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
