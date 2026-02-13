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
                toProcess = text;
                leftover = string.Empty;
            }
            else if (lastWhitespace == -1)
            {
                leftover = text;
                continue;
            }
            else
            {
                toProcess = text.Substring(0, lastWhitespace + 1);
                leftover = text.Substring(lastWhitespace + 1);
            }

            foreach (var word in ExtractWords(toProcess))
            {
                yield return word;
            }
        }

        if (!string.IsNullOrWhiteSpace(leftover))
        {
            foreach (var word in ExtractWords(leftover))
            {
                yield return word;
            }
        }
    }

    private static IEnumerable<string> ExtractWords(string text)
    {
        var words = text.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries);
        foreach (var word in words)
        {
            var cleaned = StripPunctuation(word).ToLowerInvariant();
            if (cleaned.Length > 0)
            {
                yield return cleaned;
            }
        }
    }

    private static string StripPunctuation(string word)
    {
        int start = 0;
        int end = word.Length - 1;

        while (start <= end && char.IsPunctuation(word[start]))
        {
            start++;
        }

        while (end >= start && char.IsPunctuation(word[end]))
        {
            end--;
        }

        return start > end ? string.Empty : word.Substring(start, end - start + 1);
    }
}