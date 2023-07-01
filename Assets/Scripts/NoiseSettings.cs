// Heavily inspired by tutorial series from Sebastian Lague found at https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NoiseSettings
{
    public enum FilterType { Simple, Rigid };
    public FilterType filterType;

    [ConditionalHide("FilterType", 0)]
    public SimpleNoiseSettings simpleNoiseSettings;
    
    [ConditionalHide("FilterType", 1)]
    public RigidNoiseSettings rigidNoiseSettings;

    [System.Serializable]
    public class SimpleNoiseSettings
    {
        [Range(0f, 5f)]
        public float strength = 1;
        [Range(1, 8)]
        public int numLayers = 1;
        [Range(0f, 5f)]
        public float baseRoughness = 1;
        [Range(0f, 10f)]
        public float roughness = 1;
        [Range(0f, 0.75f)]
        public float persistence = 0.5f;
        public Vector3 centre;
        [Range(0f, 2f)]
        public float minValue;
    }

    [System.Serializable]
    public class RigidNoiseSettings : SimpleNoiseSettings
    {
        [Range(0f, 1f)]
        public float weightMultiplier = 0.8f;
    }
}
