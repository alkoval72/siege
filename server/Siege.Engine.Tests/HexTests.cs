namespace Siege.Engine.Tests;

public class HexTests
{
    [Fact]
    public void DistanceFromCenter_Center_IsZero()
    {
        Assert.Equal(0, Hex.Center.DistanceFromCenter);
    }

    [Theory]
    [InlineData(1, 0, 1)]
    [InlineData(1, -1, 1)]
    [InlineData(-3, 1, 3)]
    [InlineData(10, -10, 10)]
    [InlineData(5, 5, 10)]
    public void DistanceFromCenter_AnyHex_IsMaxOfAxes(int q, int r, int expected)
    {
        Assert.Equal(expected, new Hex(q, r).DistanceFromCenter);
    }

    [Fact]
    public void Neighbors_AnyHex_ReturnsSixDistinctAdjacentHexes()
    {
        var hex = new Hex(2, -1);

        var neighbors = hex.Neighbors().ToList();

        Assert.Equal(6, neighbors.Distinct().Count());
        Assert.All(neighbors, n => Assert.Equal(1, new Hex(n.Q - hex.Q, n.R - hex.R).DistanceFromCenter));
    }
}
