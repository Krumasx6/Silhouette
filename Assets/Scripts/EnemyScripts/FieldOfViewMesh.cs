using System.Collections.Generic;
using UnityEditor.MPE;
using UnityEngine;
using UnityEngine.AI;

public class FieldOfViewMesh : MonoBehaviour
{   
    private EnemyAttributes ea;
    FieldOfView fov;
    Mesh mesh;
    RaycastHit2D hit;   
    [SerializeField] float meshRes = 2;
    [HideInInspector] public Vector3[] vertices;
    [HideInInspector] public int[] triangles;
    [HideInInspector] public int stepCount;
    
    // Material and color variables
    private MeshRenderer meshRenderer;
    private Color normalColor = new Color(255f/255f, 255f/255f, 255f/255f, 80f/255f); // White
    private Color cautiousColor = new Color(255f/255f, 255f/255f, 0f/255f, 80f/255f); // Yellow
    private Color detectedColor = new Color(255f/255f, 0f/255f, 0f/255f, 80f/255f); // Red

    void Start()
    {
        mesh = GetComponent<MeshFilter>().mesh;
        fov = GetComponentInParent<FieldOfView>();
        meshRenderer = GetComponent<MeshRenderer>();
        ea = GetComponentInParent<EnemyAttributes>();
        
        // Set initial color
        if (meshRenderer != null && meshRenderer.material != null)
        {
            meshRenderer.material.color = normalColor;
        }
    }

    void Update()
    {
        MakeMesh();
        UpdateMeshColor();
    }
    
    void UpdateMeshColor()
    {
        if (meshRenderer != null && meshRenderer.material != null)
        {
            // Change color based on player detection
            if (ea.sawPlayer)
            {
                meshRenderer.material.color = detectedColor;
            }
            else if (ea.isCautious)
            {
                meshRenderer.material.color = cautiousColor;
            }
            else
            {
                meshRenderer.material.color = normalColor;
            }
        }
    }

    void MakeMesh()
    {
        stepCount = Mathf.RoundToInt(fov.viewAngle * meshRes);
        float stepAngle = fov.viewAngle / stepCount;

        List<Vector3> viewVertex = new List<Vector3>();

        for (int i = 0; i <= stepCount; i++)
        {
            float angle = fov.transform.eulerAngles.z - fov.viewAngle / 2 + stepAngle * i;
            Vector3 dir = fov.DirFromAngle(angle, true);
            hit = Physics2D.Raycast(transform.position, dir, fov.viewRadius, fov.obstacleMask);

            if (hit.collider == null)
            {
                viewVertex.Add(transform.position + dir.normalized * fov.viewRadius);
            }
            else
            {
                viewVertex.Add(transform.position + dir.normalized * hit.distance);
            }
        }

        int vertexCount = viewVertex.Count + 1;

        vertices = new Vector3[vertexCount];
        triangles = new int[(vertexCount - 2) * 3];

        vertices[0] = Vector3.zero;

        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewVertex[i]);

            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }

        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
    }
}