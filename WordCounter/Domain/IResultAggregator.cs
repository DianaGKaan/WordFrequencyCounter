namespace WordCounter.Domain;

public interface IResultAggregator
{
    Dictionary<string, long> Merge(IEnumerable<Dictionary<string, long>> counts);
}
