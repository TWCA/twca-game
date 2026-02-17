using UnityEngine;

public class DogTestMove : MonoBehaviour
{
    private PathFollower pathFollower;

    void Start()
    {
        pathFollower = GetComponent<PathFollower>();

        Vector2 testTarget = new Vector2(0, 0);
        pathFollower.PathfindTo(testTarget);
    }
}
