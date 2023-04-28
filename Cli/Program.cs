using System.Collections.Immutable;
using Algorithms.Searches;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

var instance = Instance.Predefined.KroA100;
var search = SearchType.Random;
Shared.Random = new(0);

ImmutableArray<NodeList> cycles = new();
var elapsed = MeasurementMethods.MeasureAverage(() => {
  var configuration = new Searchable.Configuration(1, instance.Dimension)
    { Initializers = new() { SearchType.Furthest }, Start = 0 };

  cycles = search.Search(instance, configuration);
}, 1);

Console.WriteLine($"Elapsed time: in second {elapsed.TotalMilliseconds}[ms]");
Console.WriteLine($"Length: {instance.Distance[cycles]}");
