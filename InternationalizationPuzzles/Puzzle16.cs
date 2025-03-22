using System.Diagnostics;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/16/
// Puzzle 16: 8-bit unboxing
public class Puzzle16(string input) : IPuzzle
{
    // string input = File.ReadAllText(@"16-test-input.txt", CodePagesEncodingProvider.Instance.GetEncoding(437));

    private readonly record struct Tile(byte Up, byte Right, byte Down, byte Left)
    {
        public static Tile Empty => new Tile(0, 0, 0, 0);

        public Tile Rotate() => new Tile(Left, Up, Right, Down);

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
    }

    private readonly record struct Coord(short Row, short Col);

    private static Tile GetTile(Tile[,] tiles, int row, int col)
    {
        if (row == -1 && col == 0)
        {
            return new Tile(0, 0, 1, 0);
        }
        else if (row == tiles.GetLength(0) && col == tiles.GetLength(1)-1)
        {
            return new Tile(1, 0, 0, 0);
        }
        else if (row < 0 || row >= tiles.GetLength(0) || col < 0 || col >= tiles.GetLength(1))
        {
            return new Tile(0, 0, 0, 0);
        }
        else
        {
            return tiles[row, col];
        }
    }

    public string Solve()
    {
        string[] lines = input.TrimEnd().ReplaceLineEndings("\n").Split('\n');
        int height = lines.Length;
        var distinctWidths = lines.Select(x => x.Length).Distinct().ToArray();
        Debug.Assert(distinctWidths.Length == 1);
        int width = distinctWidths[0];
        Tile[,] tiles = new Tile[height, width];
        HashSet<Coord> pipeCoords = new();
        for (int row = 0; row < height; row++)
        {
            for (int col = 0; col < lines[row].Length; col++)
            {
                Tile tile = Tile.FromChar(lines[row][col]);
                tiles[row, col] = tile;
                if (tile != Tile.Empty)
                {
                    pipeCoords.Add(new Coord((short)row, (short)col));
                }
            }
        }

        int rotations = 0;
        void Visit(Coord from, Coord current)
        {
            // TODO
        }
        Visit(new Coord(-1, 0), new Coord(0, 0));
        int answer = rotations; // TODO
        return answer.ToString();
    }
}
