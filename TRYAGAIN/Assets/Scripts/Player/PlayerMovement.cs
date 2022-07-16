
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private PlayerStates playerState;
    Rigidbody2D rb;
    [SerializeField] InputHandler _input;
	
    Vector2 _moveInput;
	private float _jumpInput;
	private bool IsFacingRight;
	
	

    [Header("Movement")]
    [SerializeField] public float maxRunSpeed;
    [SerializeField] float jumpPower;
	[SerializeField] float runAccel;
	[SerializeField] float runDecel;
	[SerializeField] float airAccel;
	[SerializeField] float airDecel;
	[SerializeField] float stopPower;
	[SerializeField] float turnPower;
	[SerializeField] float accelPower;
	[SerializeField] float wallJumpPowerX;
	[SerializeField] float wallJumpPowerY;
	[SerializeField] Vector2 slidePower;
	
	[SerializeField] float slideCD;
	[SerializeField] float wallJumpCount;
	float mostRecentJump;
	
	float slideCDTimer;
	float slideDuration;

	private Vector2 ledgePosBot;
	private Vector2 ledgePos1;
	private Vector2 ledgePos2;
	public float ledgeClimbXOffset1 = 0f;
	public float ledgeClimbXOffset2 = 0f;
	public float ledgeClimbYOffset1 = 0f;
	public float ledgeClimbYOffset2 = 0f;

	private void Awake()
    {
		IsFacingRight = true;
		playerState = GetComponent<PlayerStates>();
        rb = GetComponent<Rigidbody2D>();
		wallJumpCount = 1;
		mostRecentJump = 0;
		slideCD = 5;
		slideCDTimer = 6;
		slideDuration = 1;

	}

	private void Update()
	{
		if (_input.MoveInput.x != 0)
			CheckDirectionToFace(_input.MoveInput.x > 0);
		if (playerState.isGrounded())
        {
			wallJumpCount = 1;
			mostRecentJump = 0;
        }

		if (mostRecentJump > 0 && !playerState.isGrounded())
        {
			mostRecentJump += Time.deltaTime;
        }

		if (slideDuration >=1 )
        {
			runDecel = 5;
        }

		slideCDTimer += Time.deltaTime;
		slideDuration += Time.deltaTime;


	
		
	}
    private void FixedUpdate()
    {
        _moveInput = new Vector2(_input.MoveInput.x, _input.MoveInput.y);

        if (!playerState.isSliding())
        {
			Run(1);
        }
		
	

		if (playerState.isGrounded() && _input.JumpInput != 0)
        {
			Jump();
			mostRecentJump = 1;
		}
		if(!playerState.isGrounded() && playerState.onWallRight() && _input.JumpInput !=0 || !playerState.isGrounded() && playerState.onWallLeft() && _input.JumpInput != 0)
        {
	
			if(wallJumpCount > 0 && mostRecentJump > 1.3)
            {
			WallJump();
				wallJumpCount -= 1;
			}
        }
		if(playerState.isGrounded() && _input.SlideInput == 1 && rb.velocity.x !=0 && slideCDTimer > slideCD)
        {
			Slide();
			slideCDTimer = 0;
			slideDuration = 0;
			
        }
		
	}



	private void Turn()
	{
		Vector3 scale = transform.localScale; 
		scale.x *= -1;
		transform.localScale = scale;

		IsFacingRight = !IsFacingRight;
	}

	

	public void CheckDirectionToFace(bool isMovingRight)
	{
		
		if (isMovingRight != IsFacingRight)
			Turn();
	}

	public void Run(float lerpAmount)
	{   
		float targetSpeed = _moveInput.x * maxRunSpeed; 
		float speedDif = targetSpeed - rb.velocity.x; 

		
		float accelRate = 0;

		if (playerState.isGrounded())
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccel : runDecel;
		else
			accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? runAccel * airAccel : runDecel * airDecel;

	
		if (((rb.velocity.x > targetSpeed && targetSpeed > 0.01f) || (rb.velocity.x < targetSpeed && targetSpeed < -0.01f)))/* doKeepRunMomentum)*/ 
		{
			accelRate = 0; //prevent any deceleration from happening, or in other words conserve are current momentum
		}
		
		
		float velPower;
		if (Mathf.Abs(targetSpeed) < 0.01f)
		{
			velPower = stopPower;
		}
		else if (Mathf.Abs(rb.velocity.x) > 0 && (Mathf.Sign(targetSpeed) != Mathf.Sign(rb.velocity.x)))
		{
			velPower = turnPower;
		}
		else
		{
			velPower = accelPower;
		}
		

		// applies acceleration to speed difference, then is raised to a set power so the acceleration increases with higher speeds, finally multiplies by sign to preserve direction
		float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);
		movement = Mathf.Lerp(rb.velocity.x, movement, lerpAmount); // lerp so that we can prevent the Run from immediately slowing the player down, in some situations eg wall jump, dash 

		rb.AddForce(movement * Vector2.right); // applies force force to rigidbody, multiplying by Vector2.right so that it only affects X axis 

		//if (_moveInput.x != 0)
			//CheckDirectionToFace(_input.MoveInput.x > 0);
	

		
	}

	private void Jump()
	{
		

		float force = jumpPower;
		if (rb.velocity.y < 0)
			force -= rb.velocity.y;

		rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		
		
	}

	private void WallJump()
    {
	
		Vector2 force = new Vector2(wallJumpPowerX, wallJumpPowerY);
		float dir;
        if (playerState.onWallLeft())
        {
			dir = 1;
			force.x *= dir;
		}
		if (playerState.onWallRight())
        {
			dir = -1;
			force.x *= dir;
		}

		if (Mathf.Sign(rb.velocity.x) != Mathf.Sign(force.x))
			force.x -= rb.velocity.x;

		if (rb.velocity.y < 0) 
			force.y -= rb.velocity.y;

		rb.AddForce(force, ForceMode2D.Impulse);
		
	}
	private void Slide()
	{
		
		rb.AddForce(slidePower * rb.velocity.x , ForceMode2D.Force);
		runDecel = 0;
	}

	private void ledgeClimb()
    {
        if (playerState.canLedgeClimb())
        {
			if (IsFacingRight)
			{
				ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + 0.01f) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
				ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + 0.01f) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
			}
			else
			{
				ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + 0.01f) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
				ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + 0.01f) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
			}
           
            }
        }
    

}
