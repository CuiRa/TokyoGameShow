using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class AIscrpt_alpha : MonoBehaviour
{

    public AKT_flag ATK;
    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private int alpha; //ランダム
    private GameObject terget;
   

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.autoBraking = false;

        alpha = Random.Range(0, points.Length);

        GotoNextPoint();
    }


    void GotoNextPoint()
    {
 
        if (points.Length == 0)
            return;

        destPoint = (destPoint + alpha) % points.Length;
        agent.destination = points[destPoint].position;
 
    }

    void Update()
    {
       
        if (!agent.pathPending && agent.remainingDistance < 1.5f) //条件成立で次のポジションに行く
            GotoNextPoint();

    }
  

    void OnTriggerStay(Collider other)
    {
        
        if(this.tag == "Ogre" && other.gameObject.tag == "Player")
        {

            agent.destination = other.transform.position;

        }

    }

}