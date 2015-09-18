using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class Polygon{
	private List<Vector2> _vertices;
	public Polygon(List<Vector2> vertices){
		_vertices=vertices;
	}
	public float area(){
		return Mathf.Abs(signedDoubleAreaF()*0.5f);
	}
	public Winding winding(){
		float signedDoubleArea =signedDoubleAreaF();
		if(signedDoubleArea<0){
			return Winding.CLOCKWISE;
		}
		if(signedDoubleArea>0){
			return Winding.COUNTERCLOCKWISE;
		}
		return Winding.NONE;
	}
	private float signedDoubleAreaF(){
		int index;
		int nextIndex;
		Vector2 point;
		Vector2 next;
		int n= _vertices.Count;
		float signedDoubleArea=0;
		for(index=0;index<n;++index){
			nextIndex=(index+1)%n;
			point =_vertices[index];
			next=_vertices[nextIndex];
			signedDoubleArea+=point.x*next.y-next.x*point.y;
		}
		return signedDoubleArea;
	}

}
