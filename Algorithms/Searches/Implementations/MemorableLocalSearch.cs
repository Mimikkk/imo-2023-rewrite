using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class MemorableLocalSearch : Search {
  protected override void Initialize(Instance instance, Configuration configuration) { }

  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    Multiple(instance, configuration.Population);

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    var candidates =
      population.SelectMany(cycle => ExchangeInternalEdgeMove.Find(instance, cycle))
        .Where(move => move.Gain > 0)
        .Select(move => new InternalMove(
          instance,
          move.Cycle,
          move.From,
          move.To
        ))
        .ToList();


    UpdateLoop:
    candidates.Sort((a, b) => a.Gain - b.Gain);

    for (var i = candidates.Count - 1; i >= 0; --i) {
      var candidate = candidates[i];
      switch (candidate.Validate()) {
        case Valid.No:
          candidates.RemoveAt(i);
          break;
        case Valid.Yes: {
          candidate.Apply();
          candidates.RemoveAt(i);
          goto UpdateLoop;
        }
      }
    }

    return population;
  }

  public enum Valid {
    Yes,
    No,
    Maybe
  }

  public class InternalMove {
    public readonly Instance Instance;
    public readonly NodeList Cycle;
    public readonly Source From;
    public readonly Source To;
    public int Gain;

    public InternalMove(
      Instance instance,
      NodeList cycle,
      int from,
      int to
    ) {
      Instance = instance;
      From = new(cycle.Neigh(from), cycle[from], from);
      To = new(cycle.Neigh(to), cycle[to], to);
      Gain = ExchangeInternalEdgeMove.CalculateGain(instance, cycle, from, to);
      Cycle = cycle;
    }

    public Valid Validate() {
      var ai = Cycle.IndexOf(From.Node);
      var bi = Cycle.IndexOf(To.Node);
      if (ai == -1 || bi == -1) return Valid.No;

      var (pi, _, _) = From;
      var (pj, _, _) = To;
      var ci = Cycle.Neigh(ai);
      var cj = Cycle.Neigh(bi);
      From.At = ai;
      To.At = bi;
      From.Neigh = (ci.a, ai, ci.c);
      To.Neigh = (cj.a, bi, cj.c);
      
      if (
        ci.a == pi.a && ci.c == pi.c && cj.a == pj.a && cj.c == pj.c
        || ci.a == pi.c && ci.c == pi.a && cj.a == pj.c && cj.c == pj.a
      ) return Valid.Yes;

      if (
        (ci.a == pi.a && ci.c == pi.c || ci.a == pi.c && ci.c == pi.a)
        && (cj.a == pj.a && cj.c == pj.c || cj.a == pj.c && cj.c == pj.a)
      ) return Valid.Maybe;

      return Valid.No;
    }

    public void Apply() => ExchangeInternalEdgeMove.Apply(Cycle, From.At, To.At);

    public record Source((int a, int b, int c) Neigh, Node Node, int At) {
      public int At { get; set; } = At;
      public (int a, int b, int c) Neigh { get; set; } = Neigh;
    }
  }


  public MemorableLocalSearch()
    : base(
      displayAs: DisplayType.Cycle,
      usesInitializers: true,
      usesRegret: true,
      usesWeight: true
    ) { }
}
