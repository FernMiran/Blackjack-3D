using UnityEngine;

[RequireComponent (typeof(CanvasGroup))]
public class UI_Panel : MonoBehaviour, IPanel
{
	[SerializeField]
	CanvasGroup _canvasGroup;

	protected virtual void Reset()
	{
		_canvasGroup = GetComponent<CanvasGroup>();
	}

	public virtual void Show()
	{
		_canvasGroup.alpha = 1;
		_canvasGroup.interactable = true;
		_canvasGroup.blocksRaycasts = true;
	}

	public virtual void Hide()
	{
		_canvasGroup.alpha = 0;
		_canvasGroup.interactable = false;
		_canvasGroup.blocksRaycasts = false;
	}
}
