using System.Collections.Immutable;
using Domain.Calculations;
using Domain.Extensions;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class FurthestSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      1 => Single(instance, configuration.Population, configuration.Start),
      2 => Double(instance, configuration.Population, configuration.Start),
      _ => Multiple(instance, configuration.Population)
    };


  private static ImmutableArray<NodeList> Single(Instance instance, ImmutableArray<NodeList> population, int? start) {
    var choice = start is null ? Shared.Random.Choose(instance.Nodes) : instance.Nodes[start.Value];
    
    AttachMove.Apply(population[0], choice);

    return population;
  }

  private static ImmutableArray<NodeList> Double(Instance instance, ImmutableArray<NodeList> population, int? start) {
    var choice = start is null ? Shared.Random.Choose(instance.Nodes) : instance.Nodes[start.Value];
    var furthest = instance.Distance.Furthest(choice);

    AttachMove.Apply(population[0], choice);
    AttachMove.Apply(population[1], furthest);
    
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
