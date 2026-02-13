using System.Text;
using WordCounter.Infrastructure;

namespace WordCounter.Tests;

public class TextSourceTests
{
    [Fact]
    public void StringTextSource_GetStream_ReturnsStreamWithContent()
    {
        var source = new StringTextSource("hello world");

        using var stream = source.GetStream();
        using var reader = new StreamReader(stream);
        var result = reader.ReadToEnd();

        Assert.Equal("hello world", result);
    }

    [Fact]
    public void StringTextSource_EmptyString_ReturnsEmptyStream()
    {
        var source = new StringTextSource("");

        using var stream = source.GetStream();
        using var reader = new StreamReader(stream);
        var result = reader.ReadToEnd();

        Assert.Equal("", result);
    }

    [Fact]
    public void StringTextSource_GetStream_CanBeCalledMultipleTimes()
    {
        var source = new StringTextSource("hello");

        using var stream1 = source.GetStream();
        using var reader1 = new StreamReader(stream1);
        var result1 = reader1.ReadToEnd();

        using var stream2 = source.GetStream();
        using var reader2 = new StreamReader(stream2);
        var result2 = reader2.ReadToEnd();

        Assert.Equal(result1, result2);
    }

    [Fact]
    public void FileTextSource_NonExistentFile_ThrowsFileNotFoundException()
    {
        Assert.Throws<FileNotFoundException>(() =>
            new FileTextSource("nonexistent_file.txt"));
    }

    [Fact]
    public void FileTextSource_ValidFile_ReturnsStreamWithContent()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tempFile, "hello world");
            var source = new FileTextSource(tempFile);

            using var stream = source.GetStream();
            using var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            Assert.Equal("hello world", result);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }

    [Fact]
    public void FileTextSource_EmptyFile_ReturnsEmptyStream()
    {
        var tempFile = Path.GetTempFileName();
        try
        {
            var source = new FileTextSource(tempFile);

            using var stream = source.GetStream();
            using var reader = new StreamReader(stream);
            var result = reader.ReadToEnd();

            Assert.Equal("", result);
        }
        finally
        {
            File.Delete(tempFile);
        }
    }
}