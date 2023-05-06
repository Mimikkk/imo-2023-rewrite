using Algorithms.Searches;
using BenchmarkDotNet.Running;
using Cli.Benchmarks;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;

BenchmarkRunner.Run<BenchmarkSearch>();

// BenchmarkRunner.Run<BenchmarkDomainCalculations>();

// var instance = Instance.Predefined.KroA200;
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
// for (var i = 0; i < 1; i++) {
//   var search = SearchType.IteratedLocal;
//   var configuration = new Searchable.Configuration(2, instance.Dimension) {
//     Start = i,
//     TimeLimit = 10,
//     Variant = "big-perturbation",
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