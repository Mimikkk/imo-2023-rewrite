using System.Collections.Immutable;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class WeightedRegretCycleExpansionSearch : Search {
  protected override void Initialize(Instance instance, Configuration configuration) =>
    configuration.Population = SearchType.Furthest.Search(instance, configuration);

  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      _ => Multiple(instance, configuration.Population, configuration.Regret!.Value, configuration.Weight!.Value)
    };

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population, int regret, float weight) {
    var usable = instance.Nodes.Except(population.Flatten()).ToHashSet();

    var counter = instance.Dimension - usable.Count;
    while (true) {
      foreach (var move in population.Select(path => WeightedRegretExpansionMove.Find(instance, path, usable, regret, weight))) {
        move.Apply();
        usable.Remove(move.Node);
        if (++counter == instance.Dimension) return population;
      }
    }
  }

  public WeightedRegretCycleExpansionSearch() : base(
    displayAs: DisplayType.Cycle,
    usesRegret: true,
    usesWeight: true
  ) {
  }
}
