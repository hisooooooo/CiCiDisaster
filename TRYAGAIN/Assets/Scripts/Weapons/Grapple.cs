using System.Collections.Generic;
using UnityEngine;

public class Grapple : MonoBehaviour
{
    [SerializeField] InputHandler _input;
    [SerializeField] Camera cam;
    public LayerMask groundLayer;
    public float PullSpeed;
    public float GrappleLength;
    private LineRenderer rope;
    [SerializeField] private Rigidbody2D rb;
    private List<Vector2> points;
    private Vector2 PositionInput;
    private Vector2 hitPoint;

    private void Awake()
    {
        rope = GetComponent<LineRenderer>();
        rb = GetComponent<Rigidbody2D>();
        cam = Camera.main;
        Vector2 PositionInput = _input.NormalizedDirectionInput;
        rope.positionCount = 0;
    }

    private void Update()
    {
        if (_input.GrappleInput == 1)
        {
            RaycastHit2D hit = Physics2D.Raycast(transform.position, PositionInput, GrappleLength, groundLayer);
            if (hit.collider != null)
            {
                hitPoint = hit.point;
                rope.positionCount = 2;
            }
            
        }
        if (hitPoint != null)
        {
            drawRope();
            rb.MovePosition(Vector2.MoveTowards(transform.position, hitPoint, Time.deltaTime * PullSpeed));
        }
        if (_input.GrappleInput == 3)
        {
            detach();
        }
    }

    private void detach()
    {
        hitPoint = new Vector2(0, 0);
    }

    private void drawRope()
    {
        if (rope.positionCount <= 0) return;
        rope.SetPosition(index: 0, transform.position);
        rope.SetPosition(index: 1, hitPoint);
    }
}