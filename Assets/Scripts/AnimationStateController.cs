using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("isWalking");
    private static readonly int IsRunning = Animator.StringToHash("isRunning");
    private Animator _animator;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();        
    }

    // Update is called once per frame
    private void Update()
    {
        var isWalking = _animator.GetBool(IsWalking);
        var isRunning = _animator.GetBool(IsRunning);
        var forwardPressed = Input.GetKey(KeyCode.W);
        var runPressed = Input.GetKey(KeyCode.LeftShift);
        
        if (isWalking == false && forwardPressed)
        {
            _animator.SetBool(IsWalking, true);
        }
        if (isWalking && forwardPressed == false)
        {
            _animator.SetBool(IsWalking, false);
        }

        if (isRunning == false && (forwardPressed && runPressed))
        {
            _animator.SetBool(IsRunning, true);
        }

        if (isRunning && (forwardPressed == false || runPressed == false))
        {
            _animator.SetBool(IsRunning, false);;
        }
    }
}
