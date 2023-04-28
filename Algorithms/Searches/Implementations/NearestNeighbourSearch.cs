using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;
using LanguageExt;

namespace Algorithms.Searches.Implementations;

public class NearestNeighbourSearch : Search {
  protected override void Configure(Instance instance, Configuration configuration) =>
    configuration.Population = SearchType.Furthest.Search(instance, configuration);

  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population.Length switch {
      _ => Multiple(instance, configuration.Population)
    };


  private static void
    FindClosestToHeadOrTailAndAppend(Instance instance, NodeList path, IDictionary<Node, int> offsets,
      ISet<Node> used) {
    var tail = path.First();
    var head = path.Last();

    Node toTail;
    var offsetTail = offsets[tail];
    while (true) {
      toTail = instance.Distance.ClosestBy(tail, offsetTail++);
      if (!used.Contains(toTail)) break;
    }

    var offsetHead = offsets[head];
    Node toHead;
    while (true) {
      toHead = instance.Distance.ClosestBy(head, offsetHead++);
      if (!used.Contains(toHead)) break;
    }

    if (instance.Distance[tail, toTail] < instance.Distance[head, toHead]) {
      path.Insert(0, toTail);
      used.Add(toTail);
      offsets[tail] = --offsetTail;
    }
    else {
      path.Add(toHead);
      used.Add(toHead);
      offsets[head] = --offsetHead;
    }
  }

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    var offsets = instance.Nodes.ToDictionary(e => e, _ => 0);
    var used = population.Flatten().ToHashSet();

    var counter = used.Count;
    while (true) {
      foreach (var path in population) {
        FindClosestToHeadOrTailAndAppend(instance, path, offsets, used);
        path.Notify();
        if (++counter == instance.Dimension) return population;
      }
    }
  }

  public NearestNeighbourSearch() : base(displayAs: DisplayType.Path) {
  }
}
