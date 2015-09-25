using UnityEngine;
using System.Collections;

public class PM_PRNG  {

	public uint seed;
	public PM_PRNG(){
		seed=1;
	}
	public uint nextInt(){
		return gen();
	}
	public float nextDouble(){
		return (gen()/int.MaxValue);
	}
	private uint gen(){
		return seed=(seed*16807)% int.MaxValue;
	}
	public float nextDoubleRange(float min,float max){
		return min+((max-min)*nextDouble());
	}
	public uint nextIntRange(float min,float max){
		min-=.4999;
		max+=.4999;
		return Mathf.Round(min+((max-min)*nextDouble()));
	}

}
