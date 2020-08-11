using UnityEngine;
using System.Collections;

public static class FalloffGenerator {
	// Make this piece of shit generate random "blobs" (multiple circle gradients of various size close to each other) this should allow for more natrual island shapes
	public static float[,] GenerateFalloffMap(int width, int height) {
		float[,] map = new float[width,height];

		for (int i = 0; i<width; i++) {
			for (int j = 0; j<height; j++) {
				float x = i / (float)width * 2 - 1;
				float y = j / (float)height * 2 - 1;

				float distance = Vector2.Distance(new Vector2(0,0),new Vector2(x,y))/(width + height);
				float value = Mathf.Max (Mathf.Abs (x), Mathf.Abs (y));
				//map[i, j]=Evaluate(distance);
				map[i, j]=Evaluate(value);
			}
		}

		return map;
	}

	static float Evaluate(float value) {
		float a = 3;
		float b = 2.2f;

		return Mathf.Pow(value, a)/(Mathf.Pow(value, a)+Mathf.Pow(b-b*value, a));
	}
}