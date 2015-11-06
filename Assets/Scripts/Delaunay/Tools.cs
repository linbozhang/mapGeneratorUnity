using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
public class Tools {
	private static Tools instance;
	public static Tools Instance{
		get{
			if(instance==null){
				instance=new Tools();
			}
			return instance;
			}
	}

	public List<LineSegment> visibleLineSegments(List<Edge> edges){
		List<LineSegment> segments=new List<LineSegment>();
		foreach(Edge edge in edges){
			if(edge.visible){
				Vector2 p1=edge.clippedEnds[LR.LEFT];
				Vector2 p2=edge.clippedEnds[LR.RIGHT];
				segments.Add(new LineSegment(p1,p2));
			}
		}
		return segments;
	}

	public List<LineSegment> kruskal(List<LineSegment> lineSegments,string type="minimum"){
		Dictionary<Vector2 ,Node> nodes=new Dictionary<Vector2, Node>();
		List<LineSegment> mst=new List<LineSegment>();
		List<Node> nodePool=new List<Node>();
		switch(type){
		case "maximum":
			lineSegments.Sort(LineSegment.compareLengths);

			break;
		default:
			lineSegments.Sort(LineSegment.compareLengths_MAX);
			break;
		}
		for(int i=lineSegments.Count;i>-1;i--){
			LineSegment lineSegment=lineSegments[i];
			Node node0;
			nodes.TryGetValue(lineSegment.p0,out node0);
			Node rootOfSet0;
			if(node0==null){
				node0=nodePool.Count>0?nodePool[nodePool.Count-1]:new Node();
				nodePool.RemoveAt(nodePool.Count-1);
				rootOfSet0=node0.parent=node0;
				node0.treeSize=1;
				nodes[lineSegment.p0]=node0;
			}else{
				rootOfSet0=find(node0);
			}

			Node node1=nodes[lineSegment.p1];
			Node rootOfSet1;
			if(node1==null){
			node1=nodePool.Count>0?nodePool[nodePool.Count-1]:new Node();
				rootOfSet1=node1.parent=node1;
				node1.treeSize=1;
				nodes[lineSegment.p1]=node1;
			}

			if(rootOfSet0!=rootOfSet1){
				mst.Add(lineSegment);
				int treeSize0=rootOfSet0.treeSize;
				int treeSize1=rootOfSet1.treeSize;
				if(treeSize0>=treeSize1){
					rootOfSet1.parent=rootOfSet0;
					rootOfSet0.treeSize+=treeSize1;
				}else{
					rootOfSet0.parent=rootOfSet1;
					rootOfSet1.treeSize+=treeSize0;
				}
			}
		}
		foreach(Node node in nodes){
			nodePool.Add(node);
		}
		return mst;
	}
	Node find(Node node){
		if(node.parent==node){
			return node;
		}else{
			Node root =find (node.parent);
			node.parent=root;
			return root;
		}
	}


	public List<Edge> selectEdgesForSitePoint(Vector2 coord,List<Edge> edgesToTest){
		List<Edge> rtn;
		foreach(Edge edge in edgesToTest){
			if((edge.leftSite&&edge.leftSite.Coord==coord)||(edge.rightSite&&edge.rightSite.Coord==coord)){
				rtn.Add(edge);
			}
		}
		return rtn;
	}
	public List<Edge> selectNonIntersectingEdges(BitmapData keepOutMask,List<Edge> edgesToTest){
		if(keepOutMask==null){
			return edgesToTest;
		}
		Vector2 zeroPoint =new Vector2();
		List<Edge> rtn;
		foreach(Edge edge in edgesToTest){
			BitmapData delaunayLineBmp =edge.makeDelaunayLineBmp();
			bool notIntersecting = !(keepOutMask.hitTest(zeroPoint, 1, delaunayLineBmp, zeroPoint, 1));
			delaunayLineBmp.dispose();
			rtn.Add(notIntersecting);
		}
		return rtn;
	}

}
class Node{
	public static List<Node> pool=new List<Node>();
	public Node parent;
	public int treeSize;
	public Node(){};
}

