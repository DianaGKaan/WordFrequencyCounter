namespace WordCounter.Domain;

public interface IWordTokenizer
{
    IEnumerable<string> Tokenize(Stream stream);
}