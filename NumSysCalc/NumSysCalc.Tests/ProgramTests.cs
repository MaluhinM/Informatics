using System.Linq;
using NUnit.Framework;
using NumSysCalc;

namespace NumSysCalc.Tests;

[TestFixture]
public class ProgramTests
{
    // -----------------------------
    // TryConvertDecimalToBase
    // -----------------------------

    [TestCase(0L, 2, "0")]
    [TestCase(10L, 2, "1010")]
    [TestCase(45L, 2, "101101")]
    [TestCase(255L, 16, "FF")]
    [TestCase(42L, 16, "2A")]
    [TestCase(-42L, 16, "-2A")]
    [TestCase(100L, 10, "100")]
    public void TryConvertDecimalToBase_ReturnsCorrectResult(
        long number,
        int toBase,
        string expected)
    {
        bool success = Program.TryConvertDecimalToBase(
            number,
            toBase,
            out string result);

        Assert.That(success, Is.True);
        Assert.That(result, Is.EqualTo(expected));
    }


    // -----------------------------
    // TryConvertBaseToDecimal
    // -----------------------------

    [TestCase("0", 2, 0L)]
    [TestCase("1010", 2, 10L)]
    [TestCase("101101", 2, 45L)]
    [TestCase("FF", 16, 255L)]
    [TestCase("2A", 16, 42L)]
    [TestCase("a", 16, 10L)]
    public void TryConvertBaseToDecimal_ReturnsCorrectResult(
        string number,
        int fromBase,
        long expected)
    {
        bool success = Program.TryConvertBaseToDecimal(
            number,
            fromBase,
            out long result);

        Assert.That(success, Is.True);
        Assert.That(result, Is.EqualTo(expected));
    }


    [TestCase("2", 2)]
    [TestCase("8", 8)]
    [TestCase("1G", 16)]
    [TestCase("Z", 35)]
    public void TryConvertBaseToDecimal_ReturnsFalseForInvalidDigit(
        string number,
        int fromBase)
    {
        bool success = Program.TryConvertBaseToDecimal(
            number,
            fromBase,
            out _);

        Assert.That(success, Is.False);
    }


    [Test]
    public void TryConvertBaseToDecimal_ReturnsFalseOnOverflow()
    {
        bool success = Program.TryConvertBaseToDecimal(
            "9223372036854775808",
            10,
            out _);

        Assert.That(success, Is.False);
    }


    // -----------------------------
    // ConvertBases
    // -----------------------------

    [TestCase("45", 10, 2, "101101")]
    [TestCase("101101", 2, 10, "45")]
    [TestCase("FF", 16, 10, "255")]
    [TestCase("FF", 16, 2, "11111111")]
    [TestCase("1010", 2, 16, "A")]
    [TestCase("123", 10, 10, "123")]
    public void ConvertBases_ReturnsCorrectResult(
        string inputNumber,
        int fromBase,
        int toBase,
        string expected)
    {
        bool success = Program.ConvertBases(
            inputNumber,
            fromBase,
            toBase,
            out string result);

        Assert.That(success, Is.True);
        Assert.That(result, Is.EqualTo(expected));
    }


    [Test]
    public void ConvertBases_ReturnsFalseForInvalidNumber()
    {
        bool success = Program.ConvertBases(
            "1G",
            16,
            2,
            out _);

        Assert.That(success, Is.False);
    }


    [Test]
    public void ConvertBases_ThrowsForTooSmallFromBase()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Program.ConvertBases("10", 1, 2, out _));
    }


    [Test]
    public void ConvertBases_ThrowsForTooLargeFromBase()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Program.ConvertBases("10", 37, 2, out _));
    }


    [Test]
    public void ConvertBases_ThrowsForTooSmallToBase()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Program.ConvertBases("10", 2, 1, out _));
    }


    [Test]
    public void ConvertBases_ThrowsForTooLargeToBase()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            Program.ConvertBases("10", 2, 37, out _));
    }


    // -----------------------------
    // ConversionExplainer
    // -----------------------------

    [Test]
    public void Explain_ReturnsMessageForSameBases()
    {
        var lines = ConversionExplainer
            .Explain("101", 2, 2)
            .ToList();

        Assert.That(lines, Has.Count.EqualTo(1));
        Assert.That(
            lines[0],
            Is.EqualTo(
                "Системы счисления совпадают, число остаётся без изменений."));
    }


    [Test]
    public void ExplainFromDecimal_ContainsDivisionSteps()
    {
        var lines = ConversionExplainer
            .Explain("45", 10, 2)
            .ToList();

        Assert.That(
            lines,
            Does.Contain("Остатки снизу вверх: 101101"));
    }


    [Test]
    public void ExplainToDecimal_ContainsPowerCalculation()
    {
        var lines = ConversionExplainer
            .Explain("101101", 2, 10)
            .ToList();

        Assert.That(
            lines,
            Does.Contain(
                "  1 × 2^5 = 1 × 32 = 32"));

        Assert.That(
            lines,
            Does.Contain(
                "Сумма: 32 + 0 + 8 + 4 + 0 + 1 = 45"));
    }


    [Test]
    public void ExplainBaseToBase_ContainsTwoSteps()
    {
        var lines = ConversionExplainer
            .Explain("FF", 16, 2)
            .ToList();

        Assert.That(
            lines,
            Does.Contain("Переводим через десятичную систему."));

        Assert.That(
            lines,
            Does.Contain("Шаг 1. Переводим в десятичную:"));

        Assert.That(
            lines,
            Does.Contain("Шаг 2. Переводим из десятичной:"));

        Assert.That(
            lines,
            Does.Contain("Остатки снизу вверх: 11111111"));
    }


    [Test]
    public void ExplainZero_ReturnsZeroMessage()
    {
        var lines = ConversionExplainer
            .Explain("0", 10, 2)
            .ToList();

        Assert.That(
            lines,
            Does.Contain("Число равно нулю, результат: 0"));
    }
}