namespace InternationalizationPuzzles;

public class Puzzle15 : IPuzzle
{
    private readonly string input;

    private readonly TimeZoneInfo trollTimeZone;
    private readonly TimeZoneInfo vostokTimeZone;

    public Puzzle15(string input)
    {
        this.input = input;
        var norwayTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Europe/Oslo");
        var norwayRule = norwayTimeZone.GetAdjustmentRules().Single(rule => rule.DateStart.Year <= 2022 && rule.DateEnd.Year >= 2022);
        var trollAdjustmentRules = TimeZoneInfo.AdjustmentRule.CreateAdjustmentRule(norwayRule.DateStart, norwayRule.DateEnd, TimeSpan.FromHours(+2), norwayRule.DaylightTransitionStart, norwayRule.DaylightTransitionEnd);
        this.trollTimeZone = TimeZoneInfo.CreateCustomTimeZone("Antarctica/Troll", TimeSpan.FromHours(+2), "Antarctica/Troll", "Antarctica/Troll", "Antarctica/Troll", [trollAdjustmentRules]);
        this.vostokTimeZone = TimeZoneInfo.CreateCustomTimeZone("Antarctica/Vostok", TimeSpan.FromHours(+5), "Antarctica/Vostok", "Antarctica/Vostok");
    }

    private record Office(string Name, TimeZoneInfo TimeZone, DateOnly[] Holidays);



    private Office ParseOffice(string officeInput)
    {
        var parts = officeInput.Split('\t');
        var name = parts[0];
        var timeZone = parts[1] switch
        {
            "America/Ciudad_Juarez" => TimeZoneInfo.FindSystemTimeZoneById("America/Denver"),
            "Antarctica/Troll" => trollTimeZone,
            "Antarctica/Vostok" => vostokTimeZone,
            _ => TimeZoneInfo.FindSystemTimeZoneById(parts[1])
        };
        var holidays = parts[2].Split(';').Select(DateOnly.Parse).ToArray();
        return new Office(name, timeZone, holidays);
    }

    public string Solve()
    {
        // Parse
        var paragraphs = input.TrimEnd().ReplaceLineEndings("\n").Split("\n\n");
        Office[] supportOffices = paragraphs[0].Split('\n').Select(ParseOffice).ToArray();
        Office[] clients = paragraphs[1].Split('\n').Select(ParseOffice).ToArray();

        DateTime yearStart = new(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        DateTime yearEnd = new(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        TimeOnly officeStart = new(8, 30);
        TimeOnly officeEnd = new(17, 0);
        int[] overTimeMinutesPerClient = clients.Select(client =>
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
