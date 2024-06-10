using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NoteTextInfo : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image background = null;
    [SerializeField] private TMP_Text tmp = null;
    [SerializeField] private InfoTextSO infoText = null;

    private bool clicked = false;
    public RectTransform RT => (RectTransform)transform;

    [ContextMenu("Name The Object")]
    private void NameNoteInfo()
    {
        if (infoText == null)
        {
            name = "GapText_NOTSET";
            tmp.text = "NOT SET";
        }
        else
        {
            name = "NoteTextInfo_" + infoText.Text;
            tmp.text = infoText.Text;
#if UNITY_EDITOR
            EditorUtility.SetDirty(tmp);
#endif
        }

        FitText();
    }
    [ContextMenu("Name All Objects")]
    private void NameGapTextAll()
    {
        foreach (var gaptext in FindObjectsOfType<NoteTextInfo>())
        {
            gaptext.NameNoteInfo();
        }
    }
    private void FitText()
    {
        RT.sizeDelta = new Vector2(tmp.preferredWidth, RT.sizeDelta.y);
    }
    private void Start()
    {
        background.color = !clicked ? InternalSettings.Get.InfoActiveColor : InternalSettings.Get.InfoGainedColor;
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (clicked) return;
        clicked = true;

        IslandManager.Instance.AddInfo(infoText, Input.mousePosition);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        background.color = InternalSettings.Get.SelectedInfoColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        background.color = !clicked ? InternalSettings.Get.InfoActiveColor : InternalSettings.Get.InfoGainedColor;
    }
}
