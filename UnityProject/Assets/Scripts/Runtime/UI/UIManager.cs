using System;
using MagicEscape.Core;
using TMPro;
using UnityEngine;

namespace MagicEscape
{
    /// <summary>
    /// 모든 화면 패널과 HUD 텍스트를 관리한다. 퍼즐 규칙은 모르고, 받은 문자열을 보여 주기만 한다.
    /// 패널: Title / HUD / Pause / Clear / Read(단서 읽기) / Objective(Tab).
    /// </summary>
    [DefaultExecutionOrder(-40)]
    [DisallowMultipleComponent]
    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        [Header("Panels")]
        [SerializeField] private GameObject titlePanel;
        [SerializeField] private GameObject hudPanel;
        [SerializeField] private GameObject pausePanel;
        [SerializeField] private GameObject clearPanel;
        [SerializeField] private GameObject readPanel;
        [SerializeField] private GameObject objectivePanel;

        [Header("HUD")]
        [SerializeField] private TMP_Text promptText;
        [SerializeField] private GameObject toastRoot;
        [SerializeField] private TMP_Text toastText;
        [SerializeField] private float toastSeconds = 3f;

        [Header("Read / Objective / Clear")]
        [SerializeField] private TMP_Text readTitleText;
        [SerializeField] private TMP_Text readBodyText;
        [SerializeField] private TMP_Text objectiveText;
        [SerializeField] private TMP_Text hintText;
        [SerializeField] private TMP_Text clearTimeText;
        [SerializeField] private TMP_Text clearMessageText;

        private float _toastHideAt;

        /// <summary>읽기 패널 열림/닫힘(true=열림). GameManager가 조작 잠금에 사용.</summary>
        public event Action<bool> ReadingChanged;

        public bool IsReading => readPanel != null && readPanel.activeSelf;

        private void Awake()
        {
            if (Instance != null && Instance != this) { Destroy(gameObject); return; }
            Instance = this;
            SetActive(readPanel, false);
            SetActive(objectivePanel, false);
            SetActive(toastRoot, false);
            SetPrompt(null);
        }

        private void OnDestroy()
        {
            if (Instance == this) Instance = null;
        }

        private void Update()
        {
            if (toastRoot != null && toastRoot.activeSelf && Time.unscaledTime >= _toastHideAt)
                toastRoot.SetActive(false);
        }

        /// <summary>흐름 상태에 맞춰 패널 전환.</summary>
        public void ShowFlow(GameFlowState state)
        {
            SetActive(titlePanel, state == GameFlowState.Title);
            SetActive(hudPanel, state == GameFlowState.Playing || state == GameFlowState.Paused);
            SetActive(pausePanel, state == GameFlowState.Paused);
            SetActive(clearPanel, state == GameFlowState.Cleared);
            if (state != GameFlowState.Playing)
            {
                SetPrompt(null);
                SetActive(objectivePanel, false);
                if (IsReading) CloseRead();
            }
        }

        /// <summary>크로스헤어 아래 "[E] ..." 프롬프트. null이면 숨김.</summary>
        public void SetPrompt(string verb)
        {
            if (promptText == null) return;
            bool show = !string.IsNullOrEmpty(verb);
            promptText.gameObject.SetActive(show);
            if (show) promptText.text = "[E] " + verb;
        }

        public void ShowToast(string message)
        {
            if (string.IsNullOrEmpty(message)) return;
            Debug.Log("[Toast] " + message);
            if (toastText == null || toastRoot == null) return;
            toastText.text = message;
            toastRoot.SetActive(true);
            _toastHideAt = Time.unscaledTime + toastSeconds;
        }

        /// <summary>단서(액자·칠판·마법서) 읽기 패널. E 또는 ESC로 닫는다.</summary>
        public void ShowRead(string title, string body)
        {
            if (readPanel == null) { ShowToast(body); return; }
            if (readTitleText != null) readTitleText.text = title;
            if (readBodyText != null) readBodyText.text = body;
            readPanel.SetActive(true);
            ReadingChanged?.Invoke(true);
        }

        public void CloseRead()
        {
            if (!IsReading) return;
            readPanel.SetActive(false);
            ReadingChanged?.Invoke(false);
        }

        public void ToggleObjective()
        {
            if (objectivePanel == null) return;
            bool show = !objectivePanel.activeSelf;
            objectivePanel.SetActive(show);
            if (show) RefreshObjective();
        }

        /// <summary>목표·힌트 텍스트를 GameManager 기준으로 갱신.</summary>
        public void RefreshObjective()
        {
            GameManager gm = GameManager.Instance;
            if (gm == null) return;
            if (objectiveText != null) objectiveText.text = gm.GetObjectiveText();
            if (hintText != null)
            {
                string hint = gm.GetCurrentHint();
                hintText.gameObject.SetActive(!string.IsNullOrEmpty(hint));
                hintText.text = string.IsNullOrEmpty(hint) ? string.Empty : "힌트: " + hint;
            }
        }

        public void ShowClear(float elapsedSeconds)
        {
            int total = Mathf.FloorToInt(elapsedSeconds);
            if (clearTimeText != null) clearTimeText.text = string.Format("탈출 시간 {0:00}:{1:00}", total / 60, total % 60);
            if (clearMessageText != null) clearMessageText.text = GameText.Ending;
        }

        // ---------- Button OnClick 연결용 ----------
        public void OnClickStart() { AudioManager.Play(SfxId.UiClick); if (GameManager.Instance != null) GameManager.Instance.StartGame(); }
        public void OnClickResume() { AudioManager.Play(SfxId.UiClick); if (GameManager.Instance != null) GameManager.Instance.Resume(); }
        public void OnClickRestart() { if (GameManager.Instance != null) GameManager.Instance.Restart(); }
        public void OnClickQuit() { if (GameManager.Instance != null) GameManager.Instance.QuitGame(); }

        private static void SetActive(GameObject go, bool active)
        {
            if (go != null && go.activeSelf != active) go.SetActive(active);
        }
    }
}
