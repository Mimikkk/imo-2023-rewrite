using System.Collections.Immutable;
using Domain.Extensions;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;
using LanguageExt;

namespace Algorithms.Searches.Implementations;

public class GreedyLocalSearch : Search {
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
      if (useInternalEdges) internalEdgesMove = ExchangeInternalEdgeMove.FirstWithGain(instance, population);
      if (useInternalVertices) internalVerticesMove = ExchangeInternalVerticesMove.FirstWithGain(instance, population);
      if (useExternalVertices) externalVerticesMove = ExchangeExternalVerticesMove.FirstWithGain(instance, population);

      var moves = new List<Action>(3);
      if (internalEdgesMove.Gain is not 0) moves.Add(internalEdgesMove.Apply);
      if (internalVerticesMove.Gain is not 0) moves.Add(internalVerticesMove.Apply);
      if (externalVerticesMove.Gain is not 0) moves.Add(externalVerticesMove.Apply);
      if (moves.Count is 0) return population;

      var move = moves.Shuffle().First();
      move.Invoke();
    }
  }

  [Flags]
  public enum Variant {
    InternalEdges = 1, InternalVertices = 2, ExternalVertices = 4,
    Mixed = InternalEdges | InternalVertices | ExternalVertices,
    InternalEdgeExternalVertices = InternalEdges | ExternalVertices,
    Vertices = InternalVertices | ExternalVertices
  }

  public GreedyLocalSearch()
    : base(
      displayAs: DisplayType.Cycle,
      usesInitializers: true,
      usesVariants: true
    ) { }
}
