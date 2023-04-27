﻿namespace Algorithms.Searches;

public abstract class Search : Searchable {
  protected Search(
    bool usesWeight = false,
    bool usesRegret = false,
    bool usesVariants = false,
    bool usesTimeLimit = false,
    bool usesInitializer = false,
    bool usesIterationLimit = false
  ) {
    UsesWeight = usesWeight;
    UsesRegret = usesRegret;
    UsesVariants = usesVariants;
    UsesTimeLimit = usesTimeLimit;
    UsesInitializer = usesInitializer;
    UsesIterationLimit = usesIterationLimit;
  }

  public readonly bool UsesWeight;
  public readonly bool UsesRegret;
  public readonly bool UsesVariants;
  public readonly bool UsesInitializer;
  public readonly bool UsesTimeLimit;
  public readonly bool UsesIterationLimit;
}
