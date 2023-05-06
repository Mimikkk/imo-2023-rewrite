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
    var useInternalEdges = variants.HasFlag(AvailableVariant.InternalEdges);
    var useInternalVertices = variants.HasFlag(AvailableVariant.InternalVertices);
    var useExternalVertices = variants.HasFlag(AvailableVariant.ExternalVertices);

    var internalEdgesMoves = useInternalEdges ? ExchangeInternalEdgeMove.AssignSpace(instance, population) : Array.Empty<ExchangeInternalEdgeMove>();
    var internalEdgeMove = default(ExchangeInternalEdgeMove);

    var internalVerticesMoves =
      useInternalVertices ? ExchangeInternalVerticesMove.AssignSpace(instance, population) : Array.Empty<ExchangeInternalVerticesMove>();
    var internalVerticesMove = default(ExchangeInternalVerticesMove);

    var externalVerticesMoves =
      useExternalVertices ? ExchangeExternalVerticesMove.AssignSpace(instance, population) : Array.Empty<ExchangeExternalVerticesMove>();
    var externalVerticesMove = default(ExchangeExternalVerticesMove);
    while (true) {
      var count = 0;
      if (useInternalEdges) {
        ExchangeInternalEdgeMove.Fill(instance, population, ref internalEdgesMoves);
        internalEdgeMove = internalEdgesMoves.FirstOrDefault(c => c.Gain > 0);
        if (internalEdgeMove.Gain is not 0) count += 1;
      }
      if (useInternalVertices) {
        ExchangeInternalVerticesMove.Fill(instance, population, ref internalVerticesMoves);
        internalVerticesMove = internalVerticesMoves.FirstOrDefault(c => c.Gain > 0);
        if (internalVerticesMove.Gain is not 0) count += 1;
      }
      if (useExternalVertices) {
        ExchangeExternalVerticesMove.Fill(instance, population, ref externalVerticesMoves);
        externalVerticesMove = externalVerticesMoves.FirstOrDefault(c => c.Gain > 0);
        if (externalVerticesMove.Gain is not 0) count += 1;
      }
      if (count is 0) return population;

      var moves = new List<Action>(3);
      if (internalEdgeMove.Gain is not 0) moves.Add(internalEdgeMove.Apply);
      if (internalVerticesMove.Gain is not 0) moves.Add(internalVerticesMove.Apply);
      if (externalVerticesMove.Gain is not 0) moves.Add(externalVerticesMove.Apply);
      if (moves.Count is 0) return population;

      var move = moves.Shuffle().First();
      move.Invoke();
    }
  }

  [Flags]
  private enum AvailableVariant {
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
