using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/17/
// Puzzle 17: ╳ marks the spot
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

    private record Coord(int Row, int Col);

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
        //var timerTask = Task.Run(TimerCallback);
        //var drawTask = Task.Run(DrawCallback);

        int totalRows = mapChunks.Sum(chunk => chunk.Length);
        Console.WriteLine($"Chunks: {mapChunks.Length}, Total rows: {totalRows}");
        var distinctLengths = mapChunks.SelectMany(chunk => chunk.Select(row => row.Bytes.Length)).Distinct().ToArray();
        Debug.Assert(distinctLengths.Length == 1);
        int chunkRowLength = distinctLengths[0];

        byte[] topLeftCornerBytes = Encoding.UTF8.GetBytes("╔");
        var topLeftCornerChunk = mapChunks.Single(chunk => chunk[0].Bytes.Take(topLeftCornerBytes.Length).SequenceEqual(topLeftCornerBytes));
        byte[] bottomLeftCornerBytes = Encoding.UTF8.GetBytes("╚");
        var bottomLeftCornerChunk = mapChunks.Single(chunk => chunk[^1].Bytes.Take(bottomLeftCornerBytes.Length).SequenceEqual(bottomLeftCornerBytes));
        byte[] topRightCornerBytes = Encoding.UTF8.GetBytes("╗");
        var topRightCornerChunk = mapChunks.Single(chunk => chunk[0].Bytes.TakeLast(topRightCornerBytes.Length).SequenceEqual(topRightCornerBytes));
        byte[] bottomRightCornerBytes = Encoding.UTF8.GetBytes("╝");
        var bottomRightCornerChunk = mapChunks.Single(chunk => chunk[^1].Bytes.TakeLast(bottomRightCornerBytes.Length).SequenceEqual(bottomRightCornerBytes));

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
            if (maxCol >= columns)
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
            //var maxRowByCol = currentMap.Keys.GroupBy(x => x.Col).ToDictionary(g => g.Key, g => g.Max(x => x.Row) + 1);
            var highestRow = currentMap.Keys.Max(coord => coord.Row);
            if (highestRow >= rows)
            {
                return null;
            }
            Coord? blankCoord = null;
            for (int mapRow = 0; mapRow < rows && blankCoord is null; mapRow++)
            {
                for (int col = 0; col < columns && blankCoord is null; col++)
                {
                    if (!currentMap.ContainsKey(new(mapRow, col)))
                    {
                        blankCoord = new(mapRow, col);
                    }
                }
            }
            Debug.Assert(blankCoord is not null);
            foreach (int chunkIndex in availableChumks)
            {
                var chunk = mapChunks[chunkIndex];
                var newAvailableChunks = availableChumks.Remove(chunkIndex);
                if (blankCoord.Col == columns - 1 && !rightChunks.Contains(chunk))
                {
                    continue;
                }
                Dictionary<Coord, ChunkRow> newMap = new(currentMap);
                for (int row = 0; row < chunk.Length; row++)
                {
                    var coord = new Coord(blankCoord.Row + row, blankCoord.Col);
                    if (currentMap.ContainsKey(coord))
                    {
                        goto NEXT_CHUNK_CANDIDATE;
                    }
                    if (chunk[row].StartsWithContinuationBytes > 0)
                    {
                        if (coord.Col == 0)
                        {
                            goto NEXT_CHUNK_CANDIDATE;
                        }
                        var leftCoord = new Coord(coord.Row, coord.Col - 1);
                        if (currentMap.TryGetValue(leftCoord, out var leftChunk) && leftChunk.MissingContinuationBytes != chunk[row].StartsWithContinuationBytes)
                        {
                            goto NEXT_CHUNK_CANDIDATE;
                        }
                    }
                    if (chunk[row].MissingContinuationBytes > 0)
                    {
                        var rightCoord = new Coord(coord.Row, coord.Col + 1);
                        if (currentMap.TryGetValue(rightCoord, out var rightChunk) && rightChunk.StartsWithContinuationBytes != chunk[row].MissingContinuationBytes)
                        {
                            goto NEXT_CHUNK_CANDIDATE;
                        }
                    }
                    newMap.Add(coord, chunk[row]);
                }
                var result = BuildMap(newMap, newAvailableChunks);
                if (result is not null)
                {
                    return result;
                }
            NEXT_CHUNK_CANDIDATE:
                ;
            }
            //for (int col = 0; col <= Math.Min(columns - 1, maxCol + 1); col++)
            //{
            //    int blankRowInCol = 0;
            //    while (currentMap.ContainsKey(new(blankRowInCol, col)))
            //    {
            //        blankRowInCol++;
            //    }
            //    if (!(/*col == 0 ||*/ currentMap.ContainsKey(new(blankRowInCol, col - 1)) || currentMap.ContainsKey(new(blankRowInCol, col + 1))))
            //    {
            //        continue;
            //    }
            //    if (blankRowInCol == 0)
            //    {
            //        break;
            //    }
            //}
            return null;
        }
        ChunkRow[][] corners = [topLeftCornerChunk, topRightCornerChunk, bottomLeftCornerChunk, bottomRightCornerChunk];
        var initialAvailableChunks = Enumerable.Range(0, mapChunks.Length)
            //.Where(i => !leftChunks.Contains(mapChunks[i]))
            .Where(i => !corners.Contains(mapChunks[i]))
            .ToImmutableHashSet();
        //var leftMiddleChunks = leftChunks.ToHashSet();
        //leftMiddleChunks.Remove(topLeftCornerChunk);
        //leftMiddleChunks.Remove(bottomLeftCornerChunk);
        Dictionary<Coord, ChunkRow> initialMap = new();
        for (int row = 0; row < topLeftCornerChunk.Length; row++)
        {
            initialMap.Add(new Coord(row, 0), topLeftCornerChunk[row]);
        }
        for (int row = 0; row < topRightCornerChunk.Length; row++)
        {
            initialMap.Add(new Coord(row, columns - 1), topRightCornerChunk[row]);
        }
        for (int row = 0; row < bottomLeftCornerChunk.Length; row++)
        {
            initialMap.Add(new Coord(rows - bottomLeftCornerChunk.Length + row, 0), bottomLeftCornerChunk[row]);
        }
        for (int row = 0; row < bottomRightCornerChunk.Length; row++)
        {
            initialMap.Add(new Coord(rows - bottomRightCornerChunk.Length + row, columns - 1), bottomRightCornerChunk[row]);
        }
        var finalResult = BuildMap(initialMap, initialAvailableChunks);
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
            var resultString = new List<string>();
            for (int row = 0; row <= mapRows; row += 2)
            {
                StringBuilder sb = new();
                for (int column = 0; column <= mapCols; column++)
                {
                    if (item.TryGetValue(new Coord(row, column), out var chunkRow))
                    {
                        sb.AppendFormat("{0:x2}", chunkRow.Bytes[0]);
                    }
                    else
                    {
                        sb.Append("  ");
                    }
                }
                resultString.Add(sb.ToString());
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
