using System.Collections.Immutable;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/19/
// Puzzle 19: Out of date
public class Puzzle19(string input) : IPuzzle
{
    public string Solve()
    {
        // Parse
        // inputDatesByPlace[place] = [list of datetime strings]
        var inputDatesByPlace = new Dictionary<string, List<string>>();
        var lines = input.TrimEnd().ReplaceLineEndings("\n").Split('\n');
        foreach (var line in lines)
        {
            var parts = line.Split(';', StringSplitOptions.TrimEntries);
            inputDatesByPlace.TryAdd(parts[1], new());
            inputDatesByPlace[parts[1]].Add(parts[0]);
        }

        // Shift
        // shifted[place][version] = [list of datetime strings]
        var shifted = new Dictionary<string, Dictionary<string, List<string>>>();
        // Credit to https://codeblog.jonskeet.uk/2019/06/30/versioning-limitations-in-net/
        // for showing how to work with multiple versions of a library
        (string, string)[] nodaTimeVersions =
        [
            ("2018c", "2.2.4"),
            ("2018g", "2.4.2"),
            ("2021b", "3.0.6"),
            ("2023d", "3.1.10"),
        ];
        foreach (var version in nodaTimeVersions)
        {
            var (tzDbVersion, nodaTimeVersion) = version;
            string nodaDllPath = Path.GetFullPath(string.Format("NodaTime.{0}.dll", nodaTimeVersion));
            var nodaTime = Assembly.LoadFile(nodaDllPath);
            dynamic zoneProvider = nodaTime.GetType("NodaTime.DateTimeZoneProviders")
                .GetProperty("Tzdb")
                .GetValue(null);
            var dateTimePatternCreator = nodaTime.GetType("NodaTime.Text.LocalDateTimePattern")
                .GetMethod("CreateWithInvariantCulture");
            dynamic inputDateTimePattern = dateTimePatternCreator.Invoke(null, ["uuuu'-'MM'-'dd' 'HH':'mm':'ss"]);
            foreach (var kvp in inputDatesByPlace)
            {
                var (place, dateTimes) = kvp;
                shifted.TryAdd(place, new());
                var placeContainer = shifted[place];
                placeContainer.TryAdd(tzDbVersion, new());
                var shiftTimes = placeContainer[tzDbVersion];
                dynamic zone = zoneProvider[place];
                foreach (var dt in dateTimes)
                {
                    dynamic dtLocalParse = inputDateTimePattern.Parse(dt);
                    dynamic dtLocal = dtLocalParse.Value;
                    dynamic dtZoned = zone.AtLeniently(dtLocal); // The puzzle is nice enough to not require handling the edge cases
                    IFormattable dtInstant = dtZoned.ToInstant();
                    string shiftedTimeStamp = dtInstant.ToString("uuuu'-'MM'-'dd'T'HH':'mm':'ss'+00:00'", CultureInfo.InvariantCulture);
                    shiftTimes.Add(shiftedTimeStamp);
                }
            }
        }

        // Find overlap
        var places = shifted.Keys.ToArray();
        var stack = new Stack<(ImmutableHashSet<string>, ImmutableList<string>)>();
        stack.Push(([], []));
        while (stack.TryPop(out var state))
        {
            var (dates, chosenVersions) = state;
            int index = chosenVersions.Count;
            if (index == places.Length)
            {
                Debug.Assert(dates.Count > 0);
                if (dates.Count > 1)
                {
                    Console.WriteLine("More than one answer");
                }
                return dates.First();
            }
            else
            {
                foreach (var kvp in shifted[places[index]])
                {
                    var (version, versionDates) = kvp;
                    var newDates = index == 0
                        ? versionDates.ToImmutableHashSet()
                        : dates.Intersect(versionDates);
                    if (newDates.Count > 0)
                    {
                        var newVersions = chosenVersions.Add(string.Format("{0} ({1})", places[index], version));
                        stack.Push((newDates, newVersions));
                    }
                }
            }
        }
        return "No answer found";
    }
}
