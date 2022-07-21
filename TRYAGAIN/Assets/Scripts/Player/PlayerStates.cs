using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStates : MonoBehaviour
{
    PlayerMovement movement;
    

    Rigidbody2D rb;
    BoxCollider2D boxCollider;
    [SerializeField] InputHandler _input;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private LayerMask wallLayer;
    [SerializeField] float currentMaxVelocity;
    [SerializeField] float currentMaxSlideVelo;
    public Transform wallCheckRight;
    public Transform wallCheckLeft;
    public float wallCheckSize;
    public Transform groundCheck;
    public float groundCheckRadius;
    private Animator anim;
    private float facing;
    

    private void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        boxCollider = GetComponent<BoxCollider2D>();
    }

    private void Update()
    {
        facing = transform.localScale.x;
        
        anim.SetBool("Grounded", isGrounded());
        anim.SetBool("Run", isRunning());
        anim.SetBool("Slide", isSliding());



    }
    
    public bool isGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

    }
    public bool onWallRight()
    {
        if (facing > 0)
        {
         return Physics2D.OverlapCircle(wallCheckRight.position, wallCheckSize, wallLayer);
        }
        return false;
    }
  

    public bool onWallLeft()
    {

        bool rightWallCheck = Physics2D.OverlapCircle(wallCheckRight.position, wallCheckSize, wallLayer);
        if (rightWallCheck && facing < 0)
        {
        return true;
        }
        return false;
            
    }

  

    public bool isRunning()
    {
        if (rb.velocity.x != 0 && isGrounded() && !isSliding())
        {
            return true;
        }
            return false;
    }   

    public bool isSliding()
    { 
        if(isGrounded() && _input.SlideInput == 1 && rb.velocity.x > currentMaxVelocity || isGrounded() && _input.SlideInput == 1 && rb.velocity.x  <currentMaxVelocity * -1)
        {
            return true;
        }
            return false;
    }

    /*public bool isSlideEnding()
    {
        if (isGrounded() && !isRunning()  && rb.velocity.x < currentMaxVelocity + 3 && rb.velocity.x > currentMaxVelocity || isGrounded() && !isRunning()  && rb.velocity.x > (currentMaxVelocity * -1) -3 && rb.velocity.x < currentMaxVelocity * -1)
        {
            return true;
        }
        return false;
    }
    */

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);


        Gizmos.DrawWireSphere(wallCheckRight.position, wallCheckSize);
        

       
    }
}
