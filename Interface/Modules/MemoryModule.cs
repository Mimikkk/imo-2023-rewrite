﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Domain.Shareable;
using Domain.Structures;
using ScottPlot;
using ScottPlot.Palettes;

namespace Interface.Modules;

internal sealed record MemoryModule(MainWindow Self) {
  public double? AverageScore { get; private set; }
  public double? BestScore { get; private set; }
  public double? WorstScore { get; private set; }

  public double? AverageTime { get; private set; }
  public double? BestTime { get; private set; }
  public double? WorstTime { get; private set; }

  public double? AverageGain { get; private set; }
  public double? BestGain { get; private set; }
  public double? WorstGain { get; private set; }

  public int BestIndex { get; private set; }
  public int WorstIndex { get; private set; }

  public void Invalidate() {
    AverageScore = null;
    BestScore = null;
    WorstScore = null;

    AverageTime = null;
    BestTime = null;
    WorstTime = null;

    AverageGain = null;
    BestGain = null;
    WorstGain = null;

    BestIndex = -1;
    WorstIndex = -1;
  }

  public void Measure(int start, int end) {
    if (AverageScore is not null) return;

    Console.Clear();
    var measurements = Enumerable.Range(start, end)
      .Select(index => {
        Console.WriteLine($"Processing index: {index}");

        var configuration = I.Parameter.Configuration with { Start = index };
        Shared.Random = new(configuration.Start ?? 999);

        var timer = Stopwatch.StartNew();
        var distance = I.Instance.Distance[I.Algorithm.Search(I.Instance, configuration)];
        var time = timer.ElapsedMilliseconds;


        return (distance, time, configuration.Gains);
      })
      .ToList();

    BestIndex = measurements.IndexOf(measurements.MinBy(m => m.distance));
    WorstIndex = measurements.IndexOf(measurements.MaxBy(m => m.distance));

    AverageScore = measurements.Average(m => m.distance);
    WorstScore = measurements.Max(m => m.distance);
    BestScore = measurements.Min(m => m.distance);
    AverageTime = measurements.Average(m => m.time);
    WorstTime = measurements.Max(m => m.time);
    BestTime = measurements.Min(m => m.time);
    AverageGain = measurements.Average(m => m.Gains.Count == 0 ? 0 : m.Gains.Average());
    WorstGain = measurements.Min(m => m.Gains.Count == 0 ? null : m.Gains.Min()) ?? 0;
    BestGain = measurements.Max(m => m.Gains.Count == 0 ? null : m.Gains.Max()) ?? 0;
  }

  public readonly IPalette Palette = new Category10();
  public readonly ObservableCollection<List<List<Node>>> Histories = new();
  private InteractionModule I => Self.Mod.Interaction;
}
