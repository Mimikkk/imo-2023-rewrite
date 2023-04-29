using Algorithms.Searches;
using Domain.Shareable;
using Domain.Structures.Instances;

var instance = Instance.Predefined.KroA100;
var search = SearchType.RegretCycleExpansion;
Shared.Random = new(0);

var configuration = new Searchable.Configuration(1, instance.Dimension)
  { Start = 0, Regret = 5 };


var start = DateTime.Now;
var cycles = search.Search(instance, configuration);
var end = DateTime.Now;
var elapsed = end - start;

Console.WriteLine($"Elapsed time: in second {elapsed.TotalMilliseconds:F2}[ms]");
Console.WriteLine($"Length: {instance.Distance[cycles]}");
