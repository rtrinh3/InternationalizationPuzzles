using System.Diagnostics;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/5/
// Day 5: Don't step in it...
public class Puzzle05(string input) : IPuzzle
{
    public string Solve()
    {
        var grid = input.ReplaceLineEndings("\n").Split('\n').ToList();
        if (grid[grid.Count - 1] == "")
        {
            grid.RemoveAt(grid.Count - 1);
        }
        var widths = grid.Select(r => r.EnumerateRunes().Count());
        int width = widths.Max();
        int row = 0;
        int col = 0;
        int poopCount = 0;
        System.Text.Rune.DecodeFromUtf16("💩", out var poopRune, out int consumed);
        Debug.Assert(consumed == "💩".Length);
        while (row < grid.Count)
        {
            var rowRunes = grid[row].EnumerateRunes().ToArray();
            Debug.Assert(rowRunes.Length == width);
            if (rowRunes[col] == poopRune)
            {
                poopCount++;
            }
            row++;
            col = (col + 2) % width;
        }
        return poopCount.ToString();
    }
}
