using System.Collections.Generic;
using UnityEngine;

public class DrunkWalkerDungeonCreator : MonoBehaviour
{
    public static DungeonCreationValues CreateDungeon(int numberOfWalkers, int numberOfIterations, bool overlapAllowed, Dictionary<Direction, float> _mapBiases)
    {
        HashSet<Vector2Int> positionsVisited = new HashSet<Vector2Int>();
        List<DrunkWalker> drunkWalkers = new List<DrunkWalker>();
        List<Vector2Int> endPositions = new List<Vector2Int>();

        positionsVisited.Add(Vector2Int.zero);
        for (int i = 0; i < numberOfWalkers; i++)
        {
            drunkWalkers.Add(new DrunkWalker(Vector2Int.zero));
        }

        for (int i = 0; i < numberOfIterations; i++)
        {
            foreach (DrunkWalker drunkWalker in drunkWalkers)
            {
                Vector2Int previousPosition = drunkWalker.Position;
                Vector2Int newPosition = Vector2Int.zero;

                if (overlapAllowed)
                {
                    newPosition = drunkWalker.WalkInRandomDirection(_mapBiases);
                    positionsVisited.Add(newPosition);
                }
                else
                {
                    // only do this if we have no overlap allowed
                    for (int k = 0; k < 10; k++)
                    {
                        drunkWalker.Position = previousPosition;
                        newPosition = drunkWalker.WalkInRandomDirection(_mapBiases);

                        if (!positionsVisited.Contains(newPosition))
                        {
                            positionsVisited.Add(newPosition);
                            break;
                        }
                    }
                }

                if (i >= numberOfIterations - 1)
                {
                    endPositions.Add(newPosition);
                }
            }
        }

        return new DungeonCreationValues(positionsVisited, endPositions);
    }
}

