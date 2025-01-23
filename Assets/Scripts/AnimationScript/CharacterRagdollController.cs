using UnityEngine;

public class CharacterRagdollController : MonoBehaviour
{
    private Rigidbody[] _ragdollRigidbodies;
    private Collider[] _ragdollColliders;
    private Animator _animator;

    private enum CharacterState
    {
        Walking,
        Ragdoll
    }

    private CharacterState _currentState = CharacterState.Walking;

    void Awake()
    {
        // Fetch all Rigidbodies and Colliders in the ragdoll hierarchy
        _ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
        _ragdollColliders = GetComponentsInChildren<Collider>();

        // Get the Animator component
        _animator = GetComponentInChildren<Animator>();

        // Start in walking mode
        SetRagdollState(false);
    }

    void Update()
    {
        // Toggle between walking and ragdoll states with 'R'
        if (Input.GetKeyDown(KeyCode.R))
        {
            ToggleRagdoll();
        }

        // In walking mode, make the character follow the parent object
        if (_currentState == CharacterState.Walking)
        {
            FollowParentPosition();
        }
    }

    private void SetRagdollState(bool isRagdoll)
    {
        // Enable/Disable Animator
        if (_animator != null)
        {
            _animator.enabled = !isRagdoll;
        }

        // Toggle ragdoll physics
        foreach (var rb in _ragdollRigidbodies)
        {
            rb.isKinematic = !isRagdoll;
        }

        // Toggle colliders
        foreach (var col in _ragdollColliders)
        {
            col.enabled = isRagdoll;
        }

        // Update the current state
        _currentState = isRagdoll ? CharacterState.Ragdoll : CharacterState.Walking;
    }

    private void ToggleRagdoll()
    {
        bool enableRagdoll = _currentState == CharacterState.Walking;
        SetRagdollState(enableRagdoll);
    }

    private void FollowParentPosition()
    {
        // Ensure the character's position matches the parent
        transform.position = transform.parent.position;
        transform.rotation = transform.parent.rotation;
    }
}
