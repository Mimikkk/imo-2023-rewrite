using Algorithms.Searches;
using BenchmarkDotNet.Running;
using Cli.Benchmarks;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;

// BenchmarkRunner.Run<BenchmarkSearch>();
// BenchmarkRunner.Run<BenchmarkDomainCalculations>();

var instance = Instance.Predefined.KroA100;
var search = SearchType.GreedyLocal;
Shared.Random = new(999);

for (var i = 0; i < 100; i++) {
  var configuration = new Searchable.Configuration(2, instance.Dimension) {
    Start = i,
    Regret = 2,
    Weight = 0.38f,
    Initializers = new() { SearchType.WeightedRegretCycleExpansion },
    Variant = "internal-vertices"
  };

  var (elapsed, cycles) = MeasurementMethods.Measure(() => search.Search(instance, configuration));
  Console.WriteLine($"Elapsed: {elapsed.TotalMilliseconds}[ms]");
  Console.WriteLine($"Cycles: {instance.Distance[cycles]}");
}
