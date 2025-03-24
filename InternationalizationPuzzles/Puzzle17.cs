using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/17/
// Puzzle 17
public class Puzzle17 : IPuzzle
{
    private record ChunkRow(byte[] Bytes, int StartsWithContinuationBytes, int MissingContinuationBytes)
    {
        public static ChunkRow FromBytes(byte[] bytes)
        {
            int startsWithContinuationBytes = bytes.TakeWhile(IsUtf8ContinuationByte).Count();
            int missingContinuationBytes = 0;
            for (int fromEnd = 1; fromEnd <= 4; fromEnd++)
            {
                byte currentByte = bytes[^fromEnd];
                if (!IsUtf8ContinuationByte(currentByte) && !IsUtf8LeadByte(currentByte))
                {
                    break;
                }
                if (IsUtf8LeadByte(currentByte) && ExpectedBytes(currentByte) > fromEnd)
                {
                    missingContinuationBytes = ExpectedBytes(currentByte) - fromEnd;
                    break;
                }
            }
            return new ChunkRow(bytes, startsWithContinuationBytes, missingContinuationBytes);
        }
    }

    private readonly record struct Coord(int Row, int Col);

    private readonly ChunkRow[][] mapChunks; // mapChunks[chunk][row].Bytes[column]
    private bool solved = false;
    private readonly ConcurrentQueue<bool> timerQueue = new();
    private readonly BlockingCollection<Dictionary<Coord, ChunkRow>?> drawQueue = new();

    public Puzzle17(string input)
    {
        string[] paragraphs = input.TrimEnd().ReplaceLineEndings("\n").Split("\n\n");
        mapChunks = paragraphs.Select(line => line.Split("\n"))
        .Select(lines =>
        {
            return lines.Select(line =>
            {
                var bytes = Convert.FromHexString(line);
                return ChunkRow.FromBytes(bytes);
            })
            .ToArray();
        })
        .ToArray();

        // Debug
        //foreach (var chunk in mapChunks)
        //{
        //    foreach (var row in chunk)
        //    {
        //        string decoded = Encoding.UTF8.GetString(row);
        //        Console.WriteLine(decoded);
        //    }
        //    Console.WriteLine();
        //}
    }

