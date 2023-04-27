using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches;

public abstract class Searchable {
  protected abstract IEnumerable<IEnumerable<Node>> Call(Instance instance, Configuration configuration);

  public delegate IEnumerable<IEnumerable<Node>> Callback(Instance instance, Configuration configuration);

  public static implicit operator Callback(Searchable value) => value.Call;

  public record struct Configuration(
    ImmutableArray<IList<Node>> Population,
    IList<int> Gains,
    Searchable? Initializer = null,
    int Regret = 0,
    float Weight = 0,
    int? Start = null,
    float TimeLimit = 0,
    int IterationLimit = 0,
    string? Variant = null
  );

  public static readonly Configuration Configure;
}
