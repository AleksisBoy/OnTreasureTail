using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour
{
    [SerializeField] private Button buttonProgress = null;
    [SerializeField] private InfoPopup infoGainedPrefab = null;
    [SerializeField] private IslandProgressPanel progressPanel = null;
    [SerializeField] private RectTransform combatLockUI = null;
    [SerializeField] private ObjectInteractionBox interactionBox = null;
    [SerializeField] private CompassUI compassUI = null;

    private Transform combatLockTarget = null;
    private RectTransform RT { get { return (RectTransform)transform; } }
    public static UserInterface Instance { get; private set; } = null;
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        ShowCompassUI(false);
    }
    private void Update()
    {
        UpdateCombatLockUIPosition();
    }
    public void GainInfoAt(string infoText, Vector2 mousePos)
    {
        InfoPopup popup = Instantiate(infoGainedPrefab, transform);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RT, mousePos, null, out Vector2 localPos);
        popup.RT.localPosition = localPos;
        popup.SetPopup(infoText, (RectTransform)buttonProgress.transform);
    }
    // Combat lock ui
    private void UpdateCombatLockUIPosition()
    {
        if (!combatLockTarget) return;

        combatLockUI.position = RectTransformUtility.WorldToScreenPoint(PlayerCamera.Instance.Current, combatLockTarget.position);
    }
    public void SetCombatLockTarget(Transform target)
    {
        combatLockTarget = target;
        if (combatLockUI) combatLockUI.gameObject.SetActive(target ? true : false);
    }
    // Button calls
    public void Button_ProgressToggle()
    {
        UIManager.Toggle(progressPanel);
    }
    // Interaction box
    public void SetInteractionBox(string text)
    {
        ObjectInteractionBox.Text = text;
        UIManager.Open(interactionBox);
    }
    // Compass UI
    public void ShowCompassUI(bool state)
    {
        compassUI.gameObject.SetActive(state);
    }
}
