using System.Text;
using WordCounter.Infrastructure;

namespace WordCounter.Tests;

public class PunctuationTests
{
    private readonly ChunkedWordTokenizer _tokenizer = new();

    private Stream CreateStream(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }

    [Fact]
    public void Tokenize_TrailingComma_StripsIt()
    {
        using var stream = CreateStream("hello, world");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("hello", result[0]);
        Assert.Equal("world", result[1]);
    }

    [Fact]
    public void Tokenize_TrailingPeriod_StripsIt()
    {
        using var stream = CreateStream("end.");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Single(result);
        Assert.Equal("end", result[0]);
    }

    [Fact]
    public void Tokenize_Apostrophe_PreservesIt()
    {
        using var stream = CreateStream("don't won't can't");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("don't", result[0]);
        Assert.Equal("won't", result[1]);
        Assert.Equal("can't", result[2]);
    }

    [Fact]
    public void Tokenize_Hyphenated_PreservesIt()
    {
        using var stream = CreateStream("self-aware well-known");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("self-aware", result[0]);
        Assert.Equal("well-known", result[1]);
    }

    [Fact]
    public void Tokenize_QuotedWord_StripsQuotes()
    {
        using var stream = CreateStream("\"hello\" 'world'");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("hello", result[0]);
        Assert.Equal("world", result[1]);
    }

    [Fact]
    public void Tokenize_Parentheses_StripsThem()
    {
        using var stream = CreateStream("(hello) [world]");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("hello", result[0]);
        Assert.Equal("world", result[1]);
    }

    [Fact]
    public void Tokenize_MultiplePunctuation_StripsAll()
    {
        using var stream = CreateStream("wow!!! really???");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("wow", result[0]);
        Assert.Equal("really", result[1]);
    }

    [Fact]
    public void Tokenize_OnlyPunctuation_SkipsIt()
    {
        using var stream = CreateStream("hello ... world");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("hello", result[0]);
        Assert.Equal("world", result[1]);
    }

    [Fact]
    public void Tokenize_MixedPunctuationAndWords_HandlesCorrectly()
    {
        using var stream = CreateStream("Well, I don't know... maybe (yes)?");
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Equal(6, result.Count);
        Assert.Equal("well", result[0]);
        Assert.Equal("i", result[1]);
        Assert.Equal("don't", result[2]);
        Assert.Equal("know", result[3]);
        Assert.Equal("maybe", result[4]);
        Assert.Equal("yes", result[5]);
    }

    [Theory]
    [InlineData("hello,", "hello")]
    [InlineData("world.", "world")]
    [InlineData("(test)", "test")]
    [InlineData("\"quoted\"", "quoted")]
    [InlineData("don't", "don't")]
    [InlineData("self-aware", "self-aware")]
    public void Tokenize_SingleWord_ReturnsExpected(string input, string expected)
    {
        using var stream = CreateStream(input);
        var result = _tokenizer.Tokenize(stream).ToList();

        Assert.Single(result);
        Assert.Equal(expected, result[0]);
    }
}