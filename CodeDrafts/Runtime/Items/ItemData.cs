using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 아이템 정의 ScriptableObject. Project 창 → Create → MagicEscape → Item Data로 7개 생성.
    /// itemId는 MagicEscape.Core.ItemIds 상수와 정확히 같아야 한다.
    /// </summary>
    [CreateAssetMenu(fileName = "Item_", menuName = "MagicEscape/Item Data")]
    public class ItemData : ScriptableObject
    {
        [Tooltip("Core.ItemIds 와 동일한 문자열 (예: ing_moonpetal)")]
        public string itemId;
        public string displayName;
        [TextArea(2, 4)] public string description;
        public Sprite icon;
    }
}
