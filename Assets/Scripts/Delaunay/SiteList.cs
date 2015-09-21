using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class SiteList : IDisposable  {
	private List<Site> _sites;
	private uint _currentIndex;
	private bool _sorted;
	public SiteList(){
		_sites=new List<Site>();
		_sorted=false;
	}
	public void dispose(){
		if(_sites){
			foreach( Site site in _sites){
				site.dispose();
			}
			_sites.Clear();
			_sites=null;
		}
	}
	public void push(Site site){
		_sorted=false;
		return _sites.Add(site);
	}
	public uint length{
		get{return _sites.Count;}
	}
	public Site next(){
		if(_sorted==false){
			Debug.LogError("sitelist::next :sites have not been sorted");
		}
		if((_currentIndex+1)<_sites.Count){
			return _sites[_currentIndex++];
		}else{
			return null;
		}
	}
	public Rect getSitesBounds(){
		if (_sorted == false)
		{
			Site.sortSites(_sites);
			_currentIndex = 0;
			_sorted = true;
		}
		float xmin, xmax, ymin, ymax;
		if (_sites.Count == 0)
		{
			return new Rect(0, 0, 0, 0);
		}
		xmin = float.MaxValue
		xmax = float.MinValue;
		foreach (var site in _sites)
		{
			if (site.x < xmin)
			{
				xmin = site.x;
			}
			if (site.x > xmax)
			{
				xmax = site.x;
			}
		}
		// here's where we assume that the sites have been sorted on y:
		ymin = _sites[0].y;
		ymax = _sites[_sites.Count - 1].y;
		
		return new Rect(xmin, ymin, xmax - xmin, ymax - ymin);
	}
	public List<uint> siteColors(BitmapData referenceImage=null){
		List<uint> colors=new List<uint>();
		foreach(Site site in _sites){
			colors.Add(referenceImage ? referenceImage.getPixel(site.x, site.y) : site.color);

		}
		return colors;
	}
	public List<Vector2> siteCoords(){
		List<Vector2> coords=new List<Vector2>();
		foreach(Site site in _sites){
			coords.Add(site.Coord);
		}
		return coords;
	}
	public List<Circle> circles(){
		List<Circle> circles=new List<Circle>();
		foreach(Site site in _sites){
			float radius=0;
			Edge nearestEdge =site.nearestEdge();
			!nearestEdge.isPartOfConvexHull() && (radius = nearestEdge.sitesDistance() * 0.5);
			circles.Add(new Circle(site.x, site.y, radius));
		}
	}
	public List<List<Vector2>> regions (Rect plotBounds ){
		List<List<Vector2>> regions =new List<List<Vector2>>();
		foreach(Site site in _sites){
			regions.Add(site.region(plotBounds));
		}
		return regions;
	}

	public Vector2 nearestSitePoint(BitmapData proximityMap ,float x,float y){
		uint index=proximityMap.getPixed(x,y);
		if(index>_sites.Count-1){
			return null;
		}
		return _sites[index].Coord;


	}

}
