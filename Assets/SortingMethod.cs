using System;

namespace Assets
{
    internal enum SortingMethod
    {
        ColorAlphabetical,
        ColorLength,
        ColorReadingOrder,
        ClockwiseTopRight,
        ScrabbleLastThree,
        ClockwiseBottomLeft,
        PartyNumber,
        PartyReadingOrder,
        NameAlphabetical,
        ColorNumber,
        PartyLength,
        ClockwiseBottomRight,
        PartyAlphabetical,
        ScrabbleFirstThree,
        ScrabbleFirstAndLast,
        NameLength,
        ClockwiseTopLeft,
        CopyLastCharacter,
        ReadingOrder
    }

    internal static class SortingMethodExtensions
    {
        public static string ToLogString(this SortingMethod sm)
        {
            switch (sm)
            {
                case SortingMethod.ColorAlphabetical:
                    return "in alphabetical order by their color names";
                case SortingMethod.ColorLength:
                    return "by the length of their color names";
                case SortingMethod.ColorReadingOrder:
                    return "in reading order based on the color name table";
                case SortingMethod.ClockwiseTopRight:
                    return "clockwise, starting from the top-right";
                case SortingMethod.ScrabbleLastThree:
                    return "by the Scrabble score of the last three letters of their names";
                case SortingMethod.ClockwiseBottomLeft:
                    return "clockwise, starting from the bottom-left";
                case SortingMethod.PartyNumber:
                    return "by the number next to their party names";
                case SortingMethod.PartyReadingOrder:
                    return "in reading order based on the party name table";
                case SortingMethod.NameAlphabetical:
                    return "in alphabetical order by their names";
                case SortingMethod.ColorNumber:
                    return "by the number next to their color names";
                case SortingMethod.PartyLength:
                    return "by the length of their party names";
                case SortingMethod.ClockwiseBottomRight:
                    return "clockwise, starting from the bottom-right";
                case SortingMethod.PartyAlphabetical:
                    return "in alphabetical order by their party names";
                case SortingMethod.ScrabbleFirstThree:
                    return "by the Scrabble score of the first three letters of their names";
                case SortingMethod.ScrabbleFirstAndLast:
                    return "by the Scrabble score of the first and last letters of their names";
                case SortingMethod.NameLength:
                    return "by the length of their names";
                case SortingMethod.ClockwiseTopLeft:
                    return "clockwise, starting from the top-left";
                case SortingMethod.CopyLastCharacter:
                    return "the same way the last character voted";
                case SortingMethod.ReadingOrder:
                    return "in reading order";
                default:
                    throw new ArgumentOutOfRangeException("sm", sm, null);
            }
        }
    }
}
