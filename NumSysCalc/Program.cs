using System.Runtime.InteropServices;

namespace NumSysCalc;

class Program
{
    public const string alphabet = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static string GetString(string prompt)
    {
        Console.Write(prompt);
        string? input = Console.ReadLine();
        if (input is null) return "0";
        return input;
    }

    private static int GetNumber(string prompt)
    {
        while (true)
        {
            string input = GetString(prompt);
            bool success = int.TryParse(input, out int result);
            if (success) return result;
            PrintError("The entered value is not a number. Please try again");
        }
    }

    private static int DetermineBase(string number)
    {
        int targetBase = 0;
        foreach (char i in number)
        {
            int index = alphabet.IndexOf(i);
            if (index > targetBase) targetBase = index + 1;
        }
        return targetBase;
    }

    public static string? ConvertBases(string inputNumber, int baseFrom, int baseTo)
    {
        List<char> targetNumber = [];
        string? result = null;
        if (baseFrom == 10)
        {
            if (!int.TryParse(inputNumber, out int number)) return null;
            while (number != 0)
            {
                char num = alphabet[number % baseTo];
                targetNumber.Add(num);
                number /= baseTo;
            }
            result = CollectionsMarshal.AsSpan(targetNumber).ToString();
        }

        return result;
    }

    public static void Main()
    {
        string inputNumber = GetString("Enter your number: ").ToUpper();
        int targetBase = DetermineBase(inputNumber);
        int baseFrom = GetNumber("Enter the first base of the numeral system: ");
        if (baseFrom < targetBase)
        {
            PrintError($"The entered number {inputNumber}, which has a minimum numeral system base of {targetBase}, cannot be represented in the numeral system with the base {baseFrom}");
            return;
        }
        int baseTo = GetNumber("Enter the second base of the numeral system: ");
        string? targetNumber = ConvertBases(inputNumber, baseFrom, baseTo);
        Console.WriteLine(targetNumber);
    }
}
