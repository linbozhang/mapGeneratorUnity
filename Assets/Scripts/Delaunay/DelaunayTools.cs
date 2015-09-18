using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class DelaunayTools  {
	public static List<LineSegment> delaunayLinesForEdges(List<edge> edges){
		List<LineSegment> segments=new List<LineSegment>();
		foreach(Edge edge in edges){
			segments.Add(edge.delaunayLine());
		}
		return segments;
	}

}
