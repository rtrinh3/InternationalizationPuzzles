using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace InternationalizationPuzzles;

// https://i18n-puzzles.com/puzzle/12/
// Puzzle 12: Sorting it out
public class Puzzle12(string input) : IPuzzle
{
    private record PhonebookEntry(string LastName, string FirstName, string PhoneNumber);

    private const CompareOptions SortOptions = CompareOptions.IgnoreCase | CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreSymbols;

    private readonly CultureInfo englishCulture = CultureInfo.GetCultureInfo("en");
    private readonly CultureInfo swedishCulture = CultureInfo.GetCultureInfo("sv");
    private readonly CultureInfo dutchCulture = CultureInfo.GetCultureInfo("nl");
    private readonly PhonebookEntry[] phonebook =
        input.TrimEnd().ReplaceLineEndings("\n").Split('\n').Select(line =>
        {
            var match = Regex.Match(line, @"^(?<LastName>.+), (?<FirstName>.+): (?<PhoneNumber>.+)$");
            return new PhonebookEntry(
                match.Groups["LastName"].Value,
                match.Groups["FirstName"].Value,
                match.Groups["PhoneNumber"].Value
            );
        })
        .ToArray();

    public string Solve()
    {
        Debug.Assert(phonebook.Length % 2 == 1);
        int middleIndex = phonebook.Length / 2;
        var englishSort = SortEnglish();
        long englishMiddle = long.Parse(englishSort[middleIndex].PhoneNumber);
        var swedishSort = SortSwedish();
        long swedishMiddle = long.Parse(swedishSort[middleIndex].PhoneNumber);
        var dutchSort = SortDutch();
        long dutchMiddle = long.Parse(dutchSort[middleIndex].PhoneNumber);
        long answer = englishMiddle * swedishMiddle * dutchMiddle;
        return answer.ToString();
    }

    private static PhonebookEntry[] SortPhonebook(PhonebookEntry[] entries, Comparison<string> nameComparer)
    {
        var copy = entries.ToArray();
        Array.Sort(copy, (a, b) =>
        {
            int result = nameComparer(a.LastName, b.LastName);
            if (result != 0)
            {
                return result;
            }
            result = nameComparer(a.FirstName, b.FirstName);
            if (result != 0)
            {
                return result;
            }
            return a.PhoneNumber.CompareTo(b.PhoneNumber);
        });
        return copy;
    }

    private PhonebookEntry[] SortEnglish() => SortPhonebook(phonebook, CompareEnglish);

    private int CompareEnglish(string a, string b) => string.Compare(a, b, englishCulture, SortOptions);

    private PhonebookEntry[] SortSwedish() => SortPhonebook(phonebook, CompareSwedish);

    private int CompareSwedish(string a, string b) => string.Compare(a, b, swedishCulture, SortOptions);

    private PhonebookEntry[] SortDutch()
    {
        var copy = phonebook.Select(entry =>
        {
            int lastNameIndex = 0;
            for (int i = 0; i < entry.LastName.Length; i++)
            {
                if (char.IsUpper(entry.LastName[i]))
                {
                    lastNameIndex = i;
                    break;
                }
            }
            if (lastNameIndex >= entry.LastName.Length)
            {
                lastNameIndex = 0;
            }
            string newLastName = entry.LastName[lastNameIndex..];
            string newFirstName = (entry.FirstName + " " + entry.LastName[..lastNameIndex]).Trim();
            return new PhonebookEntry(newLastName, newFirstName, entry.PhoneNumber);
        }).ToArray();
        return SortPhonebook(copy, CompareDutch);
    }

    private int CompareDutch(string a, string b) => string.Compare(a, b, dutchCulture, SortOptions);
}
