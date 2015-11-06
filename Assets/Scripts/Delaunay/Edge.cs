using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Edge  {
	private static List<Edge> _pool=new List<Edge>();
	public static Edge createBisectingEdge(Site site0,Site site1){
		float dx,dy,absdx,absdy;
		float a,b,c;
		dx=site1.x-site0.x;
		dy=site1.y-site0.y;
	absdx=dx>0?dx:-dx;
	absdy=dy>0?dy:-dy;
		c=site0.x*dx+site0.y*dy+(dx*dx+dy*dy)*0.5;
		if (absdx > absdy)
		{
			a = 1.0; b = dy/dx; c /= dx;
		}
		else
		{
			b = 1.0; a = dx/dy; c /= dy;
		}
		Edge edge=Edge.create();
		edge.leftSite = site0;
		edge.rightSite = site1;
		site0.addEdge(edge);
		site1.addEdge(edge);
		
		edge._leftVertex = null;
		edge._rightVertex = null;
		
		edge.a = a; edge.b = b; edge.c = c;
		//trace("createBisectingEdge: a ", edge.a, "b", edge.b, "c", edge.c);
		
		return edge;
	}
	private static Edge create(){
		Edge edge;
		if (_pool.Count > 0)
		{
			edge = _pool[_pool.Count];
			_pool.RemoveAt(_pool.Count-1);
			edge.init();
		}
		else
		{
			edge = new Edge(typeof(PrivateConstrutorEnforcer));
		}
		return edge;
	}
	private static readonly Sprite LINESPRITE=new Sprite();
	private static readonly Graphics GRAPHICS=LINESPRITE.texture ;
	private Texture2D _delaunayLineBmp;
	public Texture2D delaunayLineBmp{
		get{if(!_delaunayLineBmp){
				_delaunayLineBmp=makeDelaunayLineBmp();
			}
			return _delaunayLineBmp;
			}
	}
	public Texture2D makeDelaunayLineBmp(){
		Vector2 p0=leftSite.Coord;
		Vector2 p1=rightSite.Coord;
		GRAPHICS.clear();
		GRAPHICS.lineStyle(0,0,1.0,false,LineScaleMove.NONE,CapsStyle.NONE);
		GRAPHICS.moveTo(p0.x, p0.y);
		GRAPHICS.lineTo(p1.x, p1.y);
		int w=(int)Mathf.Ceil(Mathf.Max(p0.x,p1.x));
		if(w<1){
			w=1;
		}
		int h=(int)Mathf.Ceil(Mathf.Max(p0.y,p1.y));
		if(h<1){
			h=1;
		}
		Texture2D bmp =new Texture2D(w,h);
		bmp.draw(LINESPRITE);
		return bmp;
	}
	public LineSegment delaunayLine(){
		return new LineSegment(leftSite.Coord,rightSite.Coord);
	}
	public LineSegment voronoiEdge(){
		if(!visible) return new LineSegment(null,null);
		return new LineSegment(_clippedVertices[LR.LEFT],_clippedVertices[LR.RIGHT]);
	

	}
	private static int _nedges=0;
	public static readonly Edge DELETED=new Edge(typeof(PrivateConstrutorEnforcer));
	public float a,b,c;
	private Vertex _leftVertex;
	public Vertex leftVertex{
		get{return _leftVertex;}
	}
	private Vertex _rightVertex;
	public Vertex rightVertex{
		get{return _rightVertex;}
	}
	public Vertex vertex(LR leftRight){
		return (leftRight==LR.LEFT)?_leftVertex:_rightVertex;
	}
	public void setVertex(LR leftRight,Vertex v){
		if(leftRight==LR.LEFT){
			_leftVertex=v;
		}else{
			_rightVertex=v;
		}
	}
	public bool isPartOfConvexHull(){
		return (_leftVertex==null || _rightVertex ==null);
	}
	public float sitesDistance(){
		return Vector2.Distance(leftSite.Coord,rightSite.Coord);
	}
	public static float compareSitesDistances_MAX(Edge edge0,Edge edge1){
		float length0=edge0.sitesDistance();
		float length1=edge1.sitesDistance();
		if(length0<length1){
			return 1;
		}
		if(length0>length1){
			return -1;
		}
		return 0;
	}
	public static float compareSitesDistances(Edge edge0,Edge edge1){
		return (-1)*compareSitesDistances_MAX(edge0,edge1);
	}
	private Dictionary<int ,Vertex> _clippedVertices;
	public Dictionary<int,Vertex> clippedEnds{
		get{return _clippedVertices;}
	}
	public bool visible{
		get{return _clippedVertices!=null;}
	}
	private Dictionary<LR,Site> _site;
	public Site leftSite{
		get{return _site[LR.LEFT];}
		set{_site[LR.LEFT]=value;}
	}
	public Site rightSite{
		get{return _site[LR.RIGHT];}
		set{_site[LR.RIGHT]=value;}
	}
	public Site site(LR leftRight){
		return _site[leftRight];
	}
	private int _edgeIndex;
	public void dispose(){
		if(_delaunayLineBmp){
			_delaunayLineBmp.dispose();
			_delaunayLineBmp=null;
		}
		_leftVertex=null;
		_rightVertex=null;
		if(_clippedVertices){
			_clippedVertices[LR.LEFT] = null;
			_clippedVertices[LR.RIGHT] = null;
			_clippedVertices = null;
		}
		_site[LR.LEFT] = null;
		_site[LR.RIGHT] = null;
		_site = null;

		_pool.RemoveAt(_pool.Count-1);
	}
	public Edge(Type _lock ){
		if(_lock!=PrivateConstrutorEnforcer){
			Debug.LogError("Edge:constructor is private ");

		}
		_edgeIndex=_nedges++;
		init();
	}
	private void init(){
		_site=new Dictionary<LR, Site>();
	}
	public string toString(){
		return "Edge " + _edgeIndex + "; sites " + _sites[LR.LEFT] + ", " + _sites[LR.RIGHT]
		+ "; endVertices " + (_leftVertex ? _leftVertex.vertexIndex : "null") + ", "
			+ (_rightVertex ? _rightVertex.vertexIndex : "null") + "::";
	}
	public void clipVertices(Rect bounds){
		float xmin=bounds.x;
		float ymin=bounds.y;
		float xmax=bounds.right;
		float ymax=bounds.bottom;

		Vertex vertex0,vertex1;
		float x0,x1,y0,y1;
		if(a==1.0&&b>=0.0){
			vertex0=_rightVertex;
			vertex1=_leftVertex;

		}
		else 
		{
			vertex0 = _leftVertex;
			vertex1 = _rightVertex;
		}
		
		if (a == 1.0)
		{
			y0 = ymin;
			if (vertex0 != null && vertex0.y > ymin)
			{
				y0 = vertex0.y;
			}
			if (y0 > ymax)
			{
				return;
			}
			x0 = c - b * y0;
			
			y1 = ymax;
			if (vertex1 != null && vertex1.y < ymax)
			{
				y1 = vertex1.y;
			}
			if (y1 < ymin)
			{
				return;
			}
			x1 = c - b * y1;
			
			if ((x0 > xmax && x1 > xmax) || (x0 < xmin && x1 < xmin))
			{
				return;
			}
			
			if (x0 > xmax)
			{
				x0 = xmax; y0 = (c - x0)/b;
			}
			else if (x0 < xmin)
			{
				x0 = xmin; y0 = (c - x0)/b;
			}
			
			if (x1 > xmax)
			{
				x1 = xmax; y1 = (c - x1)/b;
			}
			else if (x1 < xmin)
			{
				x1 = xmin; y1 = (c - x1)/b;
			}
		}
		else
		{
			x0 = xmin;
			if (vertex0 != null && vertex0.x > xmin)
			{
				x0 = vertex0.x;
			}
			if (x0 > xmax)
			{
				return;
			}
			y0 = c - a * x0;
			
			x1 = xmax;
			if (vertex1 != null && vertex1.x < xmax)
			{
				x1 = vertex1.x;
			}
			if (x1 < xmin)
			{
				return;
			}
			y1 = c - a * x1;
			
			if ((y0 > ymax && y1 > ymax) || (y0 < ymin && y1 < ymin))
			{
				return;
			}
			
			if (y0 > ymax)
			{
				y0 = ymax; x0 = (c - y0)/a;
			}
			else if (y0 < ymin)
			{
				y0 = ymin; x0 = (c - y0)/a;
			}
			
			if (y1 > ymax)
			{
				y1 = ymax; x1 = (c - y1)/a;
			}
			else if (y1 < ymin)
			{
				y1 = ymin; x1 = (c - y1)/a;
			}
		}
		
		_clippedVertices = new Dictionary<int, Vertex>();
		if (vertex0 == _leftVertex)
		{
			_clippedVertices[LR.LEFT] = new Vector2(x0, y0);
			_clippedVertices[LR.RIGHT] = new Vector2(x1, y1);
		}
		else
		{
			_clippedVertices[LR.RIGHT] = new Vector2(x0, y0);
			_clippedVertices[LR.LEFT] = new Vector2(x1, y1);
		}
	}

	
	
	
	
	
	























}
