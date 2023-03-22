
using System.Reflection.Metadata.Ecma335;

double tolerance = 0;
double value = 0;
string colorCode = args[0];
string errormessage = string.Empty;
int errorType = -1;
int digit = 0;

int lenge = colorCode.Length;

if (args.Length != 1)
{
    errorType = 4;
}
if (colorCode.Length != 15 && colorCode.Length != 19)
{
    if (colorCode.Length < 3) { errorType = 0; }
    else
    {
        while (colorCode != string.Empty)
        {
            if (TryConvertColorDigit(colorCode.Substring(0, 3), out digit))
            {
                colorCode = colorCode.Substring(4);
            }
            if (colorCode.Length == 3)
            {
                if (TryConvertColorDigit(colorCode, out digit))
                {
                    colorCode = string.Empty;
                    errorType = 3;
                }
            }
            else errorType = 0; break;
        }
    }
}
else if (TryDecode4or5ColorBands(colorCode, out value, out tolerance, out errorType) == true)
{
    Console.WriteLine($"color code:  {colorCode}");
    Console.WriteLine($"value = {double.Round(value, 2)} Ohm     tolerance = +/- {tolerance}%");
}

Console.ForegroundColor = ConsoleColor.Red;
Console.WriteLine(TypeOfError(errorType));
Console.ResetColor();

bool TryConvertColorDigit(string input, out int digit)
{
    string[] color = new[] { "Bla", "Bro", "Red", "Ora", "Yel", "Gre", "Blu", "Vio", "Gra", "Whi", "Gol", "Sil" };
    digit = Array.IndexOf(color, input);
    return digit != -1;

}
double GetMultiplierFromColor(string input)
{
    int digit = 0;
    TryConvertColorDigit(input, out digit);

    if (digit == 10) { return 0.1; }
    else if (digit == 11) { return 0.01; }
    return Math.Pow(10, digit);
}
double GetToleranceFromColor(string input, out int digit)
{

    TryConvertColorDigit(input, out digit);

    switch (digit)
    {
        case 1: return 1;
        case 2: return 2;
        case 5: return 0.5;
        case 6: return 0.25;
        case 7: return 0.10;
        case 8: return 0.05;
        case 10: return 5;
        case 11: return 10;
    }
    return 0;

}
bool TryDecode4or5ColorBands(string input, out double value, out double tolerance, out int errorType)
{
    string output = string.Empty;
    string currentInput = input;
    int digit = 0;
    int count = 0;
    bool fourColors = false;
    bool fiveColors = false;
    int validPositionGolOrSil = 0;

    if (input.Length == 15) { fourColors = true; }
    else { fiveColors = true; }

    while (currentInput != string.Empty)
    {
        count++;

        if (!TryConvertColorDigit(currentInput.Substring(0, 3), out digit))
        {
            value = -1;
            tolerance = -1;
            errorType = 1;

            return false;
        }
        if (fourColors)
        {
            validPositionGolOrSil = 2;
            if (count <= 3) { currentInput = currentInput.Substring(4); }
            else { currentInput = string.Empty; }
        }
        if (fiveColors)
        {
            validPositionGolOrSil = 3;
            if (count <= 4) { currentInput = currentInput.Substring(4); }
            else { currentInput = string.Empty; }

        }
        if (count < validPositionGolOrSil && (currentInput.Substring(0, 3) == "Sil" || currentInput.Substring(0, 3) == "Gol"))
        {
            value = -1;
            tolerance = -1;
            errorType = 2;

            return false;
        }
    }


    if (fiveColors)
    {
        TryConvertColorDigit(input.Substring(0, 3), out digit);
        output += Convert.ToString(digit);
        input = input.Substring(4);
    }
    TryConvertColorDigit(input.Substring(0, 3), out digit);
    output += Convert.ToString(digit);
    input = input.Substring(4);
    TryConvertColorDigit(input.Substring(0, 3), out digit);
    output += Convert.ToString(digit);
    input = input.Substring(4);

    value = Convert.ToDouble(output) * GetMultiplierFromColor(input.Substring(0, 3));
    input = input.Substring(4);

    tolerance = GetToleranceFromColor(input, out digit);
    errorType = -1;

    return true;

}
string TypeOfError(int type)
{
    return
        type == 0 ? "Invalid color code!"
        : type == 1 ? "Input contains an invalid color code"
        : type == 2 ? "This input has Gold as one of the significant digit or multiplier bands,\nwhich is not valid according to the resistor color code chart."
        : type == 3 ? "Invalid input length!\n Please provide 4 or 5 color bands."
        : type == 4 ? "Wrong number of assignments!"
          : string.Empty;
}