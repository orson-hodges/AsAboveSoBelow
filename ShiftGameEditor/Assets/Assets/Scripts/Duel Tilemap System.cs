using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileType;

public class DuelTilemapSystem : MonoBehaviour
{
    // The 4 neighours of each wall visuals tiles.
    private Vector3Int[] Neighbours = new Vector3Int[]
    {
        new Vector3Int(0, 0, 0), // Top Right
        new Vector3Int(1, 0, 0), // Top Left
        new Vector3Int(0, 1, 0), // Bottom Right
        new Vector3Int(1, 1, 0) // Bottom Left
    };

    public Tilemap WallCollisionTilemap;
    public Tilemap WallVisualsTilemap; 

    // Determine the height and width of the tilemap.
    public int TilemapWidth;
    public int TilemapHeight;

    // The arrays for each tile sprite and their variants
    public Tile[] TileSet0;
    public Tile[] TileSet1;
    public Tile[] TileSet2;
    public Tile[] TileSet3;
    public Tile[] TileSet4;
    public Tile[] TileSet5;
    public Tile[] TileSet6;
    public Tile[] TileSet7;
    public Tile[] TileSet8;
    public Tile[] TileSet9;
    public Tile[] TileSet10;
    public Tile[] TileSet11;
    public Tile[] TileSet12;
    public Tile[] TileSet13;
    public Tile[] TileSet14;
    public Tile[] TileSet15;

    private Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile[]> TileRules;

    void Start()
    {
        // TileRules is used to determine which tile sprite to use depending on the neighouring wall collision tile types.
        TileRules = new()
        {
            {new (Empty, Empty, Empty, Empty), TileSet0}, // Empty Tile
            {new (Placed, Empty, Empty, Empty), TileSet1}, // Top Left Corner Tile
            {new (Empty, Placed, Empty, Empty), TileSet2}, // Top Right Corner Tile
            {new (Empty, Empty, Placed, Empty), TileSet3}, // Bottom Left Corner Tile
            {new (Empty, Empty, Empty, Placed), TileSet4}, // Bottom Right Corner Tile
            {new (Placed, Placed, Empty, Empty), TileSet5}, // Top Column Tile
            {new (Placed, Empty, Placed, Empty), TileSet6}, // Left Row Tile
            {new (Placed, Empty, Empty, Placed), TileSet7}, // Checker Board 1 Tile
            {new (Empty, Placed, Placed, Empty), TileSet8}, // Checker Board 2 Tile
            {new (Empty, Placed, Empty, Placed), TileSet9}, // Right Row Tile
            {new (Empty, Empty, Placed, Placed), TileSet10}, // Bottom Column Tile
            {new (Placed, Placed, Placed, Empty), TileSet11}, // Upside Down L Tile
            {new (Placed, Placed, Empty, Placed), TileSet12}, // L Tile
            {new (Placed, Empty, Placed, Placed), TileSet13}, // Reversed Upside Down L Tile 
            {new (Empty, Placed, Placed, Placed), TileSet14}, // Reversed L Tile
            {new (Placed, Placed, Placed, Placed), TileSet15} // Full Tile
        };

        RenderVisualTilemap();
    }

    // Goes through every wall collision tile in the area
    void RenderVisualTilemap() 
    {
        for (int width = -TilemapWidth/2; width < TilemapWidth/2; width++) 
        {
            for (int height = -TilemapHeight/2; height < TilemapHeight/2; height++) 
            {
                SetWallVisualsTileSprite(new Vector3Int(width, height, 0));
            }
        }
    }

    // Set the tile sprite for each of the neighouring wall visuals tiles.
    void SetWallVisualsTileSprite(Vector3Int position)
    {
        for (int neighour = 0; neighour < Neighbours.Length; neighour++) 
        {
            Vector3Int NewPosition = position + Neighbours[neighour];
            WallVisualsTilemap.SetTile(NewPosition, CalculateWallVisualsTileSprite(NewPosition));
        }
    }

    // Calculates the tile sprite of the wall visuals tile based on the neighouring wall collision tiles.
    Tile CalculateWallVisualsTileSprite(Vector3Int coordinates)
    {
        // checks if there is a placed tile in the neighouring wall collision tiles.
        TileType TopRight = GetWallCollisionTileType(coordinates - Neighbours[0]);
        TileType TopLeft = GetWallCollisionTileType(coordinates - Neighbours[1]);
        TileType BottomRight = GetWallCollisionTileType(coordinates - Neighbours[2]);
        TileType BottomLeft = GetWallCollisionTileType(coordinates - Neighbours[3]);

        // Picks the tile Set based on TileRules
        Tuple<TileType, TileType, TileType, TileType> NeighourTiles = new(TopLeft, TopRight, BottomLeft, BottomRight);
        Tile[] TileSet = TileRules[NeighourTiles];

        // Picks a random variant of the selected tile set.
        int Index = UnityEngine.Random.Range(0, TileSet.Length);

        // Sets the tile sprite.
        return TileSet[Index];
    }

    // Gets the tile type of the neighour tile.
    TileType GetWallCollisionTileType(Vector3Int coordinates)
    {
        if (WallCollisionTilemap.GetTile(coordinates) != null)
            return Placed;
        else
            return Empty;
    }
}

enum TileType
{
        Placed,
        Empty
}
