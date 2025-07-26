using UnityEngine;
using UnityEngine.Events;
using com.cyborgAssets.inspectorButtonPro;





public class PlayerAnimations : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Animator animator;
    [SerializeField] private PlayerMovement playerMovement;
    public bool IsWalkingState { get; set; }
    private const string IS_MOVING_ANIM_TAG = "isMoving";
    void Start()
    {
        IsWalkingState = false;
        animator.SetBool(IS_MOVING_ANIM_TAG, IsWalkingState);
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    
    public void BeginToMove()
    {
        IsWalkingState = true;
        animator.SetBool(IS_MOVING_ANIM_TAG, IsWalkingState);
    }

    [ProButton]
    public void StopMoving()
    {
        IsWalkingState = false;
        animator.SetBool(IS_MOVING_ANIM_TAG, IsWalkingState);
    }
}
