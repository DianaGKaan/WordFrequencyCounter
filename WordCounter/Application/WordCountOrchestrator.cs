namespace WordCounter.Application;

using System.Collections.Concurrent;
using WordCounter.Domain;

public class WordCountOrchestrator
{
    private readonly IWordTokenizer _tokenizer;
    private readonly IResultAggregator _aggregator;
    private readonly int _maxDegreeOfParallelism;

    public WordCountOrchestrator(
        IWordTokenizer tokenizer,
        IResultAggregator aggregator,
        int maxDegreeOfParallelism = -1)
    {
        _tokenizer = tokenizer;
        _aggregator = aggregator;
        _maxDegreeOfParallelism = maxDegreeOfParallelism == -1
            ? Environment.ProcessorCount
            : maxDegreeOfParallelism;
    }

    public Dictionary<string, long> Process(IEnumerable<ITextSource> sources)
    {
        var sourceList = sources.ToList();
        var allCounts = new ConcurrentBag<Dictionary<string, long>>();

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = _maxDegreeOfParallelism
        };

        Parallel.ForEach(sourceList, options, source =>
        {
            var localCounts = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

            using var stream = source.GetStream();
            foreach (var word in _tokenizer.Tokenize(stream))
            {
                if (!localCounts.TryGetValue(word, out var count))
                    localCounts[word] = 1;
                else
                    localCounts[word] = count + 1;
            }

            allCounts.Add(localCounts);
        });

        return _aggregator.Merge(allCounts);
    }
}