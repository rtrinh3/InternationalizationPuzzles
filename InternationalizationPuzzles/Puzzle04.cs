using System.Text.RegularExpressions;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/4/
// Puzzle 4: A trip around the world
public class Puzzle04(string input) : IPuzzle
{
    public string Solve()
    {
        var normalized = input.TrimEnd().ReplaceLineEndings("\n");
        var paragraphs = normalized.Split("\n\n");
        int tripTime = 0;
        foreach (var trip in paragraphs)
        {
            DateTime start = new();
            DateTime end = new();
            foreach (var line in trip.Split('\n'))
            {
                var match = Regex.Match(line, @"^(\S+):\s+(\S+)\s+(\w.+)$");
                var location = match.Groups[2].Value;
                var timezone = TimeZoneInfo.FindSystemTimeZoneById(location);
                var timestamp = match.Groups[3].Value;
                var localTime = DateTime.ParseExact(timestamp, @"MMM dd, yyyy, HH:mm", System.Globalization.CultureInfo.InvariantCulture);
                var utcTime = TimeZoneInfo.ConvertTimeToUtc(localTime, timezone);
                if (match.Groups[1].Value == "Departure")
                {
                    start = utcTime;
                }
                else if (match.Groups[1].Value == "Arrival")
                {
                    end = utcTime;
                }
            }
            tripTime += (int)(end - start).TotalMinutes;
        }
        return tripTime.ToString();
    }
}
