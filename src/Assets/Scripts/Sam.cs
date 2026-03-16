using UnityEngine;

public class Dog : PathFollower
{
    public enum DogState
    {
        Follow,
        Wander,
        Wait,
        BeingPet
    }
    private Vector2 wanderTarget = Vector2.zero;
    private PlayerControl player;
    private float decisionTimer;
    private float petTimer;

    public DogState currentState;
    public float walkSpeed = 40f;
    public float runSpeed = 80f;
    public float followDistance = 40f;
    public float followRunFactor = 1.5f;
    public float decisionInterval = 1f;
    public float petCooldown = 2f;

    void Wander()
    {
        // If we don't currently have a wander target, set one
        if (wanderTarget.Equals(Vector2.zero))
        {
            wanderTarget = new Vector2Int(Random.Range(-1, 1), Random.Range(-1, 1));;
        } else {
            WalkTowards(wanderTarget, Time.deltaTime);
        }
    }

    void Start()
    {
        player = PlayerControl.Instance;

        currentState = DogState.Follow;
    }

    void HandleState()
    {
        // Override if the player starts moving
        // if (player.IsMoving() || (TooFarFromPlayer() && currentState != DogState.Wander)) {
        if (player.IsMoving()) {
            currentState = DogState.Follow;
        }

        switch (currentState)
        {
            case DogState.Follow:
                Follow();
                break;

            case DogState.Wander:
                Wander();
                break;

            case DogState.Wait:
                StopPathfinding();
                WaitAnimation();
                break;

            case DogState.BeingPet:
                StopPathfinding();
                PetAnimation();
                break;
        }
    }

    bool TooFarFromPlayer(float factor = 1) {
        Vector2 playerPosition = player.gameObject.transform.position;
        float distance = Vector2.Distance(transform.position, playerPosition);

        return distance > (followDistance * factor);
    }

    void Follow()
    {
        if (player == null) return;

        if (TooFarFromPlayer())
        {
            if (TooFarFromPlayer(followRunFactor)) {
                speed = runSpeed;
            } else {
                speed = walkSpeed;
            }

            if (!IsPathfinding())
            {
                Vector2 playerPosition = player.gameObject.transform.position;
                PathfindTo(playerPosition);
            }
        }
        else
        {
            StopPathfinding();
        }
    }

    void MakeStateDecision()
    {
        decisionTimer = 0f;
        wanderTarget = Vector2.zero;

        if (currentState == DogState.BeingPet || player.IsMoving() || TooFarFromPlayer()) return;

        // For now just randomly pick between Wait and Wander
        int randomChoice = Random.Range(0, 2);

        if (randomChoice == 0)
            currentState = DogState.Wander;
        else
            currentState = DogState.Wait;
    }

    public override void Update()
    {
        base.Update();

        decisionTimer += Time.deltaTime;
        petTimer += Time.deltaTime;

        if (decisionTimer >= decisionInterval)
        {
            MakeStateDecision();
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
