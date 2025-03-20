namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/14/
// Puzzle 14: Metrification in Japan
public class Puzzle14(string input) : IPuzzle
{
    public string Solve()
    {
        var lines = input.TrimEnd().ReplaceLineEndings("\n").Split('\n');
        decimal totalAreaSquareShaku = 0;
        foreach (var line in lines)
        {
            var parts = line.Split('×', StringSplitOptions.TrimEntries);
            var measurements = parts.Select(ParseMeasurement);
            var area = measurements.Aggregate((a, b) => a * b);
            totalAreaSquareShaku += area;
        }
        decimal answerDecimal = totalAreaSquareShaku * 10 * 10 / 33 / 33;
        long answer = (long)answerDecimal;
        return answer.ToString();
    }

    private decimal ParseMeasurement(string measurement)
    {
        var unit = measurement[^1];
        decimal multiplier = unit switch
        {
            '尺' => 1,
            '間' => 6,
            '丈' => 10,
            '町' => 360,
            '里' => 12960,
            '毛' => 0.0001m,
            '厘' => 0.001m,
            '分' => 0.01m,
            '寸' => 0.1m,
            _ => throw new Exception($"Unknown unit: " + unit)
        };
        var number = ParseNumber(measurement[..^1]);
        var result = number * multiplier;
        return result;
    }

    private decimal ParseNumber(string number)
    {
        decimal result = 0;
        decimal accumulator = 0;
        int multiplier = 0;
        foreach (char c in number)
        {
            if (c == '一')
            {
                multiplier = 1;
            }
            else if (c == '二')
            {
                multiplier = 2;
            }
            else if (c == '三')
            {
                multiplier = 3;
            }
            else if (c == '四')
            {
                multiplier = 4;
            }
            else if (c == '五')
            {
                multiplier = 5;
            }
            else if (c == '六')
            {
                multiplier = 6;
            }
            else if (c == '七')
            {
                multiplier = 7;
            }
            else if (c == '八')
            {
                multiplier = 8;
            }
            else if (c == '九')
            {
                multiplier = 9;
            }
            else if (c == '十')
            {
                accumulator += Math.Max(1, multiplier) * 10;
                multiplier = 0;
            }
            else if (c == '百')
            {
                accumulator += Math.Max(1, multiplier) * 100;
                multiplier = 0;
            }
            else if (c == '千')
            {
                accumulator += Math.Max(1, multiplier) * 1000;
                multiplier = 0;
            }
            else if (c == '万')
            {
                result += Math.Max(1, accumulator + multiplier) * 10000;
                accumulator = 0;
                multiplier = 0;
            }
            else if (c == '億')
            {
                result += Math.Max(1, accumulator + multiplier) * 10000_0000;
                accumulator = 0;
                multiplier = 0;
            }
            else
            {
                throw new Exception("Unknown number: " + c);
            }
        }
        result += accumulator + multiplier;
        return result;
    }
}
