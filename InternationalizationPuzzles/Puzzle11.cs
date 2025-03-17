using System.Text;
using System.Text.RegularExpressions;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/11/
// Puzzle 11: Homer's cipher
public class Puzzle11(string input) : IPuzzle
{
    public string Solve()
    {
        var lines = input.TrimEnd().ReplaceLineEndings("\n").Split('\n');
        int answer = 0;
        foreach (var line in lines)
        {
            for (int shift = 0; shift < 24; shift++)
            {
                var decrypted = Decrypt(line, shift);
                if (Regex.IsMatch(decrypted, @"Οδυσσε(υς|ως|ι|α|υ)"))
                {
                    answer += shift;
                    break;
                }
            }
        }
        return answer.ToString();
    }

    private static string Decrypt(string input, int shift)
    {
        string upper = "ΑΒΓΔΕΖΗΘΙΚΛΜΝΞΟΠΡΣΤΥΦΧΨΩ";
        string lower = "αβγδεζηθικλμνξοπρστυφχψω";
        string replaceSigma = input.Replace('ς', 'σ');
        StringBuilder answerBuffer = new(replaceSigma.Length);
        foreach (char c in replaceSigma)
        {
            if (char.IsLetter(c))
            {
                string source = char.IsUpper(c) ? upper : lower;
                int index = source.IndexOf(c);
                int newIndex = (index + shift) % source.Length;
                if (newIndex < 0)
                {
                    newIndex += source.Length;
                }
                answerBuffer.Append(source[newIndex]);
            }
            else
            {
                answerBuffer.Append(c);
            }
        }
        string answerRaw = answerBuffer.ToString();
        string answer = Regex.Replace(answerRaw, @"σ(?![α-ω])", "ς");
        return answer;
    }
}
