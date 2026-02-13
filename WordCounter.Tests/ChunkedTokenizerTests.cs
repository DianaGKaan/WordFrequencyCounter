using System.Text;
using WordCounter.Infrastructure;

namespace WordCounter.Tests;

public class ChunkedTokenizerTests
{
    private Stream CreateStream(string content)
    {
        return new MemoryStream(Encoding.UTF8.GetBytes(content));
    }

    [Fact]
    public void Tokenize_BasicSentence_ReturnsSplitWords()
    {
        var tokenizer = new ChunkedWordTokenizer();
        using var stream = CreateStream("hello world");
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
        Assert.Equal("hello", result[0]);
        Assert.Equal("world", result[1]);
    }

    [Fact]
    public void Tokenize_MixedCase_ReturnsLowercase()
    {
        var tokenizer = new ChunkedWordTokenizer();
        using var stream = CreateStream("Hello WORLD hElLo");
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.All(result, word => Assert.Equal(word, word.ToLowerInvariant()));
    }

    [Fact]
    public void Tokenize_EmptyStream_ReturnsNoWords()
    {
        var tokenizer = new ChunkedWordTokenizer();
        using var stream = CreateStream("");
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.Empty(result);
    }

    [Fact]
    public void Tokenize_WordSplitAcrossChunkBoundary_ReturnsWholeWord()
    {
        // Buffer size 8: "hello wo" | "rld foo"
        // "world" spans the boundary
        var tokenizer = new ChunkedWordTokenizer(bufferSize: 8);
        using var stream = CreateStream("hello world foo");
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("hello", result[0]);
        Assert.Equal("world", result[1]);
        Assert.Equal("foo", result[2]);
    }

    [Fact]
    public void Tokenize_VerySmallBuffer_StillReturnsCorrectWords()
    {
        var tokenizer = new ChunkedWordTokenizer(bufferSize: 4);
        using var stream = CreateStream("the quick brown fox");
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.Equal(4, result.Count);
        Assert.Equal("the", result[0]);
        Assert.Equal("quick", result[1]);
        Assert.Equal("brown", result[2]);
        Assert.Equal("fox", result[3]);
    }

    [Fact]
    public void Tokenize_SingleLongLine_ReturnsAllWords()
    {
        var words = Enumerable.Range(0, 10000).Select(i => $"word{i}");
        var longLine = string.Join(" ", words);

        var tokenizer = new ChunkedWordTokenizer(bufferSize: 128);
        using var stream = CreateStream(longLine);
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.Equal(10000, result.Count);
        Assert.Equal("word0", result[0]);
        Assert.Equal("word9999", result[9999]);
    }

    [Fact]
    public void Tokenize_WordLongerThanBuffer_ReturnsWholeWord()
    {
        var longWord = new string('a', 20);
        var input = $"hello {longWord} world";

        var tokenizer = new ChunkedWordTokenizer(bufferSize: 8);
        using var stream = CreateStream(input);
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.Equal(3, result.Count);
        Assert.Equal("hello", result[0]);
        Assert.Equal(longWord, result[1]);
        Assert.Equal("world", result[2]);
    }

    [Fact]
    public void Tokenize_MultipleWhitespace_SkipsEmpty()
    {
        var tokenizer = new ChunkedWordTokenizer(bufferSize: 8);
        using var stream = CreateStream("hello    world");
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Tokenize_TabsAndNewlines_SplitsCorrectly()
    {
        var tokenizer = new ChunkedWordTokenizer();
        using var stream = CreateStream("hello\tworld\nfoo");
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.Equal(3, result.Count);
    }

    [Fact]
    public void Tokenize_TrailingWhitespace_NoEmptyWords()
    {
        var tokenizer = new ChunkedWordTokenizer(bufferSize: 8);
        using var stream = CreateStream("hello world   ");
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.Equal(2, result.Count);
    }

    [Fact]
    public void Tokenize_OnlyWhitespace_ReturnsNoWords()
    {
        var tokenizer = new ChunkedWordTokenizer(bufferSize: 8);
        using var stream = CreateStream("     \t\n   ");
        var result = tokenizer.Tokenize(stream).ToList();

        Assert.Empty(result);
    }
}