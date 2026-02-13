using UnityEngine;
using TMPro;
using System.Collections;

public class TextAnimation : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;

    [SerializeField] private string fullText;

    [SerializeField] private float delayBetweenLetters = 0.05f;
    [SerializeField] private float waitForRestart = 3f;

    private void OnEnable()
    {
        StartCoroutine(TypeText());
    }

    private IEnumerator TypeText()
    {
        text.text = fullText;
        text.ForceMeshUpdate();

        text.maxVisibleCharacters = 0;

        int totalCharacters = text.textInfo.characterCount;

        for (int i = 0; i <= totalCharacters; i++)
        {
            text.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delayBetweenLetters);
        }
        
        yield return new WaitForSeconds(waitForRestart);
        StartCoroutine(TypeText());
    }
}
