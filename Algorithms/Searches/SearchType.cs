﻿using Algorithms.Searches.Implementations;

namespace Algorithms.Searches;

public static class SearchType {
  public static readonly Searchable.Callback Identity = new IdentitySearch();
}
