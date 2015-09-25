using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Center{
	public int index;
	public Vector2 point;
	public bool water;
	public bool ocean;
	public bool coast;
	public bool border;
	public string biome;
	public float elevation;
	public float moisture;

	public List<Center> neighbors;
	public List<Edge> borders;
	public List<Corner> corners;
}
public class Corner{
	public int index;
	public Vector2 point;
	public bool water;
	public bool ocean;
	public bool coast;
	public bool border;
	//public string biome;
	public float elevation;
	public float moisture;
	
	public List<Center> touches;
	public List<Edge> protrudes;
	public List<Corner> adjacent;

	public int river;
	public Corner downslope;
	public Corner watershed;
	public int watershed_size;
}

public delegate  bool IslandShapeDelegate( Vector2 point,int seed=0);
public delegate List<Vector2> PointSelectorDelegate(int size,int seed);
public class Map  {
	static public float LAKE_THRESHOLD=0.3f;
	public float SIZE;
	public IslandShapeDelegate islandShape;
	public bool needsMoreRandomness;
	public PointSelectorDelegate pointSelector;
	public int numPoints;
	public List<Vector2> points;
	public List<Vector2> centers;
	public List<Vector2> corners;
	public List<Edge> edges;
	public PM_PRNG mapRandom=new PM_PRNG();
	public Map( float size){
		SIZE=size;
		numPoints=1;
		reset();
	}
	public void go(int first ,int last){
		Array stages=new Array();


	}
	public void placePoint_(){
		reset();
		points=pointSelector(numPoints,0);
	}
	public void buildGraph_(){
		Voronoi voronoi=new Voronoi(points,null,new Rect(0,0,SIZE,SIZE));
		buildGraph(points,voronoi);
		improveCorners();
		voronoi.dispose();
		voronoi=null;
		points=null;
	}



	public void reset(){
		Center p;Corner q;//Edge edge;
		// Break cycles so the garbage collector will release data.
		if (points) {
			points.Clear();
		}
		if (edges) {

			edges.Clear();
		}
		if (centers) {
			centers.Clear();
		}
		if (corners) {
			corners.Clear();
		}
		
		// Clear the previous graph data.
		if (!points) points = new List<Point>();
		if (!edges) edges = new List<Edge>();
		if (!centers) centers = new List<Center>();
		if (!corners) corners = new List<Corner>();
		
		//System.gc();
	}
	public void newIsland( string islandType,string pointType,int numPoints_,int seed,int variant ){
		islandShape=makePerlin;
		pointSelector=generateRelaxed;
		needsMoreRandomness=false;
		mapRandom.seed=variant;
	}
	public bool makePerlin(Vector2 point, int seed=0){
//		Texture2D perlin=new Texture2D (256,256);
//		perlin.perlinNoise(seed);
		float c= Mathf.PerlinNoise(seed+point.x,seed+point.y);
		bool ret=c>(0.3f+0.3f* Mathf.Pow(Vector2.Distance(point,Vector2.zero),2));
		return ret;

	}

	public List<Vector2> generateRelaxed(int size,int seed){
		int i;Vector2 p;Vector2 q;Voronoi voronoi;List<Vector2> region;
		for(i=0;i<2;i++){
			voronoi =new Voronoi(points,null,new Rect(0,0,size,size));
			foreach( Vector2  v in points){
				region=voronoi.region(v);
				v=Vector2.zero;
				foreach(Vector2 v2 in region){
					v.x+=v2.x;
					v.y+=v2.y;
				}
				v.x/=region.Count;
				v.y/=region.Count;
				region.Clear();
			}
			voronoi.dispose();
		}
		return points;
	}




}










