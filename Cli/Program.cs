using Algorithms.Searches;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;

var instance = Instance.Predefined.KroA100;
Shared.Random = new(42);

var search = SearchType.Identity;
var configuration = new Searchable.Configuration(2, instance.Dimension);

var elapsed = MeasurementMethods.Measure(() => search(instance, configuration));

Console.WriteLine($"Elapsed time: in second {elapsed.TotalMilliseconds}[ms]");
