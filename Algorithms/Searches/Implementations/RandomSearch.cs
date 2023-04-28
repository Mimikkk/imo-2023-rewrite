using System.Collections.Immutable;
using Domain.Extensions;
using Domain.Shareable;
using Domain.Structures;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;
using LanguageExt;

namespace Algorithms.Searches.Implementations;

public class RandomSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      _ => Multiple(instance, configuration.Population)
    };


  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    var used = population.Flatten().ToHashSet();

    var counter = used.Count;
    while (true) {
      foreach (var path in population) {
        var choice = Shared.Random.Choose(instance.Nodes.Except(used));
        path.Add(choice);
        used.Add(choice);
        path.Notify();
        if (++counter == instance.Dimension) return population;
      }
    }
  }

  public RandomSearch() : base(displayAs: DisplayType.Path) {
  }
}
