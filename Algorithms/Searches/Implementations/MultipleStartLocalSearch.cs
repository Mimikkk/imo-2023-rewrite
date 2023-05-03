using System.Collections.Immutable;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class MultipleStartLocalSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      _ => Multiple(instance, configuration.Population, configuration.IterationLimit!.Value)
    };

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population, int iterations) =>
    Enumerable.Range(0, iterations)
      .AsParallel()
      .Select(_ => SearchType.SteepestLocal.Search(instance, new(population.Length, instance.Dimension) {
        Initializers = { (SearchType.Random, new(population.Length, instance.Dimension)) },
        Variant = "internal-edges-external-vertices"
      }))
      .MinBy(x => instance.Distance[x]);

  public MultipleStartLocalSearch()
    : base(usesIterationLimit: true) { }
}
