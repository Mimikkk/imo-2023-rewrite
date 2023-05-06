using System.Collections.Immutable;
using Algorithms.Searches;
using Algorithms.Searches.Implementations;
using Charts.Extensions;
using Domain.Extensions;
using Domain.Shareable;
using Domain.Structures.Instances;
using Domain.Structures.NodeLists;

namespace Cli.Benchmarks;

public static class BenchmarkMemory {
  public static readonly Instance Instance = Instance.Predefined.KroB200;
  public static Searchable.Configuration Configuration => new(2, Instance.Dimension) {
    IterationLimit = 100,
    Variant = (int?)SteepestLocalSearch.Variant.Mixed,
    Initializers = { (SearchType.Random, new(2, Instance.Dimension)) }
  };
  public static Searchable Search => SearchType.SteepestLocal;
  public const string SearchName = nameof(SearchType.SteepestLocal);
  public const int Iterations = 100;

  public static readonly List<ImmutableArray<NodeList>> Results = new();

  public static void Save() {
    var distances = Results.TakeLast(Iterations).Select(cycles => (cycles, distance: Instance.Distance[cycles])).ToArray();
    var best = distances.MinBy(x => x.distance);
    var worst = distances.MaxBy(x => x.distance);
    var average = distances.Select(x => x.distance).Average();

    File.WriteAllText(Files.Distances.FullName, $"{best.distance}{Environment.NewLine}{worst.distance}{Environment.NewLine}{average}{Environment.NewLine}");

    var plot = new ScottPlot.Plot();
    foreach (var cycle in best.cycles) plot.Add.Cycle(cycle, Instance);
    plot.Add.Label($"Łączna długość: {Instance.Distance[best.cycles]}");
    plot.Save($"best-{Instance.Name}-{SearchName}-cycles");
    plot.Clear();

    // foreach (var cycle in worst.cycles) plot.Add.Cycle(cycle, Instance);
    // plot.Add.Label($"Łączna długość: {Instance.Distance[worst.cycles]}");
    // plot.Save($"worst-{Instance.Name}-{SearchName}-cycles");
    // plot.Clear();

    Results.Clear();
  }

  public static (double min, double max, double average) LoadDistances() {
    var lines = File.ReadAllLines(Files.Distances.FullName);
    var best = double.Parse(lines[0]);
    var worst = double.Parse(lines[1]);
    var average = double.Parse(lines[2]);

    return (best, worst, average);
  }

  private static class Files {
    public static FileInfo Distances {
      get {
        if (!_distances.Exists) _distances.Create().Close();
        return _distances;
      }
    }

    private static readonly FileInfo _distances = new($"{Shared.Directories.Memory}/distances.txt");
  }
}
