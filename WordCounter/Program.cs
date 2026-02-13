using WordCounter.Application;
using WordCounter.Infrastructure;

if (args.Length == 0)
{
    Console.WriteLine("Usage: WordCounter <file1> <file2> ...");
    return;
}

var tokenizer = new BasicWordTokenizer();
var aggregator = new DictionaryAggregator();
var orchestrator = new WordCountOrchestrator(tokenizer, aggregator);

var sources = args.Select(path => new FileTextSource(path)).ToArray();

var results = orchestrator.Process(sources);

foreach (var entry in results.OrderByDescending(e => e.Value))
{
    Console.WriteLine($"{entry.Key}: {entry.Value}");
}