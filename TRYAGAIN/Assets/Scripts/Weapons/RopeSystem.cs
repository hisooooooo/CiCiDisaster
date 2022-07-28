using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class RopeSystem : MonoBehaviour
{

    public GameObject ropeHingeAnchor;
    public DistanceJoint2D ropeJoint;
    public Transform crosshair;
    public SpriteRenderer crossHairSprite;
    public PlayerMovement movement;
    public InputHandler _input;
    private bool ropeAttached;
    private Vector2 playerPosition;
    private Rigidbody2D ropeRb;
    private SpriteRenderer ropeHingeAnchorSprite;

    public LineRenderer ropeRenderer;
    public LayerMask ropeLayerMask;
    public float ropeMaxCastDistance = 20f;
    public float reelSpeed;
    public List<Vector2> ropePositions = new List<Vector2>();
    private bool distanceSet;

    public Transform player;




    private void Awake()
    {

        ropeJoint.enabled = false;
        playerPosition = player.transform.position;
        ropeRb = ropeHingeAnchor.GetComponent<Rigidbody2D>();
        ropeHingeAnchorSprite = ropeHingeAnchor.GetComponent<SpriteRenderer>();

       
    }

    private void Update()
    {


        var worldMousePosition =
    Camera.main.ScreenToWorldPoint(new Vector3(_input.UltraRawDirection.x, _input.UltraRawDirection.y, 0f));
        var facingDirection = worldMousePosition - player.transform.position;
        var aimAngle = Mathf.Atan2(facingDirection.y, facingDirection.x);
        if (aimAngle < 0f)
        {
            aimAngle = Mathf.PI * 2 + aimAngle;
        }

        // 4
        var aimDirection = Quaternion.Euler(0, 0, aimAngle * Mathf.Rad2Deg) * Vector2.right;
        // 5
        playerPosition = player.position;


       

        HandleInput(aimDirection);
        UpdateRopePositions();
        Reel();
    }

  private void Reel()
    {
        if (ropeAttached)
        {
            ropeJoint.distance -= reelSpeed; ;
        }
    }

    private void HandleInput (Vector2 aimDirection)
    {
        if (_input.GrappleInput == 2)
        {
            if (ropeAttached) return;
            ropeRenderer.enabled = true;

            var hit = Physics2D.Raycast(playerPosition, aimDirection, ropeMaxCastDistance, ropeLayerMask);

            if (hit.collider !=null)
            {
                ropeAttached = true;
                if(!ropePositions.Contains(hit.point))
                {
                    ropePositions.Add(hit.point);
                    ropeJoint.distance = Vector2.Distance(playerPosition, hit.point);
                    ropeJoint.enabled = true;
                    ropeHingeAnchorSprite.enabled = true;
                }
            }
            else
            {
                ropeRenderer.enabled = false;
                ropeAttached = false;
                ropeJoint.enabled = false;
            }
        }
        if (_input.GrappleInput == 3)
        {
            ResetRope();
        }

    }

    private void ResetRope()
    {
        ropeJoint.enabled = false;
        ropeAttached = false;
        movement.isSwinging = false;
        ropeRenderer.positionCount = 2;
        ropeRenderer.SetPosition(0, player.transform.position);
        ropeRenderer.SetPosition(1, player.transform.position);
        ropePositions.Clear();
        ropeHingeAnchorSprite.enabled = false;
    }

    private void UpdateRopePositions()
    {
        // 1
        if (!ropeAttached)
        {
            return;
        }

        // 2
        ropeRenderer.positionCount = ropePositions.Count + 1;

        // 3
        for (var i = ropeRenderer.positionCount - 1; i >= 0; i--)
        {
            if (i != ropeRenderer.positionCount - 1) // if not the Last point of line renderer
            {
                ropeRenderer.SetPosition(i, ropePositions[i]);

                // 4
                if (i == ropePositions.Count - 1 || ropePositions.Count == 1)
                {
                    var ropePosition = ropePositions[ropePositions.Count - 1];
                    if (ropePositions.Count == 1)
                    {
                        ropeRb.transform.position = ropePosition;
                        if (!distanceSet)
                        {
                            ropeJoint.distance = Vector2.Distance(player.transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                    else
                    {
                        ropeRb.transform.position = ropePosition;
                        if (!distanceSet)
                        {
                            ropeJoint.distance = Vector2.Distance(player.transform.position, ropePosition);
                            distanceSet = true;
                        }
                    }
                }
                // 5
                else if (i - 1 == ropePositions.IndexOf(ropePositions.Last()))
                {
                    var ropePosition = ropePositions.Last();
                    ropeRb.transform.position = ropePosition;
                    if (!distanceSet)
                    {
                        ropeJoint.distance = Vector2.Distance(player.transform.position, ropePosition);
                        distanceSet = true;
                    }
                }
            }
            else
            {
                // 6
                ropeRenderer.SetPosition(i, player.transform.position);
            }
        }
    }




  

}
