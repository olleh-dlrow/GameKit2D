using UnityEngine;
using UnityEngine.EventSystems;

namespace Platformer
{
    public class DragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler
    {
        private Vector3 offset;

        public void OnBeginDrag(PointerEventData eventData)
        {
            // 计算鼠标指针和游戏对象之间的偏移量
            offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(eventData.position.x, eventData.position.y, transform.position.z));
        }

        public void OnDrag(PointerEventData eventData)
        {
            // 更新游戏对象的位置
            Vector3 newPosition = new Vector3(eventData.position.x, eventData.position.y, transform.position.z);
            transform.position = Camera.main.ScreenToWorldPoint(newPosition) + offset;
        }
    }

}