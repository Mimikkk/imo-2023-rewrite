using Algorithms.Searches;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;

// BenchmarkRunner.Run<BenchmarkSearch>();
// BenchmarkRunner.Run<BenchmarkDomainCalculations>();

var instance = Instance.Predefined.KroA100;
Shared.Random = new(999);

for (var i = 0; i < 1; i++) {
  var search = SearchType.MemorableLocal;
  var configuration = new Searchable.Configuration(2, instance.Dimension) {
    Start = i,
    Regret = 2,
    Weight = 0.38f,
    Variant = "internal-edges",
    Initializers = new() { SearchType.Random }
  };

  var (elapsed, cycles) = MeasurementMethods.Measure(() => search.Search(instance, configuration));
  Console.WriteLine($"Elapsed: {elapsed.TotalMilliseconds}[ms]");
  Console.WriteLine($"Cycles: {instance.Distance[cycles]}");
  if (instance.Distance[cycles] > 100000) {
    Console.WriteLine("Something went wrong... too long.");
    break;
  }
}
for (var i = 0; i < 1; i++) {
  var search = SearchType.MemoryLocal;
  var configuration = new Searchable.Configuration(2, instance.Dimension) {
    Start = i,
    Regret = 2,
    Weight = 0.38f,
    Variant = "internal-edges",
    Initializers = new() { SearchType.Random }
  };

  var (elapsed, cycles) = MeasurementMethods.Measure(() => search.Search(instance, configuration));
  Console.WriteLine($"Elapsed: {elapsed.TotalMilliseconds}[ms]");
  Console.WriteLine($"Cycles: {instance.Distance[cycles]}");
  if (instance.Distance[cycles] > 100000) {
    Console.WriteLine("Something went wrong... too long.");
    break;
  }
}
