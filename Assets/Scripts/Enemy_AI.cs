using UnityEngine;
using UnityEngine.AI;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public float distance;
    public Transform Player;
    public NavMeshAgent navMeshAgent;
    // public Animator Anim;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(this.transform.position, Player.position);
        if(distance < 10){
            navMeshAgent.destination = Player.position;
        }

        // if(navMeshAgent.velocity.magnitude > 1){
        //     Anim.SetInteger("Mode", 1);
        // }
        // else {
        //     if(distance > 5){
        //         Anim.SetInteger("Mode", 0);
        //     }
        // }
    }
}
