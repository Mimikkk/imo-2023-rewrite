using Algorithms.Searches;
using BenchmarkDotNet.Running;
using Cli.Benchmarks;
using Domain.Calculations;
using Domain.Structures.Instances;

BenchmarkRunner.Run<BenchmarkSearch>();
// BenchmarkRunner.Run<BenchmarkDomainCalculations>();

// var instance = Instance.Predefined.KroA100;
// var search = SearchType.WeightedRegretCycleExpansion;
// for (var i = 0; i < 100; ++i) {
//   var configuration = new Searchable.Configuration(1, instance.Dimension) {
//     Start = i,
//     Regret = 2,
//     Weight = 0.38f
//   };
//   search.Search(instance, configuration);
// }
