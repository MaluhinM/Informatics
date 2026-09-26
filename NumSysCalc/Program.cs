namespace Test;

class Program
{
    private static void PrintError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    private static int GetNumber(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            bool success = Int32.TryParse(Console.ReadLine(), out int input);
            if (success) return input;
            PrintError("The entered value is not a number. Please try again");
        }
    }

    public static void Main()
    {
        int baseFrom = GetNumber("Enter the first base of the numeral system: ");
        int baseTo = GetNumber("Enter the second base of the numeral system: ");

    }
}
