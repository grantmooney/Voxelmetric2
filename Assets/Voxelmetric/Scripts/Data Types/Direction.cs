public enum Direction {
    north,
    east,
    south,
    west,
    up,
    down
}

public static class DirectionUtils
{
    /// <summary>
    /// All available direction values
    /// </summary>
    public static Direction[] Directions = new Direction[] {
        Direction.north, Direction.east, Direction.south, Direction.west, Direction.up, Direction.down
    };

    public static System.Collections.Generic.Dictionary<Direction, Direction> Opposites = new System.Collections.Generic.Dictionary<Direction, Direction>
    {
        { Direction.north, Direction.south },
        { Direction.east, Direction.west },
        { Direction.south, Direction.north },
        { Direction.west, Direction.east },
        { Direction.up, Direction.down },
        { Direction.down, Direction.up }
    };
}