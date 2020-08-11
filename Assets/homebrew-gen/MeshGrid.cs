using UnityEngine;

public class MeshGrid : MonoBehaviour {

    public float cellSize = 1;
    public Vector3 gridOffset;
    public Vector2Int gridSize;

    public Vector3[] vertices;
    private int[] triangles;

    private Mesh mesh;

    // Generate mesh based on GameGrid cell tileHeight

    public void Init(Cell[,] mapGrid) {
        gridSize.x=mapGrid.GetUpperBound(0);
        gridSize.y=mapGrid.GetUpperBound(1);
        GetComponent<MeshFilter>().mesh=mesh=new Mesh();
        mesh.name="Procedural Grid";
        mesh.Clear();

        vertices=new Vector3[(gridSize.x+1)*(gridSize.y+1)];
        triangles = new int[gridSize.x*gridSize.y*6];

        for (int i = 0, y = 0; y<=gridSize.y; y++) {
            for (int x = 0; x<=gridSize.x; x++, i++) {
                float h = mapGrid[x,y].TileHeight;
                int tileType = mapGrid[x,y].Type;
                if (h==0&&tileType==0) {
                    // Water
                    vertices[i]=new Vector3(x, h-2f, y);
                } else if (h==0&&tileType==1) {
                    // Ground level
                    vertices[i]=new Vector3(x, h, y);
                }
                if (h==1&&tileType==1) {
                    // Height 1
                    vertices[i]=new Vector3(x, h*2, y);
                }
                if (h==2&&tileType==1) {
                    // Height 2
                    vertices[i]=new Vector3(x, h*2, y);
                }
            }
            
        }
        mesh.vertices=vertices;

        for (int ti = 0, vi = 0, y = 0; y<gridSize.y; y++, vi++) {
            for (int x = 0; x<gridSize.x; x++, ti+=6, vi++) {
                triangles[ti]=vi;
                triangles[ti+3]=triangles[ti+2]=vi+1;
                triangles[ti+4]=triangles[ti+1]=vi+gridSize.x+1;
                triangles[ti+5]=vi+gridSize.x+2;
            }
        }

        Debug.Log("Triangles length: "+triangles.Length);
        
        mesh.triangles=triangles;
        mesh.RecalculateNormals();
    }

}
