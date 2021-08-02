using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace WrightWay.SBEPIS.Modus
{
	public abstract class FetchModus
	{
		public FetchModus(FetchModus oldModus)
		{
			if (oldModus != null)
				foreach (CaptchalogueCard card in oldModus.cards)
					if (card)
						InsertCard(card);
		}

		public abstract ICollection<CaptchalogueCard> cards { get; }
		public virtual bool flippedInsert => false;
		public virtual bool flippedRetrieve => false;

		/// <returns>An item to eject</returns>
		public virtual Item Insert(Item item)
		{
			CaptchalogueCard card = EjectCard();
			Item rtn = card.heldItem;
			card.Eject();
			card.Captchalogue(item);
			InsertCard(card);
			return rtn;
		}

		public abstract void InsertCard(CaptchalogueCard card);

		public abstract CaptchalogueCard Display();

		public virtual Item Retrieve()
		{
			CaptchalogueCard card = RetrieveCard();
			Item rtn = card.heldItem;
			card.Eject();
			InsertCard(card);
			return rtn;
		}

		public virtual CaptchalogueCard RetrieveCard()
		{
			CaptchalogueCard rtn = Display();
			cards.Remove(rtn);
			return rtn;
		}

		protected abstract CaptchalogueCard EjectCard();
	}

	public class StackModus : FetchModus
	{
		public StackModus(FetchModus oldModus) : base(oldModus) { }

		private List<CaptchalogueCard> _cards = new List<CaptchalogueCard>();
		public override ICollection<CaptchalogueCard> cards => _cards;
		public override bool flippedRetrieve => true;

		public override void InsertCard(CaptchalogueCard card)
		{
			if (card.heldItem)
				_cards.Insert(0, card);
			else
				_cards.Add(card);
		}

		public override CaptchalogueCard Display()
		{
			return _cards.Count == 0 ? null : _cards[0];
		}

		protected override CaptchalogueCard EjectCard()
		{
			CaptchalogueCard card = _cards[_cards.Count - 1];
			_cards.RemoveAt(_cards.Count - 1);
			return card;
		}
	}

	public class QueueModus : FetchModus
	{
		public QueueModus(FetchModus oldModus) : base(oldModus) { }

		private List<CaptchalogueCard> _cards = new List<CaptchalogueCard>();
		public override ICollection<CaptchalogueCard> cards => _cards;

		public override void InsertCard(CaptchalogueCard card)
		{
			if (card.heldItem)
				_cards.Add(card);
			else
				_cards.Insert(0, null);
		}

		public override CaptchalogueCard Display()
		{
			return _cards.Count == 0 ? null : _cards[Mathf.Max(_cards.FindIndex(card => card.heldItem != null), 0)];
		}

		protected override CaptchalogueCard EjectCard()
		{
			CaptchalogueCard card = _cards[_cards.Count - 1];
			_cards.RemoveAt(_cards.Count - 1);
			return card;
		}
	}
}