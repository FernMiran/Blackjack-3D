using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_BettingChip : MonoBehaviour
{
	[SerializeField]
	Text _text;

	private void Reset()
	{
		_text = GetComponentInChildren<Canvas>().GetComponentInChildren<Text>();
	}

	public void SetTextValue(int value)
	{
		// Rules to contract thousands to replace zeroes for "k"'s
		_text.text = value.ToString();
	}
}
