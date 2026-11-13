using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using static TileType;

public class DuelGridTilemapSystem : MonoBehaviour
{
    // The 4 neighours of each wall visuals tiles.
    private Vector3Int[] TileNeighbours = new Vector3Int[]
    {
        new Vector3Int(0, 0, 0), // Top Right
        new Vector3Int(1, 0, 0), // Top Left
        new Vector3Int(0, 1, 0), // Bottom Right
        new Vector3Int(1, 1, 0) // Bottom Left
    };

    // Tilemap used to determine the sprites of the visual tilemap.
    public Tilemap PlacementTilemap;

    // The visual tilemap with all the sprites.
    public Tilemap SpriteTilemap; 

    // Determine the height and width of the tilemap.
    public int TilemapWidth;
    public int TilemapHeight;

    // The arrays for each tile sprite and their variants
    public Tile[] Tile0Set;
    public Tile[] Tile1Set;
    public Tile[] Tile2Set;
    public Tile[] Tile3Set;
    public Tile[] Tile4Set;
    public Tile[] Tile5Set;
    public Tile[] Tile6Set;
    public Tile[] Tile7Set;
    public Tile[] Tile8Set;
    public Tile[] Tile9Set;
    public Tile[] Tile10Set;
    public Tile[] Tile11Set;
    public Tile[] Tile12Set;
    public Tile[] Tile13Set;
    public Tile[] Tile14Set;
    public Tile[] Tile15Set;

    // The dictionary used to store the rules for tiles.
    private Dictionary<Tuple<TileType, TileType, TileType, TileType>, Tile[]> TileRules;

    void Start()
    {
        // TileRules is used to determine which tile sprite set to use depending on the neighouring placement tile types.
        // (1, 2, 3, 4) = [1][2]
        //                [3][4]
        TileRules = new()
        {
            {new (Empty, Empty, Empty, Empty), Tile0Set}, // Empty Tile
            {new (Placed, Empty, Empty, Empty), Tile1Set}, // Top Left Corner Tile
            {new (Empty, Placed, Empty, Empty), Tile2Set}, // Top Right Corner Tile
            {new (Empty, Empty, Placed, Empty), Tile3Set}, // Bottom Left Corner Tile
            {new (Empty, Empty, Empty, Placed), Tile4Set}, // Bottom Right Corner Tile
            {new (Placed, Placed, Empty, Empty), Tile5Set}, // Top Column Tile
            {new (Placed, Empty, Placed, Empty), Tile6Set}, // Left Row Tile
            {new (Placed, Empty, Empty, Placed), Tile7Set}, // Checker Board 1 Tile
            {new (Empty, Placed, Placed, Empty), Tile8Set}, // Checker Board 2 Tile
            {new (Empty, Placed, Empty, Placed), Tile9Set}, // Right Row Tile
            {new (Empty, Empty, Placed, Placed), Tile10Set}, // Bottom Column Tile
            {new (Placed, Placed, Placed, Empty), Tile11Set}, // Upside Down L Tile
            {new (Placed, Placed, Empty, Placed), Tile12Set}, // L Tile
            {new (Placed, Empty, Placed, Placed), Tile13Set}, // Reversed Upside Down L Tile 
            {new (Empty, Placed, Placed, Placed), Tile14Set}, // Reversed L Tile
            {new (Placed, Placed, Placed, Placed), Tile15Set} // Full Tile
        };

        RenderSpriteTilemap();
    }

    // Goes through every placement tile in the area
    void RenderSpriteTilemap() 
    {
        for (int width = -TilemapWidth/2; width < TilemapWidth/2; width++) 
        {
            for (int height = -TilemapHeight/2; height < TilemapHeight/2; height++) 
            {
                SetSpriteTile(new Vector3Int(width, height, 0));
            }
        }
    }

    // Set the sprite for each of the neighouring sprite tiles.
    void SetSpriteTile(Vector3Int position)
    {
        for (int neighour = 0; neighour < TileNeighbours.Length; neighour++) 
        {
            Vector3Int NewPosition = position + TileNeighbours[neighour];
            SpriteTilemap.SetTile(NewPosition, CalculateSpriteTile(NewPosition));
        }
    }

    // Calculates the sprite of the sprite tile based on the neighouring placement tiles.
    Tile CalculateSpriteTile(Vector3Int coordinates)
    {
        // checks if there is a placed tile in the neighouring placement tiles.
        TileType TopRight = GetPlacementTileType(coordinates - TileNeighbours[0]);
        TileType TopLeft = GetPlacementTileType(coordinates - TileNeighbours[1]);
        TileType BottomRight = GetPlacementTileType(coordinates - TileNeighbours[2]);
        TileType BottomLeft = GetPlacementTileType(coordinates - TileNeighbours[3]);

        // Picks the tile set based on TileRules
        Tuple<TileType, TileType, TileType, TileType> NeighourTiles = new(TopLeft, TopRight, BottomLeft, BottomRight);
        Tile[] TileSet = TileRules[NeighourTiles];

        // Picks a random variant of the selected tile set.
        int Index = UnityEngine.Random.Range(0, TileSet.Length);

        // Sets the tile sprite.
        return TileSet[Index];
    }

    // Gets the tile type of the neighour placement tile.
    TileType GetPlacementTileType(Vector3Int coordinates)
    {
        if (PlacementTilemap.GetTile(coordinates) != null)
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
