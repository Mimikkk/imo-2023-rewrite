﻿using Domain.Structures.Instances;

namespace Algorithms.Searches;

public abstract class Search : Searchable {
  protected virtual void Initialize(Instance instance, Configuration configuration) {}

  protected sealed override void Configure(Instance instance, Configuration configuration) {
    if (UsesWeight && configuration.Weight is null) throw new("No weight was provided");
    if (UsesTimeLimit && configuration.TimeLimit is null) throw new("No time limit was provided");
    if (UsesIterationLimit && configuration.IterationLimit is null) throw new("No iteration limit was provided");
    if (UsesRegret && configuration.Regret is null) throw new("No regret was provided");
    if (UsesVariants && configuration.Variant is null) throw new("No variant was provided");
    Initialize(instance, configuration);
  }

  protected Search(
    DisplayType? displayAs = null,
    bool usesWeight = false,
    bool usesRegret = false,
    bool usesVariants = false,
    bool usesTimeLimit = false,
    bool usesInitializers = false,
    bool usesIterationLimit = false
  ) {
    DisplayAs = displayAs;
    UsesWeight = usesWeight;
    UsesRegret = usesRegret;
    UsesVariants = usesVariants;
    UsesTimeLimit = usesTimeLimit;
    UsesInitializers = usesInitializers;
    UsesIterationLimit = usesIterationLimit;
  }

  public readonly bool UsesWeight;
  public readonly bool UsesRegret;
  public readonly bool UsesVariants;
  public readonly bool UsesInitializers;
  public readonly bool UsesTimeLimit;
  public readonly bool UsesIterationLimit;
  public readonly DisplayType? DisplayAs;

  public enum DisplayType {
    Path,
    Cycle
  }
}
