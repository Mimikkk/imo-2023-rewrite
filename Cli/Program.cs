using Algorithms.Searches;
using BenchmarkDotNet.Running;
using Cli.Benchmarks;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;

BenchmarkRunner.Run<BenchmarkSearch>();

// BenchmarkRunner.Run<BenchmarkDomainCalculations>();

// var instance = Instance.Predefined.KroA100;
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
// for (var i = 0; i < 100; i++) {
//   var search = SearchType.GreedyLocal;
//   var configuration = new Searchable.Configuration {
//     Initializers = { (SearchType.Random, new(2, instance.Dimension)) },
//     Variant = "internal-edges",
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
// | Method |    Mean |    Error |   StdDev |     Min |     Max |       Gen0 | Average distance | Min distance | Max distance |      Gen1 | Allocated |
// |   KroA | 3.095 s | 0.0196 s | 0.0116 s | 3.078 s | 3.116 s | 39000.0000 |         43832.50 |     42989.00 |     44585.00 | 4000.0000 | 315.81 MB |
// |------- |--------:|---------:|---------:|--------:|--------:|-----------:|-----------------:|-------------:|-------------:|----------:|----------:|
// |   kroB | 3.050 s | 0.0253 s | 0.0151 s | 3.017 s | 3.072 s | 39000.0000 |         43902.30 |     43327.00 |     44306.00 | 4000.0000 | 315.82 MB |
