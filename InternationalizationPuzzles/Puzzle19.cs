using System.Collections.Immutable;
using System.Data;
using System.Globalization;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/19/
public class Puzzle19(string input) : IPuzzle
{
    public string Solve()
    {
        // Parse
        var lines = input.Split('\n');
        var timestampsByLocation = lines.Select(line =>
        {
            var parts = line.Split(';', StringSplitOptions.TrimEntries);
            var date = DateTime.ParseExact(parts[0], @"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            var place = parts[1];
            return (place, date);
        }).GroupBy(x => x.place, x => x.date)
        .ToArray();
        // Prepare variant time zones
        // https://lists.iana.org/hyperkitty/list/tz-announce@iana.org/latest
        (string, DateTime)[] tzVersions = 
        [
            ("2018c", new(2018, 1, 23)),
            ("2018g", new(2018, 10, 27)),
            ("2021b", new(2021, 9, 24)),
            ("2023d", new(2023, 12, 22)),
        ];
        TimeZoneInfo[][] tzsByZone = timestampsByLocation.Select(group => 
            tzVersions.Select(version => {
                var (versionName, versionDate) = version;
                var actualTimeZone = TimeZoneInfo.FindSystemTimeZoneById(group.Key);
                var actualRules = actualTimeZone.GetAdjustmentRules();
                var relevantRules = actualRules.Where(r => r.DateStart <= versionDate && versionDate < r.DateEnd);
                var extendedRules = relevantRules.Select(r => 
                TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                    r.DateStart, 
                    DateTime.MaxValue, 
                    r.DaylightDelta, 
                    r.DaylightTransitionStart, 
                    r.DaylightTransitionEnd, 
                    r.BaseUtcOffsetDelta)
                ).ToArray();
                string suffix = "-" + versionName;
                var newTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                    actualTimeZone.Id + suffix,
                    actualTimeZone.BaseUtcOffset, 
                    actualTimeZone.DisplayName + suffix, 
                    actualTimeZone.StandardName + suffix, 
                    actualTimeZone.DaylightName + suffix, 
                    extendedRules);
                return newTimeZone;
            }).ToArray()
        ).ToArray();
        // Search
        var stack = new Stack<(ImmutableHashSet<DateTime> Dates, ImmutableList<TimeZoneInfo> SelectedZones)>();
        stack.Push(([], []));
        while (stack.TryPop(out var state))
        {
            var (dates, selectedZones) = state;            
            int currentZoneIndex = selectedZones.Count;
            if (currentZoneIndex >= timestampsByLocation.Length)
            {
                if (dates.Count > 1)
                {
                    throw new Exception("Too many events?!");
                }
                if (dates.Count == 1)
                {
                    var answerDate = dates.Single();
                    var answer = answerDate.ToString(@"yyyy-MM-ddTHH:mm:ss+00:00");
                    return answer;
                }
            }
            else
            {
                var newTimestamps = timestampsByLocation[currentZoneIndex];
                var zoneVariants = tzsByZone[currentZoneIndex];
                foreach (var zone in zoneVariants)
                {
                    var newTimestampsUtc = newTimestamps.Select(t => TimeZoneInfo.ConvertTimeToUtc(t, zone));
                    var newDates = currentZoneIndex == 0
                    ? newTimestampsUtc.ToImmutableHashSet()
                    : dates.Intersect(newTimestampsUtc);
                    if (newDates.Count > 0)
                    {
                        var newZones = selectedZones.Add(zone);
                        stack.Push((newDates, newZones));
                    }
                }
            }
        }
        throw new Exception("No solution found");
    }
}
