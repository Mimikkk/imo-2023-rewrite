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

      Node closestToHead;
      var offset = 0;
      while (true) {
        closestToHead = instance.Distance.ClosestBy(head, offset++);
        if (!path.Contains(closestToHead)) break;
      }

      offset = 0;
      Node closestToTail;
      while (true) {
        closestToTail = instance.Distance.ClosestBy(tail, offset++);
        if (!path.Contains(closestToTail)) break;
      }

      if (instance.Distance[head, closestToHead] < instance.Distance[tail, closestToTail]) {
        path.Insert(0, closestToHead);
      }
      else {
        path.Add(closestToTail);
      }
    }

    return population;
  }

  private static ImmutableArray<NodeList> Double(Instance instance, ImmutableArray<NodeList> population) {
    return population;
  }

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    return population;
  }

  public NearestNeighbourSearch() : base(usesInitializer: true, displayAs: DisplayType.Path) {
  }
}
