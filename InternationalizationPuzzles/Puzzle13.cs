using System.Text;
using System.Text.RegularExpressions;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/13/
// Puzzle 13: Gulliver's puzzle dictionary
public class Puzzle13(string input) : IPuzzle
{
    public string Solve()
    {
        // Parse
        var paragraphs = input.TrimEnd().ReplaceLineEndings("\n").Split("\n\n");
        var dictionaryRaw = paragraphs[0].Split('\n');
        var searches = paragraphs[1].Split('\n').Select(line => new Regex("^" + line.Trim() + "$")).ToArray();

        // Build dictionary
        Encoding[] encodings =
        {
            Encoding.UTF8,
            Encoding.Latin1,
            Encoding.Unicode,
            Encoding.BigEndianUnicode
        };
        string[] dictionary = dictionaryRaw.Select(line =>
        {
            var bytes = Convert.FromHexString(line);
            foreach (var encoding in encodings)
            {
                string s = encoding.GetString(bytes);
                if (s.StartsWith('\xfeff'))
                {
                    s = s[1..];
                }
                if (s.All(char.IsLower))
                {
                    return s;
                }
            }
            throw new Exception("No encoding found");
        }).ToArray();

        // Search
        int answer = 0;
        foreach (var search in searches)
        {
            for (int wordIndex = 0; wordIndex < dictionary.Length; wordIndex++)
            {
                string word = dictionary[wordIndex];
                if (search.IsMatch(word))
                {
                    answer += wordIndex + 1;
                    break;
                }
            }
        }

        return answer.ToString();
    }
}
