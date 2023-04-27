using Algorithms.Searches;
using Domain.Extensions;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;

var instance = Instance.Predefined.KroA100;
Shared.Random = new(42);
var nodes = instance.Nodes;

var timed = MeasurementMethods.MeasureAverage(() => {
  Enumerable.Range(0, 1000000).ForEach(() => {
    instance.Distance.Closest(nodes[0]);
  });
});
Console.WriteLine($"Elapsed time: in second {timed.TotalMilliseconds}[ms]");


// var search = SearchType.Identity;
// var configuration = new Searchable.Configuration(2, instance.Dimension);
//
// var elapsed = MeasurementMethods.Measure(() => search(instance, configuration));
//
// Console.WriteLine($"Elapsed time: in second {elapsed.TotalMilliseconds}[ms]");
