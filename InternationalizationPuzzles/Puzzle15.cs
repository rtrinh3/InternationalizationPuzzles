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

        // Simulate
        DateTime yearStart = new(2022, 1, 1, 0, 0, 0);
        DateTime yearEve = yearStart.AddDays(-1);
        DateTime yearEnd = new(2023, 1, 1, 0, 0, 0);
        TimeOnly officeStart = new(8, 30);
        TimeOnly officeEnd = new(17, 0);
        int[] overTimeMinutesPerClient = clients.Select(client =>
        {
            int overTimeMinutes = 0;
            bool simulationActive = false;
            bool clientWorkingDay = false;
            int openSupportOffices = 0;
            List<(DateTime dateTime, Action action)> queue = new();
            // Simulation bounds
            queue.Add((yearStart, () => simulationActive = true));
            queue.Add((yearEnd, () => simulationActive = false));
            // Client days
            for (DateTime localDateTime = yearEve; localDateTime < yearEnd; localDateTime = localDateTime.AddDays(1))
            {                
                bool isBusinessDay = !(localDateTime.DayOfWeek == DayOfWeek.Saturday || localDateTime.DayOfWeek == DayOfWeek.Sunday || client.Holidays.Contains(DateOnly.FromDateTime(localDateTime)));
                DateTime midnight = localDateTime.Date;
                while (client.TimeZone.IsInvalidTime(midnight))
                {
                    midnight = midnight.AddMinutes(1);
                }
                DateTime midnightInUtc = TimeZoneInfo.ConvertTimeToUtc(midnight, client.TimeZone);
                queue.Add((midnightInUtc, () => clientWorkingDay = isBusinessDay));
            }
            // Support office days
            foreach (var office in supportOffices)
            {
                for (DateTime localDate = yearEve; localDate < yearEnd; localDate = localDate.AddDays(1))
                {
                    bool isBusinessDay = !(localDate.DayOfWeek == DayOfWeek.Saturday || localDate.DayOfWeek == DayOfWeek.Sunday || office.Holidays.Contains(DateOnly.FromDateTime(localDate)));
                    if (isBusinessDay)
                    {
                        var openingLocalDateTime = localDate.Add(officeStart.ToTimeSpan());
                        var openingUtcDateTime = TimeZoneInfo.ConvertTimeToUtc(openingLocalDateTime, office.TimeZone);
                        queue.Add((openingUtcDateTime, () => openSupportOffices++));
                        var closingLocalDateTime = localDate.Add(officeEnd.ToTimeSpan());
                        var closingUtcDateTime = TimeZoneInfo.ConvertTimeToUtc(closingLocalDateTime, office.TimeZone);
                        queue.Add((closingUtcDateTime, () => openSupportOffices--));
                    }
                }
            }
            // Process queue
            queue.Sort((a, b) => a.dateTime.CompareTo(b.dateTime));
            for (int i = 0; i < queue.Count; i++)
            {
                queue[i].action();
                if (simulationActive && clientWorkingDay && openSupportOffices <= 0)
                {
                    int minutes = (int)(queue[i + 1].dateTime - queue[i].dateTime).TotalMinutes;
                    overTimeMinutes += minutes;
                }
            }
            return overTimeMinutes;
        }).ToArray();

        var maxOverTimeMinutes = overTimeMinutesPerClient.Max();
        var minOverTimeMinutes = overTimeMinutesPerClient.Min();

        var answer = maxOverTimeMinutes - minOverTimeMinutes;
        return answer.ToString();
    }
}
