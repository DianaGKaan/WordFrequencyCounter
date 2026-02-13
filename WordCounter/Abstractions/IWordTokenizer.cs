namespace WordCounter.Abstractions;

public interface IWordTokenizer
{
    IEnumerable<string> Tokenize(Stream stream);
}