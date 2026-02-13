using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    public string LevelToLoad;
    public Vector2 PortalPosition; // Where the player actually walks to in the final animation
    private TransitionController transitionController;

    void Start()
    {
        transitionController = TransitionController.Instance;
        transitionController.RegisterLevelPortal(transform.position, PortalPosition);
    }

    // Handle when the player reaches the level portal trigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (collisionObject != null && collision.gameObject.GetComponent<PlayerControl>()) {
            PlayerControl playerControl = collisionObject.GetComponent<PlayerControl>();
            PathFollower pathFollower = collisionObject.GetComponent<PathFollower>();

            DoFinalMove(playerControl, pathFollower);
        }
    }

    // The last walk to the exit / portal / door that the player does
    private void DoFinalMove(PlayerControl playerControl, PathFollower pathFollower) {
        playerControl.AllowControl = false;
        pathFollower.PathfindTo(PortalPosition);
        pathFollower.DonePathing += () => transitionController.SwitchScenes(LevelToLoad);
    }
}
