using UnityEngine;
using System.Collections;

public class Circle{
	public Vector2 center;
	public float radius;
	public Circle(float centerX,float centerY,float radius)
	{
		this.center=new Vector2(centerX,centerY);
		this.radius=radius;
	}
	public string toString(){
		return "Circle (center :"+center+";radius:"+radius+")";
	}
}
