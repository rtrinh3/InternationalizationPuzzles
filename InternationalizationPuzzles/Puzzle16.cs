using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/16/
// Puzzle 16: 8-bit unboxing
public class Puzzle16 : IPuzzle
{
    private readonly ImmutableDictionary<Coord, Tile> initialTiles;
    private readonly int height;
    private readonly int width;

    private bool solved = false;
    private readonly ConcurrentQueue<bool> timerQueue = new();
    private readonly BlockingCollection<Tuple<ImmutableDictionary<Coord, Tile>, Coord, int>?> drawQueue = new();

    private enum Direction
    {
        Up,
        Right,
        Down,
        Left
    }

    private readonly record struct Coord(int Row, int Col)
    {
        public Coord Next(Direction d) => d switch
        {
            Direction.Up => new Coord(Row - 1, Col),
            Direction.Right => new Coord(Row, Col + 1),
            Direction.Down => new Coord(Row + 1, Col),
            Direction.Left => new Coord(Row, Col - 1),
            _ => throw new System.ArgumentException("Invalid direction", nameof(d))
        };
    }

    private readonly record struct Tile(byte Up, byte Right, byte Down, byte Left)
    {
        public static Tile Empty => new();

        public Tile Rotate() => new Tile(Left, Up, Right, Down);

        public byte Get(Direction d) => d switch
        {
            Direction.Up => Up,
            Direction.Right => Right,
            Direction.Down => Down,
            Direction.Left => Left,
            _ => throw new System.ArgumentException("Invalid direction", nameof(d))
        };

        public static Tile FromChar(char c)
        {
            // https://en.wikipedia.org/wiki/Box-drawing_characters#DOS
            return c switch
            {
                '│' => new Tile(1, 0, 1, 0),
                '┤' => new Tile(1, 0, 1, 1),
                '╡' => new Tile(1, 0, 1, 2),
                '╢' => new Tile(2, 0, 2, 1),
                '╖' => new Tile(0, 0, 2, 1),
                '╕' => new Tile(0, 0, 1, 2),
                '╣' => new Tile(2, 0, 2, 2),
                '║' => new Tile(2, 0, 2, 0),
                '╗' => new Tile(0, 0, 2, 2),
                '╝' => new Tile(2, 0, 0, 2),
                '╜' => new Tile(2, 0, 0, 1),
                '╛' => new Tile(1, 0, 0, 2),
                '┐' => new Tile(0, 0, 1, 1),
                '└' => new Tile(1, 1, 0, 0),
                '┴' => new Tile(1, 1, 0, 1),
                '┬' => new Tile(0, 1, 1, 1),
                '├' => new Tile(1, 1, 1, 0),
                '─' => new Tile(0, 1, 0, 1),
                '┼' => new Tile(1, 1, 1, 1),
                '╞' => new Tile(1, 2, 1, 0),
                '╟' => new Tile(2, 1, 2, 0),
                '╚' => new Tile(2, 2, 0, 0),
                '╔' => new Tile(0, 2, 2, 0),
                '╩' => new Tile(2, 2, 0, 2),
                '╦' => new Tile(0, 2, 2, 2),
                '╠' => new Tile(2, 2, 2, 0),
                '═' => new Tile(0, 2, 0, 2),
                '╬' => new Tile(2, 2, 2, 2),
                '╧' => new Tile(1, 2, 0, 2),
                '╨' => new Tile(2, 1, 0, 1),
                '╤' => new Tile(0, 2, 1, 2),
                '╥' => new Tile(0, 1, 2, 1),
                '╙' => new Tile(2, 1, 0, 0),
                '╘' => new Tile(1, 2, 0, 0),
                '╒' => new Tile(0, 2, 1, 0),
                '╓' => new Tile(0, 1, 2, 0),
                '╫' => new Tile(2, 1, 2, 1),
                '╪' => new Tile(1, 2, 1, 2),
                '┘' => new Tile(1, 0, 0, 1),
                '┌' => new Tile(0, 1, 1, 0),
                _ => new Tile(0, 0, 0, 0)
            };
        }

        public char Representation()
        {
            if (this == new Tile(1, 0, 1, 0))
                return '│';
            else if (this == new Tile(1, 0, 1, 1))
                return '┤';
            else if (this == new Tile(1, 0, 1, 2))
                return '╡';
            else if (this == new Tile(2, 0, 2, 1))
                return '╢';
            else if (this == new Tile(0, 0, 2, 1))
                return '╖';
            else if (this == new Tile(0, 0, 1, 2))
                return '╕';
            else if (this == new Tile(2, 0, 2, 2))
                return '╣';
            else if (this == new Tile(2, 0, 2, 0))
                return '║';
            else if (this == new Tile(0, 0, 2, 2))
                return '╗';
            else if (this == new Tile(2, 0, 0, 2))
                return '╝';
            else if (this == new Tile(2, 0, 0, 1))
                return '╜';
            else if (this == new Tile(1, 0, 0, 2))
                return '╛';
            else if (this == new Tile(0, 0, 1, 1))
                return '┐';
            else if (this == new Tile(1, 1, 0, 0))
                return '└';
            else if (this == new Tile(1, 1, 0, 1))
                return '┴';
            else if (this == new Tile(0, 1, 1, 1))
                return '┬';
            else if (this == new Tile(1, 1, 1, 0))
                return '├';
            else if (this == new Tile(0, 1, 0, 1))
                return '─';
            else if (this == new Tile(1, 1, 1, 1))
                return '┼';
            else if (this == new Tile(1, 2, 1, 0))
                return '╞';
            else if (this == new Tile(2, 1, 2, 0))
                return '╟';
            else if (this == new Tile(2, 2, 0, 0))
                return '╚';
            else if (this == new Tile(0, 2, 2, 0))
                return '╔';
            else if (this == new Tile(2, 2, 0, 2))
                return '╩';
            else if (this == new Tile(0, 2, 2, 2))
                return '╦';
            else if (this == new Tile(2, 2, 2, 0))
                return '╠';
            else if (this == new Tile(0, 2, 0, 2))
                return '═';
            else if (this == new Tile(2, 2, 2, 2))
                return '╬';
            else if (this == new Tile(1, 2, 0, 2))
                return '╧';
            else if (this == new Tile(2, 1, 0, 1))
                return '╨';
            else if (this == new Tile(0, 2, 1, 2))
                return '╤';
            else if (this == new Tile(0, 1, 2, 1))
                return '╥';
            else if (this == new Tile(2, 1, 0, 0))
                return '╙';
            else if (this == new Tile(1, 2, 0, 0))
                return '╘';
            else if (this == new Tile(0, 2, 1, 0))
                return '╒';
            else if (this == new Tile(0, 1, 2, 0))
                return '╓';
            else if (this == new Tile(2, 1, 2, 1))
                return '╫';
            else if (this == new Tile(1, 2, 1, 2))
                return '╪';
            else if (this == new Tile(1, 0, 0, 1))
                return '┘';
            else if (this == new Tile(0, 1, 1, 0))
                return '┌';
            else
                return ' ';
        }
    }

