using System;

namespace Assets
{
    internal enum SortingMethod
    {
        ColorAlphabetical,
        ColorLengthDescending,
        ColorReadingOrder,
        ClockwiseTopRight,
        ScrabbleLastThreeDescending,
        ClockwiseBottomLeft,
        PartyNumberDescending,
        PartyReadingOrder,
        NameAlphabetical,
        ColorNumberDescending,
        PartyLengthDescending,
        ClockwiseBottomRight,
        PartyAlphabetical,
        ScrabbleFirstThreeDescending,
        ScrabbleFirstAndLastDescending,
        NameLengthDescending,
        ClockwiseTopLeft,
        CopyLastCharacter,
        ReadingOrder,
        ColorReverseAlphabetical,
        ColorLengthAscending,
        ColorReverseReadingOrder,
        CounterTopRight,
        ScrabbleLastThreeAscending,
        CounterBottomLeft,
        PartyNumberAscending,
        PartyReverseReadingOrder,
        NameReverseAlphabetical,
        ColorNumberAscending,
        PartyLengthAscending,
        CounterBottomRight,
        PartyReverseAlphabetical,
        ScrabbleFirstThreeAscending,
        ScrabbleFirstAndLastAscending,
        NameLengthAscending,
        CounterTopLeft,
        ReverseReadingOrder
    }

    internal static class SortingMethodExtensions
    {
        public static string ToLogString(this SortingMethod sm)
        {
            switch (sm)
            {
                case SortingMethod.ColorAlphabetical:
                    return "in alphabetical order by their color names";
                case SortingMethod.ColorLengthDescending:
                    return "by the length of their color names";
                case SortingMethod.ColorReadingOrder:
                    return "in reading order based on the color name table";
                case SortingMethod.ClockwiseTopRight:
                    return "clockwise, starting from the top-right";
                case SortingMethod.ScrabbleLastThreeDescending:
                    return "by the Scrabble score of the last three letters of their names";
                case SortingMethod.ClockwiseBottomLeft:
                    return "clockwise, starting from the bottom-left";
                case SortingMethod.PartyNumberDescending:
                    return "by the number next to their party names";
                case SortingMethod.PartyReadingOrder:
                    return "in reading order based on the party name table";
                case SortingMethod.NameAlphabetical:
                    return "in alphabetical order by their names";
                case SortingMethod.ColorNumberDescending:
                    return "by the number next to their color names";
                case SortingMethod.PartyLengthDescending:
                    return "by the length of their party names";
                case SortingMethod.ClockwiseBottomRight:
                    return "clockwise, starting from the bottom-right";
                case SortingMethod.PartyAlphabetical:
                    return "in alphabetical order by their party names";
                case SortingMethod.ScrabbleFirstThreeDescending:
                    return "by the Scrabble score of the first three letters of their names";
                case SortingMethod.ScrabbleFirstAndLastDescending:
                    return "by the Scrabble score of the first and last letters of their names";
                case SortingMethod.NameLengthDescending:
                    return "by the length of their names";
                case SortingMethod.ClockwiseTopLeft:
                    return "clockwise, starting from the top-left";
                case SortingMethod.CopyLastCharacter:
                    return "the same way the last character voted";
                case SortingMethod.ReadingOrder:
                    return "in reading order";
                case SortingMethod.ColorReverseAlphabetical:
                    return "in reverse alphabetical order by their color names";
                case SortingMethod.ColorLengthAscending:
                    return "by the length of their color names (smaller)";
                case SortingMethod.ColorReverseReadingOrder:
                    return "in reverse reading order based on the color name table";
                case SortingMethod.CounterTopRight:
                    return "counterclockwise, starting from the top-right";
                case SortingMethod.ScrabbleLastThreeAscending:
                    return "by the Scrabble score of the last three letters of their names (smaller)";
                case SortingMethod.CounterBottomLeft:
                    return "counterclockwise, starting from the bottom-left";
                case SortingMethod.PartyNumberAscending:
                    return "by the number next to their party names (smaller)";
                case SortingMethod.PartyReverseReadingOrder:
                    return "in reverse reading order based on the party name table";
                case SortingMethod.NameReverseAlphabetical:
                    return "in reverse alphabetical order by their names";
                case SortingMethod.ColorNumberAscending:
                    return "by the number next to their color names (smaller)";
                case SortingMethod.PartyLengthAscending:
                    return "by the length of their party names (smaller)";
                case SortingMethod.CounterBottomRight:
                    return "counterclockwise, starting from the bottom-right";
                case SortingMethod.PartyReverseAlphabetical:
                    return "in reverse alphabetical order by their party names";
                case SortingMethod.ScrabbleFirstThreeAscending:
                    return "by the Scrabble score of the first three letters of their names (smaller)";
                case SortingMethod.ScrabbleFirstAndLastAscending:
                    return "by the Scrabble score of the first and last letters of their names (smaller)";
                case SortingMethod.NameLengthAscending:
                    return "by the length of their names (smaller)";
                case SortingMethod.CounterTopLeft:
                    return "counterclockwise, starting from the top-left";
                case SortingMethod.ReverseReadingOrder:
                    return "in reverse reading order";
                default:
                    throw new ArgumentOutOfRangeException("sm", sm, null);
            }
        }
    }
}
