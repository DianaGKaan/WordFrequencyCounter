using BenchmarkDotNet.Attributes;
using WordCounter.Application;
using WordCounter.Infrastructure;

namespace WordCounter.Benchmarks;

[MemoryDiagnoser]
public class WordCountBenchmarks
{
    private StringTextSource _smallSource = null!;
    private StringTextSource _largeSource = null!;
    private StringTextSource[] _multipleSources = null!;
    private ChunkedWordTokenizer _tokenizer = null!;
    private DictionaryAggregator _aggregator = null!;

    [GlobalSetup]
    public void Setup()
    {
        _tokenizer = new ChunkedWordTokenizer();
        _aggregator = new DictionaryAggregator();

        // Small input — baseline
        _smallSource = new StringTextSource(
            "Go do that thing that you do so well");

        // Large single-line input — tests chunked reading performance
        var random = new Random(42);
        var words = new[] { "the", "quick", "brown", "fox", "jumps",
            "over", "lazy", "dog", "and", "the" };
        var largeText = string.Join(" ",
            Enumerable.Range(0, 100_000)
                .Select(_ => words[random.Next(words.Length)]));
        _largeSource = new StringTextSource(largeText);

        // Multiple sources — tests parallel processing benefit
        _multipleSources = Enumerable.Range(0, 10)
            .Select(_ => _largeSource)
            .ToArray();
    }

    [Benchmark(Baseline = true)]
    public Dictionary<string, long> SmallInput()
    {
        var orchestrator = new WordCountOrchestrator(_tokenizer, _aggregator);
        return orchestrator.Process(new[] { _smallSource });
    }

    [Benchmark]
    public Dictionary<string, long> LargeInput_SingleSource()
    {
        var orchestrator = new WordCountOrchestrator(_tokenizer, _aggregator);
        return orchestrator.Process(new[] { _largeSource });
    }

    [Benchmark]
    public Dictionary<string, long> LargeInput_10Sources_Parallel()
    {
        var orchestrator = new WordCountOrchestrator(_tokenizer, _aggregator);
        return orchestrator.Process(_multipleSources);
    }
}