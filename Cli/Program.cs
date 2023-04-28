using System.Collections.Immutable;
using Algorithms.Searches;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

var instance = Instance.Predefined.KroA200;
Shared.Random = new(0);


var search = SearchType.NearestNeighbour;

ImmutableArray<NodeList> cycles = new();
var elapsed = MeasurementMethods.MeasureAverage(() => {
  var configuration = new Searchable.Configuration(3, instance.Dimension)
    { Initializers = new() { SearchType.Furthest } };

  cycles = search.Search(instance, configuration);
}, 1);

Console.WriteLine($"Elapsed time: in second {elapsed.TotalMilliseconds}[ms]");
Console.WriteLine($"Length: {instance.Distance[cycles]}");
