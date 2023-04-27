using System.Collections.Immutable;
using Domain.Extensions;
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
    return population;
  }
}
