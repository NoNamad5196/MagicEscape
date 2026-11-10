namespace MagicEscape
{
    /// <summary>
    /// E키로 상호작용 가능한 모든 오브젝트의 공통 계약.
    /// Collider가 있는 GameObject(또는 그 부모)에 구현 컴포넌트를 붙인다.
    /// </summary>
    public interface IInteractable
    {
        /// <summary>
        /// 크로스헤어 아래 표시할 동사구(예: "문양석 누르기"). UI가 "[E] " 접두어를 붙인다.
        /// null 또는 빈 문자열이면 프롬프트를 표시하지 않고 상호작용도 막는다.
        /// </summary>
        string GetPrompt(PlayerInteraction interactor);

        /// <summary>E키를 눌렀을 때 호출. 인벤토리 등은 interactor를 통해 접근한다.</summary>
        void Interact(PlayerInteraction interactor);
    }
}
