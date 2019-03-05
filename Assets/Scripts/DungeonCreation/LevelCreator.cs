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
    public LevelCreationData _levelCreationData;
    public Tilemap _tilemapToDrawFloor;
    public Tilemap _tilemapForFill;

    public TileBase _worldFill;
    public TileBase _fillTile;
    public TileBase _fillRuleTile;

    private HashSet<Vector2Int> _dungeonTiles;
    private List<Vector2Int> _mapRect;
    private LevelPlacer _placer;

    private void Start()
    {
        _dungeonTiles = DrunkWalkerDungeonCreator.CreateDungeon(_levelCreationData._numberOfWalkers, _levelCreationData._numberOfIterations, _levelCreationData._overlapAllowed);

        if (_levelCreationData._tileSize > 1)
        {
            _dungeonTiles = ReturnListOfScaledTiles(_dungeonTiles);
        }

        List<Vector2Int> edgeTiles = ReturnEdgeTiles(_dungeonTiles);
        HashSet<Vector2Int> fillTiles = new HashSet<Vector2Int>(_dungeonTiles.Except(edgeTiles));

        _placer = GetComponent<LevelPlacer>();
        _placer.PlaceThings(fillTiles);

        // a vector indicating places to draw grass or random environment details
        // grass
        // water
        // flowers / mushrooms etc

        DrawWorldFill(_mapRect, _tilemapForFill, _worldFill);
        DrawDungeonTiles(_dungeonTiles, _tilemapForFill, _fillTile);
        DrawDungeonTiles(_dungeonTiles, _tilemapToDrawFloor, _fillRuleTile);
    }


    //private void Update()
    //{
        //if (_scanned == false && Time.time - _startTime > _scanTime)
        //{
        //    AstarPath.active.Scan();
        //    _scanned = true;
        //}
    //}


    //private void SetUpAStarGraph()
    //{
    //    // This holds all graph data
    //    AstarData data = AstarPath.active.data;

    //    // This creates a Grid Graph
    //    GridGraph gg = data.gridGraph;
    //    gg.rotation = new Vector3(gg.rotation.y - 90, 270, 90);

    //    // set grid graph settings
    //    gg.collision.use2D = true;
    //    gg.collision.type = ColliderType.Sphere;
    //    gg.collision.diameter = 1;
    //    gg.collision.mask = LayerMask.GetMask("MapEdge");

    //    // Setup a grid graph with some values
    //    int width = _mapRect[1].x - _mapRect[0].x;
    //    int depth = _mapRect[1].y - _mapRect[0].y;
    //    float nodeSize = 1;

    //    gg.center = new Vector3(_mapRect[0].x + width / 2f, _mapRect[0].y + depth / 2f, 0);
        

    //    // Updates internal size from the above values
    //    gg.SetDimensions(width, depth, nodeSize);
    //}


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


    private void DrawDungeonTiles(IEnumerable<Vector2Int> tiles, Tilemap tilemapToUse, TileBase tilesToUse)
    {
        foreach (Vector2Int tileLocation in tiles)
        {
            tilemapToUse.SetTile(new Vector3Int(tileLocation.x, tileLocation.y, 0), tilesToUse);
        }
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

                    // set map bounds
                    if (newPosition.x < lowerLeft.x)
                    {
                        lowerLeft.x = newPosition.x;
                    }
                    else if (newPosition.y < lowerLeft.y)
                    {
                        lowerLeft.y = newPosition.y;
                    }

                    if (newPosition.x > topRight.x)
                    {
                        topRight.x = newPosition.x;
                    }
                    else if (newPosition.y > topRight.y)
                    {
                        topRight.y = newPosition.y;
                    }

                    scaledTiles.Add(newPosition);
                }
            }
        }

        _mapRect = new List<Vector2Int>() { lowerLeft, topRight };
        return scaledTiles;
    }

    private List<Vector2Int> ReturnEdgeTiles(HashSet<Vector2Int> dungeonPositions)
    {
        List<Vector2Int> edgeTiles = new List<Vector2Int>();

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



























