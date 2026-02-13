namespace WordCounter.Infrastructure;

using WordCounter.Domain;

public class BasicWordTokenizer : IWordTokenizer
{
    public IEnumerable<string> Tokenize(Stream stream)
    {
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            var words = line.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);

            foreach (var word in words)
            {
                yield return word.ToLowerInvariant();
            }
        }
    }
}