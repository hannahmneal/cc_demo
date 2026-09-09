using CC_Demo.Models.Marvel;

namespace CC_Demo.Tests.Marvel;

public class MarvelResourceTypesTests
{
    [Theory]
    [InlineData("creators")]
    [InlineData("characters")]
    [InlineData("series")]
    [InlineData("stories")]
    [InlineData("comics")]
    [InlineData("events")]
    [InlineData("Creators")]
    [InlineData("COMICS")]
    public void IsValid_accepts_known_resource_types_case_insensitively(string resource)
    {
        Assert.True(MarvelResourceTypes.IsValid(resource));
    }

    [Theory]
    [InlineData("")]
    [InlineData("creator")]      // singular
    [InlineData("widgets")]
    [InlineData("comics ")]      // trailing space
    [InlineData("all")]
    public void IsValid_rejects_everything_else(string resource)
    {
        Assert.False(MarvelResourceTypes.IsValid(resource));
    }
}
