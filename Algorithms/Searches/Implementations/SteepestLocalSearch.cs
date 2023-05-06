using System.Collections.Immutable;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;
using LanguageExt;

namespace Algorithms.Searches.Implementations;

public class SteepestLocalSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Variant switch {
      "internal-edges" =>
        Variants(instance, configuration.Population, AvailableVariant.InternalEdges),
      "internal-vertices" =>
        Variants(instance, configuration.Population, AvailableVariant.InternalVertices),
      "external-vertices" =>
        Variants(instance, configuration.Population, AvailableVariant.ExternalVertices),
      "vertices" when configuration.Population.Length > 1 =>
        Variants(instance, configuration.Population, AvailableVariant.Vertices),
      "internal-edges-external-vertices" when configuration.Population.Length > 1 =>
        Variants(instance, configuration.Population, AvailableVariant.InternalEdgeExternalVertices),
      "mixed" when configuration.Population.Length > 1 =>
        Variants(instance, configuration.Population, AvailableVariant.Mixed),
    };

  private static ImmutableArray<NodeList> Variants(Instance instance, ImmutableArray<NodeList> population, AvailableVariant variants) {
    var useInternalVertices = variants.HasFlag(AvailableVariant.InternalVertices);
    var useInternalEdges = variants.HasFlag(AvailableVariant.InternalEdges);
    var useExternalVertices = variants.HasFlag(AvailableVariant.ExternalVertices);

    ExchangeInternalEdgeMove? internalEdgeMove = null;
    ExchangeInternalVerticesMove? internalVerticesMove = null;
    ExchangeExternalVerticesMove? externalVerticesMove = null;
    while (true) {
      if (useInternalEdges) {
        internalEdgeMove = FindInternalEdges(instance, population).MaxBy(c => c.Gain)!;
      }
      if (useInternalVertices) {
        internalVerticesMove = FindInternalVertices(instance, population).MaxBy(c => c.Gain)!;
      }
      if (useExternalVertices) {
        externalVerticesMove = FindExternalVertices(instance, population).MaxBy(c => c.Gain)!;
      }

      var moves = new List<(Action apply, int gain)>(3);
      if (internalEdgeMove.HasValue) moves.Add((internalEdgeMove.Value.Apply, internalEdgeMove.Value.Gain));
      if (internalVerticesMove.HasValue) moves.Add((internalVerticesMove.Value.Apply, internalVerticesMove.Value.Gain));
      if (externalVerticesMove.HasValue) moves.Add((externalVerticesMove.Value.Apply, externalVerticesMove.Value.Gain));
      if (moves.Count == 0) return population;

      var move = moves.MaxBy(c => c.gain)!;
      move.apply();
    }
  }

  [Flags]
  private enum AvailableVariant {
    InternalEdges = 1, InternalVertices = 2, ExternalVertices = 4,
    Mixed = InternalEdges | InternalVertices | ExternalVertices,
    InternalEdgeExternalVertices = InternalEdges | ExternalVertices,
    Vertices = InternalVertices | ExternalVertices
  }

  private static IEnumerable<ExchangeInternalEdgeMove> FindInternalEdges(Instance instance, ImmutableArray<NodeList> population) =>
    population.SelectMany(cycle => ExchangeInternalEdgeMove.Find(instance, cycle));

  private static IEnumerable<ExchangeExternalVerticesMove> FindExternalVertices(Instance instance, ImmutableArray<NodeList> population) =>
    ExchangeExternalVerticesMove.Find(instance, population);

  private static IEnumerable<ExchangeInternalVerticesMove> FindInternalVertices(Instance instance, ImmutableArray<NodeList> population) =>
    population.SelectMany(cycle => ExchangeInternalVerticesMove.Find(instance, cycle));


  public SteepestLocalSearch()
    : base(
      displayAs: DisplayType.Cycle,
      usesInitializers: true,
      usesVariants: true
    ) { }
}
