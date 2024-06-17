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
    private void Update()
    {
        UpdateCombatLockUIPosition();
    }
    private void UpdateCombatLockUIPosition()
    {
        if (!combatLockTarget) return;

        combatLockUI.position = RectTransformUtility.WorldToScreenPoint(PlayerCamera.Instance.Current, combatLockTarget.position);
    }
    public void Button_ProgressToggle()
    {
        UIManager.Toggle(progressPanel);
    }
    public void GainInfoAt(string infoText, Vector2 mousePos)
    {
        InfoPopup popup = Instantiate(infoGainedPrefab, transform);
        
        RectTransformUtility.ScreenPointToLocalPointInRectangle(RT, mousePos, null, out Vector2 localPos);
        popup.RT.localPosition = localPos;
        popup.SetPopup(infoText, (RectTransform)buttonProgress.transform);
    }
    public void SetCombatLockTarget(Transform target)
    {
        combatLockTarget = target;
        combatLockUI.gameObject.SetActive(target ? true : false);
    }
}
