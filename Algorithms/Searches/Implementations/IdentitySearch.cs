using System.Collections.Immutable;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Algorithms.Searches.Implementations;

public class IdentitySearch : Search {
  protected override ImmutableArray<NodeList> Call(Instance instance, Configuration configuration) =>
    configuration.Population;
}
