using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;


public class PerlinNoiseEditor : MonoBehaviour {

    public MapGeneratorPerlinNoise mapGenerator;

    public Toggle keepSeed;
    public InputField seed;
    public InputField scale;
    public InputField octave;
    public InputField lacunarity;
    public InputField elevation;
    public Slider persistance;

    private InputField[] colorHeightInputsFields;
    private bool seedModified = false;
  
    private void Start() {
        seed.text = mapGenerator.Seed.ToString();
        scale.text = mapGenerator.Scale.ToString();
        octave.text = mapGenerator.Octaves.ToString();
        lacunarity.text = mapGenerator.Lacunarity.ToString();
        elevation.text = mapGenerator.Elevation.ToString();
        persistance.value = mapGenerator.Persistance;
        keepSeed.isOn = mapGenerator.KeepSeed;

        List<GameObject> colorHeightGO = new List<GameObject>();
        Transform colorPanel = transform.GetChild(0).GetChild(0);
        for (int i = 1; i < colorPanel.childCount; i++) {
            colorHeightGO.Add(colorPanel.GetChild(i).GetChild(1).gameObject);
        }

        
        colorHeightInputsFields = new InputField[colorHeightGO.Count];
        for (int i = 0; i < colorHeightInputsFields.Length; i++) {
            colorHeightInputsFields[i] = colorHeightGO[i].GetComponent<InputField>();
            colorHeightInputsFields[i].text = mapGenerator.colors[i].ToString();
        }
    }

    public void Update() {
        if (!seed.isFocused) {
            seed.text = mapGenerator.Seed.ToString();
        }
    }

    public void UpdateKeepSeed(bool value)
    {
        mapGenerator.KeepSeed = value;
    }

    public void UpdateSeed(string value) {
        mapGenerator.Seed = int.Parse(value);
        seedModified = true;
        mapGenerator.KeepSeed = true;
    }

    public void UpdateScale(string value) {
        mapGenerator.Scale = int.Parse(value);
    }

    public void UpdateOctaves(string value) {
        mapGenerator.Octaves = int.Parse(value);
    }

    public void UpdateLacunarity(string value) {
        mapGenerator.Lacunarity = float.Parse(value);
    }

    public void UpdateElevation(string value) {
        mapGenerator.Elevation = int.Parse(value);
    }

    public void UpdatePersistance(float value) {
        mapGenerator.Persistance = value;
    }

    public void UpdateColorHeights() {
        float[] heights = new float[colorHeightInputsFields.Length];
        for (int i = 0; i < colorHeightInputsFields.Length; i++) {
            mapGenerator.colors[i] = float.Parse(colorHeightInputsFields[i].text);
        }
    }

    public void Generate() {
        mapGenerator.GenerateMap();
        if (seedModified) {
            mapGenerator.KeepSeed = false;
        }
    }

   
}
