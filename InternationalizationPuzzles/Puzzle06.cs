using System.Text;
using System.Text.RegularExpressions;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/6/
// Puzzle 6: Mojibake puzzle dictionary
public class Puzzle06(string input) : IPuzzle
{
    public string Solve()
    {
        var paragraphs = input.TrimEnd().ReplaceLineEndings("\n").Split("\n\n");
        var mangledDictionary = paragraphs[0].Split('\n');
        string[] dictionary = new string[mangledDictionary.Length];
        for (int i = 0; i < mangledDictionary.Length; i++)
        {
            string entry = mangledDictionary[i];
            if (i % 3 == 2)
            {
                entry = Unmangle(entry);
            }
            if (i % 5 == 4)
            {
                entry = Unmangle(entry);
            }
            dictionary[i] = entry;
        }
        var searches = paragraphs[1].Split('\n').Select(s => "^" + s.Trim() + "$").ToArray();

        int answer = 0;
        foreach (var search in searches)
        {
            for (int i = 0; i < dictionary.Length; i++)
            {
                if (Regex.IsMatch(dictionary[i], search))
                {
                    answer += i + 1;
                    break;
                }
            }
        }
        return answer.ToString();
    }

    private static string Unmangle(string mangled)
    {
        var latinBytes = Encoding.Latin1.GetBytes(mangled);
        var unicodeString = Encoding.UTF8.GetString(latinBytes);
        return unicodeString;
    }
}
