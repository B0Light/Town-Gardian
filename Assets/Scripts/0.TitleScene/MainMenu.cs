using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
	public abstract class MainMenu : MonoBehaviour
	{
		protected IMainMenuPage m_CurrentPage;
		protected Stack<IMainMenuPage> m_PageStack = new Stack<IMainMenuPage>();
		
		protected virtual void ChangePage(IMainMenuPage newPage)
		{
			DeactivateCurrentPage();
			ActivateCurrentPage(newPage);
		}

		protected void DeactivateCurrentPage()
		{
			if (m_CurrentPage != null)
				m_CurrentPage.Hide();
		}
		
		protected void ActivateCurrentPage(IMainMenuPage newPage)
		{
			m_CurrentPage = newPage;
			m_CurrentPage.Show();
			m_PageStack.Push(m_CurrentPage);
		}
		
		protected void SafeBack(IMainMenuPage backPage)
		{
			DeactivateCurrentPage();
			ActivateCurrentPage(backPage);
		}
		
		public virtual void Back()
		{
			if (m_PageStack.Count == 0)
			{
				return;
			}

			DeactivateCurrentPage();
			m_PageStack.Pop();
			ActivateCurrentPage(m_PageStack.Pop());
		}
		
		public virtual void Back(IMainMenuPage backPage)
		{
			int count = m_PageStack.Count;
			if (count == 0)
			{
				SafeBack(backPage);
				return;
			}

			for (int i = count - 1; i >= 0; i--)
			{
				IMainMenuPage currentPage = m_PageStack.Pop();
				if (currentPage == backPage)
				{
					SafeBack(backPage);
					return;
				}
			}

			SafeBack(backPage);
		}
	}
}