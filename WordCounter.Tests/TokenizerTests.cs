using System.Text;
using WordCounter.Infrastructure;

namespace WordCounter.Tests;

public class TokenizerTests
{
    private readonly BasicWordTokenizer _tokenizer = new();

    private Stream CreateStream(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }

    [Fact]
    public void Tokenize_BasicSentence_ReturnsSplitWords()
    {
        using var stream = CreateStream("hello world");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("hello", result[0]);
        Assert.Equal("world", result[1]);
    }

    [Fact]
    public void Tokenize_MixedCase_ReturnsLowercase()
    {
        using var stream = CreateStream("Hello WORLD hElLo");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.All(result, word => Assert.Equal(word, word.ToLowerInvariant()));
    }

    [Fact]
    public void Tokenize_MultipleWhitespace_SkipsEmpty()
    {
        using var stream = CreateStream("hello    world");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Tokenize_TabsAndNewlines_SplitsCorrectly()
    {
        using var stream = CreateStream("hello\tworld\nfoo");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("hello", result[0]);
        Assert.Equal("world", result[1]);
        Assert.Equal("foo", result[2]);
    }

    [Fact]
    public void Tokenize_LeadingAndTrailingWhitespace_TrimsCorrectly()
    {
        using var stream = CreateStream("  hello  ");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Single(result);
        Assert.Equal("hello", result[0]);
    }

    [Fact]
    public void Tokenize_EmptyStream_ReturnsNoWords()
    {
        using var stream = CreateStream("");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void Tokenize_MultipleLines_ReturnsAllWords()
    {
        using var stream = CreateStream("hello world\nfoo bar\nbaz");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(5, result.Count);
    }

    [Theory]
    [InlineData("one", 1)]
    [InlineData("one two three", 3)]
    [InlineData("  spaces  everywhere  ", 2)]
    public void Tokenize_VariousInputs_ReturnsExpectedCount(string input, int expectedCount)
    {
        using var stream = CreateStream(input);
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(expectedCount, result.Count);
    }
}