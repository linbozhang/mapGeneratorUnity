using UnityEngine;
using System.Collections;

public static class ExtensionMethod {

	public static void  perlinNoise( this Texture2D texture,int seed){
		Color [] pix=new Color[texture.width*texture.height];
		int y = 0;
		int xOrg=0;
		int yOrg=0;
		int scale=1;
		while (y < texture.height) {
			int x = 0;
			while (x < texture.width) {
				float xCoord = seed + (float)x / texture.width * scale;
				float yCoord = seed + (float)y / texture.height * scale;
				float sample =Mathf.Lerp( 0.46f,0.54f, Mathf.PerlinNoise(xCoord, yCoord));
				pix[y * texture.width + x] = new Color(sample, sample, sample);
				x++;
			}
			y++;
		}
		texture.SetPixels(pix);
		texture.Apply();
	}
}
