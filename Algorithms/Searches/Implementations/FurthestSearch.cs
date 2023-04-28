using System.Collections.Immutable;
using Domain.Calculations;
using Domain.Extensions;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class FurthestSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      1 => Single(instance, configuration.Population),
      2 => Double(instance, configuration.Population),
      _ => Multiple(instance, configuration.Population)
    };


  private static ImmutableArray<NodeList> Single(Instance instance, ImmutableArray<NodeList> population) {
    var choice = Shared.Random.Choose(instance.Nodes);
    var first = population[0];
    first.Add(choice);
    first.Notify();
    return population;
  }

  private static ImmutableArray<NodeList> Double(Instance instance, ImmutableArray<NodeList> population) {
    var choice = Shared.Random.Choose(instance.Nodes);
    var first = population[0];
    var second = population[1];

    first.Add(choice);
    first.Notify();

    choice = instance.Distance.Furthest(choice);
    second.Add(choice);
    second.Notify();

    return population;
  }

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    foreach (var (cycle, node) in population.Zip(NodesCalculations.Hull(instance.Nodes)
                 .Combinations(population.Length)
                 .MaxBy(nodes =>
                   NodesCalculations.Edges(nodes).Sum(edge => instance.Distance[edge]))!
             )) {
      cycle.Add(node);
      cycle.Notify();
    }

    return population;
  }
}
