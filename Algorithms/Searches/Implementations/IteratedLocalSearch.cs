using System.Collections.Immutable;
using Domain.Extensions;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class IteratedLocalSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    (Variant)configuration.Variant!.Value switch {
      Variant.SmallPerturbation => SmallPerturbation(instance, configuration.Population, configuration.TimeLimit!.Value),
      Variant.BigPerturbation   => BigPerturbation(instance, configuration.Population, configuration.TimeLimit!.Value),
    };

  private static ImmutableArray<NodeList> SmallPerturbation(Instance instance, ImmutableArray<NodeList> population, float timelimit) {
    IList<IPerturbation> CreatePerturbations(ImmutableArray<NodeList> cycles) {
      var perturbations = new List<IPerturbation>();

      foreach (var cycle in cycles) {
        perturbations.Add(new InternalEdgePerturbation(cycle, 0.02f));
        perturbations.Add(new InternalVerticesPerturbation(cycle, 0.02f));
      }

      for (var i = 0; i < cycles.Length; ++i)
      for (var j = i + 1; j < cycles.Length; ++j)
        perturbations.Add(new ExternalVerticesPerturbation(cycles[i], cycles[j], 0.02f));

      return perturbations;
    }
    ImmutableArray<NodeList> ApplyPerturbation(ImmutableArray<NodeList> population) {
      var valid = CreatePerturbations(population);
      var perturbations = valid.Where(_ => Shared.Random.NextDouble() < 0.15).ToArray();
      if (perturbations.Length is 0) perturbations = new[] { valid.First() };
      foreach (var perturbation in perturbations) perturbation.Apply();
      return population;
    }
    Configuration CreateConfiguration(ImmutableArray<NodeList> population) => new() {
      Population = ApplyPerturbation(population.Select(x => x.Clone()).ToImmutableArray()),
      Regret = 2,
      Weight = 0.38f,
    };


    var best = SearchType.Random.Search(instance, CreateConfiguration(population));
    var ts = TimeSpan.FromSeconds(timelimit);

    var start = DateTime.Now;
    while (DateTime.Now - start < ts) {
      var candidate = SearchType.SteepestLocal.Search(instance, CreateConfiguration(best) with {
        Variant = (int?)SteepestLocalSearch.Variant.InternalEdgeExternalVertices
      });

      if (instance.Distance[candidate] >= instance.Distance[best]) continue;
      best = candidate;
    }


    return best;
  }
  private static ImmutableArray<NodeList> BigPerturbation(Instance instance, ImmutableArray<NodeList> population, float timelimit) {
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


    var best = SearchType.SteepestLocal.Search(instance, new() {
      Initializers = { (SearchType.Random, new() { Population = population }) }
    });
    var ts = TimeSpan.FromSeconds(timelimit);

    var start = DateTime.Now;
    while (DateTime.Now - start < ts) {
      var candidate = SearchType.WeightedRegretCycleExpansion.Search(instance, CreateConfiguration(best));

      if (instance.Distance[candidate] >= instance.Distance[best]) continue;
      best = candidate;
    }


    return best;
  }

  public IteratedLocalSearch()
    : base(
      DisplayType.Cycle,
      usesTimeLimit: true,
      usesVariants: true
    ) { }

  [Flags]
  public enum Variant {
    SmallPerturbation = 1,
    BigPerturbation = 2
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
  private sealed record InternalEdgePerturbation(NodeList Cycle, float Weight) : IPerturbation {
    public void Apply() {
      for (var index = Cycle.Count - 1; index >= 0; index--) {
        var _ = Cycle[index];
        if (Shared.Random.NextDouble() < Weight) continue;

        var a = Shared.Random.Next(Cycle.Count);
        var b = a;
        while (b == a) b = Shared.Random.Next(Cycle.Count);

        ExchangeInternalEdgeMove.Apply(Cycle, a, b);
      }
    }
  }
  private sealed record InternalVerticesPerturbation(NodeList Cycle, float Weight) : IPerturbation {
    public void Apply() {
      for (var index = Cycle.Count - 1; index >= 0; --index) {
        var _ = Cycle[index];
        if (Shared.Random.NextDouble() < Weight) continue;
        var a = Shared.Random.Next(Cycle.Count);
        var b = a;
        while (b == a) b = Shared.Random.Next(Cycle.Count);

        ExchangeInternalVerticesMove.Apply(Cycle, a, b);
      }
    }
  }
  private sealed record ExternalVerticesPerturbation(NodeList First, NodeList Second, float Weight) : IPerturbation {
    public void Apply() {
      for (var index = First.Count - 1; index >= 0; --index) {
        var _ = First[index];
        if (Shared.Random.NextDouble() < Weight) continue;
        ExchangeExternalVerticesMove.Apply(
          First,
          Second,
          Shared.Random.Next(First.Count),
          Shared.Random.Next(Second.Count)
        );
      }

    }
  }
}
