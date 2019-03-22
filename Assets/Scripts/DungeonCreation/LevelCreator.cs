using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.Tilemaps;

/// <summary>
/// The purpose of this class is SOLELY to create the level layout.
/// Another class will be used for creating enemies and dropss
/// </summary>
public class LevelCreator : MonoBehaviour
{
    public GameObject _bushProto;
    public GameObject _treeProto;
    public LevelCreationData _levelCreationData;
    public Tilemap _tilemapToDrawFloor;
    public Tilemap _tilemapToDrawBottom;

    public TileBase _fillRuleTile;
    public TileBase _bottomTile;

    private DungeonCreationValues _dungeonCreationValues;
    private HashSet<Vector2Int> _dungeonTiles;
    private LevelPlacer _placer;

    private Dictionary<Direction, float> _mapBiases;

    private void Start()
    {
        _mapBiases = new Dictionary<Direction, float>
        {
            { Direction.NORTH, .25f },
            { Direction.EAST, .25f },
            { Direction.SOUTH, .25f },
            { Direction.WEST, .25f }
        };

        _dungeonCreationValues = DrunkWalkerDungeonCreator.CreateDungeon(_levelCreationData._numberOfWalkers, _levelCreationData._numberOfIterations, _levelCreationData._overlapAllowed, _mapBiases);
        _dungeonTiles = _dungeonCreationValues.GetDungeonFill();

        if (_levelCreationData._tileSize > 1)
        {
            _dungeonTiles = ReturnListOfScaledTiles(_dungeonTiles);
        }

        HashSet<Vector2Int> edgeTiles = ReturnEdgeTiles(_dungeonTiles);
        HashSet<Vector2Int> fillTiles = new HashSet<Vector2Int>(_dungeonTiles.Except(edgeTiles));

        _placer = GetComponent<LevelPlacer>();
        _placer.PlaceThings(fillTiles, _dungeonCreationValues.GetDungeonEndPosition(), _levelCreationData._tileSize);

        // a vector indicating places to draw grass or random environment details
        // grass
        // water
        // flowers / mushrooms etc

        // dont need the one below, just change the physics shape of the sprite
        //DrawDungeonTiles(edgeTiles, _tilemapForFill, _fillTile);
        DrawDungeonTiles(_dungeonTiles.ToList(), _tilemapToDrawFloor, edgeTiles);
    }


    private void DrawWorldFill(List<Vector2Int> mapRect, Tilemap tileMapToDrawOn, TileBase fillTile)
    {
        Vector2Int bottomLeft = mapRect[0];
        Vector2Int topRight = mapRect[1];

        bottomLeft.x -= 5;
        bottomLeft.y -= 5;

        topRight.x += 5;
        topRight.y += 5;

        for (int i = bottomLeft.x; i < topRight.x; i++)
        {
            for (int j = bottomLeft.y; j < topRight.y; j++)
            {
                tileMapToDrawOn.SetTile(new Vector3Int(i, j, 0), fillTile);
            }
        }
    }


    private void DrawDungeonTiles(List<Vector2Int> tiles, Tilemap tilemapToUse, HashSet<Vector2Int> edges)
    {
        // get some random tiles and create squares of different sizes on them
        // a percent of the amount of tiles that we have 5%
        int maxSideLength = 5;
        HashSet<Vector2Int> bushes = CreateSquares(tiles, maxSideLength);

        foreach (Vector2Int tileLocation in tiles)
        {
            tilemapToUse.SetTile(new Vector3Int(tileLocation.x, tileLocation.y, 0), _fillRuleTile);

            if (bushes.Contains(tileLocation) && edges.Contains(tileLocation) == false)
            {
                float xmin = edges.Contains(tileLocation + Vector2Int.left) ? 0f : -.5f;
                float xmax = edges.Contains(tileLocation + Vector2Int.right) ? 0f : .5f;
                Instantiate(_bushProto, new Vector3(tileLocation.x + Random.Range(xmin, xmax), tileLocation.y + Random.Range(0, .7f), 0), Quaternion.identity);
            }

            if (edges.Contains(tileLocation) == true
                && edges.Contains(tileLocation + Vector2Int.left)
                && edges.Contains(tileLocation + Vector2Int.right)
                && _dungeonTiles.Contains(tileLocation + Vector2Int.down) == false)
            {
                _tilemapToDrawBottom.SetTile(new Vector3Int(tileLocation.x, tileLocation.y, 0), _bottomTile);
            }

            if (edges.Contains(tileLocation) && .01f >= Random.value)
            {
                Instantiate(_treeProto, new Vector3(tileLocation.x, tileLocation.y + Random.Range(1f, 2f), -4f), Quaternion.identity);
            }
        }
    }

    private HashSet<Vector2Int> CreateSquares(List<Vector2Int> tiles, int maxSideLength)
    {
        HashSet<Vector2Int> squareTiles = new HashSet<Vector2Int>();

        int numberOfTilesToMakeIntoSquares = (int)(tiles.Count * .01f);
        for (int i = 0; i < numberOfTilesToMakeIntoSquares; i++)
        {
            int width = Random.Range(maxSideLength / 3, maxSideLength);
            int length = Random.Range(maxSideLength / 3, maxSideLength);

            // get a random tile and start to create the square there
            Vector2Int startTile = tiles[Random.Range(0, tiles.Count)];

            for (int row = 0; row < length; row++)
            {
                for (int column = 0; column < width; column++)
                {
                    squareTiles.Add(new Vector2Int(startTile.x + column, startTile.y + row));
                }
            }
        }

        return squareTiles;
    }

    
    private HashSet<Vector2Int> ReturnListOfScaledTiles(IEnumerable<Vector2Int> tiles)
    {
        HashSet<Vector2Int> scaledTiles = new HashSet<Vector2Int>();
        Vector2Int lowerLeft = Vector2Int.zero;
        Vector2Int topRight = Vector2Int.zero;

        foreach (Vector2Int tileLocation in tiles)
        {
            Vector2Int startPosition = tileLocation * _levelCreationData._tileSize;
            Vector2Int newPosition;

            for (int i = 0; i < _levelCreationData._tileSize; i++)
            {
                for (int j = 0; j < _levelCreationData._tileSize; j++)
                {
                    newPosition = new Vector2Int(i, j) + new Vector2Int(startPosition.x, startPosition.y);
                    scaledTiles.Add(newPosition);
                }
            }
        }

        return scaledTiles;
    }

    private HashSet<Vector2Int> ReturnEdgeTiles(HashSet<Vector2Int> dungeonPositions)
    {
        HashSet<Vector2Int> edgeTiles = new HashSet<Vector2Int>();

        foreach (Vector2Int tilePosition in dungeonPositions)
        {
            if (IsTileSurroundedOnAllSides(dungeonPositions, tilePosition) == false)
            {
                edgeTiles.Add(tilePosition);
            }
        }

        return edgeTiles;
    }

    private bool IsTileSurroundedOnAllSides(HashSet<Vector2Int> dungeonPositions, Vector2Int positionToCheck)
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Vector2Int toCheck = new Vector2Int(positionToCheck.x + i, positionToCheck.y + j);
                if (dungeonPositions.Contains(toCheck) == false)
                {
                    return false;
                }
            }
        }

        return true;
    }
}



























