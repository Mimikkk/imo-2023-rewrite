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

    var internalEdgesMoves =
      useInternalEdges ? ExchangeInternalEdgeMove.AssignSpace(instance, population) : Array.Empty<ExchangeInternalEdgeMove>();
    var internalEdgesMove = default(ExchangeInternalEdgeMove);

    var internalVerticesMoves =
      useInternalVertices ? ExchangeInternalVerticesMove.AssignSpace(instance, population) : Array.Empty<ExchangeInternalVerticesMove>();
    var internalVerticesMove = default(ExchangeInternalVerticesMove);

    var externalVerticesMoves =
      useExternalVertices ? ExchangeExternalVerticesMove.AssignSpace(instance, population) : Array.Empty<ExchangeExternalVerticesMove>();
    var externalVerticesMove = default(ExchangeExternalVerticesMove);
    while (true) {
      if (useInternalEdges) {
        ExchangeInternalEdgeMove.Fill(instance, population, ref internalEdgesMoves);
        internalEdgesMove = internalEdgesMoves.MaxBy(c => c.Gain)!;
      }
      if (useInternalVertices) {
        ExchangeInternalVerticesMove.Fill(instance, population, ref internalVerticesMoves);
        internalVerticesMove = internalVerticesMoves.MaxBy(c => c.Gain)!;
      }
      if (useExternalVertices) {
        ExchangeExternalVerticesMove.Fill(instance, population, ref externalVerticesMoves);
        externalVerticesMove = externalVerticesMoves.MaxBy(c => c.Gain)!;
      }

      var moves = new List<(Action apply, int gain)>(3);
      if (internalEdgesMove.Gain > 0) moves.Add((internalEdgesMove.Apply, internalEdgesMove.Gain));
      if (internalVerticesMove.Gain > 0) moves.Add((internalVerticesMove.Apply, internalVerticesMove.Gain));
      if (externalVerticesMove.Gain > 0) moves.Add((externalVerticesMove.Apply, externalVerticesMove.Gain));
      if (moves.Count is 0) return population;

      var move = moves.MaxBy(c => c.gain)!;
      move.apply();
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
