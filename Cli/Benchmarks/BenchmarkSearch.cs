using Algorithms.Searches;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using Cli.Benchmarks.Columns;
using Domain.Shareable;
using Domain.Structures.Instances;
using static Algorithms.Searches.Searchable;

namespace Cli.Benchmarks;

[MemoryDiagnoser]
[Config(typeof(Config))]
[SimpleJob(iterationCount: BenchmarkMemory.Iterations, warmupCount: 24)]
public class BenchmarkSearch {
  public static Instance Instance => BenchmarkMemory.Instance;
  public static Configuration Configuration => BenchmarkMemory.Configuration;

  private class Config : ManualConfig {
    public Config() {
      AddColumn(StatisticColumn.Min);
      AddColumn(StatisticColumn.Max);
      AddColumn(DistanceColumn.Average);
      AddColumn(DistanceColumn.Min);
      AddColumn(DistanceColumn.Max);
    }
  }

  private readonly Callback Search = SearchType.Random;
  private Configuration _configuration = null!;

  private int _iteration;
  private int IterationOffset => _iteration - 26;
  private int Start => IterationOffset < 0 ? 0 : IterationOffset;

  [IterationSetup]
  public void Setup() {
    Shared.Random = new(++_iteration);
    _configuration = Configuration with { Start = Start };
  }

  [Benchmark]
  public void Test() => BenchmarkMemory.Results.Add(Search(Instance, _configuration));

  [GlobalCleanup]
  public void Cleanup() => BenchmarkMemory.Save();
}
