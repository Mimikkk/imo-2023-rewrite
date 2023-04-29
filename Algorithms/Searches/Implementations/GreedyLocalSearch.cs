using System.Collections.Immutable;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class GreedyLocalSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      _ => Multiple(instance, configuration.Population)
    };

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    while (true) {
      var (move, gain) =
        population.SelectMany(ExchangeInternalEdgeMove.Find)
          .Select(m => (move: m, gain: instance.Gain.ExchangeEdge(m.Cycle, m.From, m.To)))
          .FirstOrDefault(c => c.gain > 0);

      if (gain is 0) return population;
      move.Apply();
      move.Cycle.Notify();
    }
  }

  public GreedyLocalSearch() : base(
    displayAs: DisplayType.Cycle,
    usesRegret: true,
    usesWeight: true,
    usesInitializers: true
  ) {
  }
}
