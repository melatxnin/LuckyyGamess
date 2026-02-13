using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BaccaratAnimation : MonoBehaviour
{
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private Image rendererImage;

    private void OnEnable()
    {
        StartCoroutine(ChangeSprite());
    }

    private IEnumerator ChangeSprite()
    {
        int i = 0;
        int direction = 1;

        while (true)
        {
            rendererImage.sprite = sprites[i];
            yield return new WaitForSeconds(0.24f);

            i += direction;

            // On atteint la fin → on repart en arrière
            if (i >= sprites.Length - 1)
            {
                i = sprites.Length - 1;
                direction = -1;
            }
            // On atteint le début → on repart en avant
            else if (i <= 0)
            {
                i = 0;
                direction = 1;
            }
        }
    }

}
