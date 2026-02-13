namespace WordCounter.Abstractions;

public interface ITextSource
{
    Stream GetStream();
}