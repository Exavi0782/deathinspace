using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    Animator animator;
    float velocityX = 0.0f;
    float velocityZ = 0.0f;
    public float acceleration = 2.0f;
    public float deceleration = 2.0f;
    public float maximumWalkVelocity = 0.5f;
    public float maximumRunVelocity = 2.0f;

    bool isJumping;
    bool isGrounded;

    int VelocityXHash;
    int VelocityZHash;
    int StayHash;

    void Start()
    {
        animator = GetComponent<Animator>();

        VelocityXHash = Animator.StringToHash("Velocity X");
        VelocityZHash = Animator.StringToHash("Velocity Z");
        StayHash = Animator.StringToHash("isStay");

    }

    void ChangeVelocity(bool forwardPressed, bool backPressed, bool leftPressed, bool rightPressed, bool runPressed, 
        float currentMaxVelocity)
    {
        if (forwardPressed && velocityZ < currentMaxVelocity)
            velocityZ += Time.deltaTime * acceleration;

        if (leftPressed && velocityX > -currentMaxVelocity)
            velocityX -= Time.deltaTime * acceleration;

        if (rightPressed && velocityX < currentMaxVelocity)
            velocityX += Time.deltaTime * acceleration;

        if (!forwardPressed && velocityZ > 0.0f)
            velocityZ -= Time.deltaTime * deceleration;

        if (leftPressed && velocityX > -currentMaxVelocity)
            velocityX -= Time.deltaTime * acceleration;

        if (rightPressed && velocityX < currentMaxVelocity)
            velocityX += Time.deltaTime * acceleration;

        if (!backPressed && velocityZ < 0.0f)
            velocityZ += Time.deltaTime * deceleration;

        if (backPressed && velocityZ > -currentMaxVelocity)
            velocityZ -= Time.deltaTime * acceleration;
    }

    void LockOrResetVelocity(bool forwardPressed, bool backPressed, bool leftPressed, bool rightPressed, bool runPressed,
        float currentMaxVelocity)
    {
        if (forwardPressed && velocityZ < currentMaxVelocity)
            velocityZ += Time.deltaTime * acceleration;

        if (!forwardPressed && velocityZ > 0.0f)
            velocityZ -= Time.deltaTime * deceleration;

        if (backPressed && velocityZ > -currentMaxVelocity)
            velocityZ -= Time.deltaTime * acceleration;

        if (!backPressed && velocityZ < 0.0f)
            velocityZ += Time.deltaTime * deceleration;

        if (!leftPressed && velocityX < 0.0f)
            velocityX += Time.deltaTime * deceleration;

        if (!rightPressed && velocityX > 0.0f)
            velocityX -= Time.deltaTime * deceleration;

        if (!leftPressed && !rightPressed && velocityX != 0.0f && (velocityX > -0.05f && velocityX < 0.05f))
            velocityX = 0.0f;

        if (forwardPressed && runPressed && velocityZ > currentMaxVelocity)
            velocityZ = currentMaxVelocity;
        else if (forwardPressed && velocityZ > currentMaxVelocity)
        {
            velocityZ -= Time.deltaTime * deceleration;
            if (velocityZ > currentMaxVelocity && velocityZ < (currentMaxVelocity + 0.05f))
                velocityZ = currentMaxVelocity;
        }
        else if (forwardPressed && velocityZ < currentMaxVelocity && velocityZ > (currentMaxVelocity - 0.05f))
            velocityZ = currentMaxVelocity;

        if (backPressed && runPressed && velocityZ > -currentMaxVelocity)
            velocityZ -= currentMaxVelocity * Time.deltaTime;
        else if (backPressed && velocityZ < -currentMaxVelocity)
        {
            velocityZ += Time.deltaTime * deceleration;
            if (velocityZ < -currentMaxVelocity && velocityZ > (currentMaxVelocity + 0.05f))
                velocityZ -= currentMaxVelocity * Time.deltaTime;
        }
        else if (backPressed && velocityZ > -currentMaxVelocity && velocityZ < (currentMaxVelocity - 0.05f))
            velocityZ -= currentMaxVelocity * Time.deltaTime;

    }

    bool StayChek(bool forwardPressed, bool backPressed, bool leftPressed, bool rightPressed)
    {
        if (forwardPressed || backPressed || leftPressed || rightPressed)
            return false;
        else return true;
    }
    void Update()
    {
        bool forwardPressed = Input.GetKey(KeyCode.W);
        bool leftPressed = Input.GetKey(KeyCode.A);
        bool rightPressed = Input.GetKey(KeyCode.D);    
        bool runPressed = Input.GetKey(KeyCode.LeftShift);
        bool backPressed = Input.GetKey(KeyCode.S);

        float currentMaxVelocity = runPressed ? maximumRunVelocity : maximumWalkVelocity;
        
        ChangeVelocity(forwardPressed, backPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);
        LockOrResetVelocity(forwardPressed, backPressed, leftPressed, rightPressed, runPressed, currentMaxVelocity);

        animator.SetFloat(VelocityZHash, velocityZ);
        animator.SetFloat(VelocityXHash, velocityX);
        animator.SetBool(StayHash, StayChek(forwardPressed, backPressed, leftPressed, rightPressed));
    }
}
