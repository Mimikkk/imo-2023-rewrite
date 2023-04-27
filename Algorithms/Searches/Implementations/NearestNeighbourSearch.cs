using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches.Implementations;

public class NearestNeighbourSearch : Search {
  protected override ImmutableArray<List<Node>> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      1 => Single(instance, configuration.Population),
      2 => Double(instance, configuration.Population),
      _ => Multiple(instance, configuration.Population)
    };


  private static ImmutableArray<List<Node>> Single(Instance instance, ImmutableArray<List<Node>> population) {
    var path = population.First();
    var used = new HashSet<Node>(path);

    while (path.Count < instance.Dimension) {
      var head = path.First();
      var tail = path.Last();

      Node closestToHead;
      var offset = 0;
      while (true) {
        closestToHead = instance.Distance.ClosestBy(head, offset++);
        if (!used.Contains(closestToHead)) break;
      }

      offset = 0;
      Node closestToTail;
      while (true) {
        closestToTail = instance.Distance.ClosestBy(tail, offset++);
        if (!used.Contains(closestToTail)) break;
      }

      if (instance.Distance[head, closestToHead] < instance.Distance[tail, closestToTail]) {
        path.Insert(0, closestToHead);
        used.Add(closestToHead);
      }
      else {
        path.Add(closestToTail);
        used.Add(closestToTail);
      }
    }

    return population;
  }

  private static ImmutableArray<List<Node>> Double(Instance instance, ImmutableArray<List<Node>> population) {
    return population;
  }

  private static ImmutableArray<List<Node>> Multiple(Instance instance, ImmutableArray<List<Node>> population) {
    return population;
  }

  public NearestNeighbourSearch() : base(usesInitializer: true, displayAs: DisplayType.Path) {
  }
}
