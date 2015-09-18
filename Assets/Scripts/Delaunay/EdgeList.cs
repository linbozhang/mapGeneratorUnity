using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class EdgeList :IDisposable {
	private float _deltax;
	private float _xmin;
	private int _hashsize;
	private List<Halfedge> _hash;
	private Halfedge _leftEnd;
	public Halfedge leftEnd{
		get{return _leftEnd;}
	}
	private Halfedge _rightEnd;
	public Halfedge rightEnd{
		get{return _rightEnd;}
	}
	public void dispose(){
		Halfedge halfEdge=_leftEnd;
		Halfedge prevHe;
		while (halfEdge != _rightEnd)
		{
			prevHe = halfEdge;
			halfEdge = halfEdge.edgeListRightNeighbor;
			prevHe.dispose();
		}
		_leftEnd = null;
		_rightEnd.dispose();
		_rightEnd = null;
		
		int i;
		for (i = 0; i < _hashsize; ++i)
		{
			_hash[i] = null;
		}
		_hash = null;
	}
	public EdgeList(float xmin,float deltax,int sqrt_nsites){
		_xmin=xmin;
		_deltax=deltax;
		_hashsize=2*sqrt_nsites;
		int i;
		_hash=new List<Halfedge>();
		_leftEnd = Halfedge.createDummy();
		_rightEnd = Halfedge.createDummy();
		_leftEnd.edgeListLeftNeighbor = null;
		_leftEnd.edgeListRightNeighbor = _rightEnd;
		_rightEnd.edgeListLeftNeighbor = _leftEnd;
		_rightEnd.edgeListRightNeighbor = null;
		_hash[0] = _leftEnd;
		_hash[_hashsize - 1] = _rightEnd;

	}
	public void insert(Halfedge lb,Halfedge newHalfedge){
		newHalfedge.edgeListLeftNeighbor=lb;
		newHalfedge.edgeListRightNeighbor=lb.edgeListRightNeighbor;
		lb.edgeListRightNeighbor.edgeListLeftNeighbor=newHalfedge;
		lb.edgeListRightNeighbor=newHalfedge;
	}
	public void remove(Halfedge halfEdge){
		halfEdge.edgeListLeftNeighbor.edgeListRightNeighbor = halfEdge.edgeListRightNeighbor;
		halfEdge.edgeListRightNeighbor.edgeListLeftNeighbor = halfEdge.edgeListLeftNeighbor;
		halfEdge.edge = Edge.DELETED;
		halfEdge.edgeListLeftNeighbor = halfEdge.edgeListRightNeighbor = null;
	
	}
	public Halfedge edgelistLeftNeighbor(Vector2 p){
		int i,bucket;
		Halfedge halfEdge;
		bucket=(p.x-_xmin)/_deltax*_hashsize;
		if (bucket < 0)
		{
			bucket = 0;
		}
		if (bucket >= _hashsize)
		{
			bucket = _hashsize - 1;
		}
		halfEdge = getHash(bucket);
		if (halfEdge == null)
		{
			for (i = 1; true ; ++i)
			{
				if ((halfEdge = getHash(bucket - i)) != null) break;
				if ((halfEdge = getHash(bucket + i)) != null) break;
			}
		}
		/* Now search linear list of halfedges for the correct one */
		if (halfEdge == leftEnd  || (halfEdge != rightEnd && halfEdge.isLeftOf(p)))
		{
			do
			{
				halfEdge = halfEdge.edgeListRightNeighbor;
			}
			while (halfEdge != rightEnd && halfEdge.isLeftOf(p));
			halfEdge = halfEdge.edgeListLeftNeighbor;
		}
		else
		{
			do
			{
				halfEdge = halfEdge.edgeListLeftNeighbor;
			}
			while (halfEdge != leftEnd && !halfEdge.isLeftOf(p));
		}
		
		/* Update hash table and reference counts */
		if (bucket > 0 && bucket <_hashsize - 1)
		{
			_hash[bucket] = halfEdge;
		}
		return halfEdge;
	}
	private Halfedge getHash(int b){
		Halfedge halfEdge;
		if (b < 0 || b >= _hashsize)
		{
			return null;
		}
		halfEdge = _hash[b]; 
		if (halfEdge != null && halfEdge.edge == Edge.DELETED)
		{
			/* Hash table points to deleted halfedge.  Patch as necessary. */
			_hash[b] = null;
			// still can't dispose halfEdge yet!
			return null;
		}
		else
		{
			return halfEdge;
		}
	}




































}
