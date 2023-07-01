// Heavily inspired by tutorial series from Sebastian Lague found at https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface INoiseFilter
{
    float Evaluate(Vector3 point);
}
