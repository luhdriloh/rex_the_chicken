using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room
{
    public Rect _roomRect;
    private Vector2Int _position;
    private Vector2Int _roomSize;
    private List<Vector2Int> _connection;


    public Room(Vector2Int position, Vector2Int widthHeight)
    {
        _position = position;
        _roomSize = widthHeight;
        _roomRect = new Rect(position.x, position.y, widthHeight.x, widthHeight.y);
    }

    public Vector2Int ReturnRoomPosition()
    {
        return _position;
    }

    public Vector2Int ReturnRoomSize()
    {
        return _roomSize;
    }

    public Vector2 ReturnRoomCenter()
    {
        Vector2 half = new Vector2(_roomSize.x / 2, _roomSize.y / 2);
        return _position * 20 + half;
    }
}
