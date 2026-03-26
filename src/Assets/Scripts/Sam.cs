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
    private Vector2 wanderTarget = Vector2.zero;
    private PlayerControl player;
    private float decisionTimer;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerDetector playerDetector;
    private PathFollower pathFollower;

    public DogState currentState;
    public float followWalkDistance = 40f;
    public float followRunDistance = 60f;
    public float decisionInterval = 1f;
    public float petCooldown = 2f;
    public int wanderOdds = 5; // For example "1 in (this value) chance of happening"
    public bool ClickToPet;

    void Start()
    {
        player = PlayerControl.Instance;
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        playerDetector = GetComponentInChildren<PlayerDetector>();
        pathFollower = GetComponent<PathFollower>();

        playerDetector.PlayerTouched += OnPlayerSamInteraction;
        playerDetector.PlayerLeft += OnPlayerLeft;

        currentState = DogState.Follow;
    }

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
                SetCurrentSpeed(pathFollower.maxSpeed);
            } else {
                SetCurrentSpeed(pathFollower.minSpeed);
            }

            if (!pathFollower.IsPathfinding())
            {
                pathFollower.PathfindTo(playerPosition);
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
            pathFollower.WalkTowards(wanderTarget, Time.deltaTime);
            FlipSprite((Vector2)transform.position + wanderTarget);
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
        if (currentState == DogState.BeingPet) {
            currentState = DogState.Wait;

            animator.SetBool("pet", false);
        } else {
            player.PathfindTo(transform.position);
            currentState = DogState.BeingPet;

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

        if (currentState != DogState.BeingPet && !animator.GetBool("pet") && !player.IsMoving()) {
            int randomChoice = Random.Range(0, wanderOdds);

            if (randomChoice == 0)
                currentState = DogState.Wander;
            else
                currentState = DogState.Wait;
        }
    }

    /*
    * Increments timers related to state decisions
    */
    void IncrementTimers() {
        decisionTimer += Time.deltaTime;
    }

    /*
    * Flips the sprite in the x direction of the position argument
    */
    void FlipSprite(Vector2 position) {
        spriteRenderer.flipX = position.x > transform.position.x;
    }

    public void Update()
    {
        IncrementTimers();

        if (decisionTimer >= decisionInterval)
        {
            MakeStateDecision();
        }

        HandleState();
    }

    public void StopPathfinding()
    {
        pathFollower.StopPathfinding();
        animator.SetBool("walk", false);
    }

    public void SetCurrentSpeed(float value)
    {
        pathFollower.SetCurrentSpeed(value);
        animator.SetFloat("movingSpeed", value / 100f);
    }

    /*
    * Handler for when the player hitbox enters Sam's hitbox
    */
    void OnPlayerSamInteraction() {
        if (ClickToPet == false || player.GoingToSam) {
            currentState = DogState.BeingPet;
            animator.SetBool("pet", true);

            player.PetSam(true);
        }
    }

    /*
    * Handler for when the player hitbox exits Sam's hitbox
    */
    void OnPlayerLeft() {
        animator.SetBool("pet", false);

        player.PetSam(false);
    }

    void OnMouseUp() {
        // Track if the player is walking to Sam
        player.GoingToSam = true;
    }
}
