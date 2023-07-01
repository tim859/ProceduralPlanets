using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralFace : MonoBehaviour
{
    [SerializeField] int xSize, ySize;
    Vector3[] vertices;
    Mesh mesh;

    private void Awake()
    {
        Generate();
    }

    void Generate()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mesh.name = "Procedural Grid";

        vertices = new Vector3[(xSize + 1) * (ySize + 1)];
        // we need to find the vertices in the mesh and store them in a list
        // we get the amount of vertices in the mesh by the equation (x+1)(y+1)
        // where x is the number of quads in the x dimension and y is the number
        // of quads in the y dimension.
        // e.g. a 2x4 mesh would be (2+1)(4+1) = 15 vertices.

        Vector2[] uv = new Vector2[vertices.Length];
        Vector4[] tangents = new Vector4[vertices.Length];
        Vector4 tangent = new(1f, 0f, 0f, -1f);

        for (int i = 0, y = 0; y <= ySize; y++)
        {
            for (int x = 0; x <= xSize; x++, i++)
            {
                vertices[i] = new Vector3(x, y);
                uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
                tangents[i] = tangent;
            }
        }
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.tangents = tangents;

        int[] triangles = new int[xSize * ySize * 6];
        for (int triangleIndice = 0, vertexIndice = 0, y = 0; y < ySize; y++, vertexIndice++)
        {
            for (int x = 0; x < xSize; x++, triangleIndice += 6, vertexIndice++)
            {
                triangles[triangleIndice] = vertexIndice;
                triangles[triangleIndice + 2] = triangles[triangleIndice + 3] = vertexIndice + 1;
                triangles[triangleIndice + 1] = triangles[triangleIndice + 4] = vertexIndice + xSize + 1;
                triangles[triangleIndice + 5] = vertexIndice + xSize + 2;
            }
        }
        //triangles[0] = 0;
        //triangles[2] = triangles[3] = 1;
        //triangles[1] = triangles[4] = xSize + 1;
        //triangles[5] = xSize + 2;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }

    //private void OnDrawGizmos()
    //{
    //    if (vertices == null)
    //    {
    //        return;
    //    }

    //    Gizmos.color = Color.black;
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i], 0.1f);
    //    }
    //}
}
