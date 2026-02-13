using WordCounter.Infrastructure;

namespace WordCounter.Tests;

public class AggregatorTests
{
    private readonly DictionaryAggregator _aggregator = new();

    [Fact]
    public void Merge_TwoDictionariesWithOverlappingKeys_SumsValues()
    {
        var counts = new List<Dictionary<string, long>>
        {
            new() { { "hello", 2 }, { "world", 1 } },
            new() { { "hello", 3 }, { "foo", 1 } }
        };

        var result = _aggregator.Merge(counts);

        Assert.Equal(5, result["hello"]);
        Assert.Equal(1, result["world"]);
        Assert.Equal(1, result["foo"]);
    }

    [Fact]
    public void Merge_NoOverlappingKeys_CombinesAll()
    {
        var counts = new List<Dictionary<string, long>>
        {
            new() { { "hello", 1 } },
            new() { { "world", 2 } }
        };

        var result = _aggregator.Merge(counts);

        Assert.Equal(2, result.Count);
        Assert.Equal(1, result["hello"]);
        Assert.Equal(2, result["world"]);
    }

    [Fact]
    public void Merge_OneEmptyDictionary_ReturnsOther()
    {
        var counts = new List<Dictionary<string, long>>
        {
            new() { { "hello", 3 } },
            new()
        };

        var result = _aggregator.Merge(counts);

        Assert.Single(result);
        Assert.Equal(3, result["hello"]);
    }

    [Fact]
    public void Merge_AllEmptyDictionaries_ReturnsEmpty()
    {
        var counts = new List<Dictionary<string, long>>
        {
            new(),
            new()
        };

        var result = _aggregator.Merge(counts);

        Assert.Empty(result);
    }

    [Fact]
    public void Merge_EmptyList_ReturnsEmptyDictionary()
    {
        var counts = new List<Dictionary<string, long>>();

        var result = _aggregator.Merge(counts);

        Assert.Empty(result);
    }

    [Fact]
    public void Merge_MultipleDictionaries_SumsCorrectly()
    {
        var counts = new List<Dictionary<string, long>>
        {
            new() { { "do", 2 }, { "well", 1 } },
            new() { { "do", 1 }, { "well", 1 }, { "things", 1 } },
            new() { { "do", 3 }, { "things", 2 } }
        };

        var result = _aggregator.Merge(counts);

        Assert.Equal(6, result["do"]);
        Assert.Equal(2, result["well"]);
        Assert.Equal(3, result["things"]);
    }

    [Fact]
    public void Merge_SingleDictionary_ReturnsSameValues()
    {
        var counts = new List<Dictionary<string, long>>
        {
            new() { { "hello", 5 }, { "world", 3 } }
        };

        var result = _aggregator.Merge(counts);

        Assert.Equal(2, result.Count);
        Assert.Equal(5, result["hello"]);
        Assert.Equal(3, result["world"]);
    }
}