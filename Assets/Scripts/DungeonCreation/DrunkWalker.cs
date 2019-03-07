using System.Collections.Generic;
using UnityEngine;


public enum Direction
{
    NORTH = 0,
    EAST = 1,
    SOUTH = 2,
    WEST = 3
};


public class DrunkWalker
{
    public Vector2Int Position { get; set; }
    private readonly Dictionary<Direction, Vector2Int> _directionToMovementMapping = new Dictionary<Direction, Vector2Int>
    {
        { Direction.NORTH, new Vector2Int(0, 1) },
        { Direction.EAST, new Vector2Int(1, 0) },
        { Direction.SOUTH, new Vector2Int(0, -1) },
        { Direction.WEST, new Vector2Int(-1, 0) }
    };


    public DrunkWalker(Vector2Int startPosition)
    {
        Position = startPosition;
    }


    public Vector2Int WalkInRandomDirection(Dictionary<Direction, float> _mapBiases)
    {
        float whichDirection = Random.value;
        float percent = 0;

        Direction directionForWalkerToGo = Direction.NORTH;

        foreach (Direction direction in _mapBiases.Keys)
        {
            percent += _mapBiases[direction];

            if (whichDirection < percent)
            {
                directionForWalkerToGo = direction;
                break;
            }
        }

        Position += _directionToMovementMapping[directionForWalkerToGo];
        return Position;
    }
}
