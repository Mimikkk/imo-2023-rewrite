using Algorithms.Searches;
using Domain.Shareable;
using Domain.Structures.Instances;

// BenchmarkRunner.Run<BenchmarkSearch>();
// BenchmarkRunner.Run<BenchmarkDomainCalculations>();

var instance = Instance.Predefined.KroA100;
var search = SearchType.GreedyLocal;
Shared.Random = new(999);

var configuration = new Searchable.Configuration(1, instance.Dimension) {
  Start = 0,
  Regret = 2,
  Weight = 0.38f
};
search.Search(instance, configuration);
