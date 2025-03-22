namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/15/
// Puzzle 15: 24/5 support
public class Puzzle15(string input) : IPuzzle
{
    private record Office(string Name, TimeZoneInfo TimeZone, DateOnly[] Holidays);

    private static TimeZoneInfo MakeTrollTimeZone()
    {
        // https://en.wikipedia.org/wiki/Time_in_Norway
        var norwayTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");
        var norwayRules = norwayTimeZone.GetAdjustmentRules();
        var trollRules = norwayRules.Select(rule => TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(rule.DateStart, rule.DateEnd, TimeSpan.FromHours(+2), rule.DaylightTransitionStart, rule.DaylightTransitionEnd, rule.BaseUtcOffsetDelta)).ToArray();
        var trollTimeZone = TimeZoneInfo.CreateCustomTimeZone("Antarctica/Troll", TimeSpan.Zero, "Antarctica/Troll", "Antarctica/Troll Standard", "Antarctica/Troll Daylight", trollRules);
        return trollTimeZone;
    }

    private static Office ParseOffice(string officeInput)
    {
        var parts = officeInput.Split('\t');
        var name = parts[0];
        if (!TimeZoneInfo.TryFindSystemTimeZoneById(parts[1], out var timeZone))
        {
            Console.WriteLine("Timezone not found: " + parts[1] + "; fallback...");
            timeZone = parts[1] switch
            {
                "America/Ciudad_Juarez" => TimeZoneInfo.FindSystemTimeZoneById("America/Denver"),
                "Antarctica/Troll" => MakeTrollTimeZone(),
                "Antarctica/Vostok" => TimeZoneInfo.FindSystemTimeZoneById("Etc/GMT-5"),
                _ => throw new Exception("Unknown time zone: " + parts[1]),
            };
        }
        var holidays = parts[2].Split(';').Select(DateOnly.Parse).ToArray();
        return new Office(name, timeZone, holidays);
    }

    public string Solve()
    {
        // Parse
        string[] paragraphs = input.TrimEnd().ReplaceLineEndings("\n").Split("\n\n");
        Office[] supportOffices = paragraphs[0].Split('\n').Select(ParseOffice).ToArray();
        Office[] clients = paragraphs[1].Split('\n').Select(ParseOffice).ToArray();

        DateTime yearStart = new(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime yearEnd = new(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeOnly officeStart = new(8, 30);
        TimeOnly officeEnd = new(17, 0);
        int[] overTimeMinutesPerClient = clients.AsParallel().Select(client =>
        {
            int overTimeMinutes = 0;
            for (DateTime dt = yearStart; dt < yearEnd; dt = dt.AddMinutes(1))
            {
                var clientDate = TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Utc, client.TimeZone);
                if (clientDate.DayOfWeek == DayOfWeek.Saturday || clientDate.DayOfWeek == DayOfWeek.Sunday || client.Holidays.Contains(DateOnly.FromDateTime(clientDate)))
                {
                    continue;
                }
                foreach (var supportOffice in supportOffices)
                {
                    DateTime supportDate = TimeZoneInfo.ConvertTime(dt, TimeZoneInfo.Utc, supportOffice.TimeZone);
                    if (supportDate.DayOfWeek == DayOfWeek.Saturday || supportDate.DayOfWeek == DayOfWeek.Sunday || supportOffice.Holidays.Contains(DateOnly.FromDateTime(supportDate)))
                    {
                        continue;
                    }
                    var supportTime = TimeOnly.FromDateTime(supportDate);
                    if (supportTime.IsBetween(officeStart, officeEnd))
                    {
                        goto FOUND_OFFICE;
                    }
                }
                overTimeMinutes++;
            FOUND_OFFICE:
                ;
            }
            return overTimeMinutes;
        }).ToArray();

        var maxOverTimeMinutes = overTimeMinutesPerClient.Max();
        var minOverTimeMinutes = overTimeMinutesPerClient.Min();

        var answer = maxOverTimeMinutes - minOverTimeMinutes;
        return answer.ToString();
    }
}
