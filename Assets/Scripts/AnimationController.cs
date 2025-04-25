using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void PlayIdleAnimation(string direction)
    {
        animator.Play($"Player_Idle{direction}");
    }

    public void PlayTurnAnimation(string direction)
    {
        animator.Play($"Player_Turn{direction}");
    }

    public void PlayWalkAnimation(string direction)
    {
        animator.Play($"Player_Move{direction}");
    }
}