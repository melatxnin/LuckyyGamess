using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace NootColis.Logic
{
    public class NootColisStore : MonoBehaviour
    {
        private string _filePath;
        private List<Colis> _entrepot = new List<Colis>();

        public static NootColisStore Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            _filePath = Path.Combine(Application.persistentDataPath, "entrepot.json");
            ChargerColis();
        }

        public void ChargerColis()
        {
            if (File.Exists(_filePath))
            {
                string json = File.ReadAllText(_filePath);
                PackageList wrapper = JsonUtility.FromJson<PackageList>(json);
                _entrepot = wrapper?.colis ?? new List<Colis>();
            }
            else
            {
                _entrepot = new List<Colis>();
            }
        }

        public void SauvegarderColis()
        {
            PackageList wrapper = new PackageList { colis = _entrepot };
            string json = JsonUtility.ToJson(wrapper, true);
            File.WriteAllText(_filePath, json);
        }

        public void AjouterColis(Colis nouveau)
        {
            _entrepot.Add(nouveau);
            SauvegarderColis();
        }

        public Colis RecupererProchain(string utilisateur)
        {
            Colis trouve = _entrepot.FirstOrDefault(c => c.destination.Equals(utilisateur, StringComparison.OrdinalIgnoreCase));
            if (trouve != null)
            {
                _entrepot.Remove(trouve);
                SauvegarderColis();
            }
            return trouve;
        }

        public List<Colis> VoirTout(string utilisateur)
        {
            return _entrepot.Where(c => c.destination.Equals(utilisateur, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        [Serializable]
        private class PackageList
        {
            public List<Colis> colis;
        }
    }
}
