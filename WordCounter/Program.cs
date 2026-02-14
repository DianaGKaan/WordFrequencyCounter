using WordCounter.Application;
using WordCounter.Infrastructure;

if (args.Length == 0)
{
    Console.WriteLine("Usage: WordCounter <file1> <file2> ...");
    Console.WriteLine("Counts the occurrences of each unique word across multiple text files.");
    return;
}

var invalidFiles = args.Where(path => !File.Exists(path)).ToList();
if (invalidFiles.Count > 0)
{
    Console.Error.WriteLine("Error: The following files were not found:");
    foreach (var file in invalidFiles)
    {
        Console.Error.WriteLine($"  - {file}");
    }
    return;
}

try
{
    var tokenizer = new ChunkedWordTokenizer();
    var aggregator = new DictionaryAggregator();
    var orchestrator = new WordCountOrchestrator(tokenizer, aggregator);

    var sources = args.Select(path => new FileTextSource(path)).ToArray();

    var results = orchestrator.Process(sources);

    if (results.Count == 0)
    {
        Console.WriteLine("No words found in the provided files.");
        return;
    }

    foreach (var entry in results.OrderByDescending(e => e.Value))
    {
        Console.WriteLine($"{entry.Key.ToLowerInvariant()}: {entry.Value}");
    }
}
catch (IOException ex)
{
    Console.Error.WriteLine($"Error reading file: {ex.Message}");
}
catch (UnauthorizedAccessException ex)
{
    Console.Error.WriteLine($"Access denied: {ex.Message}");
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Unexpected error: {ex.Message}");
}