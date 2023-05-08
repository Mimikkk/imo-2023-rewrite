using Algorithms.Searches.Implementations;

namespace Algorithms.Searches;

public static class SearchType {
  public static readonly Search Identity = new IdentitySearch();
  public static readonly Search Furthest = new FurthestSearch();
  public static readonly Search NearestNeighbour = new NearestNeighbourSearch();
  public static readonly Search Random = new RandomSearch();
  public static readonly Search CycleExpansion = new CycleExpansionSearch();
  public static readonly Search RegretCycleExpansion = new RegretCycleExpansionSearch();
  public static readonly Search WeightedRegretCycleExpansion = new WeightedRegretCycleExpansionSearch();
  public static readonly Search GreedyLocal = new GreedyLocalSearch();
  public static readonly Search SteepestLocal = new SteepestLocalSearch();
  public static readonly Search CandidateLocal = new CandidateLocalSearch();
  public static readonly Search MultipleStartLocal = new MultipleStartLocalSearch();
  public static readonly Search IteratedLocal = new IteratedLocalSearch();
  public static readonly Search Evolutionary = new EvolutionarySearch();
}
