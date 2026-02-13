using UnityEngine;
using System.Collections.Generic;

public class CreateBricks : MonoBehaviour
{
    public List<GameObject> bricks;

    [SerializeField] private List<Sprite> availableSprites;

    [SerializeField] private List<Sprite> sprites;

    private void Awake()
    {
        Shuffle(availableSprites);
        AssignBricks();
    }

    private void AssignBricks()
    {
        int count = Mathf.Min(bricks.Count, availableSprites.Count);

        for (int i = 0; i < count; i++)
        {
            Sprite sprite = availableSprites[i];
            GameObject brick = bricks[i];

            SpriteRenderer sr = brick.GetComponent<SpriteRenderer>();
            BrickData brickData = brick.GetComponent<BrickData>();
            sr.sprite = sprite;

            for (int j = 0; j < sprites.Count; j++)
            {
                if (sprites[j] == sprite)
                {
                    brickData.sprite = sprites[j];
                }
            }

        }
    }

    private void Shuffle<T>(List<T> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);

            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }
}
