using WordCounter.Application;
using WordCounter.Infrastructure;

namespace WordCounter.Tests;

public class ParallelProcessingTests
{
    private WordCountOrchestrator CreateOrchestrator(int maxParallelism = 4)
    {
        return new WordCountOrchestrator(
            new ChunkedWordTokenizer(),
            new DictionaryAggregator(),
            maxDegreeOfParallelism: maxParallelism);
    }

    [Fact]
    public void Process_MultipleSources_ParallelResultMatchesSequential()
    {
        var sources = new[]
        {
            new StringTextSource("Go and do that thing more that you do not so well"),
            new StringTextSource("I do many things very well")
        };

        var sequential = new WordCountOrchestrator(
            new ChunkedWordTokenizer(),
            new DictionaryAggregator(),
            maxDegreeOfParallelism: 1);

        var parallel = CreateOrchestrator(maxParallelism: 4);

        var sequentialResult = sequential.Process(sources);
        var parallelResult = parallel.Process(sources);

        Assert.Equal(sequentialResult.Count, parallelResult.Count);
        foreach (var key in sequentialResult.Keys)
        {
            Assert.Equal(sequentialResult[key], parallelResult[key]);
        }
    }

    [Fact]
    public void Process_ManySources_CountsAreCorrect()
    {
        var sources = Enumerable.Range(0, 50)
            .Select(_ => new StringTextSource("hello world"))
            .ToArray();

        var orchestrator = CreateOrchestrator();
        var result = orchestrator.Process(sources);

        Assert.Equal(50, result["hello"]);
        Assert.Equal(50, result["world"]);
    }

    [Fact]
    public void Process_ManySourcesDifferentContent_AggregatesCorrectly()
    {
        var sources = new[]
        {
            new StringTextSource("alpha beta"),
            new StringTextSource("beta gamma"),
            new StringTextSource("gamma delta"),
            new StringTextSource("delta alpha"),
            new StringTextSource("alpha alpha")
        };

        var orchestrator = CreateOrchestrator();
        var result = orchestrator.Process(sources);

        Assert.Equal(4, result["alpha"]);
        Assert.Equal(2, result["beta"]);
        Assert.Equal(2, result["gamma"]);
        Assert.Equal(2, result["delta"]);
    }

    [Fact]
    public void Process_EmptySources_ReturnsEmptyDictionary()
    {
        var sources = Array.Empty<StringTextSource>();

        var orchestrator = CreateOrchestrator();
        var result = orchestrator.Process(sources);

        Assert.Empty(result);
    }

    [Fact]
    public void Process_MixOfEmptyAndNonEmptySources_CountsCorrectly()
    {
        var sources = new[]
        {
            new StringTextSource("hello"),
            new StringTextSource(""),
            new StringTextSource("hello world"),
            new StringTextSource(""),
            new StringTextSource("world")
        };

        var orchestrator = CreateOrchestrator();
        var result = orchestrator.Process(sources);

        Assert.Equal(2, result["hello"]);
        Assert.Equal(2, result["world"]);
    }
}