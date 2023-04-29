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

  private readonly Search _search = SearchType.WeightedRegretCycleExpansion;
  private Searchable.Configuration _configuration = null!;

  private int _iteration;
  private const int IterationOffset = 26;

  [IterationSetup]
  public void Setup() {
    _configuration = new(2, Instance.Dimension) {
      Start = _iteration < IterationOffset ? null : _iteration - IterationOffset,
      Regret = 2,
      Weight = 0.38f
    };
    Shared.Random = new(_iteration++);
  }

  [Benchmark]
  public void Test() => BenchmarkMemory.Results.Add(_search.Search(Instance, _configuration));

  [GlobalCleanup]
  public void Cleanup() => BenchmarkMemory.Save();
}
