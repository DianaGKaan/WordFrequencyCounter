namespace WordCounter.Infrastructure;

using WordCounter.Domain;

public class DictionaryAggregator : IResultAggregator
{
    public Dictionary<string, long> Merge(IEnumerable<Dictionary<string, long>> counts)
    {
        var result = new Dictionary<string, long>();

        foreach (var dictionary in counts)
        {
            foreach (var entry in dictionary)
            {
                if (result.ContainsKey(entry.Key))
                {
                    result[entry.Key] += entry.Value;
                }
                else
                {
                    result[entry.Key] = entry.Value;
                }
            }
        }

        return result;
    }
}