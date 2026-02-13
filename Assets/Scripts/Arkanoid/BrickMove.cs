using UnityEngine;

public class BrickMove : MonoBehaviour
{
    [SerializeField] private CreateBricks createBricks;
    [SerializeField] private ArkanoidManager arkanoidManager;

    [SerializeField] private float speed = 1f;
    [SerializeField] private float leftLimit = -2.25f;
    [SerializeField] private float resetX = 3.15f;

    private void Update()
    {
        if (arkanoidManager.isPaused)
        {
            return;
        }
        
        for (int i = 0; i < createBricks.bricks.Count; i++)
        {
            Vector3 pos = createBricks.bricks[i].transform.localPosition;
            pos.x -= speed * Time.deltaTime;

            if (pos.x <= leftLimit)
            {
                pos.x = resetX;
            }

            createBricks.bricks[i].transform.localPosition = pos;
        }
    }
}
