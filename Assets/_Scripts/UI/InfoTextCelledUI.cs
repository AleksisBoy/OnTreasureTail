using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoTextCelledUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textUI = null;
    [SerializeField] private Outline outline = null;

    private InfoCelled infoCelled = null;
    private RectTransform RT { get { return (RectTransform)transform; } }
    public void Set(InfoCelled infoCelled)
    {
        this.infoCelled = infoCelled;

        RT.sizeDelta = new Vector2(infoCelled.cornerEnd.x - infoCelled.cornerStart.x, infoCelled.cornerEnd.y - infoCelled.cornerStart.y);
        RT.position = infoCelled.cornerStart + new Vector2(RT.sizeDelta.x / 2, RT.sizeDelta.y / 2);
        textUI.text = ((InfoTextSO)infoCelled.info).Text;
    }
}
