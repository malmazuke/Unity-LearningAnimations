using UnityEngine;

public class TwoDimensionalAnimationStateController : MonoBehaviour
{
    [SerializeField] private float acceleration = 1.0f;
    [SerializeField] private float deceleration = 5.0f;

    private static readonly int VelocityX = Animator.StringToHash("VelocityX");
    private static readonly int VelocityZ = Animator.StringToHash("VelocityZ");
    private Animator _animator;
    private float _velocityX;
    private float _velocityZ;
    private const float MaximumWalkVelocity = 0.5f;
    private const float MaximumRunVelocity = 2.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        _animator = GetComponent<Animator>();        
    }

    // Update is called once per frame
    private void Update()
    {
        var forwardPressed = Input.GetKey(KeyCode.W);
        var leftPressed = Input.GetKey(KeyCode.A);
        var rightPressed = Input.GetKey(KeyCode.D);
        var runPressed = Input.GetKey(KeyCode.LeftShift);
        
        var currentMaxVelocity = runPressed ? MaximumRunVelocity : MaximumWalkVelocity;
        
        // Handle forward/backward movement
        UpdateForwardVelocity(forwardPressed, currentMaxVelocity);
        
        // Handle left/right movement
        UpdateHorizontalVelocity(leftPressed, rightPressed, currentMaxVelocity);
        
        // Apply velocity adjustment for precision near zero
        ApplyDeadzone(leftPressed, rightPressed);
        
        // Final adjustments for run/walk transitions
        AdjustVelocityForRunningState(forwardPressed, runPressed, currentMaxVelocity);
        
        // Apply velocities to animator
        _animator.SetFloat(VelocityX, _velocityX);
        _animator.SetFloat(VelocityZ, _velocityZ);
    }
    
    private void UpdateForwardVelocity(bool forwardPressed, float maxVelocity)
    {
        if (forwardPressed && _velocityZ < maxVelocity)
        {
            _velocityZ = AccelerateTowards(_velocityZ, maxVelocity, acceleration);
        }
        else if (!forwardPressed && _velocityZ > 0f)
        {
            _velocityZ = DecelerateTowards(_velocityZ, 0f, deceleration);
        }
        else if (!forwardPressed)
        {
            _velocityZ = 0f;
        }
    }
    
    private void UpdateHorizontalVelocity(bool leftPressed, bool rightPressed, float maxVelocity)
    {
        // Handle left movement
        if (leftPressed && _velocityX > -maxVelocity)
        {
            _velocityX = AccelerateTowards(_velocityX, -maxVelocity, acceleration);
        }
        else if (!leftPressed && _velocityX < 0f)
        {
            _velocityX = DecelerateTowards(_velocityX, 0f, deceleration);
        }
        
        // Handle right movement
        if (rightPressed && _velocityX < maxVelocity)
        {
            _velocityX = AccelerateTowards(_velocityX, maxVelocity, acceleration);
        }
        else if (!rightPressed && _velocityX > 0f)
        {
            _velocityX = DecelerateTowards(_velocityX, 0f, deceleration);
        }
    }
    
    private void ApplyDeadzone(bool leftPressed, bool rightPressed)
    {
        const float deadzone = 0.05f;
        
        if (!leftPressed && !rightPressed && _velocityX != 0f && _velocityX is > -deadzone and < deadzone)
        {
            _velocityX = 0f;
        }
    }
    
    private void AdjustVelocityForRunningState(bool forwardPressed, bool runPressed, float maxVelocity)
    {
        const float snapThreshold = 0.05f;

        // Adjust Z velocity (forward/backward)
        if (forwardPressed)
        {
            if (runPressed && _velocityZ > maxVelocity)
            {
                // If running and over max velocity, snap down to max
                _velocityZ = maxVelocity;
            }
            else if (_velocityZ > maxVelocity)
            {
                // If not running but still over max velocity, gradually slow down
                _velocityZ = DecelerateTowards(_velocityZ, maxVelocity, acceleration);
                    
                // Snap to max velocity if we're close
                if (_velocityZ > maxVelocity && _velocityZ < maxVelocity + snapThreshold)
                {
                    _velocityZ = maxVelocity;
                }
            }
            else if (_velocityZ < maxVelocity && _velocityZ > maxVelocity - snapThreshold)
            {
                // If we're close to max velocity, snap to it
                _velocityZ = maxVelocity;
            }
        }
        
        // Adjust X velocity (horizontal movement)
        // Check if we need to decelerate from running to walking speed
        if (Mathf.Abs(_velocityX) > maxVelocity)
        {
            // If we're over max velocity and not running, gradually slow down
            if (_velocityX > 0)
            {
                _velocityX = DecelerateTowards(_velocityX, maxVelocity, acceleration);
                
                // Snap to max velocity if we're close
                if (_velocityX > maxVelocity && _velocityX < maxVelocity + snapThreshold)
                {
                    _velocityX = maxVelocity;
                }
            }
            else // _velocityX < 0
            {
                _velocityX = DecelerateTowards(_velocityX, -maxVelocity, acceleration);
                
                // Snap to negative max velocity if we're close
                if (_velocityX < -maxVelocity && _velocityX > -maxVelocity - snapThreshold)
                {
                    _velocityX = -maxVelocity;
                }
            }
        }
        // If running, make sure we don't exceed max velocity
        else if (runPressed)
        {
            // We don't need additional logic here since UpdateHorizontalVelocity already handles acceleration
            // up to the current maxVelocity value
        }
    }
    
    private float AccelerateTowards(float currentValue, float targetValue, float rate)
    {
        var delta = Time.deltaTime * rate;
        
        // Accelerate towards the target without overshooting
        if (currentValue < targetValue)
        {
            currentValue += delta;
            return Mathf.Min(currentValue, targetValue);
        }
        // For negative acceleration (slowing down)
        else if (currentValue > targetValue)
        {
            currentValue -= delta;
            return Mathf.Max(currentValue, targetValue);
        }
        
        return currentValue;
    }
    
    private float DecelerateTowards(float currentValue, float targetValue, float rate)
    {
        var delta = Time.deltaTime * rate;
        
        // Decelerate towards the target without overshooting
        if (currentValue > targetValue)
        {
            currentValue -= delta;
            return Mathf.Max(currentValue, targetValue);
        }
        // For negative deceleration (accelerating in reverse)
        else if (currentValue < targetValue)
        {
            currentValue += delta;
            return Mathf.Min(currentValue, targetValue);
        }
        
        return currentValue;
    }
    
}
