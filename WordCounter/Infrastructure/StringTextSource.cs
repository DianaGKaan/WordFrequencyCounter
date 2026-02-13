namespace WordCounter.Infrastructure;

using System.Text;
using WordCounter.Domain;

public class StringTextSource : ITextSource
{
    private readonly string _content;

    public StringTextSource(string content)
    {
        _content = content;
    }

    public Stream GetStream()
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(_content));
    }
}