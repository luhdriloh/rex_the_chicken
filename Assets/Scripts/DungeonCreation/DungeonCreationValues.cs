using System.Collections.Generic;
using UnityEngine;

public class DungeonCreationValues
{
	private HashSet<Vector2Int> _dungeonFillPositions;
	private List<Vector2Int> _endPositions;

    public DungeonCreationValues(HashSet<Vector2Int> dungeonFillPositions, List<Vector2Int> endPositions)
    {
        _dungeonFillPositions = dungeonFillPositions;
        _endPositions = endPositions;
    }

    public HashSet<Vector2Int> GetDungeonFill()
    {
        return _dungeonFillPositions;
    }

    public List<Vector2Int> GetDungeonEndPosition()
    {
        return _endPositions;
    }
}
