using System.Collections.Generic;
using UnityEngine;

public class DrunkWalkerDungeonCreator : MonoBehaviour
{
    public static HashSet<Vector2Int> CreateDungeon(int numberOfWalkers, int numberOfIterations, bool overlapAllowed)
    {
        HashSet<Vector2Int> positionsVisited = new HashSet<Vector2Int>();
        List<DrunkWalker> drunkWalkers = new List<DrunkWalker>();

        for (int i = 0; i < numberOfWalkers; i++)
        {
            drunkWalkers.Add(new DrunkWalker(Vector2Int.zero));
        }

        for (int i = 0; i < numberOfIterations; i++)
        {
            foreach (DrunkWalker drunkWalker in drunkWalkers)
            {
                Vector2Int previousPosition = drunkWalker.Position;
                Vector2Int newPosition;

                if (overlapAllowed)
                {
                    newPosition = drunkWalker.WalkInRandomDirection();
                    positionsVisited.Add(newPosition);
                    continue;
                }

                // only do this if we have no overlap allowed
                for (int k = 0; k < 10; k++)
                {
                    drunkWalker.Position = previousPosition;
                    newPosition = drunkWalker.WalkInRandomDirection();

                    if (!positionsVisited.Contains(newPosition))
                    {
                        positionsVisited.Add(newPosition);
                        break;
                    }
                }
            }
        }

        return positionsVisited;
    }
}

