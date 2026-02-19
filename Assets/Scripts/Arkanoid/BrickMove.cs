using UnityEngine;

// Ce script est attaché au GameObject Bricks et ne son rôle est de ne pas bouger
// lui même, mais de faire bouger l'ensemble des briques d'un mouvement horizontal
// de droite vers la gauche perpétuellement
public class BrickMove : MonoBehaviour
{
    // On a besoin de CreateBricks.cs car c'est lui qui contient
    // la liste de toutes les briques présentes dans le jeu
    [SerializeField] private CreateBricks createBricks;
    // On a besoin de ArkanoidManager.cs pour savoir si le jeu
    // est en pause (isPaused)
    [SerializeField] private ArkanoidManager arkanoidManager;

    // Informations sur la vitesse des briques, la position gauche maximale,
    // et la position où ils doivent se replacer
    [SerializeField] private float speed = 1f;
    [SerializeField] private float leftLimit = -2.25f;
    [SerializeField] private float resetX = 3.15f;

    private void Update()
    {
        // Si le jeu est actuellement en pause on cesse de déplacer les briques,
        // car on ne veut pas surprendre le joueur avec des briques présentes à 
        // des positions qui n'étaient même pas les leur avant de mettre pause
        if (arkanoidManager.isPaused)
        {
            return;
        }
        
        // Pour chaque brique dans notre liste de briques dans createBricks,
        // on va prendre sa position sous forme de Vector3 en la nommant pos,
        // et cette position va continuellement se déplacer vers la gauche 
        // au rythme de speed * Time.deltaTime, car on soustrait cette vitesse
        // à la position, si on additionerait cette vitesse nos briques
        // partiraient à droite
        for (int i = 0; i < createBricks.bricks.Count; i++)
        {
            Vector3 pos = createBricks.bricks[i].transform.localPosition;
            pos.x -= speed * Time.deltaTime;

            // Lorsqu'une brique atteint la position x gauche limite, 
            // on la replace a droite (en dehors de l'écran)
            if (pos.x <= leftLimit)
            {
                pos.x = resetX;
            }

            // On attribue donc la position calculée à chaque brique
            createBricks.bricks[i].transform.localPosition = pos;
        }
    }
}
