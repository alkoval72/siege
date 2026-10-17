namespace Siege.Engine;

/// <summary>
/// Клетка гексагонального поля в осевых координатах (q, r), ориентация острой вершиной вверх.
/// </summary>
/// <param name="Q">Координата по оси q.</param>
/// <param name="R">Координата по оси r.</param>
public readonly record struct Hex(int Q, int R)
{
    /// <summary>
    /// Шесть направлений к соседним клеткам в порядке обхода (architecture.md, раздел 4.1).
    /// </summary>
    public static IReadOnlyList<Hex> Directions { get; } =
    [
        new(1, 0),
        new(1, -1),
        new(0, -1),
        new(-1, 0),
        new(-1, 1),
        new(0, 1),
    ];

    /// <summary>
    /// Центр поля, куда ставится первая фишка партии.
    /// </summary>
    public static Hex Center { get; } = new(0, 0);

    /// <summary>
    /// Расстояние от центра поля в шагах: max(|q|, |r|, |q + r|).
    /// </summary>
    public int DistanceFromCenter => Math.Max(Math.Abs(Q), Math.Max(Math.Abs(R), Math.Abs(Q + R)));

    /// <summary>
    /// Возвращает шесть соседних клеток в порядке <see cref="Directions"/>.
    /// Границы поля здесь не учитываются.
    /// </summary>
    public IEnumerable<Hex> Neighbors()
    {
        foreach (var d in Directions)
        {
            yield return new Hex(Q + d.Q, R + d.R);
        }
    }
}
