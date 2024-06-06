using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ParagraphTextUI : MonoBehaviour
{
    [SerializeField] private TMP_Text textPrefab = null;

    [ContextMenu("SetupParagraph")]
    private void SetupParagraph()
    {
        TMP_Text tmp = GetComponent<TMP_Text>();
        if (tmp == null) return;

        string text = tmp.text;
        if (!text.Contains("<info")) return;

        foreach(Transform child in transform)
        {
            DestroyImmediate(child.gameObject);
        }

        int index = text.IndexOf("<info");
        while (index > -1)
        {
            int indexLast = text.IndexOf("</info>");
            int length = indexLast - (index + 6);
            text = text.Remove(index, 6);
            indexLast -= 6;
            text = text.Remove(indexLast, 7);
            tmp.text = text;

            TMP_Text newText = Instantiate(textPrefab, transform);
            newText.text = text.Substring(index, length);

            newText.rectTransform.position = tmp.rectTransform.TransformPoint(tmp.textInfo.characterInfo[index].bottomLeft);

            index = text.IndexOf("<info");
        }
        tmp.text = text;
    }
}
