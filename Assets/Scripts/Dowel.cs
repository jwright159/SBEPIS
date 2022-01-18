using System;
using UnityEngine;
using WrightWay.SBEPIS.Util;

namespace WrightWay.SBEPIS
{
	public class Dowel : MonoBehaviour
	{
		[SerializeField]
		private new SkinnedMeshRenderer renderer;
		[SerializeField]
		private Itemkind defaultItem;

		public long captchaHash { get; set; }

		private void Start()
		{
			if (defaultItem)
			{
				captchaHash = defaultItem.captchaHash;
				for (int i = 0; i < 8; i++)
					SetDepth(i, CaptchaUtil.GetCaptchaPercent(captchaHash, i));
			}
		}

		public void SetDepth(int i, float fraction)
		{
			renderer.SetBlendShapeWeight(renderer.sharedMesh.GetBlendShapeIndex("Carve " + (i + 1)), fraction * 100f);
		}

		public float GetDepth(int i)
		{
			return renderer.GetBlendShapeWeight(renderer.sharedMesh.GetBlendShapeIndex("Carve " + (i + 1))) / 100f;
		}

		public void SetEdgeDistance(int i, bool rightEdge, float fraction)
		{
			fraction = fraction.Map(0, 1, -1, 1);
			renderer.SetBlendShapeWeight(renderer.sharedMesh.GetBlendShapeIndex("Edge " + (rightEdge ? i + 1 : i)), (rightEdge ? fraction : 1 - fraction) * 100f);
		}

		public float GetEdgeDistance(int i, bool rightEdge)
		{
			float weight = renderer.GetBlendShapeWeight(renderer.sharedMesh.GetBlendShapeIndex("Edge " + (rightEdge ? i + 1 : i))) / 100f;
			return (rightEdge ? weight : 1 - weight).Map(-1, 1, 0, 1);
		}

		public void SetEdgeDepth(int i, bool rightEdge, float fraction)
		{
			renderer.SetBlendShapeWeight(renderer.sharedMesh.GetBlendShapeIndex("Face " + (rightEdge ? i + 1 : i)), fraction * 100f);
		}

		public float GetEdgeDepth(int i, bool rightEdge)
		{
			return renderer.GetBlendShapeWeight(renderer.sharedMesh.GetBlendShapeIndex("Face " + (rightEdge ? i + 1 : i))) / 100f;
		}
	}
}