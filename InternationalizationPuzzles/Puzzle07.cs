namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/7/
// Day 7: The audit trail fixer
public class Puzzle07(string input) : IPuzzle
{
    public string Solve()
    {
        long answer = 0;
        var lines = input.TrimEnd().ReplaceLineEndings("\n").Split('\n');
        for (int i = 0; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] parts = line.Split('\t');
            DateTimeOffset dt = DateTimeOffset.Parse(parts[0]);
            long incorrect = long.Parse(parts[1]);
            long correct = long.Parse(parts[2]);
            var corrected = CalculateTime(dt, incorrect, correct);
            answer += (i + 1) * corrected.Hour;
        }
        return answer.ToString();
    }

    private static readonly TimeZoneInfo halifax = TimeZoneInfo.FindSystemTimeZoneById("America/Halifax");
    private static readonly TimeZoneInfo santiago = TimeZoneInfo.FindSystemTimeZoneById("America/Santiago");

    private static DateTimeOffset CalculateTime(DateTimeOffset dto, long incorrectLog, long correctLog)
    {
        DateTime dt = dto.DateTime;
        var correctZone = halifax.GetUtcOffset(dt) == dto.Offset ? halifax : santiago;
        var correctedTime = dto.AddMinutes(incorrectLog - correctLog);
        var localCorrectedTime = TimeZoneInfo.ConvertTime(correctedTime, correctZone);
        return localCorrectedTime;
    }
}
