using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof(HexGrid))]
public class MapGeneratorPerlinNoise : MonoBehaviour {

    public float Scale;
    public bool KeepSeed;
    public int Seed;
    public int Octaves;
    public float Lacunarity;
    [Range(0, 1)]
    public float Persistance;
    public int Elevation;


    public float[] colors;
    

    private HexGrid hexGrid;
    private int mapWidth, mapHeight;

    private float waterLevel;
    private float levelHeight;
    private float[] heights;    

    private void Start() {
        hexGrid = GetComponent<HexGrid>();
        mapWidth = hexGrid.chunkCountX * HexMetrics.chunkSizeX;
        mapHeight = hexGrid.chunkCountZ * HexMetrics.chunkSizeZ;
        waterLevel = 1 - colors[1];
        levelHeight = waterLevel / Elevation;
    }

    private void OnValidate() {
        if (Lacunarity < 1) {
            Lacunarity = 1;
        }
        if (Octaves < 0) {
            Octaves = 0;
        }
    }

    public float GetPersistance() {
        return Persistance;
    }

    public void GenerateMap() {        

        float[,] noiseMap = GenerateNoiseMap();

        AssignColors(noiseMap);
    }

    public float[,] GenerateNoiseMap() {

        float[,] noiseMap = new float[mapWidth, mapHeight];

        
        Vector2[] octaveOffsets = GenerateOctaveOffsets();

        if (Scale <= 0) {
            Scale = 0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
      

        for (int z = 0; z < mapHeight; z++) {
            for (int x = 0; x < mapWidth; x++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i < Octaves; i++) {
                    float sampleX = x / Scale * frequency + octaveOffsets[i].x;
                    float sampleY = z / Scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY) * 2 - 1;
                    noiseHeight += perlinValue * amplitude;

                    amplitude *= Persistance;
                    frequency *= Lacunarity;
                }   

                if (noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight = noiseHeight;
                }
                else if (noiseHeight < minNoiseHeight) {
                    minNoiseHeight = noiseHeight;
                }

                noiseMap[x, z] = noiseHeight;

            }
        }

        for (int z = 0; z < mapHeight; z++) {
            for (int x = 0; x < mapWidth; x++) {
                noiseMap[x, z] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, z]);
            }
        }
        return noiseMap;
    }


    public Vector2[] GenerateOctaveOffsets() {

        if (!KeepSeed) {
            Seed = Random.Range(0, 1000000);
        }

        System.Random prng = new System.Random(Seed);
        Vector2[] octaveOffsets = new Vector2[Octaves];
        for (int i = 0; i < Octaves; i++) {
            float offsetX = prng.Next(-100000, 100000);
            float offsetY = prng.Next(-100000, 100000);
            octaveOffsets[i] = new Vector2(offsetX, offsetY);
        }
        return octaveOffsets;
    }

    public void AssignColors(float[,] noiseMap) {

        heights = new float[Elevation];
        if (colors.Length > 0) {
            waterLevel = 1 - colors[1];
            levelHeight = waterLevel / Elevation;

            for (int i = 0; i < Elevation; i++) {
                heights[i] = colors[1] + levelHeight * i;
            }
        }

        for (int z = 0; z < mapHeight; z++) {
            for (int x = 0; x < mapWidth; x++) {
                HexCell cell = hexGrid.GetCell(HexCoordinates.FromOffsetCoordinates(x, z));

                float noiseValue = noiseMap[x, z];

                if (colors.Length > 1) {
                    for (int i = 1; i < colors.Length; i++) {
                        if (noiseValue < colors[i]) {
                            cell.SetTerrainTypeIndex(i - 1);
                            SetElevation(cell, noiseValue);
                            break;
                        }
                        if (i == colors.Length - 1) {
                            cell.SetTerrainTypeIndex(i);
                            SetElevation(cell, noiseValue);
                        }
                    }
                }
                else if (colors.Length == 1){
                    cell.SetTerrainTypeIndex(0);
                    cell.SetElevation(0);
                }

                if ((x + 1) % HexMetrics.chunkSizeX == 0) {
                    cell.Refresh();
                }
            }
        }
    }

    public void SetElevation(HexCell cell, float noiseValue) {
        for (int i = 0; i < heights.Length; i++) {
            if (noiseValue < heights[i]) {
                cell.SetElevation(i);
                break;
            }
            if (i == heights.Length - 1) {
                cell.SetElevation(i);
            }
        }
    }
}



