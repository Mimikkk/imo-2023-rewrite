using System.Collections.Immutable;
using Domain.Calculations;
using Domain.Extensions;
using Domain.Shareable;
using Domain.Structures;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class EvolutionarySearch : Search {
  private const int Patience = 300;
  private const int PopulationSize = 20;
  private const int MinDifference = 64;

  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) {
    var timelimit = configuration.TimeLimit!.Value;
    var population = configuration.Population;

    int iterations;
    var result = (Variant)configuration.Variant!.Value switch {
      Variant.Local =>
        Local(instance, population, timelimit, Patience, PopulationSize, MinDifference, out iterations),
      Variant.Constructive =>
        Constructive(instance, population, timelimit, Patience, PopulationSize, MinDifference, out iterations)
    };
    configuration.Memo["iterations"] = iterations;
    return result;
  }

  private static ImmutableArray<NodeList> Mutate(Instance instance, ImmutableArray<NodeList> a, ImmutableArray<NodeList> b) {
    var candidate = a.Select(x => x.Clone()).ToImmutableArray();

    var removed = new HashSet<Node>();
    foreach (var first in candidate.Where(cycle => cycle.Count > 1)) {

      for (var i = first.Count - 1; i >= 0; --i) {
        var p = first[i];
        var q = first[first.Next(i)];

        foreach (var second in b) {
          for (var j = second.Count - 1; j >= 0; --j) {
            var u = second[j];
            var v = second[second.Next(j)];

            if ((p == u && q == v) || (p == v && q == u)) goto Remove;
          }
        }
        continue;

        Remove:
        removed.Add(p);
        removed.Add(q);
        i--;
      }

      for (var i = 0; i < first.Count; ++i) {
        var neigh = first.NeighNodes(i);

        if (removed.Contains(neigh.a) && !removed.Contains(neigh.b) && removed.Contains(neigh.c)) removed.Add(neigh.b);
      }

      foreach (var node in first) {
        if (removed.Contains(node) || Shared.Random.NextDouble() > 0.2) continue;
        removed.Add(node);
      }
    }

    foreach (var cycle in candidate)
      for (var i = cycle.Count - 1; i >= 0; --i)
        if (removed.Contains(cycle[i]))
          cycle.RemoveAt(i);

    return SearchType.WeightedRegretCycleExpansion.Search(instance, new() {
      Population = candidate,
      Regret = 2,
      Weight = 0.38f,
    });
  }

  private static ImmutableArray<NodeList> Local(Instance instance, ImmutableArray<NodeList> population,
    float timelimit, int patience, int size, int minDifference, out int iterations) {

    var candidates = Enumerable.Range(0, size)
      .Select(
        _ => SearchType.SteepestLocal.Search(instance, new() {
          Initializers = { (SearchType.Random, new(population.Length, instance.Dimension)) },
          Variant = (int?)SteepestLocalSearch.Variant.InternalEdgeExternalVertices
        })
      )
      .Select(cycles => (cycles, distance: instance.Distance[cycles]))
      .ToList();

    iterations = 0;
    var remainingPatience = patience;

    var ts = TimeSpan.FromSeconds(timelimit);
    var start = DateTime.Now;

    var best = candidates.MinBy(x => x.distance);
    var bestAt = candidates.IndexOf(best);
    var lastBestDistance = 0;

    bool IsTooSimilarToCandidates(int distance) =>
      candidates.Any(candidate => Math.Abs(candidate.distance - distance) < minDifference);


    while (DateTime.Now - start < ts) {
      iterations += 1;
      var worst = candidates.MaxBy(candidate => candidate.distance);
      var worstAt = candidates.IndexOf(worst);

      var (first, second) = Shared.Random.Choose2(candidates);
      var mutated = SearchType.SteepestLocal.Search(instance, new() {
        Variant = (int?)SteepestLocalSearch.Variant.InternalEdgeExternalVertices,
        Population = Mutate(instance, first.cycles, second.cycles)
      });

      var candidate = (mutated, distance: instance.Distance[mutated]);
      if (lastBestDistance > candidate.distance) {
        lastBestDistance = candidate.distance;
        candidates[bestAt] = candidate;
      } else if (candidate.distance < worst.distance && !IsTooSimilarToCandidates(candidate.distance)) {
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
    float timelimit, int patience, int size, int minDifference, out int iterations) {

    var candidates = Enumerable.Range(0, size)
      .Select(
        _ => SearchType.SteepestLocal.Search(instance, new() {
          Initializers = { (SearchType.Random, new(population.Length, instance.Dimension)) },
          Variant = (int?)SteepestLocalSearch.Variant.InternalEdgeExternalVertices
        })
      )
      .Select(cycles => (cycles, distance: instance.Distance[cycles]))
      .ToList();
    iterations = 0;

    var remainingPatience = patience;

    var ts = TimeSpan.FromSeconds(timelimit);
    var start = DateTime.Now;

    var best = candidates.MinBy(x => x.distance);
    var bestAt = candidates.IndexOf(best);
    var lastBestDistance = 0;


    bool IsTooSimilarToCandidates(int distance) =>
      candidates.Any(candidate => Math.Abs(candidate.distance - distance) < minDifference);


    while (DateTime.Now - start < ts) {
      iterations += 1;
      var worst = candidates.MaxBy(candidate => candidate.distance);
      var worstAt = candidates.IndexOf(worst);

      var (first, second) = Shared.Random.Choose2(candidates);
      var mutated = Mutate(instance, first.cycles, second.cycles);

      var candidate = (mutated, distance: instance.Distance[mutated]);
      if (lastBestDistance > candidate.distance) {
        lastBestDistance = candidate.distance;
        candidates[bestAt] = candidate;
      } else if (candidate.distance < worst.distance && !IsTooSimilarToCandidates(candidate.distance)) {
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

  public EvolutionarySearch()
    : base(
      DisplayType.Cycle,
      usesTimeLimit: true,
      usesVariants: true
    ) { }

  [Flags]
  public enum Variant {
    Local = 1,
    Constructive = 2,
  }
}
