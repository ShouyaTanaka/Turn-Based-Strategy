using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Utility;

public class BoardGenerate : MonoBehaviour
{
    [Header("Tile Sprite")]

    [Tooltip("通行不可")]
    public Sprite blocked;
    [Tooltip("通行可能")]
    public Sprite ground;

    public int mapWidth;
    public int mapHeight;

    public void Start()
    {
        int[,] mapData = CSVLoader.CSVMapLoad("map00");

        mapWidth = mapData.GetLength(0);
        mapHeight = mapData.GetLength(1);

        GenerateMap(mapData);
    }

    public void GenerateMap(int[,] mapData)
    {
        for(int x = 0; x < mapWidth; x++)
        {
            for(int y = 0; y < mapHeight; y++)
            {
                Sprite tileSprite;
                int type = mapData[x,y];

                tileSprite = (Map)type == Map.Blocked? blocked : ground; // 種類が増えたらswitch文

                SetSprite(x,y,tileSprite,mapData);
            }
        }
    }

    public void SetSprite( float x, float y, Sprite tileSprite, int[,] mapData)
    {
        GameObject newTile = new GameObject();

        newTile.transform.parent = this.transform;

        TileData tileData = newTile.AddComponent<TileData>();
        tileData.gridPos = new Vector2Int((int)x, (int)y);
        tileData.tileType = (Map)mapData[(int)x, (int)y];
        tileData.isWalkable = tileData.tileType != Map.Blocked;

        newTile.AddComponent<SpriteRenderer>();
        newTile.GetComponent<SpriteRenderer>().sprite = tileSprite;
        newTile.name = tileSprite.name;
        newTile.transform.position = new Vector2( x + 0.5f, y + 0.5f );
    }
}
