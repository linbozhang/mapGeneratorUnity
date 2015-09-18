﻿using UnityEngine;
using System.Collections;

public class LineSegment{
	public static float compareLengths_MAX(LineSegment segment0,LineSegment segment1){
		float length0=Vector2.Distance(segment0.p0,segment0.p1);
		float length1=Vector2.Distance(segment1.p0,segment1.p1);
		if(length0<length1){
			return 1;
		}
		if(length0>length1){
			return -1;
		}
		return 0;
	}
	public static float compareLengths(LineSegment edge0,LineSegment edge1){
		return compareLengths_MAX(edge0,edge1)*(-1);
	}
	public LineSegment(Vector2 p0,Vector2 p1){
		this.p0=p0;
		this.p1=p1;
	}
	public Vector2 p0;
	public Vector2 p1;

}
