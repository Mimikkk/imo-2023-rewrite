namespace Domain.Methods; 

public static class MeasurementMethods {
  public static TimeSpan Measure(Action action) {
    var start = DateTime.Now;
    action();
    var end = DateTime.Now;

    return end - start;
  }
}
