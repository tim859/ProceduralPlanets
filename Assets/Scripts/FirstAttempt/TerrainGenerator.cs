using UnityEngine;

public class TerrainGenerator : MonoBehaviour
{
    public float scale = 1f; // Controls the scale of the noise
    public float height = 1f; // Controls the height of the terrain
    public float randomOffset = 0.1f; // Controls the amount of randomness added to the noise values

    void Start()
    {
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        Vector3[] vertices = mesh.vertices;

        for (int i = 0; i < vertices.Length; i++)
        {
            float x = vertices[i].x * scale;
            float y = vertices[i].y * scale;
            float z = vertices[i].z * scale;

            float noiseValue = Mathf.PerlinNoise(x, z) * height; // Replace with Simplex noise or other noise generator

            noiseValue += Random.Range(-randomOffset, randomOffset); // Add randomness to the noise value

            vertices[i].y += noiseValue;
        }

        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }
}