using UnityEngine;

public class Dog : MonoBehaviour
{
    public enum DogState
    {
        Follow,
        Wander,
        Wait
    }
    private Vector2 wanderTarget;
    private bool hasWanderTarget = false;


    public DogState currentState;

    private PathFollower pathFollower;
    private Transform player;

    public float followDistance = 4f;
    public float wanderRadius = 5f;
    public float decisionInterval = 3f;


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


    private float decisionTimer;

    void Start()
    {
        pathFollower = GetComponent<PathFollower>();
        player = GameObject.FindGameObjectWithTag("Player").transform;

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
            break;
    }
}
void Follow()
{
    if (pathFollower == null || player == null) return;

    float distance = Vector2.Distance(transform.position, player.position);

    if (distance > followDistance)
    {
        if (!pathFollower.IsPathfinding())
        {
            pathFollower.PathfindTo(player.position);
        }
    }
    else
    {
        pathFollower.StopPathfinding();
    }
}



void MakeStateDecision()
{
    float distance = Vector2.Distance(transform.position, player.position);

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

        if (decisionTimer >= decisionInterval)
        {
            MakeStateDecision();
            decisionTimer = 0f;
        }

        HandleState();
    }
}
