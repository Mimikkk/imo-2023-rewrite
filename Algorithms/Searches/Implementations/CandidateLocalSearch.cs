using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class CandidateLocalSearch : Search {
  protected override void Initialize(Instance instance, Configuration configuration) {
  }

  private static IEnumerable<(Node a, Node b)> CreateCandidates(Instance instance,
    int size) =>
    instance.Nodes.SelectMany(a =>
      Enumerable.Range(0, size).Select(kth => (a, instance.Distance.ClosestBy(a, kth))));

  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    Multiple(instance, configuration.Population);

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    var candidates = CreateCandidates(instance, 10).ToArray();

    while (true) {
      IMove? best = null;

      foreach (var (a, b) in candidates) {
        var first = population.First(c => c.Contains(a));
        var second = population.First(c => c.Contains(b));
        var i = first.IndexOf(a);
        var j = second.IndexOf(b);

        if (first == second) {
          var gain = ExchangeInternalEdgeMove.CalculateGain(instance, first, i, j);
          if (best is null || gain > best.Gain)
            best = new ExchangeInternalEdgeMove(first, i, j, gain);
          gain = ExchangeInternalEdgeMove.CalculateGain(instance, first, first.Previous(i), first.Previous(j));
          if (gain > best.Gain)
            best = new ExchangeInternalEdgeMove(first, first.Previous(i), first.Previous(j), gain);
        }
        else {
          ExchangeExternalVerticesMove.CalculateGain(instance, first, second, i, j);
          var gain = ExchangeExternalVerticesMove.CalculateGain(instance, first, second, first.Next(i), j);
          if (best is null || gain > best.Gain)
            best = new ExchangeExternalVerticesMove(first, second, first.Next(i), j, gain);
          gain = ExchangeExternalVerticesMove.CalculateGain(instance, first, second, i, second.Next(j));
          if (gain > best.Gain)
            best = new ExchangeExternalVerticesMove(first, second, i, second.Next(j), gain);
        }
      }

      if (best!.Gain <= 0) return population;
      best.Apply();
    }
  }


  public CandidateLocalSearch() : base(
    displayAs: DisplayType.Cycle,
    usesInitializers: true,
    usesRegret: true,
    usesWeight: true
  ) {
  }
}
