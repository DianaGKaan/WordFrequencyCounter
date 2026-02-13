namespace WordCounter.Infrastructure;

using WordCounter.Domain;

public class ChunkedWordTokenizer : IWordTokenizer
{
    private readonly int _bufferSize;

    public ChunkedWordTokenizer(int bufferSize = 4096)
    {
        _bufferSize = bufferSize;
    }

    public IEnumerable<string> Tokenize(Stream stream)
    {
        using var reader = new StreamReader(stream);
        var buffer = new char[_bufferSize];
        var leftover = string.Empty;

        int charsRead;
        while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            var chunk = new string(buffer, 0, charsRead);
            var text = leftover + chunk;

            int lastWhitespace = -1;
            for (int i = text.Length - 1; i >= 0; i--)
            {
                if (char.IsWhiteSpace(text[i]))
                {
                    lastWhitespace = i;
                    break;
                }
            }

            string toProcess;
            if (charsRead < buffer.Length)
            {
                // Last chunk — process everything
                toProcess = text;
                leftover = string.Empty;
            }
            else if (lastWhitespace == -1)
            {
                // No whitespace found — entire chunk is one partial word
                leftover = text;
                continue;
            }
            else
            {
                toProcess = text.Substring(0, lastWhitespace + 1);
                leftover = text.Substring(lastWhitespace + 1);
            }

            var words = toProcess.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                yield return word.ToLowerInvariant();
            }
        }

        // Handle any remaining leftover
        if (!string.IsNullOrWhiteSpace(leftover))
        {
            var words = leftover.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
            foreach (var word in words)
            {
                yield return word.ToLowerInvariant();
            }
        }
    }
}