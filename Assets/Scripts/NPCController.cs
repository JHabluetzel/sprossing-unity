using System.Collections;
using UnityEngine;

public class NPCController : MovementController
{
    [SerializeField] private float timeToMove = 0.4f;
    [SerializeField] private float timeToTurn = 0.1f;
    [SerializeField] private Transform target;
    private Vector3[] path;
    private bool isBusy;

    private void Update()
    {
        if (!isBusy)
        {
            isBusy = true;
            PathRequestManager.RequestPath(transform.position, target.position, OnPathFound);
        }
    }

    private void OnPathFound(Vector3[] path, bool wasSuccess)
    {
        if (wasSuccess)
        {
            this.path = path;

            if (path.Length > 1)
            {
                StartCoroutine(FollowPath());
            }
        }
    }

    IEnumerator FollowPath()
    {
        for (int i = 0; i < path.Length - 1; i++)
        {
            startPosition = transform.position;
            targetPosition = path[i];

            Vector3 pathDirection = targetPosition - startPosition;
            Vector3Int direction = new Vector3Int(Mathf.RoundToInt(pathDirection.x), Mathf.RoundToInt(pathDirection.y), 0);
            string dir = GetDirection(direction);

            if (direction != lastDirection) //turn first
            {
                //https://discussions.unity.com/t/vector2-angle-how-do-i-get-if-its-cw-or-ccw/101180/5
                bool clockwise = Mathf.Sign(lastDirection.x * direction.y - lastDirection.y * direction.x) <= 0;
                int nrOfTurns = Mathf.RoundToInt(Vector3.Angle(direction, lastDirection) / 45f);

                string[] turns = GetTurns(GetDirection(lastDirection), nrOfTurns, clockwise);

                for (int j = 0; j < nrOfTurns; j++)
                {
                    animator.PlayTurnAnimation(turns[j]);
                    yield return new WaitForSeconds(timeToTurn);
                }
            }

            float elapsedTime = 0f;

            animator.PlayWalkAnimation(dir);

            while (elapsedTime < timeToMove)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            lastDirection = direction;
        }

        Vector3 checkDirection = path[path.Length - 1] - transform.position;
        Vector3Int lookDirection = new Vector3Int(Mathf.RoundToInt(checkDirection.x), Mathf.RoundToInt(checkDirection.y), 0);
        string look = GetDirection(lookDirection);

        if (lookDirection != lastDirection)
        {
            bool clockwise = Mathf.Sign(lastDirection.x * lookDirection.y - lastDirection.y * lookDirection.x) <= 0;
            int nrOfTurns = Mathf.RoundToInt(Vector3.Angle(lookDirection, lastDirection) / 45f);

            string[] turns = GetTurns(GetDirection(lastDirection), nrOfTurns, clockwise);

            for (int j = 0; j < nrOfTurns; j++)
            {
                animator.PlayTurnAnimation(turns[j]);
                yield return new WaitForSeconds(timeToTurn);
            }

            lastDirection = lookDirection;
        }

        animator.PlayIdleAnimation(GetDirection(lastDirection));
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.black;

        if (path != null && path.Length > 0)
        {
            for (int i = 0; i < path.Length; i++)
            {
                Gizmos.DrawCube(path[i], Vector3.one * 0.25f);
            }
        }
    }
}