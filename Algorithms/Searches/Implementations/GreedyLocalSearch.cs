using System.Collections.Immutable;
using Domain.Extensions;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class GreedyLocalSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Variant switch {
      "internal-edges" => InternalEdges(instance, configuration.Population),
      "internal-vertices" => InternalEdges(instance, configuration.Population),
      "external-vertices" => ExternalVertices(instance, configuration.Population),
      "vertices" => InternalEdges(instance, configuration.Population),
      "internal-edges-external-vertices" => InternalEdges(instance, configuration.Population),
      "mixed" => InternalEdges(instance, configuration.Population),
    };

  private static ImmutableArray<NodeList> InternalEdges(Instance instance, ImmutableArray<NodeList> population) {
    while (true) {
      var (move, gain) =
        population.SelectMany(ExchangeInternalEdgeMove.Find)
          .Select(m => (move: m, gain: instance.Gain.ExchangeEdge(m.Cycle, m.From, m.To)))
          .Shuffle()
          .FirstOrDefault(c => c.gain > 0);

      if (gain is 0) return population;
      move.Apply();
    }
  }

  private static ImmutableArray<NodeList> ExternalVertices(Instance instance, ImmutableArray<NodeList> population) {
    while (true) {
      var (move, gain) =
        ExchangeExternalVerticesMove.Find(population)
          .Select(m => (move: m, gain: instance.Gain.ExchangeVertex(m.First, m.Second, m.From, m.To)))
          .Shuffle()
          .FirstOrDefault(c => c.gain > 0);

      if (gain is 0) return population;
      move.Apply();
    }
  }

  public GreedyLocalSearch() : base(
    displayAs: DisplayType.Cycle,
    usesRegret: true,
    usesWeight: true,
    usesInitializers: true,
    usesVariants: true
  ) {
  }
}
