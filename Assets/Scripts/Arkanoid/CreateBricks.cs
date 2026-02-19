using UnityEngine;
using System.Collections.Generic;

// Ce script va se charger de créer les briques du jeu aléatoirement
public class CreateBricks : MonoBehaviour
{
    // La liste des briques dans la scene, en public pour que BrickMove.cs y ai accès
    public List<GameObject> bricks;

    // Les sprites étant disponibles, dans mon jeu :
    // 6 Seven
    // 9 Flower
    // 10 Coin
    // 11 Letter
    [SerializeField] private List<Sprite> availableSprites;

    // Les quatre sprites Seven, Flower, Coin, Letter
    [SerializeField] private List<Sprite> sprites;

    // Au démarrage on veut mélanger les briques et les assigner
    private void Awake()
    {
        Shuffle(availableSprites);
        AssignBricks();
    }

    // Tout simplement on transmet à cette fonction fisher-yates si je me rappelle
    // bien le nom notre liste des 36 sprites disponibles et elle va les mélanger
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

    private void AssignBricks()
    {
        // On met Mathf.Min pour éviter le dépassement d'index, mais normalement
        // la liste bricks et availableSprites font la meme taille dans linspector
        int count = Mathf.Min(bricks.Count, availableSprites.Count);

        // Pour chaque brique on fait une attribution visuelle, on fait la liaison
        // avec BrickData.cs donc le visuel et relier à la logique
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
}
