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
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.Write(prompt);
        Console.ResetColor();
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

    public static string? DecimalToBase(string inputNumber, int baseTo)
    {
        Stack<char> stack = [];
        if (!int.TryParse(inputNumber, out int number)) return null;
        if (new[] { 2, 8, 16 }.Contains(baseTo))
            return Convert.ToString(number, baseTo);
        while (number != 0)
        {
            char num = alphabet[number % baseTo];
            stack.Push(num);
            number /= baseTo;
        }
        return new string(stack.ToArray());
    }

    public static string BaseToDecimal(string inputNumber, int baseFrom)
    {
        int result;
        return "";
    }

    public static string? ConvertBases(string inputNumber, int baseFrom, int baseTo)
    {
        string? result = null;
        if (baseFrom == 10)
            result = DecimalToBase(inputNumber, baseTo);

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
