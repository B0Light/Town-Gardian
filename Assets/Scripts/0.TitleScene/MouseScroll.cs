using UnityEngine;
using UnityEngine.UI;
using UnityInput = UnityEngine.Input;


[RequireComponent(typeof(ScrollRect))]
public class MouseScroll : MonoBehaviour
{
	
	public bool clampScroll = true;
	public float scrollXBuffer;
	public float scrollYBuffer;
	protected ScrollRect m_ScrollRect;
	protected RectTransform m_ScrollRectTransform;

	protected bool m_OverrideScrolling,
	               m_HasRightBuffer;

	public void SetHasRightBuffer(bool rightBuffer)
	{
		m_HasRightBuffer = rightBuffer;
	}
	void Start()
	{
#if UNITY_STANDALONE || UNITY_EDITOR //전처리기 standalone || 유니티에디서에서만 컴파일 
		m_ScrollRect = GetComponent<ScrollRect>();
		m_ScrollRect.enabled = false;
		m_OverrideScrolling = true;
		m_ScrollRectTransform = (RectTransform) m_ScrollRect.transform;
#else
		m_OverrideScrolling = false;
#endif
	}

	void Update()
	{
		if (!m_OverrideScrolling)
		{
			return;
		}
		Vector3 mousePosition = UnityInput.mousePosition;

		// only scroll if mouse is inside ScrollRect
		bool inside = RectTransformUtility.RectangleContainsScreenPoint(m_ScrollRectTransform, mousePosition);
		if (!inside) return;
		

		Rect rect = m_ScrollRectTransform.rect;
		float adjustmentX = rect.width * scrollXBuffer,
		      adjustmentY = rect.height * scrollYBuffer;

		Vector2 localPoint;
		RectTransformUtility.ScreenPointToLocalPointInRectangle(m_ScrollRectTransform, mousePosition, null, out localPoint);

		Vector2 pivot = m_ScrollRectTransform.pivot;
		float x = (localPoint.x + (rect.width - adjustmentX) * pivot.x) / (rect.width - 2 * adjustmentX);
		float y = (localPoint.y + (rect.height - adjustmentY) * pivot.y) / (rect.height - 2 * adjustmentY);

		if (clampScroll)
		{
			x = Mathf.Clamp01(x);
			y = Mathf.Clamp01(y);
		}

		m_ScrollRect.normalizedPosition = new Vector2(x, y);
	}
	
	public void SelectChild(LevelSelectButton levelSelectButton)
	{
		// minus one if  buffer
		int childCount = levelSelectButton.transform.parent.childCount - (m_HasRightBuffer ? 1 : 0);
		if (childCount > 1)
		{
			float normalized = (float)levelSelectButton.transform.GetSiblingIndex() / ( childCount - 1);
			m_ScrollRect.normalizedPosition = new Vector2(normalized, 0);
		}
	}
}
