using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private GridManager gridManager;
    [SerializeField] private Rigidbody2D body;
    [SerializeField] Rigidbody2D rd;
    public Transform movePoint;

    private Vector3 originalPosition, targetPosition;
    private float movementSpeed = 5f;


    public event EventHandler<OnPlayerMovedArgs> OnPlayerMoved;
    public class OnPlayerMovedArgs : EventArgs
    {
        public Vector3 currentPosition;
        public Vector3 previousPosition;
    }
    public event EventHandler OnUndoPressed;

    // Start is called before the first frame update
    void Start()
    {
        // initializing movePoint (pre mover)
        movePoint.position = transform.position;
        movePoint.parent = null;
    }


    public void Death()
    {
        //Debug.Log("Stepped on painted block");
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.MoveTowards(transform.position, movePoint.position, movementSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, movePoint.position) == 0f)
        {
            if ((Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow)) && IsValidMove(Vector3.up))
            {
                OnPlayerMoved?.Invoke(this, new OnPlayerMovedArgs { currentPosition = movePoint.position + Vector3.up, previousPosition = transform.position });
                movePoint.position += Vector3.up;
            }

            else if ((Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow)) && IsValidMove(Vector3.left))
            {
                OnPlayerMoved?.Invoke(this, new OnPlayerMovedArgs { currentPosition = transform.position + Vector3.left, previousPosition = transform.position });
                movePoint.position += Vector3.left;
            }

            else if ((Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow)) && IsValidMove(Vector3.down))
            {
                OnPlayerMoved?.Invoke(this, new OnPlayerMovedArgs { currentPosition = transform.position + Vector3.down, previousPosition = transform.position });
                movePoint.position += Vector3.down;
            }

            else if ((Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow)) && IsValidMove(Vector3.right))
            {
                OnPlayerMoved?.Invoke(this, new OnPlayerMovedArgs { currentPosition = transform.position + Vector3.right, previousPosition = transform.position });
                movePoint.position += Vector3.right;
            }
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            OnUndoPressed?.Invoke(this, EventArgs.Empty);
        }
    }


    private bool IsValidMove(Vector3 direction)
    {
        Vector3 positionToCheck = transform.position + direction;
        Collider2D collider = GetColliderAt(positionToCheck);
        if (collider.CompareTag("WallTile"))
            return false;
        return true;
    }

    public Collider2D GetColliderAt(Vector3 position)
    {
        RaycastHit2D hit = Physics2D.CircleCast(position, .5f, Vector3.zero);
        if (hit)
        {
            // Debug.Log(hit.collider);
        }
        return hit.collider;
    }
}
