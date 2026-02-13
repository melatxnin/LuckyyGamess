using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class TitleAnimation : MonoBehaviour
{
    [SerializeField] private RectTransform titleTransform;
    
    [SerializeField] private float minScale = 0.85f;
    [SerializeField] private float maxScale = 1.05f;
    [SerializeField] private float pulseSpeed = 1.5f;

    private void Start()
    {
        StartCoroutine(TitlePulse());
    }

    private IEnumerator TitlePulse()
    {
        float t = 0f;
        bool growing = true;

        while (true)
        {
            t += Time.deltaTime * pulseSpeed;
            float lerpValue = growing ? t : 1f - t;

            float scale = Mathf.Lerp(minScale, maxScale, lerpValue);
            titleTransform.localScale = Vector3.one * scale;

            if (t >= 1f)
            {
                t = 0f;
                growing = !growing;
            }

            yield return null;
        }
    }
}
