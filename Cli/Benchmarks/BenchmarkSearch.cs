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
[SimpleJob(iterationCount: BenchmarkMemory.Iterations, warmupCount: 4)]
public class BenchmarkSearch {


  [IterationSetup]
  public void Setup() {
    _configuration = Configuration;
    Shared.Random = new(Guid.NewGuid().GetHashCode());
  }

  [Benchmark]
  public void Test() {
    var cycles = Search(Instance, _configuration);
    var iterations = _configuration.Memo["iterations"];

    BenchmarkMemory.Results.Add((cycles, iterations));
  }

  [GlobalCleanup]
  public void Cleanup() => BenchmarkMemory.Save();

  private Configuration _configuration = null!;
  private static Instance Instance => BenchmarkMemory.Instance;
  private static Configuration Configuration => BenchmarkMemory.Configuration;
  private static Callback Search => BenchmarkMemory.Search;

  private class Config : ManualConfig {
    public Config() {
      AddColumn(StatisticColumn.Min);
      AddColumn(StatisticColumn.Max);
      AddColumn(DistanceColumn.Average);
      AddColumn(DistanceColumn.Min);
      AddColumn(DistanceColumn.Max);
      AddColumn(IterationColumn.Average);
      AddColumn(IterationColumn.Min);
      AddColumn(IterationColumn.Max);
    }
  }
}
