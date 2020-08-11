using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerationController : MonoBehaviour {
	// Here will be public methods that can be used to generate maps according to different rulesets

	private GameGrid mapGrid;
	private MeshGrid meshGrid;
	private Material mapMaterial;
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

	public bool drawVertices = true;

	public List<List<Cell>> waterGroups;

    // Generates an "outdoor" terrain one large island sporting, foliage, bodies of water, etc.
    public void GenerateTerrain() {
		DateTime before = DateTime.Now;
		DestroyImmediate(parent);
		//Destroy(parent);
		mapGrid=new GameGrid(width, height);
		parent=new GameObject("MAP");
		parent.AddComponent<MeshFilter>();
		parent.AddComponent<MeshRenderer>();
		parent.AddComponent<MeshGrid>();

		//mapMaterial=parent.GetComponent<MeshRenderer>().material;
		parent.GetComponent<MeshRenderer>().material=Resources.Load("test/MapMaterial", typeof(Material)) as Material;

		meshGrid=parent.GetComponent<MeshGrid>();

		if (seed == 0) {	seed=UnityEngine.Random.Range(0, int.MaxValue);	}

		// Apply perlin noise 
		GameGrid.GenerateNoiseMap(mapGrid.grid, seed, perlinModifier, octaves, persistance, lacrinarity);

		float[,] falloff = FalloffGenerator.GenerateFalloffMap(width, height);
		float[,] heightMap = new float[width,height];
		for(int x = 0; x < mapGrid.getX(); x++) {
			for(int y = 0; y < mapGrid.getY(); y++) {
				mapGrid.grid[x, y].HeightMap=(mapGrid.grid[x, y].HeightMap*10)-falloff[x, y]*10;
				heightMap[x, y]=mapGrid.grid[x, y].HeightMap;
				// Set first layer of terrain
				if (mapGrid.grid[x,y].HeightMap>= 1.5) {
					mapGrid.setCellTileHeight(x, y, 0);
					mapGrid.setCellType(x, y, 1);
                } else {
					mapGrid.setCellType(x, y, 0);
				}
                if (mapGrid.grid[x, y].HeightMap>=5.0) {
                    mapGrid.setCellTileHeight(x, y, 1);
                }

			}
        }
		//waterGroups=mapGrid.FindConnectedGroups(mapGrid, 0);
		//mapGrid = Grid.RemoveWater(waterGroups, mapGrid,Mathf.RoundToInt(waterGroups.Count / 1.5f));

		// Use Moore Automata to smooth the map
		if (smoothCount>0) {
			GameGrid.SmoothMooreCellularAutomata(mapGrid.grid, smoothCount);
			GameGrid.SmoothHeightCellularAutomata(mapGrid.grid, smoothCount);
		}

		// Removes tiles at the edge, map gen leaves random cells behind.
		mapGrid.EdgeEraser();

		// If cell.Type >= 1 set walkable to true
		//Grid.CheckWalkable(mapGrid.grid);

		// Render the gird
		//RenderGrid();
		/*
		 * 
		 *			THIS IS THE WORK
		 *			WORK ON THIS!!!
		 * 
		 * 
		 */
		meshGrid.Init(mapGrid.grid);

		DateTime after = DateTime.Now;
		TimeSpan duration = after.Subtract(before);
		Debug.Log("Terrain Gen took: "+duration.Milliseconds+" ms");
	}

	void OnDrawGizmos() {
		if(drawVertices== true) {
			for (int i = 0; i<meshGrid.vertices.Length; i++) {
				Gizmos.color=Color.black;
				Gizmos.DrawSphere(meshGrid.vertices[i], 0.05f);
				Gizmos.color=Color.white;
				Handles.Label(new Vector3(meshGrid.vertices[i].x, meshGrid.vertices[i].y, meshGrid.vertices[i].z), i.ToString());
			}
		}
	}

	private void RenderGrid() {
		for (int i = 0; i<mapGrid.getX(); i++) {
			for (int j = 0; j<mapGrid.getY(); j++) {

                if (mapGrid.getCellType(i, j)==0) {
                    /*GameObject water = (GameObject) GameObject.Instantiate(Resources.Load("test/water"));
                    water.transform.position=new Vector3(i, -0.4f, j);
                    water.transform.parent=parent.transform;*/
                }

                if (mapGrid.getCellType(i, j)==1) {
                    GameObject aFloor =(GameObject) GameObject.Instantiate(Resources.Load("test/cube"));
                    aFloor.transform.position=new Vector3(i, mapGrid.getCellTileHeight(i, j), j);
                    aFloor.transform.parent=parent.transform;
				} 

				if (mapGrid.getCellType(i, j)==3) {
                    GameObject aWall =(GameObject) GameObject.Instantiate(Resources.Load("test/ramp"));
                    aWall.transform.position=new Vector3(i*4, aWall.GetComponent<Renderer>().bounds.size.y/2, j*4);
                    aWall.transform.parent=parent.transform;
                }

                if (mapGrid.getCellType(i, j)==4) {
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
