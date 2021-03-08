using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MovingObject : MonoBehaviour
{
    protected virtual void AttempMove<T>(int xDir, int yDir)
        where T : Component
    {
        RaycastHit2D hit;

        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return;
        T hitComponent = hit.transform.GetComponent<T>();
        if (!canMove && hitComponent != null)
            OnCantMove(hitComponent);
        
        // GameController.instance.playerTurn = true;
    }

    public LayerMask blockingLayer;
    public float moveTime = 0.1f;

    BoxCollider2D boxCollider2D;
    Rigidbody2D rb2D;
    float inverseMoveTime;

    protected virtual void Start() {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb2D = GetComponent<Rigidbody2D>();
        inverseMoveTime = 1f / moveTime;
    }

    protected IEnumerator SmoothMovement(Vector3 end) {
        float sqrtRemainDistance = (transform.position - end).sqrMagnitude;

        while (sqrtRemainDistance > float.Epsilon) {
            Vector3 newPos = Vector3.MoveTowards(rb2D.position, end, inverseMoveTime + Time.deltaTime);
            rb2D.MovePosition(newPos);
            sqrtRemainDistance = (transform.position - end).sqrMagnitude;
            yield return null;
        }
        // GameController.instance.playerTurn = true;
    }

    protected abstract void OnCantMove <T> (T component)
        where T : Component;

    protected bool Move(int xDir, int yDir, out RaycastHit2D hit) {
        Vector2 start = transform.position;
        Vector2 end = start + new Vector2(xDir, yDir);

        boxCollider2D.enabled = false;
        hit = Physics2D.Linecast(start, end, blockingLayer);
        boxCollider2D.enabled = true;

        if (hit.transform == null) {
            StartCoroutine(SmoothMovement(end));
            return true;
        }
        return false;
    }
}
