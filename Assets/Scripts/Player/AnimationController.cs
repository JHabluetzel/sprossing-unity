using UnityEngine;

public class AnimationController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private string currentAnimation;

    public void PlayIdleAnimation(string direction)
    {
        string animation = $"Player_Idle{direction}";
        PlayAnimation(animation);
    }

    public void PlayTurnAnimation(string direction)
    {
        string animation = $"Player_Turn{direction}";
        PlayAnimation(animation);
    }

    public void PlayWalkAnimation(string direction)
    {
        string animation = $"Player_Move{direction}";
        PlayAnimation(animation);
    }

    private void PlayAnimation(string animation)
    {
        if (animation != currentAnimation)
        {
            animator.Play(animation);
            currentAnimation = animation;
        }
    }
}