using System.Collections.Immutable;
using Domain.Extensions;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class SteepestLocalSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Variant switch {
      "internal-edges" => Variants(instance, configuration.Population, new[] {
        AvailableMove.InternalEdges,
      }),
      "internal-vertices" => Variants(instance, configuration.Population, new[] {
        AvailableMove.InternalVertices,
      }),
      "external-vertices" => Variants(instance, configuration.Population, new[] {
        AvailableMove.ExternalVertices,
      }),
      "vertices" when configuration.Population.Length > 1 => Variants(instance, configuration.Population, new[] {
        AvailableMove.InternalVertices,
        AvailableMove.ExternalVertices,
      }),
      "internal-edges-external-vertices" when configuration.Population.Length > 1 => Variants(instance,
        configuration.Population, new[] {
          AvailableMove.InternalEdges,
          AvailableMove.ExternalVertices,
        }),
      "mixed" when configuration.Population.Length > 1 => Variants(instance, configuration.Population, new[] {
        AvailableMove.InternalEdges,
        AvailableMove.InternalVertices,
        AvailableMove.ExternalVertices,
      }),
    };

  private static ImmutableArray<NodeList> Variants(Instance instance, ImmutableArray<NodeList> population,
    IList<AvailableMove> moves) {
    var i = 0;
    while (true) {
      if (i++ > 1000) return population;
      var move = moves.SelectMany(m => m.Find(instance, population)).MaxBy(c => c.Gain)!;
      if (move.Gain <= 0) return population;

      move.Apply();
      population.ForEach(p => p.Notify());
    }
  }

  private sealed record AvailableMove(Func<Instance, ImmutableArray<NodeList>, IEnumerable<IMove>> Find) {
    public static readonly AvailableMove InternalEdges = new(
      (instance, population) => population.SelectMany(cycle => ExchangeInternalEdgeMove.Find(instance, cycle))
    );

    public static readonly AvailableMove InternalVertices = new(
      (instance, population) => population.SelectMany(cycle => ExchangeInternalVerticesMove.Find(instance, cycle))
    );

    public static readonly AvailableMove ExternalVertices = new(
      (instance, population) => ExchangeExternalVerticesMove.Find(instance, population)
    );
  }

  public SteepestLocalSearch() : base(
    displayAs: DisplayType.Cycle,
    usesRegret: true,
    usesWeight: true,
    usesInitializers: true,
    usesVariants: true
  ) {
  }
}
