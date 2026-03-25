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
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    public DogState currentState;
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
                break;

            case DogState.Wander:
                Wander();
                break;

            case DogState.Wait:
                StopPathfinding();
                break;

            case DogState.BeingPet:
                StopPathfinding();
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
                SetCurrentSpeed(maxSpeed);
            } else {
                SetCurrentSpeed(minSpeed);
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

        animator.SetBool("walk", true);
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
            wanderTarget = new Vector2Int(Random.Range(-1, 2), Random.Range(-1, 2));
            Debug.Log(wanderTarget);
        } else {
            WalkTowards(wanderTarget, Time.deltaTime);
        }

        animator.SetBool("walk", true);
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

            animator.SetBool("pet", true);
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

    void FlipSprite(Vector2 direction) {
        spriteRenderer.flipX = direction.x > 0;
    }

    void Start()
    {
        player = PlayerControl.Instance;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        currentState = DogState.Follow;
    }

    public override void Update()
    {
        base.Update();

        IncrementTimers();

        if (decisionTimer >= decisionInterval && currentState != DogState.BeingPet && !player.IsMoving())
        {
            MakeStateDecision();
        }

        HandleState();
    }

    public override void StopPathfinding()
    {
        base.StopPathfinding();
        animator.SetBool("walk", false);
    }

    public override Vector2 WalkTowards(Vector2 targetDirection, float delta)
    {
        FlipSprite(targetDirection);
        return base.WalkTowards(targetDirection, delta);
    }

    public override bool PathfindTo(Vector2 target)
    {
        FlipSprite(target);
        return base.PathfindTo(target);
    }

    void OnMouseUp() {
        HandlePet();
    }
}
