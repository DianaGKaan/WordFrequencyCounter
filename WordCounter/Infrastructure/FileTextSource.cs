namespace WordCounter.Infrastructure;

using WordCounter.Domain;

public class FileTextSource : ITextSource
{
    private readonly string _filePath;

    public FileTextSource(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"File not found: {filePath}", filePath);
        }

        _filePath = filePath;
    }

    public Stream GetStream()
    {
        return new FileStream(
            _filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            FileOptions.SequentialScan);
    }
}