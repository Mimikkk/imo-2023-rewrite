using System.Collections.Immutable;
using Domain.Structures;
using Domain.Structures.Instances;

namespace Algorithms.Searches.Implementations;

public class IdentitySearch : Search {
  protected override ImmutableArray<List<Node>> Call(Instance instance, Configuration configuration) =>
    configuration.Population;
}
