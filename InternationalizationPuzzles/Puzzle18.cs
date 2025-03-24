using MathNet.Symbolics;
using System.Text.RegularExpressions;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/18/
// Puzzle 18: Rex To Lynx
public class Puzzle18(string input) : IPuzzle
{
    public string Solve()
    {
        string[] lines = input.TrimEnd().ReplaceLineEndings("\n").Split('\n');
        Dictionary<string, FloatingPoint> noValues = new();
        double answer = 0;
        foreach (string line in lines)
        {
            string strippedLine = string.Join("", line.Where(char.IsAscii));
            string apparentLine = RenderBiDi(line);
            var strippedExpr = SymbolicExpression.Parse(strippedLine);
            var apparentExpr = SymbolicExpression.Parse(apparentLine);
            var strippedValue = strippedExpr.Evaluate(noValues);
            var apparentValue = apparentExpr.Evaluate(noValues);
            var absoluteDifference = Math.Abs(strippedValue.RealValue - apparentValue.RealValue);
            answer += absoluteDifference;
        }
        return answer.ToString();
    }

    private static string RenderBiDi(string line)
    {
        LinkedList<string> buffer = new();
        var firstNode = buffer.AddFirst("");
        var tokens = Regex.Matches(line, @"\+|\-|\*|\/|\(|\)|\d+|\u2067|\u2066|\u2069");
        Stack<LinkedListNode<string>> insertionStack = new();
        insertionStack.Push(firstNode);
        bool IsLtr() => insertionStack.Count % 2 != 0;
        foreach (Match token in tokens)
        {
            if (token.Value == "\u2067")
            {
                // If currently LTR, switch to RTL
                if (IsLtr())
                {
                    var cursor = insertionStack.Peek();
                    var newCursor = buffer.AddBefore(cursor, "");
                    insertionStack.Push(newCursor);
                }
            }
            else if (token.Value == "\u2066")
            {
                // If currently RTL, switch to LTR
                if (!IsLtr())
                {
                    var cursor = insertionStack.Peek();
                    var newCursor = buffer.AddAfter(cursor, "");
                    insertionStack.Push(newCursor);
                }
            }
            else if (token.Value == "\u2069")
            {
                insertionStack.Pop();
            }
            else if (token.Value == "(")
            {
                var cursor = insertionStack.Peek();
                if (IsLtr())
                {
                    buffer.AddBefore(cursor, "(");
                }
                else
                {
                    buffer.AddAfter(cursor, ")");
                }
            }
            else if (token.Value == ")")
            {
                var cursor = insertionStack.Peek();
                if (IsLtr())
                {
                    buffer.AddBefore(cursor, ")");
                }
                else
                {
                    buffer.AddAfter(cursor, "(");
                }
            }
            else
            {
                var cursor = insertionStack.Peek();
                if (IsLtr())
                {
                    buffer.AddBefore(cursor, token.Value);
                }
                else
                {
                    buffer.AddAfter(cursor, token.Value);
                }
            }
        }

        var result = string.Join("", buffer);
        return result;
    }
}
