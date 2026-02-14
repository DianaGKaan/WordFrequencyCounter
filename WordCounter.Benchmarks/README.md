# WordCounter.Benchmarks

Performance benchmarks for the WordCounter application using [BenchmarkDotNet](https://benchmarkdotnet.org/).

## Why Benchmark?

The task requires minimizing running time and memory usage. Claims about performance are only meaningful when backed by measurements. This project validates that the architecture delivers on those requirements.

## Scenarios

| Benchmark | What it measures |
|-----------|-----------------|
| **SmallInput** | Baseline overhead for a short sentence (9 words) |
| **LargeInput_SingleSource** | Chunked tokenization performance on 100,000 words in a single line |
| **LargeInput_10Sources_Parallel** | Parallel processing benefit across 10 large sources |

## Running

From the solution root:

```
cd WordCounter.Benchmarks
dotnet run -c Release
```

**Important:** Always run benchmarks in Release mode. Debug mode disables compiler optimizations and produces misleading results.

The run takes 2-5 minutes. BenchmarkDotNet executes each scenario multiple times to produce statistically reliable measurements.

## Reading the Results

The output table includes:

- **Mean** - average execution time
- **Allocated** - total memory allocated (via `[MemoryDiagnoser]`)

Key things to look for:

- `LargeInput_10Sources_Parallel` should be significantly faster than 10× `LargeInput_SingleSource`, proving the parallel Map-Reduce approach works
- Memory allocation scales linearly with input size, confirming the streaming approach avoids loading entire files into memory