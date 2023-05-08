using System.Collections.Immutable;
using Domain.Extensions;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class EvolutionarySearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) {
    int iterations;
    var result = (Variant)configuration.Variant!.Value switch {
      Variant.Local => Local(instance, configuration.Population, configuration.TimeLimit!.Value,
        out iterations),
      Variant.Constructive => Constructive(instance, configuration.Population,
        configuration.TimeLimit!.Value, out iterations),
    };
    configuration.Memo["iterations"] = iterations;
    return result;
  }

  private static ImmutableArray<NodeList> Local(Instance instance, ImmutableArray<NodeList> population,
    float timelimit, out int iterations) {
    Configuration CreateConfiguration(ImmutableArray<NodeList> population) {
      population = population.Select(x => x.Clone()).ToImmutableArray();
      var perturbations = population.Select(x => new DestructPerturbation(x, 0.2f)).ToArray();
      foreach (var perturbation in perturbations) perturbation.Apply();

      return new() {
        Population = population,
        Regret = 2,
        Weight = 0.38f,
      };
    }

    var candidates = Enumerable.Range(0, 20)
      .Select(
        _ => SearchType.SteepestLocal.Search(instance, new() {
          Initializers = { (SearchType.Random, new(population.Length, instance.Dimension)) },
          Variant = (int?)SteepestLocalSearch.Variant.InternalEdgeExternalVertices
        })
      )
      .Select(cycles => (cycles, distance: instance.Distance[cycles]))
      .ToList();
    iterations = 0;

    const int patience = 300;
    const int minDifference = 50;
    var remainingPatience = patience;

    var ts = TimeSpan.FromSeconds(timelimit);
    var start = DateTime.Now;

    var best = candidates.MinBy(x => x.distance);
    var bestAt = candidates.IndexOf(best);
    var lastBestDistance = 0;

    ImmutableArray<NodeList> Mutate(ImmutableArray<NodeList> a, ImmutableArray<NodeList> b) {
      a = a.Select(x => x.Clone()).ToImmutableArray();
      b = b.Select(x => x.Clone()).ToImmutableArray();

      foreach (var first in a) {
        var n = first.Count;
        if (n is 1) continue;

        throw new NotImplementedException();
      }

      var f = a[0];
      var s = b[1];
      return SearchType.WeightedRegretCycleExpansion.Search(instance, new() {
        Population = new[] { f, s }.ToImmutableArray(),
        Regret = 2,
        Weight = 0.38f,
      });
    }

    bool IsTooSimilarToCandidates(int distance) =>
      candidates.Any(candidate => Math.Abs(candidate.distance - distance) < minDifference);


    while (DateTime.Now - start < ts) {
      iterations += 1;
      var worst = candidates.MaxBy(candidate => candidate.distance);
      var worstAt = candidates.IndexOf(worst);

      var (first, second) = Shared.Random.Choose2(candidates);
      var mutated = SearchType.SteepestLocal.Search(instance, new() {
        Variant = (int?)SteepestLocalSearch.Variant.InternalEdgeExternalVertices,
        Population = Mutate(first.cycles, second.cycles)
      });

      var candidate = (mutated, distance: instance.Distance[mutated]);
      if (lastBestDistance > candidate.distance) {
        lastBestDistance = candidate.distance;
        candidates[bestAt] = candidate;
      }
      else if (candidate.distance < worst.distance && !IsTooSimilarToCandidates(candidate.distance)) {
        candidates[worstAt] = candidate;
      }

      best = candidates.MinBy(x => x.distance);
      bestAt = candidates.IndexOf(best);

      if (lastBestDistance < best.distance) {
        lastBestDistance = best.distance;
        remainingPatience = patience;
      }

      if (--remainingPatience == 0) break;
    }

    return best.cycles;
  }

  private static ImmutableArray<NodeList> Constructive(Instance instance, ImmutableArray<NodeList> population,
    float timelimit, out int iterations) {
    iterations = 0;

    return population;
  }

  public EvolutionarySearch()
    : base(
      DisplayType.Cycle,
      usesTimeLimit: true,
      usesVariants: true
    ) {
  }

  [Flags]
  public enum Variant {
    Local = 1,
    Constructive = 2,
  }

  private interface IPerturbation {
    public void Apply();
  }

  private sealed record DestructPerturbation(NodeList Cycle, float Weight) : IPerturbation {
    public void Apply() {
      for (var index = Cycle.Count - 1; index >= 0; --index) {
        if (Shared.Random.NextDouble() < Weight) continue;
        Cycle.RemoveAt(index);
      }
    }
  }
}
