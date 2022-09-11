using UnityEngine;
using UnityEngine.EventSystems;

namespace MKStudio.UI.MessageBox
{
    public class SetTip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public string Tip;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (string.IsNullOrEmpty(Tip))
            {
                Debug.LogWarning("Tooltip message empty.");
            }
            else
            {
                TipDisplayer.Instance.SetTip(Tip);
            }
        }


        public void OnPointerExit(PointerEventData eventData)
        {
            TipDisplayer.Instance.SetTip(string.Empty);
        }
    }

}