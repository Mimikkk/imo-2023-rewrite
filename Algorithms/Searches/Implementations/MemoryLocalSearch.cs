using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class MemoryLocalSearch : Search {
  protected override void Initialize(Instance instance, Configuration configuration) { }

  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    Multiple(instance, configuration.Population);

  private static readonly Func<Instance, ImmutableArray<NodeList>, IEnumerable<IMemoryMove>>[]
    Moves = { InternalMove.Find };

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    var candidates =
      Moves.SelectMany(s => s.Invoke(instance, population))
        .Where(move => move.Gain > 0)
        .ToList();


    UpdateLoop:
    candidates.Sort((a, b) => a.Gain - b.Gain);

    for (var i = candidates.Count - 1; i >= 0; --i) {
      var candidate = candidates[i];
      switch (candidate.Validate(instance)) {
        case IMemoryMove.Valid.No:
          candidates.RemoveAt(i);
          break;
        case IMemoryMove.Valid.Yes: {
          candidate.Apply();
          // candidates.AddRange(candidate.New(instance, population));
          candidates.RemoveAt(i);
          goto UpdateLoop;
        }
        case IMemoryMove.Valid.Maybe:
        default: break;
      }
    }

    return population;
  }

  public MemoryLocalSearch()
    : base(
      displayAs: DisplayType.Cycle,
      usesInitializers: true,
      usesRegret: true,
      usesWeight: true
    ) { }

  private interface IMemoryMove : IMove {
    public Valid Validate(Instance instance);
    public IEnumerable<IMemoryMove> New(Instance instance, ImmutableArray<NodeList> population);

    public enum Valid : byte {
      Yes,
      No,
      Maybe
    }
  }

  private sealed record ExternalMove(
    NodeList First,
    NodeList Second,
    ((int a, int b, int c) indices, (Node a, Node b, Node c) vertices, Node node) From,
    ((int a, int b, int c) indices, (Node a, Node b, Node c) vertices, Node node) To,
    int Gain
  ) : IMemoryMove {
    public void Apply() => ExchangeExternalVerticesMove.Apply(First, Second, From.indices.b, To.indices.b);

    public static IEnumerable<IMemoryMove> Find(Instance instance, ImmutableArray<NodeList> cycles) =>
      ExchangeExternalVerticesMove.Find(instance, cycles)
        .Select(move => {
          var (first, second, from, to, gain) = move;
          var via = first.Neigh(from);
          var vib = second.Neigh(to);
          var vna = first.NeighNodes(via);
          var vnb = second.NeighNodes(vib);

          return new ExternalMove(first, second, (via, vna, first[from]), (vib, vnb, second[to]), gain);
        });

    public IMemoryMove.Valid Validate(Instance instance) {
      var (first, second, (pva, _, a), (pvb, _, b), _) = this;


      if (
        first.Contains(a) && first.Contains(b)
        || second.Contains(a) && second.Contains(b)
      ) return IMemoryMove.Valid.No;

      var cna = first.Neigh(a);
      var cnb = second.Neigh(b);

      if (
        ((pva.b != cna.a || pva.c != cna.c) && (pva.c != cna.a || pva.a != cna.c))
        || ((pvb.b != cnb.c || pvb.c != cnb.a) && (pvb.a != cnb.c || pvb.c != cnb.a))
      ) return IMemoryMove.Valid.No;
      Gain = ExchangeInternalEdgeMove.CalculateGain(instance, first, cna.b, cnb.b);
      if (Gain <= 0) return IMemoryMove.Valid.No;
      From = From with { indices = cna };
      To = To with { indices = cnb };

      return IMemoryMove.Valid.Yes;
    }

    public IEnumerable<IMemoryMove> New(Instance instance, ImmutableArray<NodeList> population) {
      var (first, second, (pva, _, _), (pvb, _, _), _) = this;
      (first, second) = (second, first);
      var va = second.Neigh(pva.b);
      var vb = first.Neigh(pvb.b);

      var candidates = new List<IMemoryMove>();
      foreach (var candidate in second.Select((node, i) => (indices: second.Neigh(i), node))) {
        var vn = candidate.indices;
        if (
          vn.b != va.a && ExchangeInternalVerticesMove.CalculateGain(instance, second, vn.b, va.a) is var g1 and > 0
        ) { } // candidates.Add(new InternalMove(second, candidate, (second.Neigh(va.a), second[va.a]), g1));}
        if (
          vn.b != va.b && ExchangeInternalVerticesMove.CalculateGain(instance, second, vn.b, va.b) is var g2 and > 0
        ) { } //candidates.Add(new InternalMove(second, candidate, (second.Neigh(va.b), second[va.b]), g2));
        if (ExchangeExternalVerticesMove.CalculateGain(instance, second, first, vn.b,
              vb.a) is var g3 and > 0) { } //candidates.Add(new ExternalMove(second, first, candidate, (first.Neigh(vb.a), first[vb.a]), g3));
        if (ExchangeExternalVerticesMove.CalculateGain(instance, second, first, vn.b,
              vb.b) is var g4 and > 0) { } //candidates.Add(new ExternalMove(second, first, candidate, (first.Neigh(vb.b), first[vb.b]), g4));
      }

      foreach (var candidate in first.Select((node, i) => (indices: first.Neigh(i), node))) {
        var vn = candidate.indices;

        if (vn.b != vb.a
            && ExchangeInternalVerticesMove.CalculateGain(instance, first, vn.b,
              vb.a) is var g1 and > 0) { } //candidates.Add(new InternalMove(first, candidate, (first.Neigh(vb.a), first[vb.a]), g1));
        if (vn.b != vb.b
            && ExchangeInternalVerticesMove.CalculateGain(instance, first, vn.b,
              vb.b) is var g2 and > 0) { } //candidates.Add(new InternalMove(first, candidate, (first.Neigh(vb.b), first[vb.b]), g2));
        if (ExchangeExternalVerticesMove.CalculateGain(instance, first, second, vn.b,
              va.a) is var g3 and > 0) { } //candidates.Add(new ExternalMove(first, second, candidate, (second.Neigh(va.a), second[va.a]), g3));
        if (ExchangeExternalVerticesMove.CalculateGain(instance, first, second, vn.b,
              va.b) is var g4 and > 0) { } //candidates.Add(new ExternalMove(first, second, candidate, (second.Neigh(va.b), second[va.b]), g4));
      }

      return candidates;
    }

    public int Gain { get; private set; } = Gain;
    public ((int a, int b, int c) indices, (Node a, Node b, Node c) vertices, Node node) To { get; private set; } = To;
    public ((int a, int b, int c) indices, (Node a, Node b, Node c) vertices, Node node) From { get; private set; } = From;
  }

  private sealed record InternalMove(
    NodeList Cycle,
    ((int a, int b, int c) indices, (Node a, Node b, Node c) vertices, Node node) From,
    ((int a, int b, int c) indices, (Node a, Node b, Node c) vertices, Node node) To,
    int Gain
  ) : IMemoryMove {
    public void Apply() => ExchangeInternalEdgeMove.Apply(Cycle, Cycle.IndexOf(From.node), Cycle.IndexOf(To.node));

    public static IEnumerable<IMemoryMove> Find(Instance instance, ImmutableArray<NodeList> cycles) =>
      cycles.SelectMany(cycle => ExchangeInternalEdgeMove.Find(instance, cycle)
        .Select(move => {
          var (cycle, from, to, gain) = move;
          var via = cycle.Neigh(from);
          var vib = cycle.Neigh(to);
          var vna = cycle.NeighNodes(via);
          var vnb = cycle.NeighNodes(vib);

          return new InternalMove(cycle, (via, vna, cycle[from]), (vib, vnb, cycle[to]), gain);
        }));

    public IMemoryMove.Valid Validate(Instance instance) {
      var (cycle, (piia, _, a), (piib, _, b), _) = this;
      var ai = cycle.IndexOf(a);
      var bi = cycle.IndexOf(b);

      if (ai == -1 || bi == -1) return IMemoryMove.Valid.No;

      var i_index = ai;
      var j_index = bi;
      var pia = cycle.NeighNodes(piia);
      var pib = cycle.NeighNodes(piib);
      // this.From = (piia, pia, a);
      // this.To = (piib, pib, b);

      var n = cycle.Count;

      var i_prev = cycle[(i_index - 1 + n) % n];
      var i_succ = cycle[(i_index + 1) % n];
      var j_prev = cycle[(j_index - 1 + n) % n];
      var j_succ = cycle[(j_index + 1) % n];

      if (
        i_prev == pia.a && i_succ == pia.c && j_prev == pib.a && j_succ == pib.c
        || i_prev == pia.c && i_succ == pia.a && j_prev == pib.c && j_succ == pib.a
      ) {
        return IMemoryMove.Valid.Yes;
      }

      if ((i_prev == pia.a && i_succ == pia.c || i_prev == pia.c && i_succ == pia.a)
          && (j_prev == pib.a && j_succ == pib.c || j_prev == pib.c && j_succ == pib.a)
         ) {
        return IMemoryMove.Valid.Maybe;
      }
      return IMemoryMove.Valid.No;
    }

    public IEnumerable<IMemoryMove> New(Instance instance, ImmutableArray<NodeList> population) {
      var (first, (via, vna, _), (vib, vnb, _), _) = this;

      var candidates = new List<IMemoryMove>();
      foreach (var candidate in first.Select((node, i) => {
                 var indices = first.Neigh(i);
                 var vertices = first.NeighNodes(indices);

                 return (indices, vertices, node);
               })) {
        var vn = candidate.vertices;
        var vi = candidate.indices;

        if (
          vn.b != vna.c
          && ExchangeInternalVerticesMove.CalculateGain(instance, first, vi.b, via.c) is var g1 and > 0
        ) candidates.Add(Create(first, candidate, via.c, g1));
        if (
          vn.b != vna.b
          && ExchangeInternalVerticesMove.CalculateGain(instance, first, vi.b, via.b) is var g2 and > 0
        ) candidates.Add(Create(first, candidate, via.b, g2));
        if (
          vn.b != vnb.b
          && ExchangeInternalVerticesMove.CalculateGain(instance, first, vi.b, vib.b) is var g3 and > 0
        ) candidates.Add(Create(first, candidate, vib.b, g3));
      }

      // var second = population.FirstOrDefault(cycle => cycle != first);
      // if (second is null) return candidates;
      // foreach (var candidate in second.Select((node, i) => {
      //            var indices = first.Neigh(i);
      //            var vertices = first.NeighNodes(indices);
      //            
      //            return (indices, vertices, node);
      //          })) {
      // var vn = candidate.indices;
      // if (ExchangeExternalVerticesMove.CalculateGain(instance, second, first, vn.b, vna.c) is var g1 and > 0)
      // candidates.Add(new ExternalMove(first, second, candidate, (second.Neigh(vna.c), second[vna.c]), g1));
      // if (ExchangeExternalVerticesMove.CalculateGain(instance, second, first, vn.b, vna.b) is var g2 and > 0)
      // candidates.Add(new ExternalMove(first, second, candidate, (second.Neigh(vna.b), second[vna.b]), g2));
      // if (ExchangeExternalVerticesMove.CalculateGain(instance, second, first, vn.b, vnb.b) is var g3 and > 0)
      // candidates.Add(new ExternalMove(first, second, candidate, (second.Neigh(vnb.b), second[vnb.b]), g3));
      // }

      return candidates;
    }

    public static InternalMove Create(
      NodeList cycle,
      ((int a, int b, int c) indices, (Node a, Node b, Node c) vertices, Node node) candidate,
      int at,
      int gain
    ) {
      var indices = cycle.Neigh(at);
      var vertices = cycle.NeighNodes(indices);

      return new InternalMove(cycle, candidate, (indices, vertices, cycle[at]), gain);
    }

    public ((int a, int b, int c) indices, (Node a, Node b, Node c) vertices, Node node) From { get; private set; } = From;
    public ((int a, int b, int c) indices, (Node a, Node b, Node c) vertices, Node node) To { get; private set; } = To;
  }
}
