using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class NearestNeighbourSearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      1 => Single(instance, configuration.Population),
      2 => Double(instance, configuration.Population),
      _ => Multiple(instance, configuration.Population)
    };


  private static ImmutableArray<NodeList> Single(Instance instance, ImmutableArray<NodeList> population) {
    var path = population.First();

    while (path.Count < instance.Dimension) {
      var head = path.First();
      var tail = path.Last();

      Node toHead;
      var offset = 0;
      while (true) {
        toHead = instance.Distance.ClosestBy(head, offset++);
        if (!path.Contains(toHead)) break;
      }

      offset = 0;
      Node toTail;
      while (true) {
        toTail = instance.Distance.ClosestBy(tail, offset++);
        if (!path.Contains(toTail)) break;
      }

      if (instance.Distance[head, toHead] < instance.Distance[tail, toTail])
        path.Insert(0, toHead);
      else
        path.Add(toTail);

      path.Notify();
    }

    return population;
  }

  private static ImmutableArray<NodeList> Double(Instance instance, ImmutableArray<NodeList> population) {
    return population;
  }

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    return population;
  }

  public NearestNeighbourSearch() : base(displayAs: DisplayType.Path) {
  }
}
