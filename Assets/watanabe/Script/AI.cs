using UnityEngine;
using UnityEngine.AI;
using System.Collections;


public class AI : MonoBehaviour
{
    public GameObject dairi;
    public Transform[] points;
    private int destPoint = 0;
    private NavMeshAgent agent;
    private int alpha; //ランダム

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();

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
        agent.destination = points[destPoint].position;

        // Choose the next point in the array as the destination,
        // cycling to the start if necessary.
        //destPoint = (destPoint + 1) % points.Length; //リセット
        destPoint = (destPoint + alpha) % points.Length;
    }

    void Update()
    {
        // Choose the next destination point when the agent gets
        // close to the current one.
        if (!agent.pathPending && agent.remainingDistance < 0.5f) //条件成立で次のポジションに行く
            GotoNextPoint();
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.tag == "Ogre")
        {
            dairi.GetComponent<OgreAI>().SetObject(this.gameObject);
        }
        else
        {
            Update();
        }
    }

}