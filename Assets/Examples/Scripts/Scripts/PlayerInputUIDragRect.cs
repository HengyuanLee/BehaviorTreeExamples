using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace MGame.PlayerInputSystem
{
    public class PlayerInputUIDragRect : MonoBehaviour, IDragHandler, IEndDragHandler, IBeginDragHandler
    {
        public Action<Vector2> onPlayerDragCallback;

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            onPlayerDragCallback(eventData.delta);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            onPlayerDragCallback(Vector2.zero);
        }
    }
}
