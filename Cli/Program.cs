using Algorithms.Searches;
using BenchmarkDotNet.Running;
using Cli.Benchmarks;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.Moves;

BenchmarkRunner.Run<BenchmarkSearch>();
// BenchmarkRunner.Run<BenchmarkDomainCalculations>();

// var instance = Instance.Predefined.KroA100;
// var search = SearchType.SteepestLocal;
// Shared.Random = new(999);
//
// for (var i = 0; i < 100; i++) {
//   var configuration = new Searchable.Configuration(2, instance.Dimension) {
//     Start = i,
//     Regret = 2,
//     Weight = 0.38f,
//     Variant = "mixed",
//     Initializers = new() { SearchType.WeightedRegretCycleExpansion }
//   };
//
//   var (elapsed, cycles) = MeasurementMethods.Measure(() => search.Search(instance, configuration));
//   Console.WriteLine($"Elapsed: {elapsed.TotalMilliseconds}[ms]");
//   Console.WriteLine($"Cycles: {instance.Distance[cycles]}");
//   if (instance.Distance[cycles] > 100000) {
//     Console.WriteLine("Something went wrong... too long.");
//     break;
//   }
// }
