using UnityEngine;
using System.Collections;

public static class TextureGenerator {

	public static Texture2D TextureFromColourMap(Color[] colourMap, int width, int height) {
		Texture2D texture = new Texture2D (width, height);
		texture.filterMode=FilterMode.Point;
		texture.wrapMode=TextureWrapMode.Clamp;
		texture.SetPixels(colourMap);
		texture.Apply();
		return texture;
	}


	public static Texture2D TextureFromHeightMap(Cell[,] mapGrid) {
		int width = mapGrid.GetUpperBound(0);
		int height = mapGrid.GetUpperBound(1);

		Color[] colourMap = new Color[width * height];
		for (int y = 0; y<height; y++) {
			for (int x = 0; x<width; x++) {
				colourMap[y*width+x]=Color.Lerp(Color.black, Color.white, mapGrid[x, y].HeightMap);
			}
		}

		return TextureFromColourMap(colourMap, width, height);
	}

}