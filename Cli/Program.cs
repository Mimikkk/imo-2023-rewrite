using System.Collections.Immutable;
using Algorithms.Searches;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures;
using Domain.Structures.Instances;

var instance = Instance.Predefined.KroA200;
Shared.Random = new(42);


var search = SearchType.Identity;
var configuration = new Searchable.Configuration(2, instance.Dimension);

ImmutableArray<List<Node>> cycles = new();
var elapsed = MeasurementMethods.Measure(() => cycles = search(instance, configuration));

Console.WriteLine($"Elapsed time: in second {elapsed.TotalMilliseconds}[ms]");
Console.WriteLine($"Length: {instance.Distance[cycles]}");
