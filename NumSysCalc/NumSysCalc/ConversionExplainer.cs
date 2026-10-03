using System.Numerics;

namespace NumSysCalc;

public static class ConversionExplainer
{
    public static IEnumerable<string> Explain(string number, int fromBase, int toBase)
    {
        if (fromBase == toBase)
        {
            yield return "Системы счисления совпадают, число остаётся без изменений.";
            yield break;
        }

        if (fromBase == 10)
        {
            foreach (string line in ExplainFromDecimal(long.Parse(number), toBase))
                yield return line;
        }
        else if (toBase == 10)
        {
            foreach (string line in ExplainToDecimal(number, fromBase))
                yield return line;
        }
        else
        {
            yield return "Переводим через десятичную систему.";
            yield return "";
            yield return "Шаг 1. Переводим в десятичную:";
            foreach (string line in ExplainToDecimal(number, fromBase))
                yield return line;

            Program.TryConvertBaseToDecimal(number, fromBase, out long decimalValue);

            yield return "";
            yield return "Шаг 2. Переводим из десятичной:";
            foreach (string line in ExplainFromDecimal(decimalValue, toBase))
                yield return line;
        }
    }

    private static IEnumerable<string> ExplainToDecimal(string number, int fromBase)
    {
        yield return $"Перевод числа {number} из системы с основанием {fromBase} в десятичную.";
        yield return "Каждую цифру умножаем на основание в степени её позиции " +
                     "(справа налево, начиная с 0) и складываем результаты:";

        bool isNegative = number[0] == '-';
        if (isNegative)
        {
            number = number.Remove(0, 1);
            yield return "Число отричательное: переводим его модуль, знак «-» припишем в конце.";
        }

        BigInteger total = 0;
        List<BigInteger> terms = [];

        for (int i = 0; i < number.Length; i++)
        {
            char c = number[i];
            Program.TryConvertCharToInt(c, out int digit);

            int power = number.Length - 1 - i;
            BigInteger weight = BigInteger.Pow(fromBase, power);
            BigInteger term = digit * weight;

            total += term;
            terms.Add(term);

            string digitText = digit >= 10 ? $"{c} (={digit})" : c.ToString();
            yield return $"  {digitText} × {fromBase}^{power} = {digit} × {weight} = {term}";
        }

        if (isNegative)
            yield return $"Сумма: -({string.Join(" + ", terms)}) = -{total}";
        else
            yield return $"Сумма: {string.Join(" + ", terms)} = {total}";
    }

    private static IEnumerable<string> ExplainFromDecimal(long number, int toBase)
    {
        yield return $"Перевод числа {number} из десятичной системы в систему с основанием {toBase}.";

        if (number == 0)
        {
            yield return "Число равно нулю, результат: 0";
            yield break;
        }

        yield return "Делим число на основание с остатком, пока частное не станет 0. " +
                     "Остатки читаем снизу вверх:";

        bool isNegative = number < 0;
        if (isNegative)
            yield return "Число отричательное: переводим его модуль, знак «-» припишем в конце.";

        BigInteger remaining = BigInteger.Abs(number);
        List<char> digits = [];

        while (remaining > 0)
        {
            int remainder = (int)(remaining % toBase);
            BigInteger quotient = remaining / toBase;
            char digitChar = (char)(remainder < 10 ? '0' + remainder : 'A' + (remainder - 10));
            digits.Add(digitChar);

            string note = remainder >= 10 ? $" (цифрв {digitChar})" : "";
            yield return $"  {remaining} : {toBase} = {quotient}, остаток {remainder}{note}";

            remaining = quotient;
        }

        digits.Reverse();
        yield return $"Остатки снизу вверх: {(isNegative ? "-" : "")}{string.Concat(digits)}";
    }
}
