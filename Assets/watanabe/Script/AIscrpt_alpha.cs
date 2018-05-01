using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class AIscrpt_alpha : MonoBehaviour
{

    public Area area;
    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private int alpha; //ランダム
    private GameObject terget;
    private float speed = 3.5f;
   

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;

        // Disabling auto-braking allows for continuous movement
        // between points (ie, the agent doesn't slow down as it
        // approaches a destination point).
        agent.autoBraking = false;

        alpha = Random.Range(0, points.Length);

        GotoNextPoint();
    }


    void GotoNextPoint()
    {
        // Returns if no points have been set up
        if (points.Length == 0)
            return;

        // Set the agent to go to the currently selected destination.
        // agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        //destPoint = (destPoint + 1) % points.Length; //リセット
        destPoint = (destPoint + alpha) % points.Length;
        agent.destination = points[destPoint].position;
       // return; 
    }

    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 1.5f) //条件成立で次のポジションに行く
            GotoNextPoint();

        if (this.tag == "Ogre")
        {
            agent.speed = 4.0f;
            if (area != null && area.tag == "Player")
            {
                agent.destination = area.pos;
            }
        }
        else if (this.tag == "Player")
        {
            if (area.tag == "Ogre")
            {
                agent.speed = 5.0f;
            }
            else
            {
                agent.speed = 3.5f;
            }
        }

    }

    void OnTriggerEnter(Collider other)
    {
        if (this.tag == "Ogre" && other.gameObject.tag == "Player")
        {
            Debug.Log("HIT");
            Destroy(other.gameObject);
        }
    }


}