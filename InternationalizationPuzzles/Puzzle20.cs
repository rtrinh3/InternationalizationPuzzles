using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/20/
// Puzzle 20: The future of Unicode
public class Puzzle20(string input) : IPuzzle
{
    public string Solve()
    {
        string text = Decode();
        var numbers = Regex.Matches(text, @"\d+").Select(m => int.Parse(m.Value)).Order().ToArray();
        Debug.Assert(numbers.Length % 2 == 1);
        var answer = numbers[numbers.Length / 2];
        return answer.ToString();
    }

    public string Decode()
    {
        var bytesFromInput = Convert.FromBase64String(input);
        var utf16 = Encoding.Unicode.GetString(bytesFromInput);
        Debug.Assert(utf16[0] == '\ufeff');
        var runes = utf16.EnumerateRunes().Skip(1).Select(r => (uint)r.Value);
        var bytesFromRunes = BytesFromCodes(runes, 20);
        var xtf8 = ParseExtendedUtf8(bytesFromRunes);
        var bytesFromXtf8 = BytesFromCodes(xtf8, 28);
        var utf8 = Encoding.UTF8.GetString(bytesFromXtf8.ToArray());
        while (utf8.EndsWith('\0'))
        {
            utf8 = utf8[..^1];
        }
        return utf8;
    }

    private static IEnumerable<byte> BytesFromCodes(IEnumerable<uint> codes, int bitsPerCode)
    {
        int bytesPerChunk = (bitsPerCode * 2) / 8;
        return codes.Chunk(2).SelectMany(chunk =>
        {
            if (chunk.Length == 2)
            {
                ulong bits = (((ulong)chunk[0]) << bitsPerCode) | ((ulong)chunk[1]);
                var bytes = BitConverter.GetBytes(bits);
                var result = bytes.Take(bytesPerChunk).Reverse();
                return result;
            }
            else
            {
                Console.WriteLine("Unpaired code");
                var bytes = BitConverter.GetBytes(chunk[0]);
                var result = bytes;
                return result;
            }
        });
    }

    private static IEnumerable<uint> ParseExtendedUtf8(IEnumerable<byte> bytes)
    {
        int continuationBytesExpected = 0;
        uint codeBuffer = 0;
        foreach (var b in bytes)
        {
            int leadingOnes = System.Numerics.BitOperations.LeadingZeroCount((uint)(~b & 0xff)) - 24;
            if (leadingOnes == 0)
            {
                if (continuationBytesExpected > 0)
                {
                    Console.WriteLine("Let's pretend this is a continuation");
                    codeBuffer = (codeBuffer << 6) | (b & 0x3fu);
                }
                else
                {
                    yield return b;
                }
            }
            else if (leadingOnes == 1)
            {
                if (continuationBytesExpected == 0)
                {
                    Console.WriteLine("Unexpected continuation");
                    yield return b;
                }
                else
                {
                    codeBuffer = (codeBuffer << 6) | (b & 0x3fu);
                    continuationBytesExpected--;
                    if (continuationBytesExpected == 0)
                    {
                        yield return codeBuffer;
                        codeBuffer = 0;
                    }
                }
            }
            else
            {
                if (continuationBytesExpected > 0)
                {
                    Console.WriteLine("Unterminated code");
                    yield return codeBuffer;
                }
                continuationBytesExpected = leadingOnes - 1;
                uint lowMask = (1u << (8 - leadingOnes)) - 1;
                codeBuffer = b & lowMask;
            }
        }
    }
}
