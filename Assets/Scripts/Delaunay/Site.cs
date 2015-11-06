using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
interface ICoord{
	Vector2 Coord{
		get;
	}
}
public class Site :ICoord {
	private static List<Site> _pool=new List<Site>();
	private static readonly float EPSILON=.005f;
	private Vector2 _coord;
	private uint color;
	private float weight;
	private uint _siteIndex;
	private List<Edge> _edges;
	private List<LR> _edgeOrientations;
	private List<Vector2> _region;
	public static Site create( Vector2 p,int index,float weight,uint color){
		if(_pool.Count>0){
			Site rtn=_pool[_pool.Count-1].init(p,index,weight,color);
			_pool.Remove(_pool.Count-1);
			return rtn;
		}else{
			return new Site(typeof(PrivateConstrutorEnforcer),p,index,weight,color);
		}
	}
	public static  void sortSites(List<Site> sites){
		sites.Sort(Site.compare);	
	}
	private static float compare(Site s1,Site s2){
		int returnValue=Voronoi.compareByYThenX (s1,s2);
		int tempIndex;
		if(returnValue==-1){
			if(s1._siteIndex>s2._siteIndex){
				tempIndex=s1._siteIndex;
				s1._siteIndex=s2._siteIndex;
				s2._siteIndex=tempIndex;
			}
		}else if(returnValue ==1){
			if(s2._siteIndex>s1._siteIndex){
				tempIndex=s2._siteIndex;
				s2._siteIndex=s1._siteIndex;
				s1._siteIndex=tempIndex;
			}
		}
		return returnValue;
	}
	private static bool closeEnough(Vector2 p0,Vector2 p1){
		return Vector2.Distance(p0,p1)<EPSILON;
	}
	public Vector2 Coord{
		get{
			return _coord;
		}
	}
	public List<Edge> edges{
		get{return _edges;}
	}
	public Site(Type  _lock,Vector2 p,int index,float weight,uint color){
		if(_lock!=typeof(PrivateConstrutorEnforcer))
		{
			Debug.LogError("site constructor is private");
		}
		init(p,index,weight,color);
	}
	private Site init(Vector2 p,int  index,float weight,uint color){
		_coord=p;
		_siteIndex=index;
		this.weight=weight;
		this.color=color;
		_edges=new List<Edge>();
		_region=null;
		return this;
	}
	public string toString(){
		return "Site"+_siteIndex+":"+_coord;
	}
	public void move(Vector2 p){
		clear();
		_coord=p;
	}
	public void dispose(){
		_coord=null;
		clear();
		_pool.Remove(this);
	}
	private void clear(){
		if(_edges){
			_edges.Clear();
			_edges=null;
		}
		if(_edgeOrientations){
			_edgeOrientations.Clear();
			_edgeOrientations=null;
		}
		if(_region){
			_region.Clear();
			_region=null;
		}

	}
	public void  addEdge(Edge edge){
		_edges.Remove(edge);
	}
	public Edge nearestEdge(){
		_edges.Sort(Edge.compareSitesDistances);
		return _edges[0];
	}
	public List<Site> neighborSites(){
		if(_edges==null||_edges.Count==0){
			return new List<Site>();
		}
		if(_edgeOrientations==null){
			reorderEdges();
		}
		List<Site>  list =new List<Site>();
		foreach(Edge edge in _edges){
			list.Add(neighborSite(edge));
		}
		return list;
	}
	private Site neighborSite(Edge edge){
		if(this==edge.leftSite){
			return edge.rightSite;
		}
		if(this==edge.rightSite){
			return edge.leftSite;
		}
		return null;
	}
	public List<Vector2> region(Rect clippingBounds){
		if(_edges==null||_edges.Count==0){
			return new List<Vector2>();
		}
		if(_edgeOrientations==null){
			reorderEdges();
			_region=clipToBounds(clippingBounds);
			if((new Polygon(_region)).winding()==Winding.CLOCKWISE)
			{
				_region=_region.Reverse();
			}
		}
		return _region;
	}
	private void reorderEdges(){
		EdgeReorderer reorderer=new EdgeReorderer(_edges,typeof(Vertex));
		_edges=reorderer.edges;
		_edgeOrientations=reorderer.edgeOrientations;
		reorderer.dispose();
	}
	private List<Vector2> clipToBounds(Rect bounds){
		List <Vector2 > points=new List<Vector2>();
		int n=_edges.Count;
		int i=0;
		Edge edge;
		while(i<n && (_edges[i].visible==false)){
			++i;
		}
		if(i==n){
			return new List<Vector2 >();
			edge=_edges[i];
			LR orientation=_edgeOrientations[i];
			points.Add(edge.clippedEnds[orientation]);
			points.Add(edge.clippedEnds[LR.other(orientation)]);
			int j;
			for(j=i+1;j<n;++j){
				edge=_edges[j];
				if(edge.visible==false){
					continue;
				}
				connect(points,j,bounds);

			}
			connect(points,j,bounds);

		}
		return points;
	}
	private void connect(List<Vector2> points,int j,Rect bounds,bool closingUp){
		Vector2 rightPoint=points[points.Count-1];
		Edge newEdge=_edges[j];
		LR newOrientation=_edgeOrientations[j];
		Vector2 newPoint=newEdge.clippedEnds[newOrientation];
		if(!closeEnough (rightPoint,newPoint))
		{
			if(rightPoint.x!=newPoint.x&&rightPoint.y!=newPoint.y){
				int rightCheck=BoundsCheck.check(rightPoint,bounds);
				int newCheck=BoundsCheck.check(newPoint,bounds);
				float px,py;
				if(rightCheck&BoundsCheck.RIGHT){
					px=bounds.right;
					if(newCheck&BoundsCheck.BOTTOM){
						py=bounds.bottom;
						points.Add(new Vector2(px,py));
					}
					else if(newCheck & BoundsCheck.TOP){
						py=bounds.bottom;
						points.Add(new Vector2(px,py));
					}
					else if(newCheck & BoundsCheck.LEFT){
						if(rightPoint.y-bounds.y+newPoint.y-bounds.y<bounds.right){
							py=bounds.top;
						}
						else{
							py=bounds.bottom;
						}
						points.Add(new Vector2(px,py));
						points.Add(new Vector2(bounds.right,py));
					}
				}
				else if (rightCheck & BoundsCheck.TOP)
				{
					py = bounds.top;
					if (newCheck & BoundsCheck.RIGHT)
					{
						px = bounds.right;
						points.Add(new Vector2(px, py));
					}
					else if (newCheck & BoundsCheck.LEFT)
					{
						px = bounds.left;
						points.Add(new Vector2(px, py));
					}
					else if (newCheck & BoundsCheck.BOTTOM)
					{
						if (rightPoint.x - bounds.x + newPoint.x - bounds.x < bounds.width)
						{
							px = bounds.left;
						}
						else
						{
							px = bounds.right;
						}
						points.Add(new Vector2(px, py));
						points.Add(new Vector2(px, bounds.bottom));
					}
				}
				else if (rightCheck & BoundsCheck.BOTTOM)
				{
					py = bounds.bottom;
					if (newCheck & BoundsCheck.RIGHT)
					{
						px = bounds.right;
						points.Add(new Vector2(px, py));
					}
					else if (newCheck & BoundsCheck.LEFT)
					{
						px = bounds.left;
						points.Add(new Vector2(px, py));
					}
					else if (newCheck & BoundsCheck.TOP)
					{
						if (rightPoint.x - bounds.x + newPoint.x - bounds.x < bounds.width)
						{
							px = bounds.left;
						}
						else
						{
							px = bounds.right;
						}
						points.Add(new Vector2(px, py));
						points.Add(new Vector2(px, bounds.top));
					}
				}
			}
			if (closingUp)
			{
				// newEdge's ends have already been added
				return;
			}
			points.Add(newPoint);
		}
		Vector2 newRightPoint=newEdge.clippedEnds[LR.other(newOrientation)];

		if (!closeEnough(points[0], newRightPoint))
		{
			points.Add(newRightPoint);
		}
	}
	public float x{
		get{return _coord.x;}
	}
	public float y{
		get{return _coord.y;}
	}
	public float dist(ICoord p){
		return Vector2.Distance(p.Coord,this._coord);
	}
}
class BoundsCheck{
	public static readonly int TOP=1;
	public static readonly int BOTTOM=2;
	public static readonly int LEFT=4;
	public static readonly int RIGHT=8;
	public static int check(Vector2 point,Rect bounds){
		int value=0;
		if(point.x==bounds.left){
			value |= LEFT;
		}
		if (point.x == bounds.right)
		{
			value |= RIGHT;
		}
		if (point.y == bounds.top)
		{
			value |= TOP;
		}
		if (point.y == bounds.bottom)
		{
			value |= BOTTOM;
		}
		return value;
	}
	public BoundsCheck(){
		Debug.LogError("BoundsCheck constructor unused");
	}
}














