﻿using InternationalizationPuzzles;
using System.Diagnostics;

string? day = args.ElementAtOrDefault(0);
string? input = args.ElementAtOrDefault(1);
#if DEBUG
const bool debug = true;
#else
const bool debug = false;
#endif
if (debug)
{
    day ??= "01";
    input ??= @"input.txt";
}
if (day == null)
{
    throw new Exception($"Missing day");
}
if (input == null)
{
    throw new Exception($"Missing input");
}
if (File.Exists(input))
{
    input = File.ReadAllText(input);
}
Func<string, IPuzzle>[] puzzles =
[
    x => new Puzzle01(x),
    x => new Puzzle02(x),
    x => new Puzzle03(x),
    x => new Puzzle04(x),
    x => new Puzzle05(x),
    x => new Puzzle06(x),
    x => new Puzzle07(x),
    x => new Puzzle08(x),
    x => new Puzzle09(x),
    x => new Puzzle10(x),
    x => new Puzzle11(x),
    x => new Puzzle12(x),
    x => new Puzzle13(x),
    x => new Puzzle14(x),
    x => new Puzzle15(x),
    x => new Puzzle16(x),
    x => new Puzzle17(x),
    x => new Puzzle18(x),
    x => new Puzzle19(x),
    x => new Puzzle20(x),
];
if (!int.TryParse(day, out int dayValue) || dayValue < 1 || dayValue > puzzles.Length)
{
    throw new Exception($"Bad day: {day}");
}

Console.WriteLine($"Loading: {day}");
var initTimer = Stopwatch.StartNew();
var solver = puzzles[dayValue - 1](input);
Console.WriteLine($"Time: {initTimer.Elapsed}");

Console.WriteLine("\nSolving");
var solveTimer = Stopwatch.StartNew();
var answer = solver.Solve();
Console.WriteLine($"Time: {solveTimer.Elapsed}");
Console.WriteLine(answer);
