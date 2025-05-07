using UnityEngine;

public class AnimationStateController : MonoBehaviour
{
    private static readonly int Velocity = Animator.StringToHash("Velocity");
    [SerializeField] private float acceleration = 1f;
    [SerializeField] private float deceleration = 5f;
    
    private Animator _animator;
    private float _velocity = 0f;
    
    private void Start()
    {
        _animator = GetComponent<Animator>();        
    }

    // Update is called once per frame
    private void Update()
    {
        var forwardPressed = Input.GetKey(KeyCode.W);
        var runPressed = Input.GetKey(KeyCode.LeftShift);

        if (forwardPressed && _velocity <= 1f)
        {
            _velocity += Time.deltaTime * acceleration;
        }
        else if (_velocity > 0f)
        {
            _velocity -= Time.deltaTime * deceleration;
        }
        else
        {
            _velocity = 0f;
        }
        
        _animator.SetFloat(Velocity, _velocity);
    }
}
