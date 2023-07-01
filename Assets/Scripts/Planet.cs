// Heavily inspired by tutorial series from Sebastian Lague found at https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

public class Planet : MonoBehaviour
{

    [Range(2, 256)]
    public int resolution = 10;
    public bool autoUpdate = true;
    public enum FaceRenderMask { All, Top, Bottom, Left, Right, Front, Back };
    public FaceRenderMask faceRenderMask;

    public ShapeSettings shapeSettings;
    public ColourSettings colourSettings;

    [HideInInspector]
    public bool shapeSettingsFoldout;
    [HideInInspector]
    public bool colourSettingsFoldout;

    ShapeGenerator shapeGenerator = new ShapeGenerator();
    ColourGenerator colourGenerator = new ColourGenerator();

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;


    void Initialize()
    {
        shapeGenerator.UpdateSettings(shapeSettings);
        colourGenerator.UpdateSettings(colourSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }
        terrainFaces = new TerrainFace[6];

        Vector3[] directions = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject meshObj = new GameObject("mesh");
                meshObj.transform.parent = transform;

                meshObj.AddComponent<MeshRenderer>();
                meshFilters[i] = meshObj.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            meshFilters[i].GetComponent<MeshRenderer>().sharedMaterial = colourSettings.planetMaterial;

            terrainFaces[i] = new TerrainFace(shapeGenerator, meshFilters[i].sharedMesh, resolution, directions[i]);
            bool renderFace = faceRenderMask == FaceRenderMask.All || (int)faceRenderMask - 1 == i;
            meshFilters[i].gameObject.SetActive(renderFace);

        }
    }

    public void GeneratePlanet()
    {
        Initialize();
        GenerateMesh();
        GenerateColours();
    }

    public void GenerateRandomPlanetShape()
    {
        resolution = 100;

        shapeSettings = CreateRandomShapeSettings();

        GeneratePlanet();
    }

    public void GenerateRandomPlanetColours()
    {
        colourSettings = CreateRandomColourSettings();

        OnColourSettingsUpdated();

    }

    public void OnShapeSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateMesh();
        }
    }

    public void OnColourSettingsUpdated()
    {
        if (autoUpdate)
        {
            Initialize();
            GenerateColours();
        }
    }

    void GenerateMesh()
    {
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].ConstructMesh();
            }
        }

        colourGenerator.UpdateElevation(shapeGenerator.elevationMinMax);
    }

    void GenerateColours()
    {
        colourGenerator.UpdateColours();

        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i].gameObject.activeSelf)
            {
                terrainFaces[i].UpdateUVs(colourGenerator);
            }
        }
    }

    public static ShapeSettings CreateRandomShapeSettings()
    {
        int counter = 1;
        string assetPath = $"Assets/Settings/NewShapeSettings{counter}.asset";
        while (File.Exists(assetPath))
        {
            counter++;
            assetPath = $"Assets/Settings/NewShapeSettings{counter}.asset";
        }

        ShapeSettings newShapeSettings = ScriptableObject.CreateInstance<ShapeSettings>();
        AssetDatabase.CreateAsset(newShapeSettings, assetPath);

        newShapeSettings.planetRadius = 10;

        newShapeSettings.noiseLayers = new ShapeSettings.NoiseLayer[2]; // Initialize with two elements

        // Create and initialize the first NoiseLayer and its properties
        ShapeSettings.NoiseLayer newNoiseLayer1 = new ShapeSettings.NoiseLayer();
        newNoiseLayer1.noiseSettings = new NoiseSettings();
        newNoiseLayer1.noiseSettings.simpleNoiseSettings = new NoiseSettings.SimpleNoiseSettings();

        // Set properties of the first NoiseLayer
        newNoiseLayer1.noiseSettings.simpleNoiseSettings.strength = UnityEngine.Random.Range(0f, 0.2f);
        newNoiseLayer1.noiseSettings.simpleNoiseSettings.numLayers = UnityEngine.Random.Range(1, 8);
        newNoiseLayer1.noiseSettings.simpleNoiseSettings.baseRoughness = UnityEngine.Random.Range(0f, 5f);
        newNoiseLayer1.noiseSettings.simpleNoiseSettings.roughness = UnityEngine.Random.Range(0f, 10f);
        newNoiseLayer1.noiseSettings.simpleNoiseSettings.persistence = UnityEngine.Random.Range(0f, 0.75f);
        // min value should be biased more towards 1 than 0.5 as this yields better results
        float randomValue = UnityEngine.Random.Range(0f, 1f);
        randomValue = Mathf.Pow(randomValue, 2);
        newNoiseLayer1.noiseSettings.simpleNoiseSettings.minValue = 0.5f + randomValue * 0.5f; // normalize to [0.5, 1]

        // Create and initialize the second NoiseLayer and its properties
        ShapeSettings.NoiseLayer newNoiseLayer2 = new ShapeSettings.NoiseLayer();
        newNoiseLayer2.noiseSettings = new NoiseSettings();
        newNoiseLayer2.noiseSettings.rigidNoiseSettings = new NoiseSettings.RigidNoiseSettings();
        newNoiseLayer2.noiseSettings.filterType = NoiseSettings.FilterType.Rigid;

        // Set properties of the second NoiseLayer
        newNoiseLayer2.useFirstLayerMask = true;
        newNoiseLayer2.noiseSettings.rigidNoiseSettings.strength = UnityEngine.Random.Range(0f, 5f);
        newNoiseLayer2.noiseSettings.rigidNoiseSettings.numLayers = UnityEngine.Random.Range(1, 8);
        newNoiseLayer2.noiseSettings.rigidNoiseSettings.baseRoughness = UnityEngine.Random.Range(0f, 5f);
        newNoiseLayer2.noiseSettings.rigidNoiseSettings.roughness = UnityEngine.Random.Range(0f, 10f);
        newNoiseLayer2.noiseSettings.rigidNoiseSettings.persistence = UnityEngine.Random.Range(0f, 0.75f);
        newNoiseLayer2.noiseSettings.rigidNoiseSettings.weightMultiplier = UnityEngine.Random.Range(0f, 1f);

        // Add the new NoiseLayers to the noiseLayers array
        newShapeSettings.noiseLayers[0] = newNoiseLayer1;
        newShapeSettings.noiseLayers[1] = newNoiseLayer2;

        // Make Unity aware of the changes to the ScriptableObject
        EditorUtility.SetDirty(newShapeSettings);
        AssetDatabase.SaveAssets();

        return newShapeSettings;
    }



    public static ColourSettings CreateRandomColourSettings()
    {
        int counter = 1;
        string assetPath = $"Assets/Settings/NewColourSettings{counter}.asset";
        while (File.Exists(assetPath))
        {
            counter++;
            assetPath = $"Assets/Settings/NewColourSettings{counter}.asset";
        }

        ColourSettings newColourSettings = ScriptableObject.CreateInstance<ColourSettings>();
        AssetDatabase.CreateAsset(newColourSettings, assetPath);

        // Load material from assets
        newColourSettings.planetMaterial = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/PlanetMaterial.mat");

        newColourSettings.biomeColourSettings = new ColourSettings.BiomeColourSettings();
        newColourSettings.biomeColourSettings.noise = new NoiseSettings();
        newColourSettings.biomeColourSettings.noiseOffset = 0;
        newColourSettings.biomeColourSettings.noiseStrength = UnityEngine.Random.Range(-0.3f, 0.3f);
        newColourSettings.biomeColourSettings.blendAmount = UnityEngine.Random.Range(0f, 1f);

        // Create 3 biomes
        newColourSettings.biomeColourSettings.biomes = new ColourSettings.BiomeColourSettings.Biome[3];
        for (int i = 0; i < 3; i++)
        {
            ColourSettings.BiomeColourSettings.Biome newBiome = new ColourSettings.BiomeColourSettings.Biome();

            Color baseColor = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));
            Color endColour = new Color(UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f), UnityEngine.Random.Range(0f, 1f));

            // Create a gradient for the biome colour with slight colour variations
            Gradient biomeGradient = new Gradient();
            biomeGradient.colorKeys = new GradientColorKey[]
            {
            new GradientColorKey(Color.Lerp(baseColor, Color.black, 0.05f), 0f), // 0%
            new GradientColorKey(Color.Lerp(baseColor, endColour, 0.05f), 0.05f), // 5%
            new GradientColorKey(Color.Lerp(baseColor, endColour, 0.20f), 0.20f), // 20%
            new GradientColorKey(Color.Lerp(baseColor, endColour, 0.35f), 0.35f), // 35%
            new GradientColorKey(Color.Lerp(baseColor, endColour, 0.45f), 0.45f), // 45%
            new GradientColorKey(Color.Lerp(baseColor, endColour, 0.90f), 0.90f)  // 90%
            };

            newBiome.gradient = biomeGradient;

            // Set start height for each biome
            if (i == 1)
            {
                newBiome.startHeight = UnityEngine.Random.Range(0.2f, 0.25f);
            }
            else if (i == 2)
            {
                newBiome.startHeight = UnityEngine.Random.Range(0.75f, 0.8f);
            }
            newColourSettings.biomeColourSettings.biomes[i] = newBiome;
        }

        // Create a gradient for the ocean color
        Gradient oceanGradient = new Gradient();
        oceanGradient.colorKeys = new GradientColorKey[]
        {
        new GradientColorKey(new Color(0f, 0f, UnityEngine.Random.Range(0.5f, 1f)), 0f),
        new GradientColorKey(new Color(0f, 0f, UnityEngine.Random.Range(0.5f, 1f)), 1f)
        };

        newColourSettings.oceanColour = oceanGradient;

        EditorUtility.SetDirty(newColourSettings);
        AssetDatabase.SaveAssets();

        return newColourSettings;
    }



}