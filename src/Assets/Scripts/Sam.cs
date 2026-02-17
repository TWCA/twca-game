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
