using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GapText : MonoBehaviour, ICellDragEnd, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Image image = null;
    [SerializeField] private TMP_Text gapText = null;
    [SerializeField] private InfoSO info = null;
    [SerializeField] private bool correctCapitalLetter = false;

    private bool lockedCorrect = false;

    private Action onTextSet = null;
    private InfoCelledUI infoCelledUI = null;
    public RectTransform RT { get { return (RectTransform)transform; } }

    private void Awake()
    {
        gapText.text = string.Empty;
        if (info == null)
        {
            Debug.LogError("INFO ID NOT SET FOR " + name);
        }
    }
    [ContextMenu("Name The Object")]
    private void NameGapText()
    {
        if (info == null)
        {
            name = "GapText_NOTSET";
            gapText.text = "NOT SET";
        }
        else
        {
            name = "GapText_" + info.name;
            gapText.text = info.name;
        }
    }
    [ContextMenu("Name All Objects")]
    private void NameGapTextAll()
    {
        foreach(var gaptext in FindObjectsOfType<GapText>())
        {
            gaptext.NameGapText();
        }
    }
    public bool IsOverObject(Vector3 mousePos)
    {
        return RectTransformUtility.RectangleContainsScreenPoint(RT, mousePos);
    }

    public bool TryDragEnd(InfoCelledUI infoCelledUI)
    {
        if (lockedCorrect) return false;

        SetInfoCelled(infoCelledUI);
        return this.infoCelledUI != null;
    }
    public void LockGapTextCorrect()
    {
        lockedCorrect = true;
        UppercaseFirstLetter();
        foreach (Cell cell in infoCelledUI.InfoCelled.cells)
        {
            cell.occupied = false;
        }
        infoCelledUI.Lock(RT.position);
    }

    private void UppercaseFirstLetter()
    {
        string theText = gapText.text;
        if (correctCapitalLetter)
        {
            gapText.text = char.ToUpper(theText[0]) + theText.Substring(1);
        }
        else
        {
            gapText.text = char.ToLower(theText[0]) + theText.Substring(1);
        }
    }

    private void SetInfoCelled(InfoCelledUI infoCelledUI)
    {
        if (this.infoCelledUI == infoCelledUI) return;

        string newText = string.Empty;
        if (infoCelledUI != null)
        {
            InfoTextSO infoText = infoCelledUI.InfoCelled.info as InfoTextSO;
            if (infoText == null) return;

            newText = infoText.Text;
        }

        this.infoCelledUI = infoCelledUI;
        gapText.text = newText;
        Invoke_OnTextSet();
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right || lockedCorrect) return;

        SetInfoCelled(null);
    }
    public void AddOnTextSet(Action newAction)
    {
        onTextSet += newAction;
    }
    private void Invoke_OnTextSet()
    {
        if (onTextSet != null) onTextSet();
    }
    public bool InfoSetCorrectly()
    {
        return info == infoCelledUI.InfoCelled.info;
    }
    public bool IsInfoSet()
    {
        return infoCelledUI != null;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!lockedCorrect) return;

        image.color += new Color(0, 0, 0, -0.1f);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!lockedCorrect) return;

        image.color += new Color(0, 0, 0, 0.1f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // set dragging of the info celled in cell based viewport 
        if (!lockedCorrect) return;

        FindObjectOfType<CellBasedViewport>().Debug_SetCurrentDragged(infoCelledUI);
        infoCelledUI.gameObject.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!lockedCorrect) return;

        infoCelledUI.gameObject.SetActive(false);
    }
}
