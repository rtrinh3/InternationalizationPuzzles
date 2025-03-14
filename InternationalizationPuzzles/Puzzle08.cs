namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/8/
// Day 8: Unicode passwords redux
public class Puzzle08(string input) : IPuzzle
{
    public string Solve()
    {
        int answer = input.TrimEnd().ReplaceLineEndings("\n").Split('\n').Count(IsValidPassword);
        return answer.ToString();
    }

    private static bool IsValidPassword(string password)
    {
        string normalized = password.Normalize(System.Text.NormalizationForm.FormD); // Decomposition
        return password.Length >= 4
            && password.Length <= 12
            && password.Any(char.IsDigit)
            && normalized.Intersect("aeiouAEIOU").Any()
            && normalized.Intersect("bcdfghjklmnpqrstvwxyzBCDFGHJKLMNPQRSTVWXYZ").Any()
            && normalized.Count(char.IsAscii) == normalized.Where(char.IsAscii).Select(char.ToLower).Distinct().Count();
    }
}
