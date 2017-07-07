//using UnityEngine;
//using UnityEngine.UI;
//using System.Collections;
//using UnityEngine.EventSystems;
//
//namespace UnityStandardAssets._2D
//{
//	public class ScratchablePlane : MonoBehaviour {
//
//		enum ScratchablePlaneState
//		{
//			Normal = 0,
//			Revealing
//		}
//
//		[SerializeField] private RawImage obscuringImage;
//		[SerializeField] private AudioSource drawingSound;
//		[SerializeField] private GameObject coinPrefab;
//		[SerializeField] private GameObject sparksEmitter;
//		[SerializeField] private GameObject bonusSparksEmitter;
//
//		private Texture2D obscuringTexture;
//		private byte[] pixels;
//		private int texXDim;
//		private int texYDim;
//		private ScratchablePlaneState state;
//		private float revealPosition = 0f;
//		private float revealSpeed = 300.0f;
//		private quizType5QuizSettings gameplaySettings;
//		private quizType5QuizController quizType5Controller;
//		private AudioManager _audioManager;
//		private float touchPressure = 0.0f;
//		private Vector2 touchPosition;
//		private Vector2 imageSize;
//
//
//		void Awake()
//		{
//			texXDim = 100;
//			texYDim = 100;
//			//Vector2 texSize = new Vector2(100,100);
//			obscuringTexture = new Texture2D(texXDim, texYDim, TextureFormat.RGBA32, false);
//			pixels = new byte[texXDim * texYDim * 4];
//			
//			FillTexture();
//
//			obscuringImage.texture = obscuringTexture;
//			drawingSound.volume = 0f;
//
//			EnableSparksEmitters(false);
//		}
//
//		// Use this for initialization
//		void Start () 
//		{
//			_audioManager = AudioManager.GetAudioManager();
//		}
//
//		public void Init(quizType5QuizSettings settings, quizType5QuizController controller)
//		{
//			gameplaySettings = settings;
//			quizType5Controller = controller;
//		}
//
//		public void FillTexture()
//		{
//			for (int x=0; x<texXDim; x++)
//			{
//				for (int y=0; y<texYDim; y++)
//				{
////					// concentric circles
////					float fx = x - texXDim/2;
////					float fy = y - texYDim/2;
////					float a = Mathf.Sqrt (fx*fx + fy*fy);
////					byte c = (byte)(((int)(a * 20))%256);
//
////					// solid white
////					byte c = 255;
//
//					// chequerboard
//					int squareSize = 10;
//					int cx = x/squareSize;
//					int cy = y/squareSize;
//					int a = (cx & 1)^(cy & 1);
//					byte c = (byte)(255 - (a*32));
//
//					int i = (y*texXDim + x)*4;
//					pixels[i++] = c;
//					pixels[i++] = c;
//					pixels[i++] = c;
//					pixels[i++] = 255;
//				}
//			}
//			UpdateTexture();
//		}
//
//		public void TouchDown(BaseEventData data)
//		{
//			PointerEventData pointerData = data as PointerEventData;
//			touchPosition = pointerData.position;
//			touchPressure = 0f;
//			TouchDraw(touchPosition);
//			//drawingSound.volume = 0.5f;
//			PositionSparksEmitter(touchPosition);
//		}
//
//		// 3D touch sets touch pressure using this function
//		public void SetPressure(float pressure)
//		{
//			Debug.Log ("Set pressure "+pressure);
//			if (touchPressure != pressure)
//			{
//				touchPressure = pressure;
//				TouchDraw(touchPosition);
//			}
//		}
//
//		private void TouchDraw(Vector2 touchPosition)
//		{
//			if (quizType5Controller.GetCardBonus() > 0f)
//			{
//				Vector3[] worldCorners = new Vector3[4];
//				gameObject.GetComponent<RectTransform>().GetWorldCorners(worldCorners);
//				imageSize = new Vector2(worldCorners[3].x - worldCorners[0].x, worldCorners[1].y - worldCorners[0].y);
//				Vector2 relativeTouchPos = new Vector2(touchPosition.x - worldCorners[0].x, touchPosition.y - worldCorners[0].y);
//				Vector2 pTouchPos = relativeTouchPos;
//				pTouchPos.x /= imageSize.x;
//				pTouchPos.y /= imageSize.y;
//				
//				Vector2 texturePos = new Vector2(pTouchPos.x*texXDim, pTouchPos.y*texYDim);
//
//				float minRadius = gameplaySettings.eraserMinimumSize;
//				float maxRadius = gameplaySettings.eraserMaximumSize;
//				float minAlpha = gameplaySettings.eraserMinimumAlpha;
//				float maxAlpha = gameplaySettings.eraserMaximumAlpha;
//				float radius = minRadius + (maxRadius - minRadius)*touchPressure;
//				float alpha = minAlpha + (maxAlpha - minAlpha)*touchPressure;
//				if (alpha > 1.0f)
//					alpha = 1.0f;
//				radius = ((texXDim*radius)/100);		// adjust for resolution
//				int pixelCount = DrawCircle((int)texturePos.x, (int)texturePos.y, new Color32(255,0,0,0), (int)radius, alpha);
//				UpdateTexture();
//
//				float numPixelsCleared = (float)pixelCount/byte.MaxValue;
//				Debug.Log ("numPixelsCleared="+numPixelsCleared+" byte maxvalue = "+byte.MaxValue);
//
//				quizType5Controller.ReduceUnusedCardsBonus(numPixelsCleared/gameplaySettings.pixelsPerBonusPoint);
//				EnableSparksEmitters(numPixelsCleared > 0);
//			}
//			else
//			{
//				//EnableSparksEmitters(false);
//				drawingSound.volume = 0f;
//			}
//		}
//
//		public void TouchUp(BaseEventData data)
//		{
//			Debug.Log ("TouchUp = ");
//			drawingSound.volume = 0f;
//			//EnableSparksEmitters(false);
//		}
//
//		public void TouchMove(BaseEventData data)
//		{
//			PointerEventData pointerData = data as PointerEventData;
//			touchPosition = pointerData.position;
//			Debug.Log ("TouchMove = ");
//			TouchDraw(touchPosition);
//			PositionSparksEmitter(touchPosition);
//		}
//
//		private void EnableSparksEmitters(bool enable)
//		{
//			sparksEmitter.SetActive(enable);
//			bonusSparksEmitter.SetActive(enable);
//
//			if (enable)
//			{
//				StopCoroutine(DisableSparkEmittersAfterADelay());
//				StartCoroutine(DisableSparkEmittersAfterADelay());
//			}
//		}
//
//		IEnumerator DisableSparkEmittersAfterADelay() 
//		{
//			yield return new WaitForSeconds(0.1f);
//			EnableSparksEmitters(false);
//		}
//		
//		private void PositionSparksEmitter(Vector2 position)
//		{
//			sparksEmitter.transform.position = position;
//		}
//
//		// AG - I didn't write this. From the mobilepaint plugin
//		// main painting function, modified from http://stackoverflow.com/a/24453110
//		// returns the number of pixels cleared
//		public int DrawCircle(int x, int y, Color32 colour, int radius, float alpha)
//		{
//			int pixel = 0;
//			int pixelCount = 0;
//			const byte coinRevealThreshold = 128;
//			float coinProbability = gameplaySettings.coinProbability;
//
//			int brushSize = radius*2;
//			int radiusSquared = radius*radius;
//			int numPixels = brushSize*brushSize;
//			byte alphaReduction = (byte)(255 * alpha);
//
//			for (int i = 0; i < numPixels; i++)
//			{
//				int tx = (i % brushSize) - radius;
//				int ty = (i / brushSize) - radius;
//				
//				if (tx*tx+ty*ty > radiusSquared) continue;
//				if (x+tx<0 || y+ty<0 || x+tx>=texXDim || y+ty>=texYDim) continue;
//				
//				pixel = (texXDim*(y+ty)+x+tx)*4;
//
////				if (pixels[pixel+3] > 0)
////					pixelCount++;
//
//				byte b = pixels[pixel+3];
//				byte bBefore = b;
//				if (b > alphaReduction)
//					b -= alphaReduction;
//				else
//					b = 0;
//
//				pixelCount += (bBefore - b);
//
//				if ((bBefore >= coinRevealThreshold) && (b < coinRevealThreshold))
//					if (Random.value < coinProbability)
//						SpawnCoin(x+tx,y+ty);
//
//				pixels[pixel+3] = b;
//
////				pixels[pixel] = colour.r;
////				pixels[pixel+1] = colour.g;
////				pixels[pixel+2] = colour.b;
////				pixels[pixel+3] = colour.a;
//			}
//
//			if (pixelCount != 0)
//				StartDrawingSound();
//
//
//			return pixelCount;
//		}
//
//		void SpawnCoin(int x, int y)
//		{
//			Vector2 pTouchPos = new Vector2(((float)x)/texXDim, ((float)y)/texYDim);
//			Vector2 relPos = new Vector2(pTouchPos.x*imageSize.x, pTouchPos.y*imageSize.y);
//			relPos.y -= imageSize.y/2;
//			relPos.x -= imageSize.x/2;
//
//			GameObject newCoin = (GameObject)Instantiate(coinPrefab, relPos, Quaternion.identity);
//
//			newCoin.transform.SetParent(transform, false);
//
//			if (_audioManager)
//				_audioManager.PlayAudioClip("coinCollect");
//		}
//
//
//		private IEnumerator stopDrawingSoundCoroutine = null;
//
//		void StartDrawingSound()
//		{
//			drawingSound.volume = 0.5f;
//
//			if (stopDrawingSoundCoroutine != null)
//				StopCoroutine(stopDrawingSoundCoroutine);
//			stopDrawingSoundCoroutine = StopDrawingSoundAfterDelay();
//			StartCoroutine(stopDrawingSoundCoroutine);
//		}
//
//		IEnumerator StopDrawingSoundAfterDelay() 
//		{
//			yield return new WaitForSeconds(0.2f);
//			drawingSound.volume = 0.0f;
//		}
//
//
//		// Update is called once per frame
//		void Update () {
//			switch(state)
//			{
//			case ScratchablePlaneState.Revealing:
//				int maxExtent = texXDim + texYDim;
//				float revealTo = revealPosition + (Time.deltaTime*revealSpeed*((float)texXDim/100));
//				if (revealTo >= maxExtent)
//				{
//					revealTo = maxExtent;
//					state = ScratchablePlaneState.Normal;
//				}
//				FillRevealArea((int)revealPosition, (int)revealTo);
//				revealPosition = revealTo;
//				break;
//			}
//		}
//
//
//
//		void FillRevealArea(int from, int to)
//		{
//			for (int x=0; x<texXDim; x++)
//			{
//				for (int y=0; y<texYDim; y++)
//				{
//					if (x+y < to)
//					{
//						int pixel = (texXDim*(texYDim-1-y)+x)*4;
//	//					pixels[pixel] = colour.r;
//	//					pixels[pixel+1] = colour.g;
//	//					pixels[pixel+2] = colour.b;
//						pixels[pixel+3] = 0;
//					}
//				}
//			}
//
//			UpdateTexture();
//		}
//
//		private void UpdateTexture()
//		{
//			obscuringTexture.LoadRawTextureData (pixels);
//			obscuringTexture.Apply (false);
//		}
//
//		public void RevealAll()
//		{
//			state = ScratchablePlaneState.Revealing;
//			revealPosition = 0.0f;
//
//			//StartCoroutine(RevealAllSequence());
//		}
//
//	//	IEnumerator RevealAllSequence() 
//	//	{
//	//		yield return new WaitForSeconds(0.1f);
//	//		CreateRedCrosses();
//	//	}
//	}
//}