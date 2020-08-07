using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class Cell {
    public Vector2Int XY { get; set; }
    public int ID { get; set; }
    public int Score { get; set; }
    public int Type { get; set; }
    public bool Walkable { get; set; }
    public bool Visited { get; set; }

    public Cell(Vector2Int XY, int ID, int Score, int Type, bool Walkable, bool Visited) {
        this.XY=XY;
        this.ID=ID;
        this.Score=Score;
        this.Type=Type;
        this.Walkable=Walkable;
        this.Visited=Visited;
    }
}
public class Grid {
    public Cell[,] grid;

    // Grid dimensions
    public int Width; // Width (ex: 250)
    public int Height; // Height (ex: 250)

    public Grid(int x, int y) {
        Width=x;
        Height=y;

        grid=new Cell[Width, Height];

        int cellID = 0;
        Vector2Int XY;

        for (int i = 0; i<Width; i++) {
            for (int k = 0; k<Height; k++) {
                XY=new Vector2Int(i, k);
                grid[i, k]=new Cell(XY, cellID, 0, 0, false, false);
                cellID++;
            }
        }

    }

    //set a cell in grid space
    public void setCellType(int x, int y, int v) {
        try {
            grid[x, y].Type=v;
        } catch {
            Debug.Log("ERROR");
            Debug.Log("w: "+Width);
            Debug.Log("h: "+Height);
            Debug.Log("x: "+x);
        }
    }

    public int getCellType(int x, int y) {
        if (x>=0&&x<=Width-1&&y>=0&&y<=Height-1) {
            return grid[x, y].Type;
        } else {
            return -1;
        }
    }

    public Cell GetCell(int x, int y) {
        if (x>=0&&x<=Width-1&&y>=0&&y<=Height-1) {
            return grid[x, y];
        } else {
            return null;
        }
    }

    public int getX() {
        return Width;
    }

    public int getY() {
        return Height;
    }

