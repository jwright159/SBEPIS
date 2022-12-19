using UnityEngine;
using UnityEditor;
using SBEPIS.Items;
using System.Linq;
using SBEPIS.Bits.Tags;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using System;
using SBEPIS.Utils;

namespace SBEPIS.Bits
{
	[CustomPropertyDrawer(typeof(TaggedBitSetFactory))]
	public class MemberedBitSetFactoryDrawer : PropertyDrawer
	{
		public override VisualElement CreatePropertyGUI(SerializedProperty property)
		{
			TaggedBitSetFactory bitsFactory = (TaggedBitSetFactory)property.boxedValue;

			Foldout foldout = new()
			{
				text = property.displayName,
				bindingPath = property.propertyPath,
			};

			foldout.Add(new PropertyField(property.FindPropertyRelative("bits")));

			foldout.Add(new ObjectPopupField()
			{
				label = "Base Item",
				bindingPath = property.FindPropertyRelative("itemBase").propertyPath,
				choices = ItemBaseManager.instance.itemBases.Cast<UnityEngine.Object>().Prepend(null).ToList(),
			});
			
			foldout.Add(new PropertyField(property.FindPropertyRelative("material")));

			return foldout;
		}
	}
}
