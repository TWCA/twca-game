using UnityEngine;

public class Dog : MonoBehaviour
{
    public enum DogState
    {
        Follow,
        Wander,
        Wait,
        BeingPet
    }
    private Vector2 wanderTarget;
    private bool hasWanderTarget = false;
    private PathFollower pathFollower;
    private PlayerControl player;
    private float decisionTimer;
    private float petTimer;

    public DogState currentState;
    public float followDistance = 4f;
    public float wanderRadius = 5f;
    public float decisionInterval = 3f;
    public float petCooldown = 2f;

    void Wander()
    {
        if (pathFollower == null) return;

        // If we don't currently have a wander target, pick one
        if (!hasWanderTarget)
        {
            Vector2 randomOffset = UnityEngine.Random.insideUnitCircle * wanderRadius;
            wanderTarget = (Vector2)transform.position + randomOffset;

            hasWanderTarget = pathFollower.PathfindTo(wanderTarget);
        }

        // If finished walking, reset so we pick a new target next time
        if (!pathFollower.IsPathfinding())
        {
            hasWanderTarget = false;
        }
    }

    void Start()
    {
        pathFollower = GetComponent<PathFollower>();
        player = PlayerControl.Instance;

        currentState = DogState.Follow;
    }

    void HandleState()
    {
        switch (currentState)
        {
            case DogState.Follow:
                Follow();
                break;

            case DogState.Wander:
                Wander();
                break;

            case DogState.Wait:
                pathFollower.StopPathfinding();
                WaitAnimation();
                break;

            case DogState.BeingPet:
                pathFollower.StopPathfinding();
                PetAnimation();
                break;
        }
    }

    void Follow()
    {
        if (pathFollower == null || player == null) return;

        Vector2 playerPosition = player.gameObject.transform.position;
        float distance = Vector2.Distance(transform.position, playerPosition);

        if (distance > followDistance)
        {
            if (!pathFollower.IsPathfinding())
            {
                pathFollower.PathfindTo(playerPosition);
            }
        }
        else
        {
            pathFollower.StopPathfinding();
        }
    }

    void MakeStateDecision()
    {
        // Don't do anything else if the dog is being pet
        if (currentState == DogState.BeingPet) return;

        Vector2 playerPosition = player.gameObject.transform.position;
        float distance = Vector2.Distance(transform.position, playerPosition);

        if (distance > followDistance)
        {
            currentState = DogState.Follow;
            return;
        }

        // For now just randomly pick between Wait and Wander
        int randomChoice = Random.Range(0, 2);

        if (randomChoice == 0)
            currentState = DogState.Wander;
        else
            currentState = DogState.Wait;
    }

    void Update()
    {
        decisionTimer += Time.deltaTime;
        petTimer += Time.deltaTime;

        if (decisionTimer >= decisionInterval)
        {
            MakeStateDecision();
            decisionTimer = 0f;
        }

        HandleState();
    }

    void OnMouseUp() {
        if (petTimer >= petCooldown) {
            Debug.Log("Pet sam");
            player.PathfindTo(transform.position);
            currentState = DogState.BeingPet;

            // Reset timer
            petTimer = 0f;

            // Do animation stuff and then set the currentState to something else to reset
            // Maybe call animation logic in HandleState() under the BeingPet case?
            // Something like that idk, whatever works
            // - adam
        }
    }

    void WaitAnimation() {
        // Do wait animation
    }

    void PetAnimation() {
        // Do animation when being pet
    }
}