    // string input = File.ReadAllText(@"16-test-input.txt", CodePagesEncodingProvider.Instance.GetEncoding(437));
    public Puzzle16(string input)
    {
        string[] lines = input.TrimEnd().ReplaceLineEndings("\n").Split('\n');
        this.height = lines.Length;
        var distinctWidths = lines.Select(x => x.Length).Distinct().ToArray();
        Debug.Assert(distinctWidths.Length == 1);
        this.width = distinctWidths[0];

        Dictionary<Coord, Tile> initialTiles = new();
        HashSet<Coord> pipeCoords = new();
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                Tile tile = Tile.FromChar(lines[row][col]);
                if (tile != Tile.Empty)
                {
                    Coord coord = new(row, col);
                    initialTiles.Add(coord, tile);
                    pipeCoords.Add(coord);
                }
            }
        }
        this.initialTiles = initialTiles.ToImmutableDictionary();
    }

    private Tile GetTile(IReadOnlyDictionary<Coord, Tile> tiles, Coord coord)
    {
        var (row, col) = coord;
        if (row == -1 && col == 0)
        {
            return new Tile(0, 0, 1, 0);
        }
        else if (row == height && col == width - 1)
        {
            return new Tile(1, 0, 0, 0);
        }
        else
        {
            return tiles.GetValueOrDefault(coord);
        }
    }

    public string Solve()
    {
        var timerThread = Task.Run(TimerCallback);
        var drawThread = Task.Run(DrawCallback);

        Stack<Tuple<ImmutableDictionary<Coord, Tile>, Coord, int>> stack = new();
        stack.Push(Tuple.Create(initialTiles, new Coord(0, 0), 0));
        int answer = initialTiles.Count * 4;
        while (stack.TryPop(out var state))
        {
            var (maze, position, rotations) = state;
            if (timerQueue.TryDequeue(out _))
            {
                drawQueue.Add(state);
            }
            if (position.Row + position.Col > height + width)
            {
                answer = Math.Min(answer, rotations);
                continue;
            }
            // Walk the grid in diagonal order (see https://en.wikipedia.org/wiki/Pairing_function )
            Coord nextPos = position.Row == 0
                ? new(position.Col + 1, 0)
                : new(position.Row - 1, position.Col + 1);
            Tile currentTile = GetTile(maze, position);
            if (currentTile == Tile.Empty)
            {
                if (GetTile(maze, position.Next(Direction.Left)).Right == 0 && GetTile(maze, position.Next(Direction.Up)).Down == 0)
                {
                    stack.Push(Tuple.Create(maze, nextPos, rotations));
                }
                continue;
            }
            List<Tuple<ImmutableDictionary<Coord, Tile>, Coord, int>> nextStates = new();
            List<Tile> seenTiles = new();
            for (int nextRotations = 0; nextRotations < 4; nextRotations++)
            {
                Tile nextTile = currentTile;
                for (int r = 0; r < nextRotations; r++)
                {
                    nextTile = nextTile.Rotate();
                }
                if (seenTiles.Contains(nextTile))
                {
                    continue;
                }
                seenTiles.Add(nextTile);
                if (nextTile.Left != GetTile(maze, position.Next(Direction.Left)).Right)
                {
                    continue;
                }
                if (nextTile.Up != GetTile(maze, position.Next(Direction.Up)).Down)
                {
                    continue;
                }
                var nextMaze = maze.SetItem(position, nextTile);
                nextStates.Add(Tuple.Create(nextMaze, nextPos, rotations + nextRotations));
            }
            foreach (var nextState in nextStates.AsEnumerable().Reverse())
            {
                stack.Push(nextState);
            }
        }
        solved = true;
        drawQueue.Add(null);
        return answer.ToString();
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
            var (maze, pos, rot) = item;
            Console.WriteLine($"Rotations: {rot}");
            for (int row = 0; row < this.height; row++)
            {
                for (int col = 0; col < this.width; col++)
                {
                    if (row + col < pos.Row + pos.Col || (row + col == pos.Row + pos.Col && col <= pos.Col))
                    {
                        Console.BackgroundColor = ConsoleColor.DarkRed;
                    }
                    else
                    {
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    var coord = new Coord(row, col);
                    var tile = maze.GetValueOrDefault(coord);
                    char tileRep = tile.Representation();
                    Console.Write(tileRep);
                }
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine();
            }
            Console.ResetColor();
        }
    }
}
