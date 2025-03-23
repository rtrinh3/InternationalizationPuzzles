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
        int totalRows = mapChunks.Sum(chunk => chunk.Length);
        Console.WriteLine($"Chunks: {mapChunks.Length}, Total rows: {totalRows}");

        var distinctLengths = mapChunks.SelectMany(chunk => chunk.Select(row => row.Bytes.Length)).Distinct().ToArray();
        Debug.Assert(distinctLengths.Length == 1);
        int chunkRowLength = distinctLengths[0];

        byte[] topLeftCornerBytes = Encoding.UTF8.GetBytes("╔");
        var topLeftCornerChunk = mapChunks.Single(chunk => chunk[0].Bytes.AsSpan(0, 3).SequenceEqual(topLeftCornerBytes));

        // data structure: map[(row,column)] = chunk
        var initialMapBuilder = ImmutableDictionary.CreateBuilder<Coord, ChunkRow>();
        for (int row = 0; row < topLeftCornerChunk.Length; row++)
        {
            initialMapBuilder.Add(new Coord(row, 0), topLeftCornerChunk[row]);
        }
        var initialMap = initialMapBuilder.ToImmutable();
        var initialAvailableChunks = Enumerable.Range(0, mapChunks.Length)
            .ToImmutableHashSet()
            .Remove(Array.IndexOf(mapChunks, topLeftCornerChunk));
        ImmutableDictionary<Coord, ChunkRow>? BuildMap(ImmutableDictionary<Coord, ChunkRow> currentMap, ImmutableHashSet<int> availableChumks)
        {
            int maxCol = currentMap.Keys.Max(coord => coord.Col);
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
            int[] maxRowByCol = new int[maxCol + 2];
            for (int col = 0; col <= maxCol; col++)
            {
                maxRowByCol[col] = currentMap.Keys.Where(x => x.Col == col).Max(x => x.Row) + 1;
            }
            Debug.Assert(maxRowByCol[^1] == 0);
            foreach (int chunkIndex in availableChumks)
            {
                var chunk = mapChunks[chunkIndex];
                var newAvailableChunks = availableChumks.Remove(chunkIndex);
                for (int col = 0; col <= maxCol + 1; col++)
                {
                    int maxRow = maxRowByCol[col];
                    var newMapBuilder = currentMap.ToBuilder();
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
                        newMapBuilder.Add(coord, chunk[row]);
                    }
                    var newMap = newMapBuilder.ToImmutable();
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
        var result = BuildMap(initialMap, initialAvailableChunks);
        Debug.Assert(result is not null);
        int maxRow = result.Keys.Max(coord => coord.Row);
        int maxColumn = result.Keys.Max(coord => coord.Col);
        var resultString = new string[maxRow + 1];
        for (int row = 0; row <= maxRow; row++)
        {
            List<byte> rowBytes = new();
            for (int column = 0; column <= maxColumn; column++)
            {
                var chunkRow = result[new Coord(row, column)];
                rowBytes.AddRange(chunkRow.Bytes);
            }
            var str = Encoding.UTF8.GetString(rowBytes.ToArray());
            resultString[row] = str;
        }
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
