using UnityEngine;
using UnityEngine.Networking;
using System;
using System.Collections.Generic;
using NootColis.Logic;

namespace NootColis.Logic
{
    [AddComponentMenu("NootColis/NootColis Manager")]
    public class NootColisManager : MonoBehaviour
    {
        public static NootColisManager Instance { get; private set; }

        [Header("Configuration Serveur")]
        private string serverUrl = "https://www.dreams.re/rubika/index.php";
        [SerializeField] private bool useVerboseLogs = true;

        [Serializable]
        public class ServeurReponse
        {
            public string statut;
            public string message;
            public Colis donnees; // Pour un seul colis
            // Note: Pour une liste, il faudrait une autre structure ou un champ flexible
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else Destroy(gameObject);
        }

        public async Awaitable EnvoyerColis(string expediteur, string destination, string contenu, Action<string> onResult)
        {
            Log($"Tentative d'envoi vers : {serverUrl}");
            
            WWWForm form = new WWWForm();
            form.AddField("action", "envoyer");
            form.AddField("expediteur", expediteur);
            form.AddField("destination", destination);
            form.AddField("contenu", contenu);

            using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))
            {
                try
                {
                    await www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        string errorMsg = $"Erreur Réseau [{www.responseCode}]: {www.error}";
                        LogError(errorMsg);
                        onResult?.Invoke(errorMsg);
                    }
                    else
                    {
                        Log($"Réponse brute reçue : {www.downloadHandler.text}");
                        onResult?.Invoke("Succès : Colis enregistré sur le serveur.");
                    }
                }
                catch (Exception e)
                {
                    LogError($"Exception lors de l'envoi : {e.Message}");
                    onResult?.Invoke($"Exception : {e.Message}");
                }
            }
        }

        public async Awaitable RecupererProchain(string nomUtilisateur, Action<bool, Colis, string> onComplete)
        {
            Log($"Récupération colis pour : {nomUtilisateur} à {serverUrl}");

            WWWForm form = new WWWForm();
            form.AddField("action", "recuperer_prochain");
            form.AddField("utilisateur", nomUtilisateur);

            using (UnityWebRequest www = UnityWebRequest.Post(serverUrl, form))
            {
                try
                {
                    await www.SendWebRequest();

                    if (www.result != UnityWebRequest.Result.Success)
                    {
                        string error = $"Erreur [{www.responseCode}]: {www.error}";
                        LogError(error);
                        onComplete?.Invoke(false, null, error);
                    }
                    else
                    {
                        string json = www.downloadHandler.text;
                        Log($"Réponse serveur : {json}");

                        // On tente de parser la réponse
                        try 
                        {
                            ServeurReponse reponse = JsonUtility.FromJson<ServeurReponse>(json);
                            if (reponse != null && reponse.statut == "success" && reponse.donnees != null)
                            {
                                onComplete?.Invoke(true, reponse.donnees, "Colis récupéré !");
                            }
                            else
                            {
                                string msg = reponse?.message ?? "Aucun colis trouvé ou format invalide.";
                                onComplete?.Invoke(false, null, msg);
                            }
                        }
                        catch (Exception parseEx)
                        {
                            LogError($"Erreur de parsing JSON : {parseEx.Message}");
                            onComplete?.Invoke(false, null, "Erreur format de données serveur.");
                        }
                    }
                }
                catch (Exception e)
                {
                    LogError($"Exception : {e.Message}");
                    onComplete?.Invoke(false, null, e.Message);
                }
            }
        }

        private void Log(string message)
        {
            if (useVerboseLogs) Debug.Log($"<color=cyan>[NootColisManager]</color> {message}");
        }

        private void LogError(string message)
        {
            Debug.LogError($"<color=red>[NootColisManager ERROR]</color> {message}");
        }
    }
}
