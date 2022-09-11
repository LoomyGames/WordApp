using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace MKStudio.UI.MessageBox
{
    public class DragHandler : MonoBehaviour, IDragHandler, IPointerDownHandler
    {
        Vector3 offset;
        CanvasScaler scaler;
        Rect dragableRect;
        float constraint;

        BoxController box;
        public DragTarget dragTarget;
        bool canDrag;

        private void Awake()
        {
            box = gameObject.GetComponentInParent<BoxController>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!canDrag)
            {
                return;
            }
            transform.parent.position = new Vector3(Mathf.Clamp((Input.mousePosition + offset).x, constraint - dragableRect.width * (scaler == null ? 1f : scaler.scaleFactor) / 2f,
                                                                (Screen.width - constraint + dragableRect.width * (scaler == null ? 1f : scaler.scaleFactor) / 2f)),
                                                    Mathf.Clamp((Input.mousePosition + offset).y, constraint - dragableRect.height / 2f * (scaler == null ? 1f : scaler.scaleFactor),
                                                                (Screen.height - dragableRect.height * (scaler == null ? 1f : scaler.scaleFactor) / 2f)));
        }

        public void OnPointerDown(PointerEventData eventData)
        {

            switch (box.CurrentDragType)
            {
                case DragTypes.TitleOnly:
                    canDrag = dragTarget == DragTarget.Title;
                    break;
                case DragTypes.TitleAndMessage:
                    canDrag = (dragTarget == DragTarget.Message || dragTarget == DragTarget.Title);
                    break;
                case DragTypes.Disabled:
                    canDrag = false;
                    break;
                default:
                    canDrag = false;
                    break;
            }

            offset = transform.parent.position - Input.mousePosition;
            dragableRect = (transform.parent as RectTransform).rect;
            scaler = gameObject.GetComponentInParent<CanvasScaler>();
            constraint = gameObject.GetComponentInParent<BoxController>().dragBoundLimit;
        }
    }
}

