using System.Collections.Immutable;
using System.ComponentModel;
using Domain.Extensions;
using Domain.Structures;
using Domain.Structures.Instances;
using Domain.Structures.Moves;
using Domain.Structures.NodeLists;
using LanguageExt.Pretty;

namespace Algorithms.Searches.Implementations;

public class MemoryLocalSearch : Search {
  protected override void Initialize(Instance instance, Configuration configuration) {
  }

  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    Multiple(instance, configuration.Population);

  private static readonly Func<Instance, ImmutableArray<NodeList>, IEnumerable<IMemoryMove>>[]
    Moves = { ExternalMove.Find, InternalMove.Find };

  private static ImmutableArray<NodeList> Multiple(Instance instance, ImmutableArray<NodeList> population) {
    var candidates =
      Moves.SelectMany(s => s.Invoke(instance, population))
        .Where(move => move.Gain > 0)
        .ToList();


    UpdateLoop:
    candidates = candidates
      .Filter(move => move.Validate(instance) is IMemoryMove.Valid.Yes or IMemoryMove.Valid.Maybe)
      .Where(a => a.Gain > 0).OrderBy(a => a.Gain).ToList();

    for (var i = candidates.Count - 1; i >= 0; --i) {
      var candidate = candidates[i];
      switch (candidate.Validate(instance)) {
        case IMemoryMove.Valid.No:
          candidates.RemoveAt(i);
          break;
        case IMemoryMove.Valid.Yes: {
          var x = (InternalMove)candidate;
          Console.WriteLine(candidate.Gain);
          Console.WriteLine(
            ExchangeInternalEdgeMove.CalculateGain(instance, x.Cycle, x.From.indices.b, x.To.indices.b));
          Console.WriteLine(x.From.indices);
          Console.WriteLine(x.To.indices);
          Console.WriteLine(x.Cycle.Neigh(x.Cycle.IndexOf(x.From.node)));
          Console.WriteLine(x.Cycle.Neigh(x.Cycle.IndexOf(x.To.node)));
          Console.WriteLine();

          candidate.Apply();
          candidates.AddRange(candidate.New(instance, population));
          candidates.RemoveAt(i);
          goto UpdateLoop;
        }
        case IMemoryMove.Valid.Maybe:
        default: break;
      }
    }

    return population;
  }

