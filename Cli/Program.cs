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
// | Method    |    Mean |    Error |   StdDev |     Min |     Max |        Gen0 | Average distance | Min distance | Max distance | Average iterations | Min iterations | Max iterations |
// |---------- |--------:|---------:|---------:|--------:|--------:|----------- :|-----------------:|-------------:|-------------:|-------------------:|---------------:|---------------:|
// | kroA-MSLS | 3.365 s | 0.0203 s | 0.0134 s | 3.344 s | 3.383 s | 40000.0000  |         35901.10 |     35416.00 |     36656.00 |                    |                |                |
// | kroB-MSLS | 3.419 s | 0.0492 s | 0.0326 s | 3.371 s | 3.477 s | 39000.0000  |         36264.00 |     35556.00 |     36740.00 |                    |                |                |
// | kroA-ILS1 | 3.510 s | 0.0074 s | 0.0049 s | 3.502 s | 3.519 s | 1000.0000   |         33728.10 |     32436.00 |     35052.00 |             188.40 |         193.00 |         196.00 |
// | kroB-ILS1 | 3.511 s | 0.0110 s | 0.0073 s | 3.502 s | 3.520 s | 1000.0000   |         34350.30 |     33238.00 |     35282.00 |             191.60 |         195.00 |         188.00 |
// | kroA-ILS2 | 3.555 s | 0.0156 s | 0.0081 s | 3.541 s | 3.566 s | 371000.0000 |         37035.70 |     35790.00 |     38526.00 |              60.70 |          59.00 |          62.00 |
// | kroB-ILS2 | 3.567 s | 0.0261 s | 0.0173 s | 3.539 s | 3.591 s | 375000.0000 |         37036.00 |     35979.00 |     38633.00 |              61.40 |          61.00 |          63.00 |
