using UnityEngine;

public class Dog : MonoBehaviour
{
    public enum DogState
    {
        Follow,
        Wander,
        Wait,
        BeingPet,
        WaitingForPet
    }
    private Vector2 wanderTarget = Vector2.zero;
    private PlayerControl player;
    private float decisionTimer;
    private Animator animator;
    private SpriteRenderer spriteRenderer;
    private PlayerDetector playerDetector;
    private PathFollower pathFollower;
    private float urgeToFollow = 0;

    public DogState currentState;
    public float followWalkDistance = 40f;
    public float followRunDistance = 60f;
    public float delayBeforeFollowing = 0.4f;
    public float decisionInterval = 1f;
    public float petCooldown = 2f;
    public int wanderOdds = 5; // For example "1 in (this value) chance of happening"
    public float wanderMin = 40;
    public float wanderMax = 60;
    public float petPadding = 10f;

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
        if (player.IsMoving() && IsTooFarFromPlayer(followWalkDistance))
        {
            urgeToFollow = Mathf.MoveTowards(urgeToFollow, delayBeforeFollowing * 2, Time.deltaTime);
        }
        else
        {
            urgeToFollow = Mathf.MoveTowards(urgeToFollow, 0, Time.deltaTime);
        }

        if (urgeToFollow > delayBeforeFollowing)
        {
            currentState = DogState.Follow;
        }

        switch (currentState)
        {
            case DogState.Follow:
                FollowState();
                break;

            case DogState.Wander:
                WanderState();
                break;

            case DogState.Wait:
                WaitingState();
                break;

            case DogState.BeingPet:
                PetState();
                break;

            case DogState.WaitingForPet:
                WaitingState();
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
    void FollowState()
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
    void WanderState()
    {
        // If we don't currently have a wander target, set one
        if (wanderTarget.Equals(Vector2.zero))
        {
            wanderTarget = RandomWanderPosition();
            pathFollower.PathfindTo(wanderTarget);
        }

        FlipSprite(wanderTarget);

        animator.SetBool("walk", true);
    }

    /*
    * Logic for waiting states
    */
    void WaitingState() {
        StopPathfinding();
    }

    /*
    * Logic for pet state
    */
    void PetState() {
        animator.SetBool("pet", true);

        Vector2 playerPosition = player.gameObject.transform.position;
        bool shouldFlip = transform.position.x > playerPosition.x;

        spriteRenderer.flipX = shouldFlip;
        player.FlipX(shouldFlip);

        WaitingState();
    }

    /*
    * Randomizer for what state to be in when the player isn't moving
    */
    void MakeStateDecision()
    {
        decisionTimer = 0f;
        wanderTarget = Vector2.zero;

        if (currentState != DogState.BeingPet && currentState != DogState.WaitingForPet && !animator.GetBool("pet") && !player.IsMoving()) {
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

    /*
    * Returns 1 or -1
    */
    int RandomSign() {
        return Random.Range(0, 2) * 2 - 1;
    }

    /*
    * Generates a random position to path to for random wandering
    */
    Vector2 RandomWanderPosition() {
        Vector2 playerPosition = player.GetPosition();

        float xWander = Random.Range(wanderMin, wanderMax) * RandomSign();
        float yWander = Random.Range(wanderMin, wanderMax) * RandomSign();

        // Debug.Log(xWander);
        // Debug.Log(yWander);

        return new Vector2(playerPosition.x + xWander, playerPosition.y + yWander);
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
        if (player.GoingToSam) {
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
        currentState = DogState.Wait;
    }

    void OnMouseUp() {
        float padding;
        Vector2 playerPosition = player.GetPosition();

        if (transform.position.x > playerPosition.x) {
            padding = -petPadding;
        } else {
            padding = petPadding;
        }

        Vector2 adjustedPosition = new(transform.position.x + padding, transform.position.y);

        // Track if the player is walking to Sam
        player.GoingToSam = true;

        player.PathfindTo(adjustedPosition);

        currentState = DogState.WaitingForPet;

        if (playerDetector.TouchingPlayer) {
            OnPlayerSamInteraction();
        }
    }
}
