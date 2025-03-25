namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/19/
public class Puzzle19(string input) : IPuzzle
{
    public string Solve()
    {
        var lines = input.Split('\n');
        HashSet<string> places = new();
        foreach (var line in lines)
        {
            var parts = line.Split(';', StringSplitOptions.TrimEntries);
            var date = DateTime.Parse(parts[0]);
            var place = parts[1];
            places.Add(place);
        }
        foreach (var place in places)
        {
            var tz = TimeZoneInfo.FindSystemTimeZoneById(place);
        }
        // TODO
        throw new NotImplementedException();
    }
}
