using WordCounter.Application;
using WordCounter.Infrastructure;

namespace WordCounter.Tests;

public class OrchestratorTests
{
    private readonly WordCountOrchestrator _orchestrator;

    public OrchestratorTests()
    {
        _orchestrator = new WordCountOrchestrator(
            new BasicWordTokenizer(),
            new DictionaryAggregator());
    }

    [Fact]
    public void Process_TaskExampleInput_ReturnsExpectedCounts()
    {
        var sources = new[]
        {
            new StringTextSource("Go and do that thing more that you do not so well"),
            new StringTextSource("I do many things very well")
        };

        var result = _orchestrator.Process(sources);

        Assert.Equal(3, result["do"]);
        Assert.Equal(2, result["well"]);
        Assert.Equal(2, result["that"]);
        Assert.Equal(1, result["go"]);
        Assert.Equal(1, result["and"]);
        Assert.Equal(1, result["thing"]);
        Assert.Equal(1, result["more"]);
        Assert.Equal(1, result["you"]);
        Assert.Equal(1, result["not"]);
        Assert.Equal(1, result["so"]);
        Assert.Equal(1, result["i"]);
        Assert.Equal(1, result["many"]);
        Assert.Equal(1, result["things"]);
        Assert.Equal(1, result["very"]);
    }

    [Fact]
    public void Process_SingleSource_ReturnsCounts()
    {
        var sources = new[]
        {
            new StringTextSource("hello world hello")
        };

        var result = _orchestrator.Process(sources);

        Assert.Equal(2, result["hello"]);
        Assert.Equal(1, result["world"]);
    }

    [Fact]
    public void Process_EmptySources_ReturnsEmptyDictionary()
    {
        var sources = Array.Empty<StringTextSource>();

        var result = _orchestrator.Process(sources);

        Assert.Empty(result);
    }

    [Fact]
    public void Process_EmptyFileContent_ReturnsEmptyDictionary()
    {
        var sources = new[]
        {
            new StringTextSource("")
        };

        var result = _orchestrator.Process(sources);

        Assert.Empty(result);
    }

    [Fact]
    public void Process_MultipleSources_AggregatesAcrossAll()
    {
        var sources = new[]
        {
            new StringTextSource("hello"),
            new StringTextSource("hello"),
            new StringTextSource("hello world")
        };

        var result = _orchestrator.Process(sources);

        Assert.Equal(3, result["hello"]);
        Assert.Equal(1, result["world"]);
    }

    [Fact]
    public void Process_CaseInsensitive_CountsAsOneWord()
    {
        var sources = new[]
        {
            new StringTextSource("Hello HELLO hello")
        };

        var result = _orchestrator.Process(sources);

        Assert.Single(result);
        Assert.Equal(3, result["hello"]);
    }
}