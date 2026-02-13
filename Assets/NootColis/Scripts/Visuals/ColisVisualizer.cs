using UnityEngine;

using NootColis.Logic;

namespace NootColis.Visuals
{
    public class ColisVisualizer : MonoBehaviour
    {
        public static ColisVisualizer Instance { get; private set; }

        [Header("Configuration Prefab")]
        [SerializeField] private string prefabPath = "Prefabs/ColisPrefab";
        
        private GameObject _colisPrefab;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            // Pré-chargement du prefab depuis Resources
            _colisPrefab = Resources.Load<GameObject>(prefabPath);
            
            if (_colisPrefab == null)
            {
                Debug.LogError($"<color=red>[ColisVisualizer]</color> Prefab introuvable à l'adresse : Resources/{prefabPath}");
            }
        }

        public void SpawnColis(Colis colis)
        {
            GameObject instance;
            
            // Position d'instanciation : Coordonnées du manager
            Vector3 spawnPosition = transform.position;
            Quaternion spawnRotation = transform.rotation;

            if (_colisPrefab != null)
            {
                // Instanciation du prefab
                instance = Instantiate(_colisPrefab, spawnPosition, spawnRotation);
                Debug.Log($"<color=green>[ColisVisualizer]</color> Prefab instancié à {spawnPosition}");
            }
            else
            {
                // Fallback sur une primitive si le prefab est manquant pour éviter de bloquer le gameplay
                Debug.LogWarning("[ColisVisualizer] Prefab manquant, utilisation d'un cube de secours.");
                instance = GameObject.CreatePrimitive(PrimitiveType.Cube);
                instance.transform.position = spawnPosition;
                instance.transform.rotation = spawnRotation;
            }

            instance.name = $"Colis_{colis.contenu}";

            // Gestion du texte 3D
            // On cherche d'abord un composant TextMesh ou TMP dans le prefab
            TextMesh textMesh = instance.GetComponentInChildren<TextMesh>();
            
            if (textMesh != null)
            {
                textMesh.text = $"{colis.contenu}\nDe: {colis.expediteur}";
            }
            else
            {
                // Si pas de texte dans le prefab, on en crée un dynamiquement comme avant
                GameObject textObj = new GameObject("DynamicLabel");
                textObj.transform.SetParent(instance.transform);
                textObj.transform.localPosition = new Vector3(0, 1.2f, 0);

                textMesh = textObj.AddComponent<TextMesh>();
                textMesh.text = $"{colis.contenu}\nDe: {colis.expediteur}";
                textMesh.anchor = TextAnchor.MiddleCenter;
                textMesh.alignment = TextAlignment.Center;
                textMesh.fontSize = 20;
                textMesh.characterSize = 0.1f;
                textMesh.color = Color.black;
                textObj.AddComponent<LookAtCamera>();
            }
        }
    }

    public class LookAtCamera : MonoBehaviour
    {
        private Transform _camTransform;

        void Start()
        {
            if (Camera.main != null)
                _camTransform = Camera.main.transform;
        }

        void Update()
        {
            if (_camTransform != null)
            {
                transform.LookAt(transform.position + _camTransform.rotation * Vector3.forward,
                                 _camTransform.rotation * Vector3.up);
            }
        }
    }
}