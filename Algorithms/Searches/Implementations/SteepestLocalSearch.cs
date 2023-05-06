using System.Collections.Immutable;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;
using LanguageExt;

namespace Algorithms.Searches.Implementations;

public class SteepestLocalSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    Variants(instance, configuration.Population, (Variant)configuration.Variant!.Value);

  private static ImmutableArray<NodeList> Variants(Instance instance, ImmutableArray<NodeList> population, Variant variants) {
    var useInternalEdges = variants.HasFlag(Variant.InternalEdges);
    var useInternalVertices = variants.HasFlag(Variant.InternalVertices);
    var useExternalVertices = variants.HasFlag(Variant.ExternalVertices);

    var internalEdgesMove = default(ExchangeInternalEdgeMove);
    var internalVerticesMove = default(ExchangeInternalVerticesMove);
    var externalVerticesMove = default(ExchangeExternalVerticesMove);
    while (true) {
      if (useInternalEdges) internalEdgesMove = ExchangeInternalEdgeMove.BestByGain(instance, population);
      if (useInternalVertices) internalVerticesMove = ExchangeInternalVerticesMove.BestByGain(instance, population);
      if (useExternalVertices) externalVerticesMove = ExchangeExternalVerticesMove.BestByGain(instance, population);

      var moves = new List<(Action apply, int gain)>(3);
      if (internalEdgesMove.Gain > 0) moves.Add((internalEdgesMove.Apply, internalEdgesMove.Gain));
      if (internalVerticesMove.Gain > 0) moves.Add((internalVerticesMove.Apply, internalVerticesMove.Gain));
      if (externalVerticesMove.Gain > 0) moves.Add((externalVerticesMove.Apply, externalVerticesMove.Gain));
      if (moves.Count is 0) return population;
      moves.MaxBy(c => c.gain)!.apply();
    }
  }

  [Flags]
  public enum Variant {
    InternalEdges = 1, InternalVertices = 2, ExternalVertices = 4,
    Mixed = InternalEdges | InternalVertices | ExternalVertices,
    InternalEdgeExternalVertices = InternalEdges | ExternalVertices,
    Vertices = InternalVertices | ExternalVertices
  }

  public SteepestLocalSearch()
    : base(
      displayAs: DisplayType.Cycle,
      usesInitializers: true,
      usesVariants: true
    ) { }
}
