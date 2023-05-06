using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;

namespace Cli.Benchmarks.Columns;

public class DistanceColumn : IColumn {
  public static readonly IColumn Min = new DistanceColumn("Min distance", () => BenchmarkMemory.LoadDistances().min);
  public static readonly IColumn Max = new DistanceColumn("Max distance", () => BenchmarkMemory.LoadDistances().max);
  public static readonly IColumn Average = new DistanceColumn("Average distance", () => BenchmarkMemory.LoadDistances().average);

  public string GetValue(Summary summary, BenchmarkCase benchmarkCase) =>
    GetValue(summary, benchmarkCase, SummaryStyle.Default);

  public string GetValue(Summary summary, BenchmarkCase benchmarkCase, SummaryStyle style) =>
    $"{Formatter():F2}";

  public bool IsDefault(Summary summary, BenchmarkCase benchmarkCase) => false;

  public bool IsAvailable(Summary summary) => true;

  public string Id => $"{nameof(DistanceColumn)}-{ColumnName}";
  public string ColumnName { get; }
  public bool AlwaysShow => true;
  public ColumnCategory Category => ColumnCategory.Metric;
  public int PriorityInCategory => 0;
  public bool IsNumeric => true;
  public UnitType UnitType => UnitType.Size;
  public string Legend => $"{ColumnName} of all solutions";
  public readonly Func<double> Formatter;

  public DistanceColumn(string name, Func<double> formatter) {
    ColumnName = name;
    Formatter = formatter;
  }
}
