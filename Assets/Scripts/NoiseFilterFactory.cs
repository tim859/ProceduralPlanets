// Heavily inspired by tutorial series from Sebastian Lague found at https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class NoiseFilterFactory
{
    public static INoiseFilter CreateNoiseFilter(NoiseSettings settings)
    {
        switch(settings.filterType)
        {
            case NoiseSettings.FilterType.Simple:
                return new SimpleNoiseFilter(settings.simpleNoiseSettings);

            case NoiseSettings.FilterType.Rigid:
                return new RigidNoiseFilter(settings.rigidNoiseSettings);
        }
        return null;
    }
}
