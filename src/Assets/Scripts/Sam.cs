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
    private PlayerDetector playerDetector;

    public DogState currentState;
    public float followWalkDistance = 40f;
    public float followRunDistance = 60f;
    public float decisionInterval = 1f;
    public float petCooldown = 2f;
    public int wanderOdds = 5; // For example "1 in (this value) chance of happening"
    public bool ClickToPet;

    /**
    * Handles / redirects the logic for each state that Sam can be in
    */
    void HandleState()
    {
        // Override if the player starts moving
        if (player.IsMoving() && currentState == DogState.BeingPet && IsTooFarFromPlayer(followWalkDistance)) {
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
                Wait();
                break;

            case DogState.BeingPet:
                Wait();
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
        Vector2 playerPosition = player.gameObject.transform.position;

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
                PathfindTo(playerPosition);
            }
        }
        else
        {
            StopPathfinding();
        }

        animator.SetBool("walk", true);
        FlipSprite(playerPosition);
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
        } else {
            WalkTowards(wanderTarget, Time.deltaTime);
        }

        animator.SetBool("walk", true);
    }

    /*
    * Logic for waiting states
    */
    void Wait() {
        if (player.IsMoving() && IsTooFarFromPlayer(followWalkDistance)) {
            currentState = DogState.Follow;
        }

        StopPathfinding();
    }

    /*
    * Logic for when Sam is pet by the player
    */
    void HandlePet() {
        if (petTimer >= petCooldown) {
            if (currentState == DogState.BeingPet) {
                currentState = DogState.Wait;

                animator.SetBool("pet", false);
            } else {
                player.PathfindTo(transform.position);
                currentState = DogState.BeingPet;

                animator.SetBool("pet", true);
            }

            // Reset timer
            petTimer = 0f;
        }
    }

    /*
    * Randomizer for what state to be in when the player isn't moving
    */
    void MakeStateDecision()
    {
        decisionTimer = 0f;
        wanderTarget = Vector2.zero;

        if (currentState != DogState.BeingPet && !animator.GetBool("pet") && !player.IsMoving()) {
            int randomChoice = Random.Range(0, wanderOdds);

            if (randomChoice == 0)
                currentState = DogState.Wander;
            else
                currentState = DogState.Wait;
        }
    }

    void IncrementTimers() {
        decisionTimer += Time.deltaTime;
        petTimer += Time.deltaTime;
    }

    void FlipSprite(Vector2 psotion) {
        spriteRenderer.flipX = psotion.x > transform.position.x;
    }

    void Start()
    {
        player = PlayerControl.Instance;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerDetector = GetComponentInChildren<PlayerDetector>();

        playerDetector.PlayerTouched += OnPlayerSamInteraction;
        playerDetector.PlayerLeft += OnPlayerLeft;

        currentState = DogState.Follow;
    }

    public override void Update()
    {
        base.Update();

        IncrementTimers();

        if (decisionTimer >= decisionInterval)
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
        FlipSprite((Vector2)transform.position + targetDirection);
        return base.WalkTowards(targetDirection, delta);
    }

    void OnPlayerSamInteraction() {
        if (ClickToPet == false || player.GoingToSam) {
            currentState = DogState.BeingPet;
            animator.SetBool("pet", true);

            player.PetSam(true);
        }
    }

    void OnPlayerLeft() {
        animator.SetBool("pet", false);

        player.PetSam(false);
    }

    void OnMouseUp() {
        player.GoingToSam = true;
    }
}
