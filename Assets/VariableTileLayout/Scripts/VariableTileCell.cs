using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.UI.Extensions
{
	public class VariableTileCell : MonoBehaviour
	{
		[SerializeField] private Graphic m_Graphic;

		public float Width
		{
			get { return m_Graphic.mainTexture.width; }
		}

		public float Height
		{
			get { return m_Graphic.mainTexture.height; }
		}
	}
}

