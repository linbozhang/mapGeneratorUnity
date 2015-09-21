using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Triangle  {
	private List<Site> _sites;
	public List<Site> sites{
		get{return _sites;}
	}
	public Triangle(Site a,Site b,Site c){
		_sites=new List<Site>();
		_sites.Add(a);
		_sites.Add(b);
		_sites.Add(c);
	}
	public void dispose(){
		_sites.Clear();
		_sites=null;
	}
}
