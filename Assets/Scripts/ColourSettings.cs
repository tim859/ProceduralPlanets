// Heavily inspired by tutorial series from Sebastian Lague found at https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class ColourSettings : ScriptableObject
{
    public Material planetMaterial;
    public BiomeColourSettings biomeColourSettings;
    public Gradient oceanColour;

    [System.Serializable]
    public class BiomeColourSettings
    {
        public Biome[] biomes;
        public NoiseSettings noise;
        [Range(-5f, 7.5f)]
        public float noiseOffset;
        [Range(-5f, 5f)]
        public float noiseStrength;

        [Range(0, 1)]
        public float blendAmount;

        [System.Serializable]
        public class Biome
        {
            public Gradient gradient;
            public Color tint;

            [Range(0, 1)]
            public float startHeight;

            [Range(0, 1)]
            public float tintPercent;
        }
    }
}