using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ProceduralGeneration : MonoBehaviour
{
    [Header("Terrain Gen")]
    [SerializeField] int width; // width of the map
    [SerializeField] int height; // height of the map
    [SerializeField] float smothness;
    int[] perlinHeightList;

    [Header("Cave Gen")]
    [Range(1, 100)]
    [SerializeField] int randomFillPrecent;
    [SerializeField] int smoothAmount;

    [Header("Tile")]
    [SerializeField] TileBase groundTile;
    [SerializeField] TileBase caveTile;
    [SerializeField] Tilemap groundTileMap;
    [SerializeField] Tilemap caveTileMap;

    [SerializeField] float seed;

    int[,] map;

    void Start()
    {
        perlinHeightList = new int[width];
        Generation();
    }

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            Generation();
            Debug.Log("keypress");
        }
    }
    void Generation()
    {
        seed = Time.time;
        clearMap();
        map = GenerateArray(width, height, true);
        map = TerrenGeneration(map);
        SmoothMap(smoothAmount);
        RenderMap(map, groundTileMap, caveTileMap, groundTile , caveTile);
    }

    public int[,] GenerateArray(int width , int height, bool empty)
    {
        int[,] map = new int[width, height];
        for (int x = 0; x < width; x++) // width of the map 
        {
            for (int y = 0; y < height; y++) // height of the map
            {
                map[x, y] = (empty) ? 0 : 1;
            }
        }
        return map;
    }
    public int [,] TerrenGeneration(int[,] map)
    {
        System.Random pesudiRandom = new System.Random(seed.GetHashCode()); // give us random value according to our seed
        int perlinHeight;
        for (int x = 0; x < width; x++) // loop throught the width
        {
            perlinHeight = Mathf.RoundToInt(Mathf.PerlinNoise(x / smothness, seed) * height/2);
            perlinHeight += height / 2;
            perlinHeightList[x] = perlinHeight;
            for (int y = 0; y < perlinHeight; y++) // loops throught the perlin height
            {
                map[x, y] = (pesudiRandom.Next(1, 100) < randomFillPrecent) ? 1 : 2;
            }
        }
        return map;
    }
    void SmoothMap(int smoothAmount)
    {
        for (int i = 0; i < smoothAmount; i++)
        {
            for (int x = 0; x < width; x++) // loop width of the map 
            {
                for (int y = 0; y < perlinHeightList[x]; y++) //  loop perlin height list of the map
                {
                    if (x == 0 || y == 0 || x == width - 1 || y == perlinHeightList[x] - 1)
                    {
                        map[x, y] = 1;
                    }
                    else
                    {
                        int surroundingGroundCount = GetSurroundingGroundCount(x, y);
                        if (surroundingGroundCount > 4)
                        {
                            map[x, y] = 1;
                        }
                        else if (surroundingGroundCount < 4)
                        {
                            map[x, y] = 2;
                        }
                    }
                }
            }
        }
        
    }
    int GetSurroundingGroundCount( int gridX , int gridY)
    {
        int groundCount = 0;
        for (int nebX = gridX-1; nebX <= gridX+1; nebX++)
        {
            for (int nebY = gridY -1; nebY <= gridY +1; nebY++)
            {
                if (nebX >= 0 && nebX < width && nebY >= 0 && nebY < height) // if we inside the map
                {
                    if(nebX != gridX || nebY != gridY)
                    {
                        if(map[nebX , nebY] == 1)
                        {
                            groundCount++;
                        }
                    }
                }
            }
        }
        return groundCount;
    }
    public void RenderMap(int[,]map , Tilemap groundTileMap, Tilemap caveTileMap , TileBase groundTile , TileBase caveTile)
    {
        for (int x = 0; x < width; x++) // loop throught the width of the map
        {
            for (int y = 0; y < height; y++) // loop throught the height of the map
            {
                if (map[x,y] ==1)
                {
                    groundTileMap.SetTile(new Vector3Int(x, y, 0), groundTile);
                }
                else if(map[x,y] == 2)
                {
                    caveTileMap.SetTile(new Vector3Int(x, y, 0), caveTile);
                }
            }
        }
    }
    void clearMap()
    {
        groundTileMap.ClearAllTiles();
        caveTileMap.ClearAllTiles();
    }
}
