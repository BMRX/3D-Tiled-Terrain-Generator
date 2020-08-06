using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {

    public Grid mapGrid;

	public List<Cell> cells;

	public int width;
	public int height;

	private GameObject parent;

	private int[,] waterBodyLocations;

	void Start() {
		Generate();
		Debug.Log("Cells: "+cells.Count);
	}

	public void Generate() {
		DateTime before = DateTime.Now;
		cells=new List<Cell>();
		Destroy(parent);

		mapGrid=new Grid(width, height);
		parent=new GameObject("MAP");

		for (int x = 0; x<mapGrid.getX(); x++) {
			for (int y = 0; y<mapGrid.getY(); y++) {
				float rnd = UnityEngine.Random.Range(0.0f, 1.0f);
				float noiseNumber = Mathf.PerlinNoise(x * 0.075f,y * 0.075f) * rnd;
				if (noiseNumber>0.18f) {
					mapGrid.setCell(x, y, 1);
				}
				Cell newCell = new Cell();
				newCell.x=x;
				newCell.y=y;
				newCell.type=mapGrid.getCell(x, y);
				cells.Add(newCell);
			}
		}

		EmptyCircleAt(0, 0, rndFloat());
		EmptyCircleAt(mapGrid.getX(), mapGrid.getY(), rndFloat());
		EmptyCircleAt(0, mapGrid.getY()/2, rndFloat());
		EmptyCircleAt(0, mapGrid.getY(), rndFloat());
		EmptyCircleAt(mapGrid.getX()/2, 0, rndFloat());
		EmptyCircleAt(mapGrid.getX(), 0, rndFloat());
		EmptyCircleAt(mapGrid.getX(), mapGrid.getY()/2, rndFloat());
		EmptyCircleAt(mapGrid.getX()/2, mapGrid.getY(), rndFloat());

		runIt(4);
		buildWalls();
		for (int x = 0; x < mapGrid.getX(); x++) {
			for (int y = 0; y < mapGrid.getY(); y++) {
				calculateRampScore(x, y);
			}
		}
		renderGrid();
		DateTime after = DateTime.Now;
		TimeSpan duration = after.Subtract(before);
		Debug.Log("Generation time: "+duration.Milliseconds);
	}

	private void renderGrid() {
			for (int i = 0; i<mapGrid.getX(); i++) {
				for (int j = 0; j<mapGrid.getY(); j++) {
					if (mapGrid.getCell(j, i)==0) {
						//GameObject water = (GameObject) GameObject.Instantiate(Resources.Load("test/water"));
						//water.transform.position=new Vector3(i*4, -0.4f, j*4);
						//water.transform.parent=parent.transform;
					}

					if (mapGrid.getCell(j, i)==1) {
						GameObject aFloor =(GameObject) GameObject.Instantiate(Resources.Load("test/ground"));
						aFloor.transform.position=new Vector3(i*4, 0, j*4);
						aFloor.transform.parent=parent.transform;
					}

					if (mapGrid.getCell(j, i)==2) {
						GameObject aWall =(GameObject) GameObject.Instantiate(Resources.Load("test/wall"));
						aWall.transform.position=new Vector3(i*4, aWall.GetComponent<Renderer>().bounds.size.y/2, j*4);
						aWall.transform.parent=parent.transform;
					}

					if (mapGrid.getCell(j, i)==3) {
						GameObject aRamp =(GameObject) GameObject.Instantiate(Resources.Load("test/ramp"));
						aRamp.transform.position=new Vector3(i*4, aRamp.GetComponent<Renderer>().bounds.size.y/2, j*4);
						aRamp.transform.parent=parent.transform;
					}
				}
			}
		
	}


	private void runIt(int a) {
		int b = 0;
		// Just to throw you for a loop...
		while(b <= a) {
			for (int x = 0; x<mapGrid.getX(); x++) {
				for (int y = 0; y<mapGrid.getY(); y++) {
					calculateCancerScore(x, y);
				}
			}
			b++;
		}
		
	}

	private void buildWalls() {
		for (int i = 0; i<mapGrid.getX(); i++) {
			for (int j = 0; j<mapGrid.getY(); j++) {
				float rnd = UnityEngine.Random.Range(0.0f, 1.0f);
				float noiseNumber = Mathf.PerlinNoise(i * 0.075f,j * 0.075f) * rnd;
				if(mapGrid.getCell(i,j) == 1) {
					if (noiseNumber<0.05f) {
						mapGrid.setCell(i, j, 2);
					}
				}
				
			}
		}
		for (int a = 0; a<6; a++) {
			for (int x = 0; x<mapGrid.getX(); x++) {
				for (int y = 0; y<mapGrid.getY(); y++) {
					calculateWallScore(x, y);
				}
			}
		}

		//GroundCircleAt(mapGrid.getX()/4, mapGrid.getY()/4, rndFloat());
	}

	private float rndFloat() {
		float value = UnityEngine.Random.Range(0.0f, 0.18f);
		return value;
	}

	private void EmptyCircleAt(int _x, int _y, float rad) {
		Vector3 target = new Vector3(_x,0,_y);
		for (int x = 0; x<mapGrid.getX(); x++) {
			for (int y = 0; y<mapGrid.getY(); y++) {
				if (getDistanceScore(x, y, target)<=rad) {
					mapGrid.setCell(x, y, 0);
				}

			}
		}
	}

	// Like EmptyCircle at, but instead it's on the ground level! Wow
	private void GroundCircleAt(int _x, int _y, float rad) {
		Vector3 target = new Vector3(_x,0,_y);
		for (int x = 0; x<mapGrid.getX(); x++) {
			for (int y = 0; y<mapGrid.getY(); y++) {
				if(mapGrid.getCell(x,y) == 2) {
					if (getDistanceScore(x, y, target)<=rad) {
						mapGrid.setCell(x, y, 1);
					}
				}
			}
		}
	}

	// I dunno if this works, it returns a value and apparently I use it twice somewhere in here
	private float getDistanceScore(int _x, int _y, Vector3 target) {
		Vector3 thisTile = new Vector3(_x,0,_y);
		float distance = Vector3.Distance(thisTile,target);
		float score = distance / (mapGrid.getX() + mapGrid.getY());
		return score;
	}

	//step 1: create a two-dimensional array to represent the cells you've been to already, and the cells you're still considering; set everything to unconsidered except for the selected cell
	//step 2: enter a for loop with the exit condition of "no pending cells remaining"
	//Check each cell for pending status.
	//If the cell is traversable, set its unconsidered neighbors to pending and itself to considered.
	//Mark it as either connected or disconnected, depending on whether it's traversable.
	public void FindLakes() {
		for(int x = 0; x < mapGrid.getX(); x++) {
			for (int y = 0; y < mapGrid.getY(); y++) {
				if(mapGrid.getCell(x,y) == 0) {
					int[,] tilesToFill = new int[0,0];
					if (mapGrid.getCell(x+1, y) == 0) {
						 
					}
				}
			}
		}
	}


	private void calculateCancerScore(int _x, int _y) {
		int score = 0;

		if (mapGrid.getCell(_x-1, _y)==1) {
			score++;
		}

		if (mapGrid.getCell(_x+1, _y)==1) {
			score++;
		}

		if (mapGrid.getCell(_x, _y+1)==1) {
			score++;
		}

		if (mapGrid.getCell(_x, _y-1)==1) {
			score++;
		}

		if (mapGrid.getCell(_x-1, _y+1)==1) {
			score++;
		}

		if (mapGrid.getCell(_x+1, _y+1)==1) {
			score++;
		}

		if (mapGrid.getCell(_x-1, _y-1)==1) {
			score++;
		}

		if (mapGrid.getCell(_x+1, _y-1)==1) {
			score++;
		}
		

		if (mapGrid.getCell(_x, _y)==1) {
			if (score>=4) {
				mapGrid.setCell(_x, _y, 1);
			} else {
				mapGrid.setCell(_x, _y, 0);
			}
		} else {
			if (score>=5) {
				mapGrid.setCell(_x, _y, 1);
			} else {
				mapGrid.setCell(_x, _y, 0);
			}
		}

	}

	private void calculateWallScore(int _x, int _y) {
		int score = 0;

		if (mapGrid.getCell(_x-1, _y)==2) {
			score++;
		}

		if (mapGrid.getCell(_x+1, _y)==2) {
			score++;
		}

		if (mapGrid.getCell(_x, _y+1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x, _y-1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x-1, _y+1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x+1, _y+1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x-1, _y-1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x+1, _y-1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x, _y)==2) {
			if (score>=2) {
				mapGrid.setCell(_x, _y, 2);
			} else {
				mapGrid.setCell(_x, _y, 1);
			}
		} else if (mapGrid.getCell(_x, _y)==1) {
			if (score>=3) {
				mapGrid.setCell(_x, _y, 2);
			} else {
				mapGrid.setCell(_x, _y, 1);
			}
		} 
	}

	private void calculateRampScore(int _x, int _y) {
		int score = 0;
		int num = UnityEngine.Random.Range(0,100);
		if (mapGrid.getCell(_x-1, _y)==2) {
			score++;
		}

		if (mapGrid.getCell(_x+1, _y)==2) {
			score++;
		}

		if (mapGrid.getCell(_x, _y+1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x, _y-1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x-1, _y+1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x+1, _y+1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x-1, _y-1)==2) {
			score++;
		}

		if (mapGrid.getCell(_x+1, _y-1)==2) {
			score++;
		}
		//
		if (mapGrid.getCell(_x-1, _y)==3) {
			score+=4;
		}

		if (mapGrid.getCell(_x+1, _y)==3) {
			score+=4;
		}

		if (mapGrid.getCell(_x, _y+1)==3) {
			score+=4;
		}

		if (mapGrid.getCell(_x, _y-1)==3) {
			score+=4;
		}

		if (mapGrid.getCell(_x-1, _y+1)==3) {
			score+=4;
		}

		if (mapGrid.getCell(_x+1, _y+1)==3) {
			score+=4;
		}

		if (mapGrid.getCell(_x-1, _y-1)==3) {
			score+=4;
		}

		if (mapGrid.getCell(_x+1, _y-1)==3) {
			score+=4;
		}

		if (mapGrid.getCell(_x, _y)==2) {
			if (score>=1&&score<=4&&num<=80) {
				mapGrid.setCell(_x, _y, 3);
			}
		}
	}
}

