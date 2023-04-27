namespace Domain.Methods; 

public static class MeasurementMethods {
  public static TimeSpan Measure(Action action) {
    var start = DateTime.Now;
    action();
    var end = DateTime.Now;

    return end - start;
  }
  public static TimeSpan MeasureAverage(Action action, int times = 100) {
    var total = TimeSpan.Zero;
    for (var i = 0; i < times; i++) total += Measure(action);

    return total / times;
  }
}
