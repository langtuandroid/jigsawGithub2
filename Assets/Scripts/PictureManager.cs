using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
	public delegate void CallBack();

	public class PictureManager : MonoBehaviour
    {
		private Texture2D[] textureList;
		private Dictionary<string, Texture2D> textureDictionary = new Dictionary<string, Texture2D>();

		// Use this for initialization
		private void Start()
		{
		}


		public void LoadAllSpecifiedTextures (string[] textureNames, string pictureSubDirectory, CallBack finishedCallback)
		{
			StartCoroutine(LoadTextures(textureNames, pictureSubDirectory, finishedCallback));
		}


		IEnumerator LoadTextures(string[] fileNames, string pictureSubDirectory, CallBack finishedCallback)
		{
			textureList = new Texture2D[fileNames.Length];

			int index = 0;
			foreach (string name in fileNames)
			{
				if (textureDictionary.ContainsKey(name))
				{
					// we've already loaded that texture. Use that instance.
					textureList[index++] = textureDictionary[name];
				}
				else
				{
					Texture2D texTmp = new Texture2D(4, 4, TextureFormat.PVRTC_RGB4, false); 
					string last4chars = 4 > name.Length ? name : name.Substring(name.Length - 4);
					if ((last4chars.ToUpper() != ".MOV") && (last4chars.ToUpper() != ".M4V"))
					{
						string streamingAssetsPath = Application.streamingAssetsPath;

						string fullPathName = "file://"+streamingAssetsPath;
						if (pictureSubDirectory != "")
							fullPathName = fullPathName+"/"+pictureSubDirectory;
						fullPathName = fullPathName+"/"+name;

						WWW www = new WWW(fullPathName);

						yield return www;

						www.LoadImageIntoTexture(texTmp);
					}

					textureList[index++] = texTmp;
					textureDictionary[name] = texTmp;
				}
			}

			yield return new WaitForSeconds(0.01f);	// avoids immediate callback in the case that there are no textures to load.

			// notify the game that we are finished loading textures
			finishedCallback();
		}

		public Texture2D GetTexture(int index)
		{
			Debug.Log ("number of textures = "+textureList.Length);
			Debug.Log ("textures = "+textureList[index]);

			return textureList[index];
		}

		public Texture2D GetTextureByName(string name)
		{
			return textureDictionary[name];
		}

		public int GetNumberOfTextures()
		{
			return textureList.Length;
		}
    }
}
