using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class Halfedge  {

	private static List<Halfedge> _pool =new List<Halfedge>();
	public static Halfedge create(Edge edge,LR lr){
		if(_pool.Count>0){
			Halfedge ret=_pool[_pool.Count-1].init(edge,lr);
			_pool.RemoveAt(_pool.Count-1);
			return ret;
		}
		else{
			return new Halfedge (typeof(PrivateConstrutorEnforcer),edge,lr);
		}
	}
	public static Halfedge createDummy(){
		return create (null,null);
	}
	public Halfedge edgeListLeftNeighbor,edgeListRightNeighbor;
	public Halfedge nextInPriorityQueue;
	public Edge edge;
	public LR leftRight;
	public Vertex vertex;
	public float ystar;
	public Halfedge(Type _lock,Edge edge=null,LR lr=null){
		if(_lock!=typeof(PrivateConstrutorEnforcer)){
			Debug.LogError("halfedge constructor is private");
		}
		init(edge,lr);
	}
	private Halfedge init(Edge edge,LR lr){
		this.edge=edge;
		leftRight=lr;
		nextInPriorityQueue=null;
		vertex=null;
		return this;
	}
	public void dispose(){
		return "Halfedge (leftRight: " + leftRight + "; vertex: " + vertex + ")";
	}
	public void dispose(){
		if(edgeListLeftNeighbor|| edgeListRightNeighbor){
			return;
		}
		if(nextInPriorityQueue){
			return;
		}
		edge=null;
		leftRight=null;
		vertex=null;
		_pool.RemoveAt(_pool.Count-1);
	}
	public void reallyDispose(){
		edgeListLeftNeighbor = null;
		edgeListRightNeighbor = null;
		nextInPriorityQueue = null;
		edge = null;
		leftRight = null;
		vertex = null;
		_pool.RemoveAt(_pool.Count-1);
	}
	public bool isLeftOf(Vector2 p){
		Site topSite;
		bool rightOfSite;bool above,fast;
		float dxp,dyp,dxs,t1,t2,t3,y1;
		topSite=edge.rightSite;
		rightOfSite=p.x>topSite.x;
		if(rightOfSite && this.leftRight==LR.LEFT){
			return true;
		}
		if(!rightOfSite && this.leftRight==LR.RIGHT){
			return false;
		}
		if(edge.a==1.0){
			dyp=p.y-topSite.y;
			dxp=p.x-topSite.x;
			fast=false;
			if((!rightOfSite && edge.b<0.0f)||(rightOfSite && edge.b>=0.0f)){
				above=dyp>edge.b*dxp;
				fast=above;
			}
			else{
				above = p.x + p.y * edge.b > edge.c;
				if (edge.b < 0.0)
				{
					above = !above;
				}
				if (!above)
				{
					fast = true;
				}
			}
			if (!fast)
			{
				dxs = topSite.x - edge.leftSite.x;
				above = edge.b * (dxp * dxp - dyp * dyp) <
					dxs * dyp * (1.0 + 2.0 * dxp/dxs + edge.b * edge.b);
				if (edge.b < 0.0)
				{
					above = !above;
				}
			}

		}
		else{
			y1=edge.c-edge.a*p.x;
			t1=p.y-y1;
			t2=p.x-topSite.x;
			t3=y1-topSite.y;
			above=t1*t1>t2*t2+t3*t3;
		}
		return this.leftRight==LR.LEFT? above:!above;

	}






















}
