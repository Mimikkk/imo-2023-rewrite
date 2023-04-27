namespace Domain.Extensions;

public static class MatrixExtensions {
  public static IEnumerable<T[]> Rows<T>(this T[,] matrix) {
    var m = matrix.GetLength(0);
    var n = matrix.GetLength(1);


    var row = new T[n];
    for (var i = 0; i < m; i++) {
      for (var j = 0; j < n; j++) row[j] = matrix[i, j];

      yield return row;
    }
  }
}
