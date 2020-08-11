using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class meshtest : MonoBehaviour
{
    public int xSize, ySize;
    public float cellSize = 1;

    private void Awake() {
        Generate();
    }

    private Vector3[] vertices;
    private int[] triangles;
    private Mesh mesh;
    private void Generate() {
        mesh=GetComponent<MeshFilter>().mesh;
        mesh.name="MeshTest";

        //vertices=new Vector3[(xSize+1)*(ySize+1)*3];
        vertices=new Vector3[9] {
            new Vector3(0, 0, 0),
            new Vector3(0f, 0, 0.5f),
            new Vector3(0f, 0, 1f),
            new Vector3(0.5f, 0, 0f),
            new Vector3(0.5f, 0, 0.5f),
            new Vector3(0.5f, 0, 1f),
            new Vector3(1f, 0, 0f),
            new Vector3(1f, 0, 0.5f),
            new Vector3(1f, 0, 1f)
        };

        /* for (int y = 0; y<=ySize; y++) {
             for (int x = 0; x<=xSize; x++) {
                 vertices[v]=new Vector3(-x*0.5f, 0, -y*0.5f);
                 vertices[v+1]=new Vector3(-x*0.5f, 0, y*0.5f);
                 vertices[v+2]=new Vector3(x, 0, y);
                 vertices[v+3]=new Vector3(x, 0, y);
                 vertices[v+4]=new Vector3(x, 0, y);
                 vertices[v+5]=new Vector3(x, 0, y);
                 vertices[v+6]=new Vector3(x, 0, y);
                 vertices[v+7]=new Vector3(x, 0, y);
                 vertices[v+8]=new Vector3(x, 0, y);
             }
             v+=9;
         }*/
        triangles=new int[9];
        triangles[0]=0;
        triangles[1]=(int)cellSize+1;
        triangles[2]=1;
        triangles[3]=0;
        triangles[4]=(int)cellSize+1;
        triangles[5]=1;
        triangles[6]=0;
        triangles[7]=(int)cellSize+1;
        triangles[8]=1;


        mesh.vertices=vertices;
        mesh.triangles=triangles;
        
    }

    private void OnDrawGizmos() {
        if (vertices==null) {
            return;
        }
        for (int i = 0; i<vertices.Length; i++) {
            Gizmos.color=Color.black;
            Gizmos.DrawSphere(vertices[i], 0.1f);
            Gizmos.color=Color.white;
            Handles.Label(new Vector3(vertices[i].x, vertices[i].y+0.25f, vertices[i].z), i.ToString());
        }
    }
}
