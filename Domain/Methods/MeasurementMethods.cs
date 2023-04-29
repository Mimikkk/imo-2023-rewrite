using System.Diagnostics;

namespace Domain.Methods;

public static class MeasurementMethods {
  public static TimeSpan Measure(Action action) {
    var start = Stopwatch.StartNew();
    action();
    var end = start.Elapsed;

    return end;
  }

  public static (TimeSpan elapsed, T result) Measure<T>(Func<T> action) {
    var start = Stopwatch.StartNew();
    var result = action();
    var end = start.Elapsed;
    
    return (end, result);
  }

  public static TimeSpan MeasureAverage(Action action, int times = 100) {
    var total = TimeSpan.Zero;
    for (var i = 0; i < times; i++) total += Measure(action);

    return total / times;
  }
}
