namespace WordCounter.Application;

using WordCounter.Domain;

public class WordCountOrchestrator
{
    private readonly IWordTokenizer _tokenizer;
    private readonly IResultAggregator _aggregator;

    public WordCountOrchestrator(IWordTokenizer tokenizer, IResultAggregator aggregator)
    {
        _tokenizer = tokenizer;
        _aggregator = aggregator;
    }

    public Dictionary<string, long> Process(IEnumerable<ITextSource> sources)
    {
        var allCounts = new List<Dictionary<string, long>>();

        foreach (var source in sources)
        {
            var localCounts = new Dictionary<string, long>();

            using var stream = source.GetStream();
            foreach (var word in _tokenizer.Tokenize(stream))
            {
                if (localCounts.ContainsKey(word))
                {
                    localCounts[word]++;
                }
                else
                {
                    localCounts[word] = 1;
                }
            }

            allCounts.Add(localCounts);
        }

        return _aggregator.Merge(allCounts);
    }
}