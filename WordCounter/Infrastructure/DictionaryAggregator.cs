namespace WordCounter.Infrastructure;

using WordCounter.Domain;

public class DictionaryAggregator : IResultAggregator
{
    public Dictionary<string, long> Merge(IEnumerable<Dictionary<string, long>> counts)
    {
        var result = new Dictionary<string, long>(StringComparer.OrdinalIgnoreCase);

        foreach (var dictionary in counts)
        {
            foreach (var entry in dictionary)
            {
                if (result.TryGetValue(entry.Key, out var existingCount))
                    result[entry.Key] = existingCount + entry.Value;
                else
                    result[entry.Key] = entry.Value;
            }
        }

        return result;
    }
}