using UnityEngine;

public class CharacterRagdollAndAnimation : MonoBehaviour
{
    Animator playerAnim;
    Vector3 lastPosition; // Stores the previous frame's position
    float movementThreshold = 0.01f; // Minimum movement to consider the character moving

    // Start is called before the first execution of Update
    void Awake()
    {
        playerAnim = GetComponent<Animator>();
        lastPosition = transform.position; // Initialize last position
    }

    // Update is called once per frame
    void Update()
    {
        // Calculate the distance moved since the last frame
        float movement = Vector3.Distance(transform.position, lastPosition);

        // Check if the movement is above the threshold
        if (movement > movementThreshold)
        {
            Debug.Log("Character is moving, triggering walking animation");
            playerAnim.SetBool("Walking", true);
            playerAnim.SetBool("Female_Idle", false);
        }
        else
        {
            Debug.Log("Character is idle, triggering idle animation");
            playerAnim.SetBool("Walking", false);
            playerAnim.SetBool("Female_Idle", true);
        }

        // Update last position for the next frame
        lastPosition = transform.position;
    }
}
