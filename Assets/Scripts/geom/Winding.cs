using UnityEngine;
using System.Collections;
using System;
public class Winding {

	public static readonly Winding CLOCKWISE =new Winding(typeof(PrivateConstrutorEnforcer),"clockwise" );
	public  static readonly Winding COUNTERCLOCKWISE=new Winding(typeof(PrivateConstrutorEnforcer),"counterclockwise");
	public static readonly Winding NONE=new Winding(typeof(PrivateConstrutorEnforcer),"none");
	private string _name;
	public Winding(Type _lock ,string name){
		if(_lock!=typeof(PrivateConstrutorEnforcer)){
			Debug.LogError("Invalid constructor access");
		}
		_name=name;
	}
	public string toString(){
		return _name;
	}

}
class PrivateConstrutorEnforcer{}