    public string Solve()
    {
        var timerTask = Task.Run(TimerCallback);
        var drawTask = Task.Run(DrawCallback);

        int totalRows = mapChunks.Sum(chunk => chunk.Length);
        Console.WriteLine($"Chunks: {mapChunks.Length}, Total rows: {totalRows}");
        var distinctLengths = mapChunks.SelectMany(chunk => chunk.Select(row => row.Bytes.Length)).Distinct().ToArray();
        Debug.Assert(distinctLengths.Length == 1);
        int chunkRowLength = distinctLengths[0];
        byte[] topLeftCornerBytes = Encoding.UTF8.GetBytes("╔");
        var topLeftCornerChunk = mapChunks.Single(chunk => chunk[0].Bytes.Take(3).SequenceEqual(topLeftCornerBytes));
        byte[] bottomLeftCornerBytes = Encoding.UTF8.GetBytes("╚");
        var bottomLeftCornerChunk = mapChunks.Single(chunk => chunk[^1].Bytes.Take(3).SequenceEqual(bottomLeftCornerBytes));
        var leftChunks = mapChunks.Where(chunk => chunk.All(row => row.StartsWithContinuationBytes == 0)).ToArray();
        Debug.Assert(leftChunks.Contains(topLeftCornerChunk) && leftChunks.Contains(bottomLeftCornerChunk));
        int leftRows = leftChunks.Sum(chunk => chunk.Length);
        var rightChunks = mapChunks.Where(chunk => chunk.All(row => row.MissingContinuationBytes == 0)).ToArray();
        int rightRows = rightChunks.Sum(chunk => chunk.Length);
        Debug.Assert(leftRows == rightRows);
        int rows = leftRows;
        Debug.Assert(totalRows % rows == 0);
        int columns = totalRows / rows;

        // data structure: map[(row,column)] = chunk
        Dictionary<Coord, ChunkRow>? BuildMap(Dictionary<Coord, ChunkRow> currentMap, ImmutableHashSet<int> availableChumks)
        {
            if (timerQueue.TryDequeue(out _))
            {
                drawQueue.Add(currentMap);
            }
            int maxCol = currentMap.Keys.Max(coord => coord.Col);
            if (maxCol > columns)
            {
                return null;
            }
            if (availableChumks.Count == 0)
            {
                //Console.WriteLine("All chunks used");
                // Check no unterminated code points
                foreach (var kvp in currentMap)
                {
                    if (kvp.Value.MissingContinuationBytes > 0)
                    {
                        var rightCoord = new Coord(kvp.Key.Row, kvp.Key.Col + 1);
                        if (!currentMap.ContainsKey(rightCoord))
                        {
                            return null;
                        }
                    }
                }
                return currentMap;
            }
            var maxRowByCol = currentMap.Keys.GroupBy(x => x.Col).ToDictionary(g => g.Key, g => g.Max(x => x.Row) + 1);
            var highestRow = maxRowByCol.Values.Max();
            if (highestRow > rows)
            {
                return null;
            }
            foreach (int chunkIndex in availableChumks)
            {
                if (solved)
                {
                    return null;
                }
                var chunk = mapChunks[chunkIndex];
                var newAvailableChunks = availableChumks.Remove(chunkIndex);
                for (int col = Math.Min(columns, maxCol + 1); col >= 0; col--)
                {
                    int maxRow = maxRowByCol.GetValueOrDefault(col);
                    if (!(col == 0 || currentMap.ContainsKey(new(maxRow, col - 1)) || currentMap.ContainsKey(new(maxRow, col + 1))))
                    {
                        continue;
                    }
                    if (col == columns && !rightChunks.Contains(chunk))
                    {
                        continue;
                    }
                    Dictionary<Coord, ChunkRow> newMap = new(currentMap);
                    for (int row = 0; row < chunk.Length; row++)
                    {
                        var coord = new Coord(maxRow + row, col);
                        if (currentMap.ContainsKey(coord))
                        {
                            goto CONTINUE_COL_LOOP;
                        }
                        if (chunk[row].StartsWithContinuationBytes > 0)
                        {
                            if (col == 0)
                            {
                                goto CONTINUE_COL_LOOP;
                            }
                            var leftCoord = new Coord(coord.Row, coord.Col - 1);
                            if (currentMap.TryGetValue(leftCoord, out var leftChunk) && leftChunk.MissingContinuationBytes != chunk[row].StartsWithContinuationBytes)
                            {
                                goto CONTINUE_COL_LOOP;
                            }
                        }
                        if (chunk[row].MissingContinuationBytes > 0)
                        {
                            var rightCoord = new Coord(coord.Row, coord.Col + 1);
                            if (currentMap.TryGetValue(rightCoord, out var rightChunk) && rightChunk.StartsWithContinuationBytes != chunk[row].MissingContinuationBytes)
                            {
                                goto CONTINUE_COL_LOOP;
                            }
                        }
                        newMap.Add(coord, chunk[row]);
                    }
                    var result = BuildMap(newMap, newAvailableChunks);
                    if (result is not null)
                    {
                        return result;
                    }
                CONTINUE_COL_LOOP:
                    ;
                }
            }
            return null;
        }
        var initialAvailableChunks = Enumerable.Range(0, mapChunks.Length)
            .Where(i => !leftChunks.Contains(mapChunks[i]))
            .ToImmutableHashSet();        
        Dictionary<Coord, ChunkRow>? finalResult = null;
        var leftMiddleChunks = leftChunks.ToHashSet();
        leftMiddleChunks.Remove(topLeftCornerChunk);
        leftMiddleChunks.Remove(bottomLeftCornerChunk);
        int permutationCount = 0;
        var allPermutations = PermutationsOf(leftMiddleChunks).ToArray();
        Parallel.ForEach(allPermutations, (permutation, loopState) =>
        {
            //Console.WriteLine($"Permutation {permutationCount++}");
            Dictionary<Coord, ChunkRow> initialMap = new();
            ChunkRow[][] chunks = [topLeftCornerChunk, .. permutation, bottomLeftCornerChunk];
            ChunkRow[] initialRows = chunks.SelectMany(c => c).ToArray();
            for (int row = 0; row < initialRows.Length; row++)
            {
                initialMap.Add(new Coord(row, 0), initialRows[row]);
            }
            var result = BuildMap(initialMap, initialAvailableChunks);
            if (result is not null)
            {
                finalResult = result;
                //break;
                loopState.Stop();
            }
        });
        solved = true;
        drawQueue.Add(null);
        Debug.Assert(finalResult is not null);
        var resultString = StringifyMap(finalResult);
        //Console.WriteLine(string.Join("\n", resultString));
        int answer = 0;
        Rune wanted = Rune.GetRuneAt("╳", 0);
        for (int row = 0; row < resultString.Length; row++)
        {
            var runes = resultString[row].EnumerateRunes().ToArray();
            for (int column = 0; column < runes.Length; column++)
            {
                if (runes[column] == wanted)
                {
                    answer = row * column;
                    break;
                }
            }
        }
        return answer.ToString();
    }

