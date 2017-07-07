using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
    public class MoveWithinRange : MonoBehaviour
    {
		[SerializeField] private Vector2 velocity;

		private Rect range;
		private bool moving;

		// Use this for initialization
		private void Start()
		{
			range = transform.GetComponentInParent<RectTransform>().rect;
			RectTransform rt = transform.parent.GetComponent<RectTransform>();
			range = rt.rect;

			moving = true;
		}

		private void Awake()
		{
			moving = false;
		}

		private void Update()
		{
			if (moving)
			{
				RectTransform rt = gameObject.GetComponent<RectTransform>();
				Vector2 pos = rt.localPosition;
				pos += velocity*Time.deltaTime;
				if (pos.x < range.xMin) 
					pos.x += (range.width);
				if (pos.y < range.yMin) 
					pos.y += (range.height);
				if (pos.x > range.xMax) 
					pos.x -= (range.width);
				if (pos.y > range.yMax) 
					pos.y -= (range.height);
				rt.localPosition = pos;
			}
		}
    }
}
