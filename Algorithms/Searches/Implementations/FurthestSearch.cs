using System.Collections.Immutable;
using Domain.Calculations;
using Domain.Extensions;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches.Implementations;

public class FurthestSearch : Search {
  protected override ImmutableArray<List<Node>> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      1 => Single(instance, configuration.Population),
      2 => Double(instance, configuration.Population),
      _ => Multiple(instance, configuration.Population)
    };


  private static ImmutableArray<List<Node>> Single(Instance instance, ImmutableArray<List<Node>> population) {
    var choice = Shared.Random.Choose(instance.Nodes);
    population.First().Add(choice);
    return population;
  }

  private static ImmutableArray<List<Node>> Double(Instance instance, ImmutableArray<List<Node>> population) {
    var choice = Shared.Random.Choose(instance.Nodes);
    population.First().Add(choice);
    choice = instance.Distance.Furthest(choice);
    population.Last().Add(choice);
    return population;
  }

  private static ImmutableArray<List<Node>> Multiple(Instance instance, ImmutableArray<List<Node>> population) {
    foreach (var (cycle, node) in population.Zip(NodesCalculations.Hull(instance.Nodes)
                 .Combinations(population.Length)
                 .MaxBy(nodes =>
                   NodesCalculations.Edges(nodes).Sum(edge => instance.Distance[edge]))!
             )) cycle.Add(node);

    return population;
  }
}
