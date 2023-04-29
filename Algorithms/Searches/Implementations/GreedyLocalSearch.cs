using System.Collections.Immutable;
using Domain.Extensions;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class GreedyLocalSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Variant switch {
      "internal-edges" => Variants(instance, configuration.Population, new[] {
        Variant.InternalEdges,
      }),
      "internal-vertices" => Variants(instance, configuration.Population, new[] {
        Variant.InternalVertices,
      }),
      "external-vertices" => Variants(instance, configuration.Population, new[] {
        Variant.ExternalVertices,
      }),
      "vertices" when configuration.Population.Length > 1 => Variants(instance, configuration.Population, new[] {
        Variant.InternalVertices,
        Variant.ExternalVertices,
      }),
      "internal-edges-external-vertices" when configuration.Population.Length > 1 => Variants(instance,
        configuration.Population, new[] {
          Variant.InternalEdges,
          Variant.ExternalVertices,
        }),
      "mixed" when configuration.Population.Length > 1 => Variants(instance, configuration.Population, new[] {
        Variant.InternalEdges,
        Variant.InternalVertices,
        Variant.ExternalVertices,
      }),
    };

  private static ImmutableArray<NodeList> Variants(Instance instance, ImmutableArray<NodeList> population,
    IList<Variant> moves) {
    while (true) {
      var move = moves.SelectMany(m => m.Find(instance, population))
        .Shuffle()
        .FirstOrDefault(c => c.Gain > 0);

      if (move is null) return population;
      move.Apply();
      population.ForEach(p => p.Notify());
    }
  }

  public record Variant(Func<Instance, ImmutableArray<NodeList>, IEnumerable<IMove>> Find) {
    public static readonly Variant InternalEdges = new(
      (instance, population) => population.SelectMany(cycle => ExchangeInternalEdgeMove.Find(instance, cycle))
    );

    public static readonly Variant InternalVertices = new(
      (instance, population) => population.SelectMany(cycle => ExchangeInternalVerticesMove.Find(instance, cycle))
    );

    public static readonly Variant ExternalVertices = new(
      (instance, population) => ExchangeExternalVerticesMove.Find(instance, population)
    );
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
