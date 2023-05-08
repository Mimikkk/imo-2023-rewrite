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


    var best = SearchType.SteepestLocal.Search(instance, new() {
      Initializers = { (SearchType.Random, new() { Population = population }) },
      Variant = (int?)SteepestLocalSearch.Variant.InternalEdgeExternalVertices
    });
    var ts = TimeSpan.FromSeconds(timelimit);
    iterations = 0;

    var start = DateTime.Now;
    while (DateTime.Now - start < ts) {
      var candidate = SearchType.WeightedRegretCycleExpansion.Search(instance, CreateConfiguration(best));
      ++iterations;

      if (instance.Distance[candidate] >= instance.Distance[best]) continue;
      best = candidate;
    }

    return best;
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
