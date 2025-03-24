using System.Diagnostics;
using System.Text.RegularExpressions;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/18/
// Puzzle 18: Rex To Lynx
public class Puzzle18(string input) : IPuzzle
{
    public string Solve()
    {
        string[] lines = input.TrimEnd().ReplaceLineEndings("\n").Split('\n');
        double answer = 0;
        foreach (string line in lines)
        {
            string strippedLine = string.Join("", line.Where(char.IsAscii));
            string apparentLine = RenderBiDi(line);
            var strippedValue = Evaluate(strippedLine);
            var apparentValue = Evaluate(apparentLine);
            var absoluteDifference = Math.Abs(strippedValue - apparentValue);
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

    private static double Evaluate(string expression)
    {
        var tokens = Regex.Matches(expression, @"\+|\-|\*|\/|\(|\)|\d+");
        var infixExpression = tokens.Select(tokens => tokens.Value);
        var rpnExpression = ShuntingYardAlgorithm(infixExpression);
        var result = EvaluateRpn(rpnExpression);
        return result;
    }

    // https://en.wikipedia.org/wiki/Shunting_yard_algorithm
    private static IEnumerable<string> ShuntingYardAlgorithm(IEnumerable<string> infixExpression)
    {
        static int Precedence(string op) => op switch
        {
            "+" => 1,
            "-" => 1,
            "*" => 2,
            "/" => 2,
            _ => 0
        };
        Stack<string> operatorStack = new();
        foreach (string token in infixExpression)
        {
            if (token.All(char.IsAsciiDigit))
            {
                yield return token;
            }
            else if (Precedence(token) > 0)
            {
                while (operatorStack.Count > 0 && operatorStack.Peek() != "(" && Precedence(operatorStack.Peek()) >= Precedence(token))
                {
                    yield return operatorStack.Pop();
                }
                operatorStack.Push(token);
            }
            else if (token == "(")
            {
                operatorStack.Push(token);
            }
            else if (token == ")")
            {
                Debug.Assert(operatorStack.Count >= 1, "Mismatched parentheses");
                while (operatorStack.Peek() != "(")
                {
                    yield return operatorStack.Pop();
                }
                Debug.Assert(operatorStack.Count >= 1 && operatorStack.Peek() == "(", "Mismatched parentheses");
                operatorStack.Pop();
            }
        }
        while (operatorStack.Count > 0)
        {
            yield return operatorStack.Pop();
        }
    }

    private static double EvaluateRpn(IEnumerable<string> rpnExpression)
    {
        Stack<double> stack = new();
        foreach (string token in rpnExpression)
        {
            if (token.All(char.IsAsciiDigit))
            {
                stack.Push(double.Parse(token));
            }
            else if (token == "+")
            {
                var operandA = stack.Pop();
                var operandB = stack.Pop();
                var newValue = operandA + operandB;
                stack.Push(newValue);
            }
            else if (token == "-")
            {
                var operandA = stack.Pop();
                var operandB = stack.Pop();
                var newValue = operandB - operandA;
                stack.Push(newValue);
            }
            else if (token == "*")
            {
                var operandA = stack.Pop();
                var operandB = stack.Pop();
                var newValue = operandA * operandB;
                stack.Push(newValue);
            }
            else if (token == "/")
            {
                var operandA = stack.Pop();
                var operandB = stack.Pop();
                var newValue = operandB / operandA;
                stack.Push(newValue);
            }
        }
        Debug.Assert(stack.Count >= 1);
        return stack.Peek();
    }
}