    private static string[] StringifyMap(Dictionary<Coord, ChunkRow> map)
    {
        int mapRows = map.Keys.Max(coord => coord.Row);
        int mapCols = map.Keys.Max(coord => coord.Col);
        var resultString = new string[mapRows + 1];
        for (int row = 0; row <= mapRows; row++)
        {
            List<byte> rowBytes = new();
            for (int column = 0; column <= mapCols; column++)
            {
                var chunkRow = map[new Coord(row, column)];
                rowBytes.AddRange(chunkRow.Bytes);
            }
            var str = Encoding.UTF8.GetString(rowBytes.ToArray());
            resultString[row] = str;
        }
        return resultString;
    }

    private void TimerCallback()
    {
        var timer = Stopwatch.StartNew();
        while (!solved)
        {
            Thread.Sleep(1000);
            Console.WriteLine($"Time: {timer.Elapsed}");
            timerQueue.Enqueue(true);
        }
    }

    private void DrawCallback()
    {
        while (!solved)
        {
            var item = drawQueue.Take();
            if (item is null)
            {
                return;
            }
            int mapRows = item.Keys.Max(coord => coord.Row);
            int mapCols = item.Keys.Max(coord => coord.Col);
            var resultString = new string[Math.Min(mapRows + 1, 48)];
            for (int row = 0; row < resultString.Length; row++)
            {
                StringBuilder sb = new();
                for (int column = 0; column <= mapCols; column++)
                {
                    if (item.TryGetValue(new Coord(row, column), out var chunkRow))
                    {
                        sb.AppendFormat("{0:x2}", chunkRow.Bytes[0]);
                        sb.AppendFormat("{0:x2}", chunkRow.Bytes[1]);
                    }
                    else
                    {
                        sb.Append("    ");
                    }
                }
                resultString[row] = sb.ToString();
            }
            var result = string.Join("\n", resultString);
            Console.WriteLine(result);
        }
    }

    private static IEnumerable<IEnumerable<T>> PermutationsOf<T>(IEnumerable<T> list)
    {
        if (list.Any())
        {
            foreach (var element in list)
            {
                var rest = new List<T>(list);
                rest.Remove(element);
                foreach (var restPermuted in PermutationsOf(rest))
                {
                    yield return restPermuted.Prepend(element);
                }
            }
        }
        else
        {
            yield return Enumerable.Empty<T>();
        }
    }

    private static bool IsUtf8LeadByte(byte b) => b >= 0b1100_0000 && b <= 0b1111_0111;

    private static int ExpectedBytes(byte b)
    {
        if (b >= 0b1100_0000 && b <= 0b1101_1111)
        {
            return 2;
        }
        if (b >= 0b1110_0000 && b <= 0b1110_1111)
        {
            return 3;
        }
        if (b >= 0b1111_0000 && b <= 0b1111_0111)
        {
            return 4;
        }
        return 1;
    }

    private static bool IsUtf8ContinuationByte(byte b) => b >= 0b1000_0000 && b <= 0b1011_1111;
}
