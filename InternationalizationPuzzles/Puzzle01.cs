namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/1/
// Length limits on messaging platforms
public class Puzzle01(string input) : IPuzzle
{
    public string Solve()
    {
        int score = 0;
        foreach (var line in input.TrimEnd().ReplaceLineEndings("\n").Split('\n'))
        {
            bool tweetLimit = line.Length <= 140;
            var bytes = System.Text.Encoding.UTF8.GetBytes(line);
            bool smsLimit = bytes.Length <= 160;
            if (tweetLimit && smsLimit)
            {
                score += 13;
            }
            else if (tweetLimit)
            {
                score += 7;
            }
            else if (smsLimit)
            {
                score += 11;
            }
        }
        return score.ToString();
    }
}
