using System;

namespace NootColis.Logic
{
    [Serializable]
    public class Colis
    {
        public string expediteur;
        public string destination;
        public string contenu;

        public Colis(string expediteur, string destination, string contenu)
        {
            this.expediteur = expediteur;
            this.destination = destination;
            this.contenu = contenu;
        }
    }
}
