using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core.UI
{
    public class SimpleMainMenuPage : MonoBehaviour, IMainMenuPage
    {

        public Canvas canvas;

        public virtual void Hide()
        {
            if (canvas != null)
            {
                canvas.enabled = false;
            }
            else
            {
                gameObject.SetActive(false);
            }
        }

        public virtual void Show()
        {
            if (canvas != null)
            {
                canvas.enabled = true;
            }
            else
            {
                gameObject.SetActive(true);
            }
        }
    }
}
