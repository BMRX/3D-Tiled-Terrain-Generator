using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerationController : MonoBehaviour {
	// Here will be public methods that can be used to generate maps according to different rulesets

	private Grid mapGrid;
	private GameObject parent;

	//Modifiers
	public int width;
	public int height;
	public float perlinModifier;
	public int octaves;
	[Range(0,1)]
	public float persistance;
	public float lacrinarity;
	public int smoothCount;
	public int seed;

	public List<List<Cell>> waterGroups;

	// Generates an "outdoor" terrain one large island sporting, foliage, bodies of water, etc.
	public void GenerateTerrain() {

		DateTime before = DateTime.Now;
		DestroyImmediate(parent);
		//Destroy(parent);
		mapGrid=new Grid(width, height);
        parent=new GameObject("MAP");

		if(seed == 0) {	seed=UnityEngine.Random.Range(0, int.MaxValue);	}
		
		// Apply perlin noise 
		Grid.GenerateNoiseMap(mapGrid.grid, seed, perlinModifier, octaves, persistance, lacrinarity);
		waterGroups=mapGrid.FindConnectedGroups(mapGrid, 0);
		mapGrid = Grid.RemoveWater(waterGroups, mapGrid,Mathf.RoundToInt(waterGroups.Count / 1.5f));

		// Make this bitch into an island, idgaf circles at 12 points around the edge! Fuck it, I'm done - fuck this shit I suck at math.
		Vector2[] blobArray = new [] {
			new Vector2(0f,0f), new Vector2(width*0.25f, 0f),new Vector2(width*0.50f, 0f), new Vector2(width*0.75f, 0f), new Vector2(width,0f),
			new Vector2(width*0.25f,height) ,new Vector2(width*0.50f,height), new Vector2(width*0.75f,height), new Vector2(width,height*0.25f),new Vector2(width,height*0.50f), new Vector2(width,height*0.75f),
			new Vector2(width,height), new Vector2(0f,height*0.25f),new Vector2(0f,height*0.50f), new Vector2(0f,height*0.75f), new Vector2(0f, height)
		};

		for (int i = 0; i<blobArray.Length; i++) {
			float rnd = UnityEngine.Random.Range(0.025f, 0.15f);
			//mapGrid.CircleEraser((int) blobArray[i].x, (int) blobArray[i].y, 1, rnd);
		}

		// Use Moore Automata to smooth the map
		if (smoothCount > 0) Grid.SmoothMooreCellularAutomata(mapGrid.grid, smoothCount);

		// Removes tiles at the edge, map gen leaves random cells behind.
		mapGrid.EdgeEraser();

		// Render the gird
		RenderGrid();

		DateTime after = DateTime.Now;
		TimeSpan duration = after.Subtract(before);
		Debug.Log("Terrain Gen took: "+duration.Milliseconds+" ms");
		
	}

	// Multiply position by scale of model else it all fucking overlaps, might have to find new sizes for assets depending on how I deal with walls...
	private void RenderGrid() {
		for (int i = 0; i<mapGrid.getX(); i++) {
			for (int j = 0; j<mapGrid.getY(); j++) {

				if (mapGrid.getCellType(j, i)==0) {
					//GameObject water = (GameObject) GameObject.Instantiate(Resources.Load("test/water"));
					//water.transform.position=new Vector3(i*4, -0.4f, j*4);
					//water.transform.parent=parent.transform;
				}

				if (mapGrid.getCellType(j, i)==1) {
					GameObject aFloor =(GameObject) GameObject.Instantiate(Resources.Load("test/ground"));
					aFloor.transform.position=new Vector3(i*4, 0, j*4); 
					aFloor.transform.parent=parent.transform;
				}

				if (mapGrid.getCellType(j, i)==2) {
					GameObject aWall =(GameObject) GameObject.Instantiate(Resources.Load("test/ramp"));
					aWall.transform.position=new Vector3(i*4, aWall.GetComponent<Renderer>().bounds.size.y/2, j*4);
					aWall.transform.parent=parent.transform;
				}

				if (mapGrid.getCellType(j, i)==3) {
					GameObject aRamp =(GameObject) GameObject.Instantiate(Resources.Load("test/wall"));
					aRamp.transform.position=new Vector3(i*4, aRamp.GetComponent<Renderer>().bounds.size.y/2, j*4);
					aRamp.transform.parent=parent.transform;
				}

			}
		}
	}

	void OnValidate() {
		if(width < 1) { width=1; }
		if (height<1) {	height=1; }
		if (lacrinarity<1) { lacrinarity=1;	}
		if (octaves<0) { octaves=0;	}
	}

}

[CustomEditor (typeof(GenerationController))]
public class GeneratorEditor : Editor {
	public override void OnInspectorGUI() {
		GenerationController mapGen = (GenerationController)target;
		DrawDefaultInspector();
		if(GUILayout.Button("Generate Terrain")) {
			mapGen.GenerateTerrain();
		}
	}
}
