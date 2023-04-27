using System.Collections.Immutable;
using Algorithms.Searches;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures;
using Domain.Structures.Instances;

var instance = Instance.Predefined.KroA200;
Shared.Random = new(0);


var search = SearchType.NearestNeighbour;
var configuration = new Searchable.Configuration(1, instance.Dimension)
  { Initializers = new() { SearchType.Furthest } };

ImmutableArray<List<Node>> cycles = new();
var elapsed = MeasurementMethods.Measure(() => cycles = search.Search(instance, configuration));

Console.WriteLine($"Elapsed time: in second {elapsed.TotalMilliseconds}[ms]");
Console.WriteLine($"Length: {instance.Distance[cycles]}");
