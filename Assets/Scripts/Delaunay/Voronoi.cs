using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Voronoi  {

	private SiteList _sites;
	private Dictionary<Vector2,Site> _sitesIndexedByLocation;
	private List<Triangle> _triangles;
	private List<Edge> _edges;
	private Rect _plotBounds;
	public Rect plotBounds{
		get{return _plotBounds;}
	}
	public void dispose(){
		int i,n;
		if (_sites)
		{
			_sites.dispose();
			_sites = null;
		}
		if (_triangles)
		{
			n = _triangles.Count;
			for (i = 0; i < n; ++i)
			{
				_triangles[i].dispose();
			}
			_triangles.Count = 0;
			_triangles = null;
		}
		if (_edges)
		{
			n = _edges.Count;
			for (i = 0; i < n; ++i)
			{
				_edges[i].dispose();
			}
			_edges.Count = 0;
			_edges = null;
		}
		_plotBounds = null;
		_sitesIndexedByLocation = null;
	}
	public Voronoi (List<Vector2> points,List<uint> colors,Rect plotBounds){
		_sites=new SiteList();
		_sitesIndexedByLocation=new Dictionary<Vector2, Site>();
		addSites(points, colors);
		_plotBounds = plotBounds;
		_triangles = new List<Triangle>();
		_edges = new List<Edge>();
		fortunesAlgorithm();
	}
	private void addSites(List<Vector2> points,List<uint> colors){
		uint length=points.Count;
		for(uint i=0;i<length;i++){
			addSite (points[i],colors?colors[i]:0,i);
		}
	}
	private void addSite(Vector2 p,uint color ,int index){
		float weight=UnityEngine.Random.Range(1,100);
		Site site=Site.create(p,index,weight,color);
		_sites.push(site);
		_sitesIndexedByLocation[p]=site;
	}
	public List<Edge> edges(){
		return _edges;
	}
	public List<Vector2> region(Vector2 p)
	{
		Site site = _sitesIndexedByLocation[p];
		if (!site)
		{
			return new List<Vector2>();
		}
		return site.region(_plotBounds);
	}
	
	// TODO: bug: if you call this before you call region(), something goes wrong :(
	public List<Vector2> neighborSitesForSite(Vector2 coord)
	{
		Vector2 points = new List<Vector2>();
		Site site = _sitesIndexedByLocation[coord];
		if (!site)
		{
			return points;
		}
		List<Site> sites = site.neighborSites();
		//Site neighbor;
		foreach ( Site neighbor in sites)
		{
			points.add(neighbor.coord);
		}
		return points;
	}
	
	public List<Circle> circles()
	{
		return _sites.circles();
	}
	
	public List<LineSegment> voronoiBoundaryForSite(Vector2 coord)
	{
		return visibleLineSegments(selectEdgesForSitePoint(coord, _edges));
	}
	
	public List<LineSegment>  delaunayLinesForSite(Vector2 coord)
	{
		return delaunayLinesForEdges(selectEdgesForSitePoint(coord, _edges));
	}
	
public List<LineSegment> voronoiDiagram()
	{
		return visibleLineSegments(_edges);
	}
	
	public List<LineSegment> delaunayTriangulation( BitmapData keepOutMask = null)
	{
		return delaunayLinesForEdges(selectNonIntersectingEdges(keepOutMask, _edges));
	}
	
	public List<LineSegment> hull()
	{
		return delaunayLinesForEdges(hullEdges());
	}
	
	private List<Edge> hullEdges()
	{
		List <Edge> rtn;
		foreach(Edge edge in _edges){
			if(edge.isPartOfConvexHull()){
				rtn.Add(edge);
			}
		}
		return rtn;

	}
	
	public List<Vector2>  hullPointsInOrder()
	{
		List<Edge> hullEdges = hullEdges();
		
		List<Vector2> points = new List<Vector2>();
		if (hullEdges.Count == 0)
		{
			return points;
		}
		
		EdgeReorderer reorderer = new EdgeReorderer(hullEdges, Site);
		hullEdges = reorderer.edges;
		List<LR> orientations = reorderer.edgeOrientations;
		reorderer.dispose();
		
		LR orientation;
		
		int n = hullEdges.length;
		for (int i = 0; i < n; ++i)
		{
			Edge edge = hullEdges[i];
			orientation = orientations[i];
			points.Add(edge.site(orientation).coord);
		}
		return points;
	}
	
	public List<LineSegment> spanningTree(string type = "minimum", BitmapData keepOutMask = null)
	{
		List<Edge> edges = selectNonIntersectingEdges(keepOutMask, _edges);
		List<LineSegment> segments = delaunayLinesForEdges(edges);
		return kruskal(segments, type);
	}
	
	public List<List<Vector2>> regions()
	{
		return _sites.regions(_plotBounds);
	}
	
	public List<uint> siteColors(BitmapData referenceImage = null)
	{
		return _sites.siteColors(referenceImage);
	}
	
	/**
		 * 
		 * @param proximityMap a BitmapData whose regions are filled with the site index values; see PlanePointsCanvas::fillRegions()
		 * @param x
		 * @param y
		 * @return coordinates of nearest Site to (x, y)
		 * 
		 */
	public Vector2 nearestSitePoint(BitmapData proximityMap,  float x,  float y)
	{
		return _sites.nearestSitePoint(proximityMap, x, y);
	}
	
	public List<Vector2> siteCoords()
	{
		return _sites.siteCoords();
	}
	
	private void fortunesAlgorithm()
	{
		Site newSite, bottomSite, topSite, tempSite;
		Vertex v, vertex;
		Vector2 newintstar;
		LR leftRight;
		Halfedge lbnd, rbnd, llbnd, rrbnd, bisector;
		Edge edge;
		
		Rect dataBounds = _sites.getSitesBounds();
		
		int sqrt_nsites = int(Math.sqrt(_sites.length + 4));
		HalfedgePriorityQueue heap = new HalfedgePriorityQueue(dataBounds.y, dataBounds.height, sqrt_nsites);
		EdgeList edgeList = new EdgeList(dataBounds.x, dataBounds.width, sqrt_nsites);
		List<Halfedge> halfEdges = new List<Halfedge> ();
		List<Vertex> vertices = new List<Vertex> ;
		
		Site bottomMostSite = _sites.next();
		newSite = _sites.next();
		
		for (;;)
		{
			if (heap.empty() == false)
			{
				newintstar = heap.min();
			}
			
			if (newSite != null 
			    &&  (heap.empty() || compareByYThenX(newSite, newintstar) < 0))
			{
				/* new site is smallest */
				//trace("smallest: new site " + newSite);
				
				// Step 8:
				lbnd = edgeList.edgeListLeftNeighbor(newSite.coord);	// the Halfedge just to the left of newSite
				//trace("lbnd: " + lbnd);
				rbnd = lbnd.edgeListRightNeighbor;		// the Halfedge just to the right
				//trace("rbnd: " + rbnd);
				bottomSite = rightRegion(lbnd);		// this is the same as leftRegion(rbnd)
				// this Site determines the region containing the new site
				//trace("new Site is in region of existing site: " + bottomSite);
				
				// Step 9:
				edge = Edge.createBisectingEdge(bottomSite, newSite);
				//trace("new edge: " + edge);
				_edges.Add(edge);
				
				bisector = Halfedge.create(edge, LR.LEFT);
				halfEdges.Add(bisector);
				// inserting two Halfedges into edgeList constitutes Step 10:
				// insert bisector to the right of lbnd:
				edgeList.insert(lbnd, bisector);
				
				// first half of Step 11:
				if ((vertex = Vertex.intersect(lbnd, bisector)) != null) 
				{
					vertices.Add(vertex);
					heap.remove(lbnd);
					lbnd.vertex = vertex;
					lbnd.ystar = vertex.y + newSite.dist(vertex);
					heap.insert(lbnd);
				}
				
				lbnd = bisector;
				bisector = Halfedge.create(edge, LR.RIGHT);
				halfEdges.Add(bisector);
				// second Halfedge for Step 10:
				// insert bisector to the right of lbnd:
				edgeList.insert(lbnd, bisector);
				
				// second half of Step 11:
				if ((vertex = Vertex.intersect(bisector, rbnd)) != null)
				{
					vertices.Add(vertex);
					bisector.vertex = vertex;
					bisector.ystar = vertex.y + newSite.dist(vertex);
					heap.insert(bisector);	
				}
				
				newSite = _sites.next();	
			}
			else if (heap.empty() == false) 
			{
				/* intersection is smallest */
				lbnd = heap.extractMin();
				llbnd = lbnd.edgeListLeftNeighbor;
				rbnd = lbnd.edgeListRightNeighbor;
				rrbnd = rbnd.edgeListRightNeighbor;
				bottomSite = leftRegion(lbnd);
				topSite = rightRegion(rbnd);
				// these three sites define a Delaunay triangle
				// (not actually using these for anything...)
				//_triangles.push(new Triangle(bottomSite, topSite, rightRegion(lbnd)));
				
				v = lbnd.vertex;
				v.setIndex();
				lbnd.edge.setVertex(lbnd.leftRight, v);
				rbnd.edge.setVertex(rbnd.leftRight, v);
				edgeList.remove(lbnd); 
				heap.remove(rbnd);
				edgeList.remove(rbnd); 
				leftRight = LR.LEFT;
				if (bottomSite.y > topSite.y)
				{
					tempSite = bottomSite; bottomSite = topSite; topSite = tempSite; leftRight = LR.RIGHT;
				}
				edge = Edge.createBisectingEdge(bottomSite, topSite);
				_edges.Add(edge);
				bisector = Halfedge.create(edge, leftRight);
				halfEdges.Add(bisector);
				edgeList.insert(llbnd, bisector);
				edge.setVertex(LR.other(leftRight), v);
				if ((vertex = Vertex.intersect(llbnd, bisector)) != null)
				{
					vertices.Add(vertex);
					heap.remove(llbnd);
					llbnd.vertex = vertex;
					llbnd.ystar = vertex.y + bottomSite.dist(vertex);
					heap.insert(llbnd);
				}
				if ((vertex = Vertex.intersect(bisector, rrbnd)) != null)
				{
					vertices.Add(vertex);
					bisector.vertex = vertex;
					bisector.ystar = vertex.y + bottomSite.dist(vertex);
					heap.insert(bisector);
				}
			}
			else
			{
				break;
			}
		}
		
		// heap should be empty now
		heap.dispose();
		edgeList.dispose();
		
		foreach (Halfedge halfEdge in halfEdges)
		{
			halfEdge.reallyDispose();
		}
		halfEdges.length = 0;
		
		// we need the vertices to clip the edges
		foreach (edge in _edges)
		{
			edge.clipVertices(_plotBounds);
		}
		// but we don't actually ever use them again!
		foreach (vertex in vertices)
		{
			vertex.dispose();
		}
		vertices.length = 0;
		
		Site leftRegion(Halfedge he)
		{
			Edge edge = he.edge;
			if (edge == null)
			{
				return bottomMostSite;
			}
			return edge.site(he.leftRight);
		}
		
		Site rightRegion(Halfedge he)
		{
			Edge edge = he.edge;
			if (edge == null)
			{
				return bottomMostSite;
			}
			return edge.site(LR.other(he.leftRight));
		}
	}
	
	internal static function compareByYThenX(s1:Site, s2:*):Number
	{
		if (s1.y < s2.y) return -1;
		if (s1.y > s2.y) return 1;
		if (s1.x < s2.x) return -1;
		if (s1.x > s2.x) return 1;
		return 0;
	}
}
