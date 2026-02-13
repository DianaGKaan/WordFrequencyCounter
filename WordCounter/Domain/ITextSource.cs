namespace WordCounter.Domain;

public interface ITextSource
{
    Stream GetStream();
}