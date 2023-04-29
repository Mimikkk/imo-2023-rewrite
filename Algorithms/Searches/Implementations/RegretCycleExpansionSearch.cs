using System.Collections.Immutable;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class RegretCycleExpansionSearch : Search {
  protected override void Initialize(Instance instance, Configuration configuration) =>
    configuration.Population = SearchType.Furthest.Search(instance, configuration);

  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      _ => Multiple(instance, configuration.Population, configuration.Regret!.Value)
    };

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population, int regret) {
    var usable = instance.Nodes.Except(population.Flatten()).ToHashSet();

    var counter = instance.Dimension - usable.Count;
    while (true) {
      foreach (var move in population.Select(path => RegretExpansionMove.Find(instance, path, usable, regret))) {
        move.Apply();
        usable.Remove(move.Node);
        if (++counter == instance.Dimension) return population;
      }
    }
  }

  public RegretCycleExpansionSearch() : base(
    displayAs: DisplayType.Cycle,
    usesRegret: true
  ) {
  }
}
