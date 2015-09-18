using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
public class HalfedgePriorityQueue {
	List<Halfedge> _hash;
	int _count;
	int _minBucket;
	int _hashsize;
	float _ymin;
	float _deltay;
	public HalfedgePriorityQueue(float ymin ,float deltay,int sqrt_nsites){
		_ymin=ymin;
		_deltay=deltay;
		_hashsize=4*sqrt_nsites;
		initialize();
	}
	public void dispose(){
		for(int i=0;i<_hashsize;i++){
			_hash[i].dispose ();
			_hash[i]=null;
		}
		_hash=null;
	}
	private void initialize(){
		int i;
		_count=0;
		_minBucket=0;
		_hash=new List<Halfedge>(_hashsize);
		for(i=0;i<_hashsize;i++){
			_hash[i]=Halfedge.createDummy();
			_hash[i].nextInPriorityQueue=null;
		}
	}
	public void insert(Halfedge halfEdge){
		Halfedge previous,next;
		int insertionBucket=bucket(halfEdge);
		if (insertionBucket < _minBucket)
		{
			_minBucket = insertionBucket;
		}
		previous = _hash[insertionBucket];
		while ((next = previous.nextInPriorityQueue) != null
		       &&     (halfEdge.ystar  > next.ystar || (halfEdge.ystar == next.ystar && halfEdge.vertex.x > next.vertex.x)))
		{
			previous = next;
		}
		halfEdge.nextInPriorityQueue = previous.nextInPriorityQueue; 
		previous.nextInPriorityQueue = halfEdge;
		++_count;
	}
	public void remove(Halfedge halfEdge){
		Halfedge previous;
		int removalBucket=bucket(halfEdge);
		if(halfEdge.vertex!=null){
			previous = _hash[removalBucket];
			while (previous.nextInPriorityQueue != halfEdge)
			{
				previous = previous.nextInPriorityQueue;
			}
			previous.nextInPriorityQueue = halfEdge.nextInPriorityQueue;
			_count--;
			halfEdge.vertex = null;
			halfEdge.nextInPriorityQueue = null;
			halfEdge.dispose();
		}
	}
	private int bucket(Halfedge halfEdge){
		int theBucket= (halfEdge.ystar - _ymin)/_deltay * _hashsize;
		if (theBucket < 0) theBucket = 0;
		if (theBucket >= _hashsize) theBucket = _hashsize - 1;
		return theBucket;
	}
	private bool isEmpty(int bucket){
		return (_hash[bucket].nextInPriorityQueue == null);
	}
	private void adjustMinBucket(){
		while (_minBucket < _hashsize - 1 && isEmpty(_minBucket))
		{
			++_minBucket;
		}
	}
	public bool empty(){
		return _count==0;
	}
	public Vector2 min(){
		adjustMinBucket ();
		Halfedge answer =_hash[_minBucket].nextInPriorityQueue;
		return new Vector2(answer.vecter.x,answer.ystar);
	}
	public Halfedge extractMin(){
		Halfedge answer;
		answer=_hash[_minBucket].nextInPriorityQueue;
		_hash[_minBucket].nextInPriorityQueue = answer.nextInPriorityQueue;
		_count--;
		answer.nextInPriorityQueue = null;
		
		return answer;
	}

































}
