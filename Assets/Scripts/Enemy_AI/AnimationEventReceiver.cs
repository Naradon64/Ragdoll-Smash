using UnityEngine;

public class AnimationEventReceiver : MonoBehaviour
{
    private EnemyAI enemyAI;

    void Start()
    {
        enemyAI = GetComponentInParent<EnemyAI>(); // Find EnemyAI on the parent
    }

    public void EnableSwordCollider()
    {
        if (enemyAI != null)
        {
            enemyAI.EnableSwordCollider();
        }
    }

    public void DisableSwordCollider()
    {
        if (enemyAI != null)
        {
            enemyAI.DisableSwordCollider();
        }
    }
}
