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
    public float followWalkDistance = 40f;
    public float followRunDistance = 60f;
    public float decisionInterval = 1f;
    public float petCooldown = 2f;

    /**
    * Handles / redirects the logic for each state that Sam can be in
    */
    void HandleState()
    {
        // Override if the player starts moving
        if (player.IsMoving()) {
            currentState = DogState.Follow;
        }

        switch (currentState)
        {
            case DogState.Follow:
                Follow();
                // Set the animator
                break;

            case DogState.Wander:
                Wander();
                // Set the animator (probably the same animation as DogState.Follow?)
                break;

            case DogState.Wait:
                StopPathfinding();
                // Set the animator
                break;

            case DogState.BeingPet:
                StopPathfinding();
                // Set the animator
                break;
        }
    }

    /*
    * If Sam is too far from the player based on the public float attributes, return true
    */
    bool IsTooFarFromPlayer(float followDistance) {
        Vector2 playerPosition = player.gameObject.transform.position;
        float distance = Vector2.Distance(transform.position, playerPosition);

        return distance > followDistance;
    }

    /*
    * Logic for Sam following the player
    */
    void Follow()
    {
        if (player == null) return;

        if (IsTooFarFromPlayer(followWalkDistance))
        {
            // Adjust speed based on distance, basically if falling too far behind, run faster
            // (probably will be important to adjust when we allow the player to run/walk faster)
            if (IsTooFarFromPlayer(followRunDistance)) {
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

    /*
    * Logic for random wandering
    * Instead of selecting a random node to pathfind to, it selects a random direction to move in
    */
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

    /*
    * Logic for when Sam is pet by the player
    */
    void HandlePet() {
        if (petTimer >= petCooldown) {
            // I am thinking its ok to just pathfind the player to the dog instead of both since it wouldn't really make sense for both of them to 
            player.PathfindTo(transform.position);
            currentState = DogState.BeingPet;

            // Reset timer
            petTimer = 0f;

            // Do animation stuff and then set the currentState to something else to reset
            // Maybe call animation setBool logic in HandleState() under the BeingPet case?
            // Something like that idk, whatever works
            // - adam
        }
    }

    /*
    * Randomizer for what state to be in when the player isn't moving
    */
    void MakeStateDecision()
    {
        decisionTimer = 0f;
        wanderTarget = Vector2.zero;

        // For now just randomly pick between Wait and Wander
        int randomChoice = Random.Range(0, 2);

        if (randomChoice == 0)
            currentState = DogState.Wander;
        else
            currentState = DogState.Wait;
    }

    void IncrementTimers() {
        decisionTimer += Time.deltaTime;
        petTimer += Time.deltaTime;
    }

    void Start()
    {
        player = PlayerControl.Instance;

        currentState = DogState.Follow;
    }

    public override void Update()
    {
        base.Update();

        IncrementTimers();

        if (decisionTimer >= decisionInterval && currentState != DogState.BeingPet && !player.IsMoving() && !IsTooFarFromPlayer(followWalkDistance))
        {
            MakeStateDecision();
        }

        HandleState();
    }

    void OnMouseUp() {
        HandlePet();
    }
}
