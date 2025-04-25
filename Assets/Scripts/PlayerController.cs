using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlayerController : MovementController
{
    [SerializeField] private Grid grid;
    [SerializeField] private WorldManager worldManager;
    [SerializeField] private float timeToMove = 0.4f;
    [SerializeField] private float timeToTurn = 0.1f;
    private bool isMoving;
    private bool isButtonDown;
    private Vector3Int input;

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            isButtonDown = false;
        }

        if (isMoving)
            return;

        float moveX = Input.GetAxis("Horizontal");
        float moveY = Input.GetAxis("Vertical");

        input.x = Mathf.RoundToInt(moveX);
        input.y = Mathf.RoundToInt(moveY);

        if ((input.x != 0 || input.y != 0) && !isButtonDown)
        {
            StartCoroutine(MovePlayer(input));
        }
        else if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            isButtonDown = true;
        }
        else if (isButtonDown)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = 0f;

            Vector3 direction = (mousePosition - transform.position).normalized;

            if (direction.x != 0f || moveY != 0f)
            {
                input.x = Mathf.RoundToInt(direction.x);
                input.y = Mathf.RoundToInt(direction.y);

                StartCoroutine(MovePlayer(input));
            }
        }
    }

    private IEnumerator MovePlayer(Vector3Int direction)
    {
        isMoving = true;

        string dir = GetDirection(direction);

        if (direction != lastDirection) //turn first
        {
            //https://discussions.unity.com/t/vector2-angle-how-do-i-get-if-its-cw-or-ccw/101180/5
            bool clockwise = Mathf.Sign(lastDirection.x * direction.y - lastDirection.y * direction.x) <= 0;
            int nrOfTurns = Mathf.RoundToInt(Vector3.Angle(direction, lastDirection) / 45f);

            string[] turns = GetTurns(GetDirection(lastDirection), nrOfTurns, clockwise);
            
            for (int i = 0; i < nrOfTurns; i++)
            {
                animator.PlayTurnAnimation(turns[i]);
                yield return new WaitForSeconds(timeToTurn);
            }
        }
        else
        {
            float elapsedTime = 0f;

            Vector3Int gridPosition = grid.WorldToCell(transform.position);

            startPosition = transform.position;
            targetPosition = grid.CellToWorld(gridPosition + direction);

            if (!worldManager.IsPositionWalkable(targetPosition))
            {
                isMoving = false;
                yield break;
            }

            if (direction.x != 0 && direction.y != 0) //diagonal movement
            {
                Vector3 checkPosition = grid.CellToWorld(gridPosition + new Vector3Int(direction.x, 0, 0));

                if (worldManager.IsPositionWalkable(checkPosition))
                {
                    checkPosition = grid.CellToWorld(gridPosition + new Vector3Int(0, direction.y, 0));
                    
                    if (!worldManager.IsPositionWalkable(checkPosition))
                    {
                        isMoving = false;
                        yield break;
                    }
                }
                else
                {
                    isMoving = false;
                    yield break;
                }
            }

            animator.PlayWalkAnimation(dir);

            //TODO: Fix diagonal movement
            while (elapsedTime < timeToMove)
            {
                transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / timeToMove);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
        }

        lastDirection = direction;
        animator.PlayIdleAnimation(dir);

        isMoving = false;
    }

    public void Terraform()
    {
        if (isMoving)
        {
            return;
        }

        if (lastDirection.x != 0 && lastDirection.y != 0)
            return;

        worldManager.Terraform(transform.position + new Vector3(lastDirection.x * grid.cellSize.x, lastDirection.y * grid.cellSize.y, 0), 0);
    }
}