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
        var timestampLocationGroups = lines.Select(line =>
        {
            var parts = line.Split(';', StringSplitOptions.TrimEntries);
            var date = DateTime.ParseExact(parts[0], @"yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            var place = parts[1];
            return (place, date);
        }).GroupBy(x => x.place, x => x.date);
        var timestampsByLocation = new List<(TimeZoneInfo, DateTime[])>();
        foreach (var group in timestampLocationGroups)
        {
            if (TimeZoneInfo.TryFindSystemTimeZoneById(group.Key, out var zone))
            {
                var timestamps = group.ToArray();
                timestampsByLocation.Add((zone, timestamps));
            }
            else
            {
                Console.WriteLine("Timezone not found: " + group.Key);
            }
        }
        // Prepare variant time zones
        DateTime ruleStart = new(2018, 1, 1);
        DateTime ruleEnd = new(2025, 1, 1);
        TimeZoneInfo[][] tzsByZone = timestampsByLocation.Select(group =>
        {
            var allRules = group.Item1.GetAdjustmentRules();
            var rules = allRules.Where(r => r.DateStart <= ruleEnd && r.DateEnd >= ruleStart).ToArray();
            var newZonePerRule = rules.Select(r =>
            {
                var newRule = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(
                    DateTime.MinValue,
                    DateTime.MaxValue,
                    r.DaylightDelta,
                    r.DaylightTransitionStart,
                    r.DaylightTransitionEnd,
                    r.BaseUtcOffsetDelta
                );
                string suffix = "--" + r.DateStart.ToString(@"yyyy-MM-dd");
                var newTimeZone = TimeZoneInfo.CreateCustomTimeZone(
                    group.Item1.Id + suffix,
                    group.Item1.BaseUtcOffset,
                    group.Item1.DisplayName + suffix,
                    group.Item1.StandardName + suffix,
                    group.Item1.DaylightName + suffix,
                    [newRule]);
                return newTimeZone;
            }).ToList();
            if (group.Item1.Id == "Antarctica/Casey")
            {
                newZonePerRule.Add(TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT-11"));
                newZonePerRule.Add(TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT-8"));
            }
            else if (rules.Length == 0)
            {
                newZonePerRule.Add(group.Item1);
            }
            return newZonePerRule.ToArray();
        }).ToArray();
        // Search
        var stack = new Stack<(ImmutableHashSet<DateTime> Dates, ImmutableList<TimeZoneInfo> SelectedZones, int Index)>();
        stack.Push(([], [], 0));
        //var answers = new Dictionary<string, int>();
        string answer = "";
        int maxZonesUsed = 0;
        while (stack.TryPop(out var state))
        {
            var (dates, selectedZones, currentZoneIndex) = state;
            if (currentZoneIndex >= timestampsByLocation.Count)
            {
                if (dates.Count != 1)
                {
                    Console.WriteLine("Irregular solution");
                }
                else
                {
                    var dateString = dates.Single().ToString(@"yyyy-MM-ddTHH:mm:ss+00:00");
                    if (selectedZones.Count > maxZonesUsed)
                    {
                        answer = dateString;
                        maxZonesUsed = selectedZones.Count;
                    }
                }
            }
            else
            {
                // Try not using this zone
                stack.Push((dates, selectedZones, currentZoneIndex + 1));
                // Use this zone
                var newTimestamps = timestampsByLocation[currentZoneIndex].Item2;
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
                        stack.Push((newDates, newZones, currentZoneIndex + 1));
                    }
                }
            }
        }
        return answer;
    }
}
