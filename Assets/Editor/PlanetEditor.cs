// Heavily inspired by tutorial series from Sebastian Lague found at https://www.youtube.com/playlist?list=PLFt_AvWsXl0cONs3T0By4puYy6GM22ko8

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Planet))]
public class PlanetEditor : Editor
{

    Planet planet;
    Editor shapeEditor;
    Editor colourEditor;

    public override void OnInspectorGUI()
    {
        using (var check = new EditorGUI.ChangeCheckScope())
        {
            base.OnInspectorGUI();
            if (check.changed)
            {
                planet.GeneratePlanet();
            }
        }

        if (GUILayout.Button("Generate Planet"))
        {
            planet.GeneratePlanet();
        }

        if (GUILayout.Button("Generate Random Planet Shape"))
        {
            planet.GenerateRandomPlanetShape();
        }

        if (GUILayout.Button("Generate Random Planet Colours"))
        {
            planet.GenerateRandomPlanetColours();
        }

        DrawSettingsEditor(planet.shapeSettings, planet.OnShapeSettingsUpdated, ref planet.shapeSettingsFoldout, ref shapeEditor);
        DrawSettingsEditor(planet.colourSettings, planet.OnColourSettingsUpdated, ref planet.colourSettingsFoldout, ref colourEditor);
    }

    void DrawSettingsEditor(Object settings, System.Action onSettingsUpdated, ref bool foldout, ref Editor editor)
    {
        if (settings != null)
        {
            foldout = EditorGUILayout.InspectorTitlebar(foldout, settings);
            using var check = new EditorGUI.ChangeCheckScope();
            if (foldout)
            {
                CreateCachedEditor(settings, null, ref editor);
                editor.OnInspectorGUI();

                if (check.changed)
                {
                    onSettingsUpdated?.Invoke();
                }
            }
        }
    }

    private void OnEnable()
    {
        planet = (Planet)target;
    }
}