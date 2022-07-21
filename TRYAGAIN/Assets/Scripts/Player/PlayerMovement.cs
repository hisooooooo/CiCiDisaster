
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	private PlayerStates playerState;
    Rigidbody2D rb;
	Animator anim;
    [SerializeField] InputHandler _input;
	
    Vector2 _moveInput;
	private float _jumpInput;
	private bool IsFacingRight;


	[Header("Movement")]
	public float coyoteTime;
	private float coyoteCounter;
    public float maxRunSpeed;
    public float jumpPower;
	public float runAccel;
	public float runDecel;
	public float airAccel;
	public float airDecel;
	public float stopPower;
	public float turnPower;
	public float accelPower;
	public float wallJumpPowerX;
	public float wallJumpPowerY;
	public Vector2 slidePower;
	public Vector2 AirDashPower;

	[Header("Ability Counts")]
	public float wallJumpCount;
	public float SetAirJumpCount;
	public float AirJumpCount;
	public float airDashCount;

	float mostRecentJump;
	float slideDuration =1;

	[Header("Checks")]
	public Transform ledgeCheckHigh;
	public Transform ledgeCheckMid;
	public float ledgeCheckSize;
	public LayerMask groundLayer;


	[Header("Ledges")]
	private Vector2 ledgePosBot;
	private Vector2 ledgePos1;
	private Vector2 ledgePos2;
	public float ledgeClimbXOffset1 = 0f;
	public float ledgeClimbXOffset2 = 0f;
	public float ledgeClimbYOffset1 = 0f;
	public float ledgeClimbYOffset2 = 0f;
	private bool ledgeDetected;
	private bool isTouchingLedge;
	private bool isTouchingWall;

	
	//Enable/Disable Functions
	private bool canMove = true;
	private bool canJump = true;
	private bool canAirJump = false;
	private bool canWallJump = true;
	private bool canClimbLedge = false;
	private bool canAirDash = true;

	//Action States
	private bool isAirJumping = false;
	private bool isAirDashing = false;



	private float mostRecentWallJump = 0;
	private float mostRecentAirJump = 0;




	[Header("Cooldowns")]
	public float JumpCD;
	private float JumpCDTimer = 10;
	public float WallJumpCD;
	private float WallJumpCDTimer = 10;
	public float airJumpCD;
	private float airJumpCDTimer = 10;
	public float slideCD;
	private float slideCDTimer = 10;
	public float AirDashCD;
	private float AirDashCDTimer = 10;



	private void Awake()
    {
		AirJumpCount = SetAirJumpCount;
		IsFacingRight = true;
		anim = GetComponent<Animator>();
		playerState = GetComponent<PlayerStates>();
        rb = GetComponent<Rigidbody2D>();
		
		mostRecentJump = 0;
	

	}

	private void Update()
	{
		//Resets, CDs, & Timers
		CDTimers();
		GroundedTimersResets();
		AirborneTimersResets();

		if (slideDuration >= 1)
		{
			runDecel = 5;
		}

		anim.SetBool("AirJump", isAirJumping);
		anim.SetBool("AirDash", isAirDashing);
	}
    private void FixedUpdate()
    {
		//Input Movement&Abilities
		CheckInputsGround();
		CheckInputsAir();
		CheckLedgeFunctions();
	}

	private void CheckInputsGround()
    {
		_moveInput = new Vector2(_input.MoveInput.x, _input.MoveInput.y);

		if (_input.MoveInput.x != 0)
			CheckDirectionToFace(_input.MoveInput.x > 0);

		if (!playerState.isSliding() && canMove)
		{
			Run(1);
		}

		if (playerState.isGrounded() && _input.SlideInput != 0 && rb.velocity.x != 0 && slideCDTimer > slideCD)
		{
			Slide();
			slideCDTimer = 0;
			slideDuration = 0;

		}

		if (coyoteCounter >= 0 && _input.JumpInput && canJump && JumpCDTimer > JumpCD && rb.velocity.y <= 0)
		{
			Jump();
			mostRecentJump = 1;
			airJumpCDTimer = 0;
			canAirJump = true;
		}

	}

	private void CheckInputsAir()
    {
		if (!playerState.isGrounded() && playerState.onWallRight() && _input.JumpInput || !playerState.isGrounded() && playerState.onWallLeft() && _input.JumpInput)
		{

			if (wallJumpCount > 0 && WallJumpCDTimer > WallJumpCD && canWallJump)
			{
				WallJump();
				wallJumpCount -= 1;
				WallJumpCDTimer = 0;
				airJumpCDTimer = 0;
				canAirJump = true;
				

			}
		}

		if (!playerState.isGrounded() && AirJumpCount > 0 && _input.JumpInput && canAirJump && airJumpCDTimer > airJumpCD)
		{
			AirJump();
			mostRecentAirJump = 1;
			isAirJumping = true;
			WallJumpCDTimer = 0;
		}

		if (!playerState.isGrounded() && _input.AirDashInput != 0 && airDashCount != 0 && !ledgeDetected && AirDashCDTimer > AirDashCD)
		{
			BeginAirDash();
		}

	}

	private void CheckLedgeFunctions()
    {
		if (IsFacingRight)
		{
			isTouchingWall = Physics2D.Raycast(ledgeCheckMid.position, transform.right, ledgeCheckSize, groundLayer);
			isTouchingLedge = Physics2D.Raycast(ledgeCheckHigh.position, transform.right, ledgeCheckSize, groundLayer);
		}
		if (!IsFacingRight)

		{
			isTouchingWall = Physics2D.Raycast(ledgeCheckMid.position, transform.right, -ledgeCheckSize, groundLayer);
			isTouchingLedge = Physics2D.Raycast(ledgeCheckHigh.position, transform.right, -ledgeCheckSize, groundLayer);
		}


		if (isTouchingWall && !isTouchingLedge && !ledgeDetected)
		{
			ledgeDetected = true;
			ledgePosBot = ledgeCheckMid.position;
		}

		if (!isAirDashing)
		{
			BeginLedgeClimb();
		}

	}

	private void CDTimers()
    {
		slideCDTimer += Time.deltaTime;
		slideDuration += Time.deltaTime;
		JumpCDTimer += Time.deltaTime;
		airJumpCDTimer += Time.deltaTime;
		WallJumpCDTimer += Time.deltaTime;
		AirDashCDTimer += Time.deltaTime;
	}

	private void GroundedTimersResets()
    {
		if (playerState.isGrounded())
		{
			wallJumpCount = 1;
			mostRecentJump = 0;
			mostRecentWallJump = 0;
			AirJumpCount = SetAirJumpCount;
			canAirJump = false;
			mostRecentAirJump = 0;
			isAirJumping = false;
			airDashCount = 1;
			canWallJump = true;
			coyoteCounter = coyoteTime;
			canAirDash = false;
			AirDashCDTimer = 0;
			WallJumpCDTimer = 0;

			//CD timers
		}
	}

	private void AirborneTimersResets()
    {
		if (!playerState.isGrounded())
		{
			coyoteCounter -= Time.deltaTime;
			canAirJump = true;
			canAirDash = true;
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

	public void Jump()
	{
		
		JumpCDTimer = 0;

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
		
		AirJumpCount = SetAirJumpCount;
		rb.AddForce(force, ForceMode2D.Impulse);
	
		
	}

	private void AirJump()
    {
	
		float force = jumpPower;
		if (rb.velocity.y < 0)
			force -= rb.velocity.y;

		rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
		AirJumpCount -= 1;
		airJumpCDTimer = 0;
        
		
		
	}
	private void endAirJumpAnimation()
	{
		isAirJumping = false;
    }
	private void Slide()
	{
		
		rb.AddForce(slidePower * rb.velocity.x , ForceMode2D.Force);
		runDecel = 0;
	}
	
	private void BeginLedgeClimb()
	{
		if (ledgeDetected && !canClimbLedge && !isAirDashing)
		{
			canAirDash = false;
			canClimbLedge = true;

			if (IsFacingRight)
			{
				ledgePos1 = new Vector2(Mathf.Floor(ledgePosBot.x + ledgeCheckSize) - ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
				ledgePos2 = new Vector2(Mathf.Floor(ledgePosBot.x + ledgeCheckSize) + ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
			}
			if (!IsFacingRight)
			{
				ledgePos1 = new Vector2(Mathf.Ceil(ledgePosBot.x - ledgeCheckSize) + ledgeClimbXOffset1, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset1);
				ledgePos2 = new Vector2(Mathf.Ceil(ledgePosBot.x - ledgeCheckSize) - ledgeClimbXOffset2, Mathf.Floor(ledgePosBot.y) + ledgeClimbYOffset2);
			}
			
			canMove = false;
			canJump = false;
			anim.SetBool("Ledge Climb", canClimbLedge);


		}

		if (canClimbLedge && !isAirDashing)
		{
			rb.gravityScale = 0;
			transform.position = ledgePos1;
		}
	}

	public void FinishLedgeClimb()
	{
        if (!isAirDashing)
        {
		canClimbLedge = false;
		transform.position = ledgePos2;
		canMove = true;
		canJump = true;
		
		ledgeDetected = false;
		anim.SetBool("Ledge Climb", canClimbLedge);
        }

		
		
	}

	public void Freeze()
    {
		rb.velocity = new Vector2(0, 0);
		rb.gravityScale = 0;	
		canMove = false;
		canJump = false;
		canAirJump = false;
		
    }

	public void UnFreeze()
    {
		rb.gravityScale = 2;
		canJump = true;
		canMove = true;
		canAirJump = true;
		isAirDashing = false;
    }

	private Vector2 InputDirectionAtTimeofAction;
	private Vector2 VeloAtAirDashInput;
	public void BeginAirDash()
    {	
		isAirDashing = true;
		VeloAtAirDashInput = rb.velocity;
		InputDirectionAtTimeofAction = _input.NormalizedDirectionInput;
		Freeze();

		airDashCount -= 1;
		airJumpCDTimer = -1;
		mostRecentAirJump = 0;
		mostRecentJump = 0;
	}
	public void AirDash()
    {   
		if(InputDirectionAtTimeofAction.x <= 0 && VeloAtAirDashInput.x >= 0)
        {
			VeloAtAirDashInput.x = -VeloAtAirDashInput.x;
        }
		if (InputDirectionAtTimeofAction.x >= 0 && VeloAtAirDashInput.x <= 0)
		{
			VeloAtAirDashInput.x = -VeloAtAirDashInput.x;
		}
		if (InputDirectionAtTimeofAction.y <= 0 && VeloAtAirDashInput.y >= 0)
		{
			VeloAtAirDashInput.y = -VeloAtAirDashInput.y;
		}
		if (InputDirectionAtTimeofAction.y >= 0 && VeloAtAirDashInput.y <= 0)
		{
			VeloAtAirDashInput.y = -VeloAtAirDashInput.y;
		}
		rb.AddForce((InputDirectionAtTimeofAction * AirDashPower) + VeloAtAirDashInput/2, ForceMode2D.Impulse);
		

    }

	public void endAirDash()
    {
		UnFreeze();
		isAirDashing = false;
    }


}
