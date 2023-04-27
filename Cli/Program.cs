using System.Collections.Immutable;
using Algorithms.Searches;
using Domain.Shareable;
using Domain.Structures;
using Domain.Structures.Instances;

var instance = Instance.Predefined.KroA100;
Shared.Random = new(42);

var search = SearchType.Identity;
var configuration = Searchable.Configure with {
  Population = new List<IList<Node>>().ToImmutableArray()
};

var elapsed = MeasurementMethods.Measure(() => search(instance, configuration));

Console.WriteLine($"Elapsed time: in second {elapsed.TotalSeconds:F2}");
