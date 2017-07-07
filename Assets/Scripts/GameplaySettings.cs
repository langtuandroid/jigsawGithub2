using System;
using UnityEngine;
using UnityEngine.UI;

namespace UnityStandardAssets._2D
{
    public class GameplaySettings
    {
		public GameplaySettings ShallowCopy()
		{
			return (GameplaySettings) this.MemberwiseClone();
		}
    }
}
