using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshGrid : MonoBehaviour { 

    public Vector3[] vertices;
    private int[] triangles;

    private Mesh mesh;

    private int width;
    private int height;

    // Generate mesh based on GameGrid cell tileHeight

    public void GenerateTerrainMesh(GameGrid mapGrid) {
        width=mapGrid.getX();
        height=mapGrid.getY();
        GetComponent<MeshFilter>().mesh=mesh=new Mesh();
        mesh.name="Procedural Grid";
        mesh.Clear();

        //vertices=new Vector3[(mapGrid.getX()+1)*(mapGrid.getY()+1)];
        

        List<Vector3> verts = new List<Vector3>();
        for (int i = 0, y = 0; y<=mapGrid.getY(); y++) {
            for (int x = 0; x<=mapGrid.getX(); x++, i++) {
                if(mapGrid.getCellType(x,y)==1) {
                    verts.Add(new Vector3(x, 0, y));
                    //vertices[i]=new Vector3(x, 0, y);
                }
                
            }
        }
        foreach(Vector3 vert in verts) {
            // Fuck this is confusing!
        }
        vertices=verts.ToArray();
        triangles=new int[(width-vertices.Length/2)*(height-vertices.Length/2)*6];
        /*List<Vector3> verts = new List<Vector3>();
        for (int y = 0; y<=mapGrid.getY(); y++) {
            for (int x = 0; x<=mapGrid.getX(); x++) {
                if (mapGrid.getCellType(x,y) == 1) {
                    // Currently this adds vertices in the wrong order
                    if (mapGrid.getCellTileHeight(x, y)==0&&GameGrid.GetMooreSurroundingTiles(mapGrid.grid, x, y)<=6) {
                        // Determine where new vertices go
                        verts.Add(new Vector3(x, mapGrid.grid[x, y].TileHeight, y));
                        verts.Add(new Vector3(x, mapGrid.grid[x, y].TileHeight-1, y));
                    } else if (mapGrid.getCellTileHeight(x, y)==1&&GameGrid.GetSurroundingHeightTiles(mapGrid.grid, x, y)<=6) {
                        verts.Add(new Vector3(x, mapGrid.grid[x, y].TileHeight, y));
                        verts.Add(new Vector3(x, mapGrid.grid[x, y].TileHeight-1, y));
                    } else {
                        verts.Add(new Vector3(x, mapGrid.grid[x, y].TileHeight, y));
                    }
                    *//*if (mapGrid.getCellTileHeight(x, y)==0&&GameGrid.GetMooreSurroundingTiles(mapGrid.grid, x, y)<=6) {
                        // Determine where new vertices go
                        verts.Add(new Vector3(x, mapGrid.grid[x, y].TileHeight, y));
                        verts.Add(new Vector3(x, mapGrid.grid[x, y].TileHeight-1, y));
                    } else if (mapGrid.getCellTileHeight(x, y)==1&&GameGrid.GetSurroundingHeightTiles(mapGrid.grid, x, y)<=6) {
                        verts.Add(new Vector3(x, mapGrid.grid[x, y].TileHeight, y));
                        verts.Add(new Vector3(x, mapGrid.grid[x, y].TileHeight-1, y));
                    } else {
                        verts.Add(new Vector3(x, mapGrid.grid[x, y].TileHeight, y));
                    }*//*
                } 
            }
        }
        vertices=verts.ToArray();*/
        mesh.vertices=vertices;

        
        for (int ti = 0, vi = 0, y = 0; y<triangles.Length/2; y++, vi++) {
            for (int x = 0; x<triangles.Length/2; x++) {
                if (mapGrid.getCellType(y, x)==1) {
                    vi++;
                    ti+=6;
                    triangles[ti]=vi;
                    triangles[ti+3]=triangles[ti+2]=vi+1;
                    triangles[ti+4]=triangles[ti+1]=vi+triangles.Length/2+1;
                    triangles[ti+5]=vi+triangles.Length/2+2;
                }
            }
        }
        Debug.Log("X: "+(int) vertices[0].x+" Y: "+(int) vertices[0].z+" H: "+ vertices[0].y);
        Debug.Log("X: "+(int) vertices[1].x+" Y: "+(int) vertices[1].z+" H: "+ vertices[0].y);
        Debug.Log("X: "+(int) vertices[2].x+" Y: "+(int) vertices[2].z+" H: "+ vertices[0].y);
        //mesh.triangles=triangles;
        mesh.RecalculateNormals();
    }

}
