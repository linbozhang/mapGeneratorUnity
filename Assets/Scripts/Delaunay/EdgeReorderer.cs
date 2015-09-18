using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class EdgeReorderer {
	private List<Edge> _edges;
	private List<LR> _edgeOrientations;
	public List<Edge> edges{
		get{return _edges;}
	}
	public List<LR> edgeOrientations{
		get{return _edgeOrientations;}
	}
	public EdgeReorderer(List<Edge> origEdges,Type criterion){
		if(criterion!=typeof(Vertex)&&criterion!=typeof(Site)){
			Debug.LogError("edges: cirterion must be vertex or site");
		}
		_edges=new List<Edge>();
		_edgeOrientations=new List<LR>();
		if(origEdges.Count>0){
			_edges=reorderEdges(origEdges,criterion);
		}
	}
	public void dispose(){
		_edges=null;
		_edgeOrientations=null;
	}
	private List<Edge> reorderEdges( List<Edge> origEdges,Type criterion){
		int i;
		int j;
		int n=origEdges.Count;
		Edge edge;
		List<bool> done =new List<bool>();
		int nDone=0;
		for(int li=0;li<n;li++){
			done.Add(false);
		}
		List<Edge> newEdges=new List<Edge>();
		i=0;
		edge=origEdges[i];
		newEdges.Add(edge);
		_edgeOrientations.Add(LR.LEFT);
		ICoord firstPoint =(criterion == typeof(Vertex))?edge.leftVertex:edge.leftSites;
		ICoord lastPoint =(criterion == typeof(Vertex))?edge.rightVertex:edge.rightSites;
		if(firstPoint==Vertex.VERTEX_AT_INFINITY||lastPoint==Vertex.VERTEX_AT_INFINITY)
		{
			return new List<Edge>();
		}
		done[i]=true;
		++nDone;
		while(nDone<n){
			for(i=1;i<n;++i){
				if(done[i]){
					continue;
				}
				edge=origEdges[i];
				ICoord leftPoint = (criterion == Vertex) ? edge.leftVertex : edge.leftSites;
			    ICoord	rightPoint= (criterion == Vertex) ? edge.rightVertex : edge.rightSites;
				if (leftPoint == Vertex.VERTEX_AT_INFINITY || rightPoint == Vertex.VERTEX_AT_INFINITY)
				{
					return new List<Edge>();
				}
				if (leftPoint == lastPoint)
				{
					lastPoint = rightPoint;
					_edgeOrientations.Add(LR.LEFT);
					newEdges.Add(edge);
					done[i] = true;
				}
				else if (rightPoint == firstPoint)
				{
					firstPoint = leftPoint;
					_edgeOrientations.Insert(0,LR.LEFT);
					newEdges.Insert(0,edge);
					done[i] = true;
				}
				else if (leftPoint == firstPoint)
				{
					firstPoint = rightPoint;
					_edgeOrientations.Insert(0,LR.RIGHT);
					newEdges.Insert(0,edge);
					done[i] = true;
				}
				else if (rightPoint == lastPoint)
				{
					lastPoint = leftPoint;
					_edgeOrientations.Add(LR.RIGHT);
					newEdges.Add(edge);
					done[i] = true;
				}
				if (done[i])
				{
					++nDone;
				}
			}
		}
		return newEdges;
	}






























}
