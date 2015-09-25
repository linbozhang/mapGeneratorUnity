using UnityEngine;
using System.Collections;

public class mapGenerator : MonoBehaviour {
	
	public int pixWidth;
	public int pixHeight;
	public float xOrg;
	public float yOrg;
	public float scale = 1.0F;
	private Texture2D noiseTex;
	private Color[] pix;
	public Renderer rend;

	public string islandType="Perlin";
	public string islandSeedInitial="85882-8";

	public string pointType="Relaxed";
	public int numPoints=2000;

	public string mapMode="smooth";
	public Map map;

	void Start() {
		//rend = GetComponent<Renderer>();
		noiseTex = new Texture2D(pixWidth, pixHeight);
		pix = new Color[noiseTex.width * noiseTex.height];
		rend.material.mainTexture = noiseTex;
		CalcNoise();
		map=new Map(pixWidth);
		go (islandType,pointType,numPoints);
	}
	void CalcNoise() {
		int y = 0;
		while (y < noiseTex.height) {
			int x = 0;
			while (x < noiseTex.width) {
				float xCoord = xOrg + (float)x / noiseTex.width * scale;
				float yCoord = yOrg + (float)y / noiseTex.height * scale;
				float sample =Mathf.Lerp( 0.46f,0.54f, Mathf.PerlinNoise(xCoord, yCoord));
				pix[y * noiseTex.width + x] = new Color(sample, sample, sample);
				x++;
			}
			y++;
		}
		noiseTex.SetPixels(pix);
		noiseTex.Apply();



	}
	void go(string newIslandType,string newPointType,int newNumPoints){
		newIsland(newIslandType,newPointType,newNumPoints);

	}
	void newIsland(string newIslandType,string newPointType,int newNumPoints){
		int seed =0;int variant=0;
		float t=Time.timeSinceLevelLoad;
		string match="8888-1";
		if(seed==0){
			seed=match.Split('-')[0];
			variant=match.Split('-')[1];
		}
		islandType=newIslandType;
		pointType=newPointType;
		numPoints=newNumPoints;
		map.newIsland(islandType,pointType,numPoints,seed,variant);

	}


	void Update() {
		//CalcNoise();
	}
}

