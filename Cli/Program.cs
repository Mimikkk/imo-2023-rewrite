using Algorithms.Searches;
using Algorithms.Searches.Implementations;
using BenchmarkDotNet.Running;
using Charts.Extensions;
using Cli.Benchmarks;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;

// BenchmarkRunner.Run<BenchmarkSearch>();

var instance = Instance.Predefined.KroA100;


// BenchmarkRunner.Run<BenchmarkDomainCalculations>();

// Shared.Random = new(999);

// JIT
// for (var i = 0; i < 10; i++) {
//   var search = SearchType.IteratedLocal;
//   var configuration = new Searchable.Configuration(2, instance.Dimension) {
//     Start = i,
//     IterationLimit = 2
//   };
//
//   search.Search(instance, configuration);
// }
//
for (var i = 0; i < 1; i++) {
  var search = SearchType.Evolutionary;
  var configuration = new Searchable.Configuration(2, instance.Dimension) {
    Variant = (int?)EvolutionarySearch.Variant.Local,
    TimeLimit = 100f,
  };

  var (elapsed, cycles) = MeasurementMethods.Measure(() => search.Search(instance, configuration));
  Console.WriteLine($"Elapsed: {elapsed.TotalMilliseconds}[ms]");
  Console.WriteLine($"Cycles: {instance.Distance[cycles]}");
  if (instance.Distance[cycles] > 100000) {
    Console.WriteLine("Something went wrong... too long.");
    break;
  }
}
// | Method    |    Mean |     Min |     Max | Average d |    Min d |    Max d | Avg i  | Min i  |  Max i  |
// |---------- |--------:|--------:|--------:|----------:|---------:|---------:|-------:|-------:|--------:|
// | kroA-MSLS | 3.565 s | 3.344 s | 3.383 s |  35901.10 | 35416.00 | 36656.00 |        |        |         |
// | kroB-MSLS | 3.519 s | 3.371 s | 3.477 s |  36264.00 | 35556.00 | 36740.00 |        |        |         |
// | kroA-ILS1 | 3.510 s | 3.502 s | 3.519 s |  33728.10 | 32436.00 | 35052.00 | 188.40 | 193.00 |  196.00 |
// | kroB-ILS1 | 3.511 s | 3.502 s | 3.520 s |  34350.30 | 33238.00 | 35282.00 | 191.60 | 195.00 |  188.00 |
// | kroA-ILS2a | 3.555 s | 3.541 s | 3.566 s |  37035.70 | 35790.00 | 38526.00 |  60.70 |  59.00 |   62.00 |
// | kroB-ILS2a | 3.567 s | 3.539 s | 3.591 s |  37036.00 | 35979.00 | 38633.00 |  61.40 |  61.00 |   63.00 |
// | kroA-ILS2 |         33470,30 |     32456,00 |     34212,00 |              61,00 |          61,00 |          61,00 |
// | kroB-ILS2 |         33846,20 |     33109,00 |     34759,00 |              56,50 |          57,00 |          56,00 |
