using System.Collections.Immutable;
using Domain.Extensions;
using Domain.Methods;
using Domain.Shareable;
using Domain.Structures;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;
using LanguageExt;

namespace Algorithms.Searches.Implementations;

public class IteratedLocalSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Variant switch {
      "small-perturbation" => SmallPerturbation(instance, configuration.Population, configuration.TimeLimit!.Value),
      "big-perturbation"   => SmallPerturbation(instance, configuration.Population, configuration.TimeLimit!.Value)
    };

  private static Configuration CreateConfiguration(ImmutableArray<NodeList> population) => new() {
    Population = Perturbation(population.Select(x => x.Clone()).ToImmutableArray()),
    Regret = 2,
    Weight = 0.38f,
  };

  private interface IPerturbation {
    public void Apply();
  }
  private sealed record InternalEdgePerturbation(NodeList Cycle, float Weight) : IPerturbation {
    public void Apply() {
      foreach (var _ in Cycle) {
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
      foreach (var _ in Cycle) {
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
      foreach (var _ in First) {
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

  private static List<IPerturbation> Perturbations(ImmutableArray<NodeList> cycles) {
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
  private static ImmutableArray<NodeList> Perturbation(ImmutableArray<NodeList> population) {
    var valid = Perturbations(population);
    var perturbations = valid.Where(_ => Shared.Random.NextDouble() < 0.15).ToArray();
    if (perturbations.Length is 0) perturbations = new[] { valid.First() };

    foreach (var perturbation in perturbations) perturbation.Apply();

    return population;
  }

  private static ImmutableArray<NodeList> SmallPerturbation(Instance instance, ImmutableArray<NodeList> population, float timelimit) {
    var best = SearchType.Random.Search(instance, CreateConfiguration(population));
    var ts = TimeSpan.FromSeconds(timelimit);

    var start = DateTime.Now;
    while (DateTime.Now - start < ts) {
      var candidate = SearchType.SteepestLocal.Search(instance, CreateConfiguration(best) with {
        Variant = "internal-edges-external-vertices"
      });

      if (instance.Distance[candidate] < instance.Distance[best]) best = candidate;
    }


    return best;
  }
  public IteratedLocalSearch()
    : base(
      usesTimeLimit: true,
      usesVariants: true
    ) { }

}
