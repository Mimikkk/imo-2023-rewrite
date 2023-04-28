using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;
using LanguageExt;

namespace Algorithms.Searches.Implementations;

public class NearestNeighbourSearch : Search {
  protected override void Configure(Instance instance, Configuration configuration) =>
    configuration.Population = SearchType.Furthest.Search(instance, configuration);

  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      _ => Multiple(instance, configuration.Population)
    };

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    var offsets = instance.Nodes.ToDictionary(e => e, _ => 0);
    var used = population.Flatten().ToHashSet();

    var counter = used.Count;
    while (true) {
      foreach (var path in population) {
        var move = ClosestToEndpointMove.Find(instance, path, offsets, used);
        move.Apply();
        if (++counter == instance.Dimension) return population;
      }
    }
  }

  public NearestNeighbourSearch() : base(displayAs: DisplayType.Path) {
  }
}
