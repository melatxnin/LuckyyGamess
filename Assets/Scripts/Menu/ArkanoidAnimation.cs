using UnityEngine;

public class ArkanoidAnimation : MonoBehaviour
{
    [SerializeField] private Transform arkanoidTransform;

    [SerializeField] private float speedRotation;
    [SerializeField] private int sign;

    private void Start()
    {
        sign = Random.value < 0.5f ? 1 : -1;
    }

    private void Update()
    {
        arkanoidTransform.Rotate(new Vector3(0f, 0f, sign * speedRotation * Time.deltaTime));
    }

    public void SignChanged(bool s)
    {
        sign = s ? 1 : -1;
    }
}
