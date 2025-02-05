using UnityEngine;

public class PlayerCollisionHandler : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Weapon")
        {
            Debug.Log($"โดน {collision.gameObject.name} โจมตี");
        }

        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log($"ชน {collision.gameObject.name}");
        }
    }
}
