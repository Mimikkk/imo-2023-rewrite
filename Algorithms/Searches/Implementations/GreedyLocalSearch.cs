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
    var useInternalVertices = variants.HasFlag(AvailableVariant.InternalVertices);
    var useInternalEdges = variants.HasFlag(AvailableVariant.InternalEdges);
    var useExternalVertices = variants.HasFlag(AvailableVariant.ExternalVertices);

    ExchangeInternalEdgeMove? internalEdgeMove = null;
    ExchangeInternalVerticesMove? internalVerticesMove = null;
    ExchangeExternalVerticesMove? externalVerticesMove = null;
    while (true) {
      if (useInternalEdges) {
        internalEdgeMove = FindInternalEdges(instance, population)
          .Shuffle()
          .FirstOrDefault(c => c.Gain > 0);
        if (internalEdgeMove.Value.Cycle is null) internalEdgeMove = null;
      }
      if (useInternalVertices) {
        internalVerticesMove = FindInternalVertices(instance, population)
          .Shuffle()
          .FirstOrDefault(c => c.Gain > 0);


        if (internalVerticesMove.Value.Cycle is null) internalVerticesMove = null;
      }
      if (useExternalVertices) {
        externalVerticesMove = FindExternalVertices(instance, population)
          .Shuffle()
          .FirstOrDefault(c => c.Gain > 0);
        if (externalVerticesMove.Value.First is null) externalVerticesMove = null;
      }

      var moves = new List<Action>(3);
      if (internalEdgeMove.HasValue) moves.Add(internalEdgeMove.Value.Apply);
      if (internalVerticesMove.HasValue) moves.Add(internalVerticesMove.Value.Apply);
      if (externalVerticesMove.HasValue) moves.Add(externalVerticesMove.Value.Apply);
      if (moves.Count == 0) return population;

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

  private static ExchangeInternalEdgeMove[] FindInternalEdges(Instance instance, ImmutableArray<NodeList> population) =>
    population.SelectMany(cycle => ExchangeInternalEdgeMove.Find(instance, cycle)).ToArray();

  private static ExchangeExternalVerticesMove[] FindExternalVertices(Instance instance, ImmutableArray<NodeList> population) =>
    ExchangeExternalVerticesMove.Find(instance, population).ToArray();

  private static ExchangeInternalVerticesMove[] FindInternalVertices(Instance instance, ImmutableArray<NodeList> population) =>
    population.SelectMany(cycle => ExchangeInternalVerticesMove.Find(instance, cycle)).ToArray();

  public GreedyLocalSearch()
    : base(
      displayAs: DisplayType.Cycle,
      usesInitializers: true,
      usesVariants: true
    ) { }
}
