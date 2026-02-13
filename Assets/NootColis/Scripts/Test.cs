using NootColis.Logic;
using UnityEngine;

namespace NootColis
{
    public class Test : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
       async void Start()
        {
            //Voici comment Envoyer un Colis
            await NootColisAPI.SendColis("Louka", "Julien", "Tomate");


            await NootColisAPI.GetColisOf("Julien");

            if(NootColisAPI.GetInboxCount("Julien") > 0)
            {
                Colis colis=NootColisAPI.PopColis("Julien");
                Debug.Log("colis recu de "+colis.expediteur+" pour "+colis.destination+" contenue: "+colis.contenu);

            }
            NootColisAPI.GetStreamOfColis("Julien");

        }

        // Update is called once per frame
        void Update()
        {
            if (NootColisAPI.GetInboxCount("Julien") > 0)
            {
                Colis colis = NootColisAPI.PopColis("Julien");
                Debug.Log("J'ai reçu : " + colis.contenu);
            }
        }
    }
}