  public MemoryLocalSearch() : base(
    displayAs: DisplayType.Cycle,
    usesInitializers: true,
    usesRegret: true,
    usesWeight: true
  ) {
  }

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
    ((int a, int b, int c) indices, Node node) From,
    ((int a, int b, int c) indices, Node node) To,
    int Gain
  ) : IMemoryMove {
    public void Apply() => ExchangeExternalVerticesMove.Apply(First, Second, From.indices.b, To.indices.b);

    public static IEnumerable<IMemoryMove> Find(Instance instance, ImmutableArray<NodeList> cycles) =>
      ExchangeExternalVerticesMove.Find(instance, cycles).Select(move => {
        var va = move.First.Neigh(move.From);
        var vb = move.Second.Neigh(move.To);
        return new ExternalMove(move.First, move.Second, (va, move.First[move.From]), (vb, move.Second[move.To]),
          move.Gain);
      });

    public IMemoryMove.Valid Validate(Instance instance) {
      var (first, second, (pva, a), (pvb, b), _) = this;


      if (
        first.Contains(a) && first.Contains(b)
        ||
        second.Contains(a) && second.Contains(b)
      ) return IMemoryMove.Valid.No;

      var cna = first.Neigh(a);
      var cnb = second.Neigh(b);

      if (
        ((pva.b != cna.a || pva.c != cna.c) && (pva.c != cna.a || pva.a != cna.c))
        ||
        ((pvb.b != cnb.c || pvb.c != cnb.a) && (pvb.a != cnb.c || pvb.c != cnb.a))
      ) return IMemoryMove.Valid.No;
      Gain = ExchangeInternalEdgeMove.CalculateGain(instance, first, cna.b, cnb.b);
      if (Gain <= 0) return IMemoryMove.Valid.No;
      From = From with { indices = cna };
      To = To with { indices = cnb };

      return IMemoryMove.Valid.Yes;
    }

    public IEnumerable<IMemoryMove> New(Instance instance, ImmutableArray<NodeList> population) {
      var (first, second, (pva, _), (pvb, _), _) = this;
      (first, second) = (second, first);
      var va = second.Neigh(pva.b);
      var vb = first.Neigh(pvb.b);

      var candidates = new List<IMemoryMove>();
      foreach (var candidate in second.Select((node, i) => (indices: second.Neigh(i), node))) {
        var vn = candidate.indices;
        if (
          vn.b != va.a &&
          ExchangeInternalVerticesMove.CalculateGain(instance, second, vn.b, va.a) is var g1 and > 0
        ) candidates.Add(new InternalMove(second, candidate, (second.Neigh(va.a), second[va.a]), g1));
        if (
          vn.b != va.b &&
          ExchangeInternalVerticesMove.CalculateGain(instance, second, vn.b, va.b) is var g2 and > 0
        ) candidates.Add(new InternalMove(second, candidate, (second.Neigh(va.b), second[va.b]), g2));
        if (ExchangeExternalVerticesMove.CalculateGain(instance, second, first, vn.b, vb.a) is var g3 and > 0)
          candidates.Add(new ExternalMove(second, first, candidate, (first.Neigh(vb.a), first[vb.a]), g3));
        if (ExchangeExternalVerticesMove.CalculateGain(instance, second, first, vn.b, vb.b) is var g4 and > 0)
          candidates.Add(new ExternalMove(second, first, candidate, (first.Neigh(vb.b), first[vb.b]), g4));
      }

      foreach (var candidate in first.Select((node, i) => (indices: first.Neigh(i), node))) {
        var vn = candidate.indices;

        if (vn.b != vb.a && ExchangeInternalVerticesMove.CalculateGain(instance, first, vn.b, vb.a) is var g1 and > 0)
          candidates.Add(new InternalMove(first, candidate, (first.Neigh(vb.a), first[vb.a]), g1));
        if (vn.b != vb.b && ExchangeInternalVerticesMove.CalculateGain(instance, first, vn.b, vb.b) is var g2 and > 0)
          candidates.Add(new InternalMove(first, candidate, (first.Neigh(vb.b), first[vb.b]), g2));
        if (ExchangeExternalVerticesMove.CalculateGain(instance, first, second, vn.b, va.a) is var g3 and > 0)
          candidates.Add(new ExternalMove(first, second, candidate, (second.Neigh(va.a), second[va.a]), g3));
        if (ExchangeExternalVerticesMove.CalculateGain(instance, first, second, vn.b, va.b) is var g4 and > 0)
          candidates.Add(new ExternalMove(first, second, candidate, (second.Neigh(va.b), second[va.b]), g4));
      }

      return candidates;
    }

    public int Gain { get; private set; } = Gain;
    public ((int a, int b, int c) indices, Node node) To { get; private set; } = To;
    public ((int a, int b, int c) indices, Node node) From { get; private set; } = From;
  }

  private sealed record InternalMove(
    NodeList Cycle,
    ((int a, int b, int c) indices, Node node) From,
    ((int a, int b, int c) indices, Node node) To,
    int Gain
  ) : IMemoryMove {
    public void Apply() => ExchangeInternalEdgeMove.Apply(Cycle, From.indices.b, To.indices.b);

    public static IEnumerable<IMemoryMove> Find(Instance instance, ImmutableArray<NodeList> cycles) =>
      cycles.SelectMany(cycle => ExchangeInternalEdgeMove.Find(instance, cycle).Select(move => {
        var va = cycle.Neigh(move.From);
        var vb = cycle.Neigh(move.To);
        return new InternalMove(move.Cycle, (va, move.Cycle[move.From]), (vb, move.Cycle[move.To]), move.Gain);
      }));

    public IMemoryMove.Valid Validate(Instance instance) {
      var (first, (pna, a), (pnb, b), _) = this;

      if (!first.Contains(a) || !first.Contains(b)) return IMemoryMove.Valid.No;
      var cna = first.Neigh(a);
      var cnb = first.Neigh(b);
      Gain = ExchangeInternalEdgeMove.CalculateGain(instance, first, cna.b, cnb.b);
      if (Gain <= 0) return IMemoryMove.Valid.No;
      From = From with { indices = cna };
      To = To with { indices = cnb };

      if (
        (pna.a == cna.a && pna.c == cna.c && pnb.a == cnb.a && pnb.c == cnb.c)
        ||
        (pna.a == cna.c && pna.c == cna.a && pnb.a == cnb.c && pnb.c == cnb.a)
      )
        return IMemoryMove.Valid.Yes;

      if (
        (pna.a == cna.a && pna.c == cna.c || pna.c == cna.a && pna.a == cna.c)
        &&
        (pnb.a == cnb.a && pnb.c == cnb.c || pnb.c == cnb.a && pnb.a == cnb.c)
      ) return IMemoryMove.Valid.Maybe;

      return IMemoryMove.Valid.No;
    }

    public IEnumerable<IMemoryMove> New(Instance instance, ImmutableArray<NodeList> population) {
      var (first, (vna, _), (vnb, _), _) = this;

      var candidates = new List<IMemoryMove>();
      foreach (var candidate in first.Select((node, i) => (indices: first.Neigh(i), node))) {
        var vn = candidate.indices;

        if (
          vn.b != vna.c
          && ExchangeInternalVerticesMove.CalculateGain(instance, first, vn.b, vna.c) is var g1 and > 0
        ) candidates.Add(new InternalMove(first, candidate, (first.Neigh(vna.c), first[vna.c]), g1));
        if (
          vn.b != vna.b
          && ExchangeInternalVerticesMove.CalculateGain(instance, first, vn.b, vna.b) is var g2 and > 0
        ) candidates.Add(new InternalMove(first, candidate, (first.Neigh(vna.b), first[vna.b]), g2));
        if (
          vn.b != vnb.b
          && ExchangeInternalVerticesMove.CalculateGain(instance, first, vn.b, vnb.b) is var g3 and > 0
        ) candidates.Add(new InternalMove(first, candidate, (first.Neigh(vnb.b), first[vnb.b]), g3));
      }

      var second = population.FirstOrDefault(cycle => cycle != first);
      if (second is null) return candidates;
      foreach (var candidate in second.Select((node, i) => (indices: second.Neigh(i), node))) {
        var vn = candidate.indices;

        if (ExchangeExternalVerticesMove.CalculateGain(instance, second, first, vn.b, vna.c) is var g1 and > 0)
          candidates.Add(new ExternalMove(first, second, candidate, (second.Neigh(vna.c), second[vna.c]), g1));

        if (ExchangeExternalVerticesMove.CalculateGain(instance, second, first, vn.b, vna.b) is var g2 and > 0)
          candidates.Add(new ExternalMove(first, second, candidate, (second.Neigh(vna.b), second[vna.b]), g2));

        if (ExchangeExternalVerticesMove.CalculateGain(instance, second, first, vn.b, vnb.b) is var g3 and > 0)
          candidates.Add(new ExternalMove(first, second, candidate, (second.Neigh(vnb.b), second[vnb.b]), g3));
      }

      return candidates;
    }

    public int Gain { get; private set; } = Gain;
    public ((int a, int b, int c) indices, Node node) To { get; private set; } = To;
    public ((int a, int b, int c) indices, Node node) From { get; private set; } = From;
  }
}
