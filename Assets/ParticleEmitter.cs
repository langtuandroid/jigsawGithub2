using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace UnityStandardAssets._2D
{
	public class ParticleEmitter : MonoBehaviour {
		
		[SerializeField] public GameObject[] particlePrefabs;
		[SerializeField] public float emissionRate;
		[SerializeField] public float emissionRadius;
		[SerializeField] public float particleSpeed;
		[SerializeField] public Transform particleParent = null;

		private GameObject[] particles = null;
		private AudioManager _audioManager;
		float timer = 0f;

		// Use this for initialization
		void Start () {
			_audioManager = AudioManager.GetAudioManager();
			timer = 0f;
		}
		
		
//		public void StarBurst(Vector2 pos, float particleSpeed)
//		{
//			if (_audioManager)
//				_audioManager.PlayAudioClip("fireworkBurst");
//			
//			GameObject burstGameObject = Instantiate(burstPrefabs[0]) as GameObject;
//			burstGameObject.transform.localPosition = pos;
//			burstGameObject.transform.SetParent(transform);
//			
//			float radius = 70f;
//			float speed = particleSpeed;	//1500f;	//500f;
//			float gravity = -1550f;
//			int numParticles = 12;
//			
//			for(int i=0; i<numParticles; i++)
//			{
//				float jSpeed = speed * UnityEngine.Random.Range(0.8f,1.2f);
//				float r = i * (Mathf.PI*2 / numParticles);
//				Vector2 pPos = new Vector2(pos.x + Mathf.Sin(r) * radius, pos.y + Mathf.Cos(r) * radius);
//				Vector2 pVel = new Vector2(Mathf.Sin(r) * jSpeed, Mathf.Cos(r) * jSpeed);
//				Vector2 pAcc = new Vector2(0f, gravity);
//				
//				GameObject particleGameObject = Instantiate(particlePrefabs[0]) as GameObject;
//				FireworkParticle particle = particleGameObject.GetComponent<FireworkParticle>();
//				particle.transform.localPosition = pPos;
//				particle.transform.SetParent(transform);
//				float lifeSpan = 0.5f + UnityEngine.Random.Range(0f,0.5f);
//				particle.SetMotion(pVel,pAcc,lifeSpan);
//			}
//		}
//		
//		//		public void ManyStarBurstsAtPosition(int number, Rect area)
//		//		{
//		//			ManyStarBursts(number, area);
//		//		}
//		
//		public void ManyStarBursts(int number, Rect? area = null, float particleSpeed = 500f, float frequency = 0.25f)
//		{
//			Rect realArea = area ?? new Rect(100f, 500f, 700f, 500f);
//			
//			StartCoroutine(SequenceOfStarbursts(number, realArea, particleSpeed, frequency));
//		}
//		
//		IEnumerator SequenceOfStarbursts(int number, Rect area, float particleSpeed, float frequency) 
//		{
//			Debug.Log ("SequenceOfStarbursts area min x "+area.min.x+", max x "+area.max.x+", min y "+area.min.y+", max y "+area.max.y);
//			for (int i=0; i<number; i++)
//			{
//				Vector2 pos = new Vector2(UnityEngine.Random.Range(area.min.x,area.max.x), UnityEngine.Random.Range(area.min.y,area.max.y));
//				StarBurst (pos, particleSpeed);
//				yield return new WaitForSeconds(UnityEngine.Random.Range(0f,frequency));
//			}
//		}
		
		// Update is called once per frame
		void Update () 
		{
			timer += Time.deltaTime;
			while (timer >= emissionRate)
			{
				timer -= emissionRate;
				SpawnParticle();
			}
		}

		private void SpawnParticle()
		{
//			if (_audioManager)
//				_audioManager.PlayAudioClip("fireworkBurst");
//			
//			GameObject burstGameObject = Instantiate(burstPrefabs[0]) as GameObject;
//			burstGameObject.transform.localPosition = pos;
//			burstGameObject.transform.SetParent(transform);
			
			float radius = emissionRadius;
			float speed = particleSpeed;
			float gravity = -1550f;
			int numParticles = 1;
			Vector2 pos = Vector2.zero;
			
			for(int i=0; i<numParticles; i++)
			{
				float jSpeed = speed * UnityEngine.Random.Range(0.5f,1.5f);
				float r = UnityEngine.Random.value * Mathf.PI*2;
				Vector2 pPos = new Vector2(pos.x + Mathf.Sin(r) * radius, pos.y + Mathf.Cos(r) * radius);
				Vector2 pVel = new Vector2(Mathf.Sin(r) * jSpeed, Mathf.Cos(r) * jSpeed);
				Vector2 pAcc = new Vector2(0f, gravity);
				
				GameObject particleGameObject = Instantiate(particlePrefabs[0]) as GameObject;
				FireworkParticle particle = particleGameObject.GetComponent<FireworkParticle>();
				particle.transform.localPosition = pPos;
				if (particleParent == null)
					particle.transform.SetParent(transform, false);
				else
				{
					Vector3 pPos3 = pPos;
					pPos3 = pPos3 + transform.position;
					particle.transform.position = pPos3;
					particle.transform.SetParent(particleParent, true);
				}
				float lifeSpan = 0.2f + UnityEngine.Random.Range(0f,0.2f);
				particle.SetMotion(pVel,pAcc,lifeSpan);
			}		
		}
	}
}
