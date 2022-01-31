using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;

public class TypingText : MonoBehaviour
{
    [SerializeField] private TMP_Text text = null;
    [SerializeField] private AudioSource typeSource = null;

    private void OnEnable() 
    {
        string txt = text.text;
        text.text = string.Empty;
        StartCoroutine(TypeRoutine(txt));
    }

    private IEnumerator TypeRoutine(string pText)
    {
        yield return new WaitForSeconds(1.0f);

        string t = string.Empty;
        for (int i = 0; i < pText.Length; i++)
        {
            t += pText[i];
            text.text = t;
            if (pText[i] != ' ')
            {
                typeSource.pitch = Random.Range(0.8f, 1.3f);
                typeSource.Play();
            }
            yield return new WaitForSeconds(Random.Range(0.4f, 0.5f));
        }
    }
}
