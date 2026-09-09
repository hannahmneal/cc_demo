namespace CC_Demo.Models.Marvel;

/// <summary>
/// The set of Marvel resource types multiplexed through the <c>resource</c> column of the <c>cc_demo</c> table.
/// </summary>
public static class MarvelResourceTypes
{
    public static readonly IReadOnlySet<string> All = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
    {
        "creators", "characters", "series", "stories", "comics", "events"
    };

    public static bool IsValid(string resource) => All.Contains(resource);
}
