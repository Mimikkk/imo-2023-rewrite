using BenchmarkDotNet.Attributes;
using Domain.Calculations;
using Domain.Shareable;

namespace Cli.Benchmarks;

[MemoryDiagnoser, SimpleJob(iterationCount: BenchmarkMemory.Iterations, warmupCount: 24)]
public class BenchmarkDomainCalculations {
  private List<int> _list = null!;

  [Params(1, 2, 3, 4, 5, 6, 7, 8, 9, 10)]
  public int Regret { get; set; }

  private int _iteration;

  [IterationSetup]
  public void Setup() {
    Shared.Random = new(_iteration++);
    _list = Enumerable.Range(0, 100).Select(_ => Shared.Random.Next()).OrderDescending().ToList();
  }

  [Benchmark]
  public void TestRegret() => DomainCalculations.Regret(_list, Regret);
}
