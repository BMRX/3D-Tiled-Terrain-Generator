using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapDisplay : MonoBehaviour {

    public Renderer textureRenderer;
    public MeshFilter meshFilter;
    public MeshRenderer meshRenderer;

    public void DrawTexture(Texture2D texture) {
        textureRenderer.sharedMaterial.mainTexture=texture;
        textureRenderer.transform.localScale=new Vector3(texture.width, 1, texture.height);
    }

    public void DrawMesh(MeshData meshData, Texture2D texture) {
        meshFilter.sharedMesh=meshData.CreateMesh();
        meshRenderer.sharedMaterial.mainTexture=texture;
    }

    public void DrawNoiseMap(Cell[,] mapGrid) {
        int width = mapGrid.GetUpperBound(0);
        int height = mapGrid.GetUpperBound(1);

        Texture2D texture = new Texture2D(width,height);

        Color[] colourMap = new Color[width*height];
        for(int x=0; x<width; x++) {
            for(int y = 0; y < height; y++) {
                colourMap[x*height+y] = Color.Lerp(Color.black, Color.white,mapGrid[x,y].HeightMap);
            }
        }

        texture.SetPixels(colourMap);
        texture.Apply();

        textureRenderer.sharedMaterial.mainTexture=texture;
        textureRenderer.transform.localScale=new Vector3(width, 1, height);
    }
}
