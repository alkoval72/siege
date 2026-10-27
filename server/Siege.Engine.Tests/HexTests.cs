namespace Siege.Engine.Tests;

public class HexTests
{
    [Fact]
    public void Center_Always_IsOrigin()
    {
        Assert.Equal(new Hex(0, 0), Hex.Center);
    }

    [Fact]
    public void Directions_Always_MatchArchitectureOrder()
    {
        Hex[] expected = [new(1, 0), new(1, -1), new(0, -1), new(-1, 0), new(-1, 1), new(0, 1)];

        Assert.Equal(expected, Hex.Directions);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(2)]
    public void Directions_OppositeDirection_IsThreeStepsAhead(int index)
    {
        var direction = Hex.Directions[index];
        var opposite = Hex.Directions[index + 3];

        Assert.Equal(Hex.Center, new Hex(direction.Q + opposite.Q, direction.R + opposite.R));
    }

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
    [InlineData(-4, -6, 10)]
    [InlineData(0, -7, 7)]
    [InlineData(-10, 10, 10)]
    [InlineData(11, 0, 11)]
    [InlineData(10, 1, 11)]
    [InlineData(-5, -6, 11)]
    public void DistanceFromCenter_AnyHex_IsMaxOfAxes(int q, int r, int expected)
    {
        Assert.Equal(expected, new Hex(q, r).DistanceFromCenter);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(1, 7)]
    [InlineData(10, 331)]
    public void DistanceFromCenter_HexesWithinRadius_CountMatchesBoardSize(int radius, int expected)
    {
        var area = Enumerable.Range(-radius - 1, 2 * radius + 3);

        var count = area.SelectMany(q => area.Select(r => new Hex(q, r)))
            .Count(h => h.DistanceFromCenter <= radius);

        Assert.Equal(expected, count);
    }

    [Fact]
    public void Neighbors_AnyHex_ReturnsSixDistinctAdjacentHexes()
    {
        var hex = new Hex(2, -1);

        var neighbors = hex.Neighbors().ToList();

        Assert.Equal(6, neighbors.Distinct().Count());
        Assert.All(neighbors, n => Assert.Equal(1, new Hex(n.Q - hex.Q, n.R - hex.R).DistanceFromCenter));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(2, -1)]
    [InlineData(-10, 4)]
    public void Neighbors_AnyHex_ReturnsHexesInDirectionsOrder(int q, int r)
    {
        var expected = Hex.Directions.Select(d => new Hex(q + d.Q, r + d.R));

        var neighbors = new Hex(q, r).Neighbors();

        Assert.Equal(expected, neighbors);
    }

    [Fact]
    public void Neighbors_OfEachNeighbor_ContainOriginalHex()
    {
        var hex = new Hex(-3, 5);

        var neighbors = hex.Neighbors().ToList();

        Assert.All(neighbors, n => Assert.Contains(hex, n.Neighbors()));
    }

    [Theory]
    [InlineData(10, 0, 3)]
    [InlineData(-10, 10, 3)]
    [InlineData(10, -5, 2)]
    [InlineData(9, 0, 0)]
    public void Neighbors_HexNearBoardEdge_IncludesHexesBeyondRadius(int q, int r, int expectedBeyond)
    {
        const int radius = 10;

        var beyond = new Hex(q, r).Neighbors().Count(n => n.DistanceFromCenter > radius);

        Assert.Equal(expectedBeyond, beyond);
    }
}
