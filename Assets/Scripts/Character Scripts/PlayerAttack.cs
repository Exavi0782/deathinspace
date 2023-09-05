using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    Animator animator;
    [SerializeField] float attcakTimer;

    int isAttackHash;
    int attackComboHash;

    float noAttackTimer;

    int buttonClickCount = 0;
    
    void Start()
    {
        animator = GetComponent<Animator>();

        isAttackHash = Animator.StringToHash("isAttack");
        attackComboHash = Animator.StringToHash("Attack Combo");
    }

    
    void Update()
    {
        bool leftMouseButton = Input.GetMouseButtonDown(0);
        bool rightMouseButton = Input.GetMouseButton(1);

        if (rightMouseButton)
        {
            animator.SetBool(isAttackHash, true);
            if (leftMouseButton)
            {
                noAttackTimer = attcakTimer;
                buttonClickCount++;

                if (buttonClickCount > 4) buttonClickCount = 0;

                animator.SetInteger(attackComboHash, buttonClickCount);


            } 
            if (!leftMouseButton)
                noAttackTimer -= Time.deltaTime;

            if (noAttackTimer < 0)
            {
                buttonClickCount = 0;
                animator.SetInteger(attackComboHash, buttonClickCount);
            }
        } else animator.SetBool(isAttackHash, false);

    }
}