    public static Cell[,] GenerateNoiseMap(Cell[,] mapGrid, int seed, float scale, int octaves, float persistance, float lacunarity) {
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
                
                mapGrid[x, y].Type=Mathf.RoundToInt(noiseHeight);
            }
        }

        for (int x = 0; x<mapGrid.GetUpperBound(0); x++) {
            for (int y = 0; y<mapGrid.GetUpperBound(1); y++) {
                mapGrid[x, y].Type=Mathf.RoundToInt(Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, mapGrid[x, y].Type));

            }
        }

        return mapGrid;
    }

    // I'm bad at math and I cannot for the life of me figure out how to get the noise gen to work properly.
    public static Cell[,] FuckingLazyConvert(Cell[,] mapGrid) {
        for (int x = 0; x<mapGrid.GetUpperBound(0); x++) {
            for (int y = 0; y<mapGrid.GetUpperBound(1); y++) {
                if(mapGrid[x,y].Type == 1) {
                    mapGrid[x, y].Type = 2;
                }
            }
        }
        return mapGrid;
    }

    static int GetMooreSurroundingTiles(Cell[,] mapGrid, int x, int y) {
        int tileCount = 0;
        for (int neighbourX = x-1; neighbourX<=x+1; neighbourX++) {
            for (int neighbourY = y-1; neighbourY<=y+1; neighbourY++) {
                if (neighbourX>=0&&neighbourX<mapGrid.GetUpperBound(0)&&neighbourY>=0&&neighbourY<mapGrid.GetUpperBound(1)) {
                    //We don't want to count the tile we are checking the surroundings of
                    if (neighbourX!=x||neighbourY!=y) {
                        tileCount+=mapGrid[neighbourX, neighbourY].Type;
                    }
                }
            }
        }
        return tileCount;
    }

    public static Cell[,] SmoothMooreCellularAutomata(Cell[,] mapGrid, int smoothCount) {
        for (int i = 0; i<smoothCount; i++) {
            for (int x = 0; x<mapGrid.GetUpperBound(0); x++) {
                for (int y = 0; y<mapGrid.GetUpperBound(1); y++) {
                    int surroundingTiles = GetMooreSurroundingTiles(mapGrid, x, y);

                    if (surroundingTiles>4) {
                        mapGrid[x, y].Type=1;
                    } else if (surroundingTiles<4) {
                        mapGrid[x, y].Type=0;
                    }
                }
            }
        }
        return mapGrid;
    }

    public float GetDistance(int _x, int _y, Vector3 target) {
        Vector3 thisTile = new Vector3(_x,0,_y);
        float distance = Vector3.Distance(thisTile,target)/(getX() + getY());
        return distance;
    }

    /*
        Visualisation of what grid looks like after generation

        0 0 0 0 0 0 0 0 0 0
        0 0 1 1 0 1 0 0 0 0
        0 1 0 0 1 0 1 1 1 0
        0 1 0 1 4 0 1 1 1 0
        0 0 1 2 2 3 1 0 0 0
        0 0 0 0 1 0 0 0 1 1
        0 0 0 0 0 0 0 0 0 0

    */

    public List<List<Cell>> FindConnectedGroups(Grid mapGrid, int type) {
        List<List<Cell>> groups = new List<List<Cell>>();

        for (int x = 0; x<mapGrid.getX(); x++) {
            for (int y = 0; y<mapGrid.getY(); y++) {
                Cell cell = mapGrid.GetCell(x,y);

                if (cell.Visited)
                    continue;

                if (cell.Type==type) {
                    if (x>0) {
                        Cell neighbor = mapGrid.GetCell(x - 1, y);
                        if (neighbor.Type==cell.Type) {
                            List<Cell> group = new List<Cell>();
                            PopulateGroup(mapGrid, group, cell);
                            groups.Add(group);
                            continue;
                        }
                    }

                    if (y>0) {
                        Cell neighbor = mapGrid.GetCell(x, y - 1);
                        if (neighbor.Type==cell.Type) {
                            List<Cell> group = new List<Cell>();
                            PopulateGroup(mapGrid, group, cell);
                            groups.Add(group);
                        }
                    }
                }
            }
        }

        mapGrid.ClearVistedState();
        return groups;
    }

    // Recursively find connected blocks (depth-first search)
    private void PopulateGroup(Grid mapGrid, List<Cell> group, Cell cell) {
        cell.Visited=true;
        group.Add(cell);

        if (cell.XY.x>0) {
            Cell neighbor = mapGrid.GetCell(cell.XY.x - 1, cell.XY.y);
            if (neighbor.Type==cell.Type && neighbor.Visited==false) {
                PopulateGroup(mapGrid, group, neighbor);
            }
        }
        if (cell.XY.x<mapGrid.getX() - 1) {
            Cell neighbor = mapGrid.GetCell(cell.XY.x + 1, cell.XY.y);
            if (neighbor.Type==cell.Type && neighbor.Visited==false) {
                PopulateGroup(mapGrid, group, neighbor);
            }
        }
        if (cell.XY.y>0) {
            Cell neighbor = mapGrid.GetCell(cell.XY.x, cell.XY.y - 1);
            if (neighbor.Type==cell.Type && neighbor.Visited==false) {
                PopulateGroup(mapGrid, group, neighbor);
            }
        }
        if (cell.XY.y<mapGrid.getY() - 1) {
            Cell neighbor = mapGrid.GetCell(cell.XY.x, cell.XY.y + 1);
            if (neighbor.Type==cell.Type && neighbor.Visited==false) {
                PopulateGroup(mapGrid, group, neighbor);
            }
        }
    }
    
    private void ClearVistedState() {
        for(int x = 0; x < getX(); x++) {
            for (int y = 0; y<getX(); y++) {
                grid[x, y].Visited=false;
            }
        }
    }

    public static Grid RemoveWater(List<List<Cell>> groups, Grid mapGrid, int count) {

        for (int i = 0; i<count; i++) {
            int index = UnityEngine.Random.Range(0, groups.Count);
            var group = groups[index];
            for (int k =0; k<group.Count; k++) {
                mapGrid.setCellType(group[k].XY.x, group[k].XY.y, 1);
            }
        }

        return mapGrid;
    }

    // Painting Methods
    public void CircleEraser(int _x, int _y, int type,float range) {
        Vector3 target = new Vector3(_x,0,_y);
        for (int x = 0; x<getX(); x++) {
            for (int y = 0; y<getY(); y++) {
                if (GetDistance(x, y, target)<=range) {
                    switch (type) {
                        case 1:
                            setCellType(x, y, 0);
                        break;
                        case 2:
                            setCellType(x, y, 1);
                        break;
                        case 0:
                        // nothing
                        break;
                    }
                }
            }
        }
    }

    public void EdgeEraser() {
        for (int x = 0; x<getX(); x++) {
            for (int y = 0; y<getY(); y++) {
                if (x==0) {
                    setCellType(x, y, 0);
                }

                if (x==getX()-1) {
                    setCellType(x, y, 0);
                }

                if (y==0) {
                    setCellType(x, y, 0);
                }

                if (y==getX()-1) {
                    setCellType(x, y, 0);
                }
            }
        }
    }
}
