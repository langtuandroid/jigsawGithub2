using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace UnityStandardAssets._2D
{
	public class EncouragementManager : MonoBehaviour {

		[SerializeField] public GameObject speedyMessagePrefab;
		[SerializeField] public GameObject superFastMessagePrefab;
		[SerializeField] public GameObject streakMessagePrefab;
		[SerializeField] public GameObject doubleDownMessagePrefab;
		[SerializeField] public GameObject perfectMessagePrefab;

		private bool playingEncouragement = false;

		class WaitingEncouragement
		{
			public EncouragementTypes type;
			public int parameter;
		}

		private Queue<WaitingEncouragement> queue = new Queue<WaitingEncouragement>();

		public enum EncouragementTypes
		{
			Speedy = 0,
			SuperFast,
			Streak,
			DoubleDown,
			Perfect
		}

		// Use this for initialization
		void Start () {
		
		}

		void FinishedEncouragement()
		{
			Debug.Log ("FinishedEncouragement. queuecount = "+queue.Count);
			playingEncouragement = false;


			if (queue.Count > 0)
			{
				Debug.Log ("PLAYING FROM QUEUE");
				WaitingEncouragement we = queue.Dequeue();
				PlayEncouragement(we.type, we.parameter);
			}
		}

		public void ShowEncouragement(EncouragementTypes type, int parameter = 0)
		{
			if (playingEncouragement)
			{
				Debug.Log ("Queing request. Length = "+(queue.Count+1));
				// queue up this new request
				WaitingEncouragement we = new WaitingEncouragement();
				we.type = type;
				we.parameter = parameter;
				queue.Enqueue(we);
				return;
			}

			PlayEncouragement(type, parameter);
		}

		public void PlayEncouragement(EncouragementTypes type, int parameter)
		{
			playingEncouragement = true;

			CallBack finishedEncouragementCallback = FinishedEncouragement;

			GameObject gameObject = null;
			switch(type)
			{
			case EncouragementTypes.Speedy:
				gameObject = Instantiate(speedyMessagePrefab) as GameObject;
				gameObject.GetComponent<EncourageMoveZoom>().SetCallback(finishedEncouragementCallback);
				gameObject.transform.SetParent(transform);
				gameObject.transform.localPosition = new Vector2(0f,0f);
				break;
			case EncouragementTypes.SuperFast:
				gameObject = Instantiate(superFastMessagePrefab) as GameObject;
				gameObject.GetComponent<EncourageMoveZoom>().SetCallback(finishedEncouragementCallback);
				gameObject.transform.SetParent(transform);
				gameObject.transform.localPosition = new Vector2(0f,0f);
				break;
			case EncouragementTypes.Streak:
				gameObject = Instantiate(streakMessagePrefab) as GameObject;
				gameObject.GetComponent<EncourageMoveStreak>().SetCallback(finishedEncouragementCallback);
				gameObject.GetComponent<EncourageMoveStreak>().SetCount(parameter);
				gameObject.transform.SetParent(transform);
				gameObject.transform.localPosition = new Vector2(0f,100f);
				gameObject.GetComponent<EncourageMoveStreak>().StartAnimation();
				break;
			case EncouragementTypes.DoubleDown:
				gameObject = Instantiate(doubleDownMessagePrefab) as GameObject;
				gameObject.GetComponent<EncourageMoveDoubleDown>().SetCallback(finishedEncouragementCallback);
				gameObject.transform.SetParent(transform);
				gameObject.transform.localPosition = new Vector2(0f,-350f);
				break;
			case EncouragementTypes.Perfect:
				gameObject = Instantiate(perfectMessagePrefab) as GameObject;
				gameObject.GetComponent<EncourageMoveZoom>().SetCallback(finishedEncouragementCallback);
				gameObject.transform.SetParent(transform);
				gameObject.transform.localPosition = new Vector2(0f,-350f);
				//gameObject.GetComponent<EncourageMoveZoom>().StartAnimation();
				break;

			}
		}

		// Update is called once per frame
		void Update () {
		
		}
	}
}
