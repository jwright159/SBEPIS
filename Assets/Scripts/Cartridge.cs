using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using WrightWay.SBEPIS.Modus;

namespace WrightWay.SBEPIS
{
	[RequireComponent(typeof(Rigidbody))]
	[RequireComponent(typeof(Item))]
	public class Cartridge : MonoBehaviour
	{
		[SerializeField]
		private TextMeshProUGUI modusText;
		[SerializeField]
		private TextMeshProUGUI modusName;
		[SerializeField]
		private Renderer[] renderers;
		[SerializeField]
		private Material colorMaterial;

		public new Rigidbody rigidbody { get; private set; }
		public Moduskind modus { get; private set; }

		private void Awake()
		{
			rigidbody = GetComponent<Rigidbody>();
			modus = (Moduskind) GetComponent<Item>().itemkind;
		}

		private void Start()
		{
			modusText.color = modus.mainColor;
			modusName.color = modus.mainColor;
			modusName.text = modus.itemName.ToLower();
			CaptchalogueCard.UpdateMaterials(0, modus.icon, modus.mainColor, renderers, null, colorMaterial, colorMaterial);
		}
	}
}