// TODO: Вывод подробной инструкции расчётов

namespace NumSysCalc;

class Program
{
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

    private static bool TryConvertCharToInt(char c, out int value)
    {
        if (c >= '0' && c <= '9')
        {
            value = c - '0';
            return true;
        }
        if (c >= 'A' && c <= 'Z')
        {
            value = c - 'A' + 10;
            return true;
        }
        if (c >= 'a' && c <= 'z')
        {
            value = c - 'a' + 10;
            return true;
        }

        value = 0;
        return false;
    }

    private static int DetermineBase(string number)
    {
        if (string.IsNullOrWhiteSpace(number))
            throw new ArgumentException("String cannot be empty", nameof(number));

        int maxDigitValue = 0;
        foreach (char c in number)
        {
            if (!TryConvertCharToInt(c, out int value))
                continue;
            if (value > maxDigitValue) maxDigitValue = value;
        }
        int targetBase = maxDigitValue + 1;
        return targetBase < 2 ? 2 : targetBase;
    }

    public static bool TryConvertDecimalToBase(long number, int toBase, out string result)
    {
        if (toBase == 2 || toBase == 8 || toBase == 10 || toBase == 16)
        {
            result = Convert.ToString(number, toBase);
            return true;
        }
        if (number == 0)
        {
            result = "0";
            return true;
        }

        bool isNegative = number < 0;
        ulong remaining = isNegative ? (ulong)-number : (ulong)number;

        // Максимальная длина для 64-битного числа в двоичной системе - 64 символа + 1 знак
        Span<char> buffer = stackalloc char[65];
        int index = buffer.Length;

        while (remaining > 0)
        {
            uint digit = (uint)(remaining % (ulong)toBase);
            buffer[--index] = (char)(digit < 10 ? '0' + digit : 'A' + (digit - 10));
            remaining /= (ulong)toBase;
        }

        if (isNegative)
            buffer[--index] = '-';

        result = new string(buffer[index..]);
        return true;
    }

    public static bool TryConvertBaseToDecimal(string number, int fromBase, out long result)
    {
        result = 0;

        foreach (char c in number)
        {
            if (!TryConvertCharToInt(c, out int value))
                return false;
            if (value >= fromBase)
                return false;
            try
            {
                checked
                {
                    // Метод Горнера: вместо возведения в степень умножаем текущий результат на основание
                    result = result * fromBase + value;
                }
            }
            catch (OverflowException)
            {
                result = 0;
                return false;
            }
        }

        return true;
    }

    public static bool ConvertBases(string inputNumber, int fromBase, int toBase, out string result)
    {
        if (fromBase < 2 || fromBase > 36)
            throw new ArgumentOutOfRangeException(nameof(fromBase), fromBase, "The base of the numeral system must be between 2 and 36");
        if (toBase < 2 || toBase > 36)
            throw new ArgumentOutOfRangeException(nameof(toBase), toBase, "The base of the numeral system must be between 2 and 36");

        bool success;
        if (fromBase == 10)
        {
            success = long.TryParse(inputNumber, out long number);
            success = TryConvertDecimalToBase(number, toBase, out result) && success;
        }
        else if (toBase == 10)
        {
            success = TryConvertBaseToDecimal(inputNumber, fromBase, out long number);
            result = number.ToString();
        }
        else
        {
            success = TryConvertBaseToDecimal(inputNumber, fromBase, out long number);
            success = TryConvertDecimalToBase(number, toBase, out result) && success;
        }

        return success;
    }

    public static void Main()
    {
        string inputNumber = GetString("Enter your number: ").ToUpper();
        int targetBase = DetermineBase(inputNumber);
        int fromBase = GetNumber("Enter the first base of the numeral system: ");
        if (fromBase < targetBase)
            throw new ArgumentException($"The entered number {inputNumber}, which has a minimum numeral system base of {targetBase}, cannot be represented in the numeral system with the base {fromBase}", nameof(inputNumber));
        int toBase = GetNumber("Enter the second base of the numeral system: ");
        if (ConvertBases(inputNumber, fromBase, toBase, out string targetNumber))
            Console.WriteLine(targetNumber);
    }
}
