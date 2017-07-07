//using UnityEngine;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.UI;
//
//namespace UnityStandardAssets._2D
//{
//	public class IndicatorRing : MonoBehaviour 
//	{
//		[SerializeField] private Material material;
//		[SerializeField] private float fillTime = 0.75f;
//
//		private AudioManager _audioManager;
//		private Mesh myMesh;
//		Renderer myRenderer;
//		CanvasRenderer myCanvasRenderer;
//		const float textureUVMultiple = 100f;	// the bigger this is, the sharper the leading/trailing edge of the bar.
//		bool active;
//		bool visible;
//		float position = 0f;
//
//		// Use this for initialization
//		protected virtual void Start () 
//		{		
//			myCanvasRenderer = gameObject.GetComponent<CanvasRenderer>();
//			BuildCanvasRenderer();
//			active = true;
//			visible = true;
//			_audioManager = AudioManager.GetAudioManager();
//		}
//		
//
//		// Update is called once per frame
//		void Update () 
//		{
//			if (visible)
//			{
//				if (active)
//				{
//					if (position == 0f)
//						if (_audioManager)
//							_audioManager.PlayAudioClip("quizType7RingBar");
//					position += Time.deltaTime/fillTime;
//					if (position > 1f)
//						position = 1f;
//				}
//
//				SetFillPercentage(position);
//			}
//			else
//			{
//				SetFillPercentage(0f);
//			}
//		}
//
//		public void Hide()
//		{
//			myCanvasRenderer.GetMaterial().mainTextureOffset = Vector2.zero;
//			visible = false;
//			position = 0f;
//		}
//		
//		public void Show()
//		{
//			visible = true;
//		}
//		
//		public void SetFillPercentage(float p)
//		{
//			// p == 0, empty
//			// p == 1, full
//
//			myCanvasRenderer.GetMaterial().mainTextureOffset = new Vector2(-textureUVMultiple*p + 0.75f,0);
//		}
//
//		public bool IsFull()
//		{
//			return (position == 1f);
//		}
//
//		private void BuildCanvasRenderer()
//		{
//			Mesh mesh = BuildRingMesh ();
//
//			Vector3[] vertices = mesh.vertices;
//			int[] triangles = mesh.triangles;
//			Vector3[] normals = mesh.normals;
//			
//			Vector2[] UVs = mesh.uv;
//			
//			List<UIVertex> uiVertices = new List<UIVertex>();
//			
//			for (int i = 0; i < triangles.Length; ++i){
//				UIVertex temp = new UIVertex();
//				temp.position = vertices[triangles[i]];
//				Vector3 p = vertices[triangles[i]];
//
//				temp.uv0 = UVs[triangles[i]];  
//				temp.normal = normals[triangles[i]];
//				uiVertices.Add (temp);
//				if (i%3 == 0)
//					uiVertices.Add (temp);
//			}
//			
//			myCanvasRenderer.Clear ();      
//			myCanvasRenderer.SetMaterial(material, null);
//			myCanvasRenderer.SetVertices(uiVertices);
//			myCanvasRenderer.SetColor(new Color(1f,1f,1f,0.4f));
//			SetFillPercentage(0f);
//		}
//		
//		private Mesh BuildRingMesh()
//		{
//			const int RING_SEGMENTS = 15;
//			const float RING_RADIUS = 0.6f*100;
//			const float RING_WIDTH = 0.1f*100;
//			const float RING_Z_OFFSET = 1.0f;
//			int nodeIndex = 0;
//			int segmentIndex = 0;
//			
//			// starting indices
//			int nNode = nodeIndex*2;
//			int nTri = segmentIndex*6;
//			int numVertices = (RING_SEGMENTS+1)*2;
//			
//			Vector3[] vertices = new Vector3[numVertices];
//			Vector2[] uv = new Vector2[numVertices];
//			int[] tri = new int[RING_SEGMENTS*6];
//			
//			for (int i=0; i <= RING_SEGMENTS; i++)
//			{
//				// i = vertex number
//				float a = ((float)i)/RING_SEGMENTS;
//				Vector3 rayDirection = Quaternion.Euler(0f, 0.0f, (360f / RING_SEGMENTS) * i) * Vector3.up;
//				
//				vertices[nNode+0] = (rayDirection * (RING_RADIUS+RING_WIDTH)) + new Vector3(0.0f, 0.0f, RING_Z_OFFSET);
//				vertices[nNode+1] = (rayDirection * RING_RADIUS) + new Vector3(0.0f, 0.0f, RING_Z_OFFSET);
//				
//				uv[nNode+0] = new Vector2((1f-a)*textureUVMultiple, 0);
//				uv[nNode+1] = new Vector2((1f-a)*textureUVMultiple, 1);
//				
//				if (i < RING_SEGMENTS)
//				{
//					// 1 set of tris for 2 vertices, 2 for 3, 3 for 4 etc.
//					tri[nTri+0] = nNode+0;
//					tri[nTri+2] = nNode+2;
//					tri[nTri+1] = nNode+1;
//					
//					tri[nTri+3] = nNode+2;
//					tri[nTri+5] = nNode+3;
//					tri[nTri+4] = nNode+1;
//					
//					nTri += 6;
//				}
//				
//				nNode += 2;
//			}
//			
//			Mesh mesh = new Mesh();
//			mesh.Clear();
//			mesh.vertices = vertices;
//			mesh.uv = uv;
//			mesh.triangles = tri;
//			mesh.RecalculateBounds();
//			mesh.RecalculateNormals();
//			
//			return mesh;
//		}	
//
//		public void Flash (int flashCount, float onTime, float offTime, bool finishVisible)
//		{
//			StartCoroutine(FlashCoroutine(flashCount, onTime, offTime, finishVisible));
//		}
//		
//		IEnumerator FlashCoroutine (int flashCount, float onTime, float offTime, bool finishVisible)
//		{
//			for (int i=0; i<flashCount; i++)
//			{
//				visible = true;
//				yield return new WaitForSeconds(onTime);
//
//				visible = false;
//				yield return new WaitForSeconds(offTime);
//			}
//			
//			if (finishVisible)
//				SetFillPercentage(1f);
//		}
//		
//	}
//}
