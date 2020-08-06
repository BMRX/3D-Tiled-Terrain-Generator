using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell {
    public int x { get; set; }
    public int y { get; set; }
    public int id { get; set; }
    public int score { get; set; }
    public int type { get; set; }
    public bool walkable { get; set; }
    public bool visted { get; set; }
}
public class Grid {
    public int[,] grid;

    // Grid dimensions
    private int xPos; // Width (ex: 250)
    private int yPos; // Height (ex: 250)

    // I dunno where to put these, used for FloodFill() and FindGroups()
    List<Cell> visitedCells = new List<Cell>();
    List<Cell> cellGroup = new List<Cell>();

    public Grid(int x, int y) {
        xPos=x;
        yPos=y;

        grid=new int[xPos, yPos];

        for (int i = 0; i<xPos; i++) {
              for (int k = 0; k < yPos; k++) {
                grid[i, k]=0;
              }
          }
        
    }

    //set a cell in grid space
    public void setCell(int x, int y, int v) {
        try {
            grid[x, y]=v;
        } catch {
            Debug.Log("ERROR");
            Debug.Log("w: "+xPos);
            Debug.Log("h: "+yPos);
            Debug.Log("x: "+x);
        }
    }

    public int getCell(int x, int y) {
        if (x>=0&&x<=xPos-1&&y>=0&&y<=yPos-1) {
            return grid[x, y];
        } else {
            return -1;
        }
    }

    public int getX() {
        return xPos;
    }

    public int getY() {
        return yPos;
    }

    public static int[,] GenerateNoiseMap(int[,] mapGrid, int seed, float scale, int octaves, float persistance, float lacunarity) {
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for(int i = 0; i < octaves; i ++) {
            float offsetX = prng.Next(-100000,100000);
            float offsetY = prng.Next(-100000,100000);
            octaveOffsets[i]=new Vector2(offsetX, offsetY);
        }

        if (scale<=0) {
            scale=0.0001f;
        }

        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        float halfWidth = mapGrid.GetUpperBound(0) / 2f;
        float halfHeight = mapGrid.GetUpperBound(1) / 2f;

        for (int x = 0; x<mapGrid.GetUpperBound(0); x++) {
            for (int y = 0; y<mapGrid.GetUpperBound(1); y++) {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int i = 0; i<octaves; i++) {
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[i].x;
                    float sampleY = (y - halfHeight) / scale * frequency + octaveOffsets[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2-1;
                    noiseHeight+=perlinValue*amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if(noiseHeight > maxNoiseHeight) {
                    maxNoiseHeight=noiseHeight;
                } else if(noiseHeight < minNoiseHeight) {
                    minNoiseHeight=noiseHeight;
                }
                mapGrid[x, y]=Mathf.RoundToInt(noiseHeight);
            }
        }

        for (int x = 0; x<mapGrid.GetUpperBound(0); x++) {
            for (int y = 0; y<mapGrid.GetUpperBound(1); y++) {
                mapGrid[x, y]=Mathf.RoundToInt(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, mapGrid[x, y]));
            }
        }

        return mapGrid;
    }

    static int GetMooreSurroundingTiles(int[,] mapGrid, int x, int y) {
        int tileCount = 0;
        for (int neighbourX = x-1; neighbourX<=x+1; neighbourX++) {
            for (int neighbourY = y-1; neighbourY<=y+1; neighbourY++) {
                if (neighbourX>=0&&neighbourX<mapGrid.GetUpperBound(0)&&neighbourY>=0&&neighbourY<mapGrid.GetUpperBound(1)) {
                    //We don't want to count the tile we are checking the surroundings of
                    if (neighbourX!=x||neighbourY!=y) {
                        tileCount+=mapGrid[neighbourX, neighbourY];
                    }
                }
            }
        }
        return tileCount;
    }
    public static int[,] SmoothMooreCellularAutomata(int[,] mapGrid, int smoothCount) {
        for (int i = 0; i<smoothCount; i++) {
            for (int x = 0; x<mapGrid.GetUpperBound(0); x++) {
                for (int y = 0; y<mapGrid.GetUpperBound(1); y++) {
                    int surroundingTiles = GetMooreSurroundingTiles(mapGrid, x, y);

                    if (surroundingTiles>4) {
                        mapGrid[x, y]=1;
                    } else if (surroundingTiles<4) {
                        mapGrid[x, y]=0;
                    }
                }
            }
        }
        return mapGrid;
    }

    public static int[,] GenerateFalloffMap(int[,] mapGrid) {
        int[,] falloffMap = new int[mapGrid.GetUpperBound(0),mapGrid.GetUpperBound(1)];
        for (int i = 0; i<mapGrid.GetUpperBound(0); i++) {
            for (int j = 0; j<mapGrid.GetUpperBound(1); j++) {
                
            }
        }

        for (int x = 0; x < mapGrid.GetUpperBound(0); x++) {
            for(int y = 0; y < mapGrid.GetUpperBound(1); y++) {
                mapGrid[x, y]=mapGrid[x, y]-falloffMap[x, y];
            }
        }
        return mapGrid;
     }

