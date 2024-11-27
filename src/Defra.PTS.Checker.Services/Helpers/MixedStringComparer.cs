using System.Diagnostics.CodeAnalysis;

namespace Defra.PTS.Checker.Services.Helpers;

[ExcludeFromCodeCoverage]
class MixedStringComparer : IComparer<string>
{
    public int Compare(string? x, string? y)
    {
        if (string.IsNullOrEmpty(x) || string.IsNullOrEmpty(y)) 
            return 0;

        int minLength = Math.Min(x.Length, y.Length);

        for (int i = 0; i < minLength; i++)
        {
            char charX = x[i];
            char charY = y[i];

            if (char.IsLetter(charX) && char.IsDigit(charY))
                return -1; // Letters come before numbers at the same position
            if (char.IsDigit(charX) && char.IsLetter(charY))
                return 1; // Numbers come after letters at the same position
            if (charX != charY)
                return charX.CompareTo(charY); // Default character comparison
        }

        // If one string is a prefix of the other, shorter string comes first
        return x.Length.CompareTo(y.Length);
    }
}
