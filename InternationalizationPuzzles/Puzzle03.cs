namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/3/
// Puzzle 3: Unicode passwords
public class Puzzle03(string input) : IPuzzle
{
    public string Solve()
    {
        int answer = input.TrimEnd().ReplaceLineEndings("\n").Split('\n').Count(IsValid);
        return answer.ToString();
    }

    private static bool IsValid(string password)
    {
        return password.Length >= 4
            && password.Length <= 12
            && password.Any(char.IsDigit)
            && password.Any(char.IsUpper)
            && password.Any(char.IsLower)
            && password.Any(c => !char.IsAscii(c));
    }
}
