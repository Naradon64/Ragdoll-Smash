using UnityEngine;

public class Animator_Skeleton_Behavior : MonoBehaviour
{
    private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check inputs or conditions to set the mode
        if (Input.GetKey(KeyCode.W))
        {
            SetMode(1); // Walking
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            SetMode(2); // Attacking
        }
        else
        {
            SetMode(0); // Idle
        }
    }

    // Method to set the mode in the Animator
    void SetMode(int mode)
    {
        if (animator != null)
        {
            animator.SetInteger("mode", mode);
        }
    }
}
