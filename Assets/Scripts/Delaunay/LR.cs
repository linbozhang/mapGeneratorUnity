using UnityEngine;
using System.Collections;
using System;
public class LR  {

	public static readonly LR LEFT=new LR(typeof(PrivateConstrutorEnforcer),"left");
	public static readonly LR RIGHT=new LR(typeof(PrivateConstrutorEnforcer),"right");
	private string _name;
	public LR(Type _lock,string name){
		if(_lock!=typeof(PrivateConstrutorEnforcer)){
			Debug.LogError("illeagal constructor access");
		}
		_name=name;
	}
	public static LR other(LR leftRight){
		return leftRight==LEFT?RIGHT:LEFT;
	}
	public string toString(){
		return _name;
	}
}
