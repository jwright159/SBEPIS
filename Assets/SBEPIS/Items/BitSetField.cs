using SBEPIS.Bits.Bits;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

namespace SBEPIS.Bits
{
	public class BitSetField : BaseField<object>
	{
		private readonly TextField codeField;
		private readonly ListView bitList;

		public BitSet bits
		{
			get => (BitSet)(value ?? default(BitSet));
			set => this.value = value;
		}

		public BitSetField() : this(null) { }
		public BitSetField(string label) : base(label, null)
		{
			VisualElement container = new();
			container.style.width = new Length(100, LengthUnit.Percent);
			Add(container);

			codeField = new()
			{
				value = bits.ToCode(),
				isDelayed = true,
			};
			codeField.style.marginLeft = 0;
			codeField.style.marginRight = 0;
			codeField.RegisterValueChangedCallback(e => bits = BitSet.FromCode(e.newValue));
			container.Add(codeField);

			List<List<Enum>> props = new Type[] { typeof(Bits1), typeof(Bits2) }.SelectMany(type => Enum.GetValues(type).Cast<Enum>())
				.Select((e, i) => new { e, i }).GroupBy(x => x.i / 6).Select(g => g.Select(x => x.e).ToList()).ToList();

			Dictionary<Toggle, EventCallback<ChangeEvent<bool>>> callbacks = new();

			bitList = new();
			bitList.itemsSource = props;
			bitList.fixedItemHeight = 16 * 6.5f;
			bitList.style.maxHeight = bitList.fixedItemHeight * 2.5f;
			bitList.selectionType = SelectionType.None;
			bitList.showAlternatingRowBackgrounds = AlternatingRowBackground.All;
			bitList.showBorder = true;
			bitList.makeItem = () => new ListView();
			bitList.bindItem = (e, i) =>
			{
				ListView list = e as ListView;
				list.itemsSource = props[i];
				list.fixedItemHeight = 16;
				list.style.paddingTop = 4;
				list.makeItem = () => new Toggle();
				list.bindItem = (e, j) =>
				{
					Toggle toggle = e as Toggle;
					toggle.text = props[i][j].ToString();
					toggle.SetValueWithoutNotify(bits.BitAt(i * 6 + j));
					toggle.style.width = new Length(95, LengthUnit.Percent);
					if (callbacks.ContainsKey(toggle))
					{
						toggle.UnregisterValueChangedCallback(callbacks[toggle]);
						callbacks.Remove(toggle);
					}
					void callback(ChangeEvent<bool> e)
					{
						BitSet bit = BitSet.FromBit(i * 6 + j);
						if (e.newValue)
							bits |= bit;
						else
							bits &= ~bit;
					}
					toggle.RegisterValueChangedCallback(callback);
					callbacks[toggle] = callback;
				};
			};
			container.Add(bitList);
		}

		public override void SetValueWithoutNotify(object newValue)
		{
			base.SetValueWithoutNotify(newValue);
			codeField.SetValueWithoutNotify(bits.ToCode());
			bitList.RefreshItems();
		}
	}
}