    public float GetDistance(int _x, int _y, Vector3 target) {
        Vector3 thisTile = new Vector3(_x,0,_y);
        float distance = Vector3.Distance(thisTile,target)/(getX() + getY());
        return distance;
    }

    public void EvaluateCells(List<Cell> cells) {
        int cellId = 0;
        // Get all cells and add them to cell list
        for (int x = 0; x<getX(); x++) {
            for (int y = 0; y<getY(); y++) {
                cellId++;
                Cell newCell = new Cell();
                newCell.x=x;
                newCell.y=y;
                newCell.id=cellId;
                newCell.type=getCell(x, y);
                cells.Add(newCell);
            }
        }
    }
    //step 1: create a two-dimensional array to represent the cells you've been to already, and the cells you're still considering; set everything to unconsidered except for the selected cell
    //step 2: enter a for loop with the exit condition of "no pending cells remaining"
    //Check each cell for pending status.
    //If the cell is traversable, set its unconsidered neighbors to pending and itself to considered.
    //Mark it as either connected or disconnected, depending on whether it's traversable.

    public List<List<Cell>> FindConnectedGroups(Grid mapGrid, int type) {
        var groups = new List<List<Cell>>();

        // Search the world grid for pairs of connected blocks.
        for (int x = 0; x<mapGrid.getX(); x++) {
            for (int y = 0; y<mapGrid.getY(); y++) {
                var cell = mapGrid.getCell(x,y);

                // Skip blocks we've already grouped.
                // If you don't want to add a visited field to every block, 
                // you can accomplish this with a parallel array instead.
                if (cell.visited)
                    continue;

                // Remove this check if you want to find groups of any color.
                if (block.color==matchColor) {
                    // Every group of 2+ blocks has a block to the right or below another,
                    // so by checking just these two directions we don't exclude any.
                    if (x>0) {
                        var neighbor = world.GetBlock(x - 1, y);
                        if (neighbor.color==block.color) {
                            var group = new List<Block>();
                            PopulateGroup(world, group, block);
                            groups.Add(group);
                            continue;
                        }
                    }
                    if (y>0) {
                        var neighbor = mapGrid.GetBlock(x, y - 1);
                        if (neighbor.color==cell.color) {
                            var group = new List<Cell>();
                            PopulateGroup(mapGrid, group, cell);
                            groups.Add(group);
                        }
                    }
                }
            }
        }

        // Set all tiles back to unvisited for the next use.
        world.ClearVisitedFlags();

        return groups;
    }

    // Recursively find connected blocks (depth-first search)
    void PopulateGroup(World world, List<Block> group, block) {
        group.Add(block);
        block.visited=true;

        // Check all four neighbors and recurse on them if needed:
        if (block.x>0) {
            var neighbor = world.GetBlock(block.x - 1, block.y);
            if (neighbor.color==block.color&&neighbour.visited==false)
                PopulateGroup(world, group, neighbor);
        }
        if (block.x<world.Width-1) {
            var neighbor = world.GetBlock(block.x + 1, block.y);
            if (neighbor.color==block.color&&neighbour.visited==false)
                PopulateGroup(world, group, neighbor);
        }
        if (block.y>0) {
            var neighbor = world.GetBlock(block.x, block.y - 1);
            if (neighbor.color==block.color&&neighbour.visited==false)
                PopulateGroup(world, group, neighbor);
        }
        if (block.y<world.Height-1) {
            var neighbor = world.GetBlock(block.x, block.y + 1);
            if (neighbor.color==block.color&&neighbour.visited==false)
                PopulateGroup(world, group, neighbor);
        }
    }

    // Painting Methods
    public void FloodFill(int x, int y, int fill, int old) {
        if ((x<0)||(x>=getX())) return;
        if ((y<0)||(y>=getY())) return;
        if (getCell(x, y)==old) {
            setCell(x, y, fill);
            FloodFill(x+1, y, fill, old);
            FloodFill(x, y+1, fill, old);
            FloodFill(x-1, y, fill, old);
            FloodFill(x, y-2, fill, old);
        }
    }

    public void CircleEraser(int _x, int _y, int type,float range) {
        Vector3 target = new Vector3(_x,0,_y);
        for (int x = 0; x<getX(); x++) {
            for (int y = 0; y<getY(); y++) {
                if (GetDistance(x, y, target)<=range) {
                    switch (type) {
                        case 1:
                            setCell(x, y, 0);
                        break;
                        case 2:
                            setCell(x, y, 1);
                        break;
                        case 0:
                        // nothing
                        break;
                    }
                }
            }
        }
    }

    public void OuterCircleEraser(int _x, int _y, int type, float range) {
        Vector3 target = new Vector3(_x,0,_y);
        for (int x = 0; x<getX(); x++) {
            for (int y = 0; y<getY(); y++) {
                if (GetDistance(x, y, target)>=range) {
                    switch (type) {
                        case 1:
                        setCell(x, y, 0);
                        break;
                        case 2:
                        setCell(x, y, 1);
                        break;
                        case 0:
                        // nothing
                        break;
                    }
                }
            }
        }
    }
}
