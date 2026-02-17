using UnityEngine;

public class Dog : MonoBehaviour
{
    public enum DogState
    {
        Follow,
        Wander,
        Wait
    }

    public DogState currentState;

    private PathFollower pathFollower;
    private Transform player;

    public float followDistance = 4f;
    public float wanderRadius = 5f;
    public float decisionInterval = 3f;

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
            break;

        case DogState.Wait:
            pathFollower.StopPathfinding();
            break;
    }
}
void Follow()
{
    if (pathFollower == null || player == null) return;

    pathFollower.PathfindTo(player.position);
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
