namespace Refresh.Common.Helpers;

public static class StringHelper
{
    /// <summary>
    /// Formats the given list of strings into a nice, readable listing.
    /// If the list has more than one string, the output will be, for example:
    /// "StringA, StringB, StringC and StringD";
    /// all but the last string will be joined by commas, and the last one with "and".
    /// <br/><br/>
    /// If only one string is given, it'll be returned,
    /// and if none are given, an empty string is returned.
    /// </summary>
    public static string FormatListingOfStrings(List<string> strings)
    {
        if (strings.Count <= 0)
            return "";

        return strings.Count > 1 
            ? $"{string.Join(", ", strings.SkipLast(1))} and {strings.Last()}"
            : strings.First();
    }
}