using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class NoiseGenerator
{
    public static float [,] GenerateN (int width , int height , int seed , float scale , int octaves , float persistence , float lacunarity , Vector2Int offset)
    {
        float[,] noise = new float[width, height];
        System.Random rand = new System.Random(seed);

        Vector2[] octavesOffset = new Vector2[octaves];

        for (int i = 0; i < octaves; i++)
        {
            float xOffset = rand.Next(-100000, 100000) + offset.x * (width / scale);
            float yOffset = rand.Next(-100000, 100000) + offset.y * (height / scale);

            octavesOffset[i] = new Vector2(xOffset / width, yOffset / height);
        }
        if (scale < 0) scale = 0.0001f;

        float hlafWidth = width / 2.0f;
        float hlafHeight = height / 2.0f;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;
                float superpositionCompensation = 0;

                for (int i = 0; i < octaves; i++)
                {
                    float xResult = (x - hlafWidth) / scale * frequency + octavesOffset[i].x * frequency;
                    float yResult = (y - hlafHeight) / scale * frequency + octavesOffset[i].y * frequency;

                    float generateValue = Mathf.PerlinNoise(xResult, yResult);

                    noiseHeight += generateValue * amplitude;
                    noiseHeight -= superpositionCompensation;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                    superpositionCompensation = amplitude / 2;

                }

                noise[x, y] = Mathf.Clamp01(noiseHeight);
            }
        }
        return noise;
    }
}
public class GenerateNioise : MonoBehaviour
{
    [SerializeField] private SpriteTileMode tile;
    [SerializeField] private Tilemap[] tilemap;
    private int idxTilemap = 0;

    [Header("Camera")]
    [SerializeField] private Camera cam;
    [Range(0, 50)] [SerializeField] private float panSpeed = 5.0f;
    private Vector3 panMovement;
    private Vector2Int posCam;
    private Vector3 direct;

    [Header("Noise Configuration")]
    [SerializeField] private int seed = 100;
    [Range(1, 100)] [SerializeField] private float scale = 30.0f;
    [Range(1, 5)] [SerializeField] private int octaves = 1;
    [Range(0, 1)] [SerializeField] private float persistence = 0.5f;
    [SerializeField] private float lacunarity = 1f;

    [Serializable]
    private struct ColorLevel
    {
        public float height;
        public Color color;
    }

    [Header("Terrain Color Configuration")]
    [SerializeField] private List<ColorLevel> ColorMap = new List<ColorLevel>();

    private int noiseDimensionalX = 150;
    private int noiseDimensionalY = 100;
    private float[,] noiseMap;

    void TilemapInit()
    {
        noiseMap = new float[noiseDimensionalX, noiseDimensionalY];

        posCam = new Vector2Int((int)cam.transform.position.x, (int)cam.transform.position.y);
        noiseMap = NoiseGenerator.GenerateN(noiseDimensionalX, noiseDimensionalY, seed, scale, octaves, persistence,lacunarity,posCam);

        for(int y = -(noiseDimensionalX/2); y<(noiseDimensionalY/2); y++)
        {
            for(int x = -(noiseDimensionalX/2); x<(noiseDimensionalY/2); x++)
            {
               // tilemap[0].SetTile(new Vector3Int(x, y, 0), tile);
                //tilemap[1].SetTile(new Vector3Int(x, y, 0), tile);

                tilemap[0].SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);
                tilemap[1].SetTileFlags(new Vector3Int(x, y, 0), TileFlags.None);

                tilemap[0].SetColor(new Vector3Int(x, y, 0), ColorMap[ColorMap.Count -1].color);

                foreach (var level in ColorMap)
                {
                    if(noiseMap[x+(noiseDimensionalX / 2),y+(noiseDimensionalY / 2)] < level.height)
                    {
                        tilemap[0].SetColor(new Vector3Int(x, y, 0), level.color);
                        break;
                    }
                }


            }
        }
    }


}
