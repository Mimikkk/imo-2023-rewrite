using Algorithms.Searches;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using Cli.Benchmarks.Columns;
using Domain.Shareable;
using Domain.Structures.Instances;

namespace Cli.Benchmarks;

[MemoryDiagnoser]
[Config(typeof(Config))]
[SimpleJob(iterationCount: BenchmarkMemory.Iterations, warmupCount: 24)]
[GcForce(false)]
public class BenchmarkSearch {
  public static readonly Instance Instance = Instance.Predefined.KroA100;

  private class Config : ManualConfig {
    public Config() {
      AddColumn(StatisticColumn.Min);
      AddColumn(StatisticColumn.Max);
      AddColumn(DistanceColumn.Average);
      AddColumn(DistanceColumn.Min);
      AddColumn(DistanceColumn.Max);
    }
  }

  private readonly Search _search = SearchType.Random;
  private Searchable.Configuration _configuration = null!;

  private int _iteration;

  [IterationSetup]
  public void Setup() {
    _configuration = new(1, Instance.Dimension);
    Shared.Random = new(_iteration++);
  }

  [Benchmark]
  public void Test() => BenchmarkMemory.Results.Add(_search.Search(Instance, _configuration));

  [GlobalCleanup]
  public void Cleanup() => BenchmarkMemory.Save();
}
