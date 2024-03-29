using UnityEngine;

namespace SBEPIS.Controller
{
	public class Orienter : MonoBehaviour
	{
		public void Orient(Vector3 up)
		{
			transform.LookAt(transform.position + Vector3.ProjectOnPlane(transform.forward, up), up);
		}
	}
}
