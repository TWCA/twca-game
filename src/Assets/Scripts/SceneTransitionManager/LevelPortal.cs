using UnityEngine;

public class LevelPortal : MonoBehaviour
{
    public string LevelToLoad;
    public Vector2 PortalPosition; // Where the player actually walks to in the final animation
    public string DialogKnot = ""; // Leave blank if no dialogue to be shown
    private TransitionController transitionController;

    void Start()
    {
        transitionController = TransitionController.Instance;
        Vector3 position = transform.position + (Vector3)GetComponent<BoxCollider2D>().offset;
        transitionController.RegisterLevelPortal(position, PortalPosition);
    }

    // Handle when the player reaches the level portal trigger
    void OnCollisionEnter2D(Collision2D collision)
    {
        GameObject collisionObject = collision.gameObject;
        if (collisionObject != null && collision.gameObject.CompareTag("Player")) {
            PlayerControl playerControl = collisionObject.GetComponent<PlayerControl>();
            PathFollower pathFollower = collisionObject.GetComponent<PathFollower>();
            AudioManager.Instance.EndofLevel();
            DoFinalMove(playerControl, pathFollower);
        }
    }

    // The last walk to the exit / portal / door that the player does
    private void DoFinalMove(PlayerControl playerControl, PathFollower pathFollower) {
        playerControl.CanMove = false;
        pathFollower.PathfindTo(PortalPosition);
        pathFollower.DonePathing += () => StartCoroutine(transitionController.SwitchScenes(LevelToLoad, DialogKnot));
    }
}
