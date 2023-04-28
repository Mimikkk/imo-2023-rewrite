using Algorithms.Searches.Implementations;

namespace Algorithms.Searches;

public static class SearchType {
  public static readonly Search Identity = new IdentitySearch();
  public static readonly Search Furthest = new FurthestSearch();
  public static readonly Search NearestNeighbour = new NearestNeighbourSearch();
  public static readonly Search Random = new RandomSearch();
}
