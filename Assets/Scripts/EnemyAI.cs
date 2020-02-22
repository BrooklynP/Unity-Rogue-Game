using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour {
    NavMeshAgent EnemyAINavMeshAgent;

    const float SightRange = 10.0f;

    float HealthPoints;
    float AttackCooldown;
    Vector3 PatrolPosition; //The position the enemy will prefer to hold
    Vector3 TargetPosition; //The position the enemy is planning to go to
    ENEMYSTATE State; //the current state of the enemy, decides how the enemy will act
                      // Use this for initialization

    GameObject Player;
    Transform FleePoint1;
    Transform FleePoint2;
    Transform FleePoint3;
    Transform FleePoint4;

    GameObject LastKnownPosOrb;
    Transform Healthbar;
    LevelGenerator LevelGeneratorScript;
    GameManagerScript GameManagerScript;

    GameObject LastKnownPos;

    RaycastHit hit; //Used to raycast between enemy and player
	void Start () {
        AttackCooldown = 0.0f;
        Player = GameObject.Find("PlayerBody");
        FleePoint1 = GameObject.Find("FleePoint 1").transform;
        FleePoint2 = GameObject.Find("FleePoint 2").transform;
        FleePoint3 = GameObject.Find("FleePoint 3").transform;
        FleePoint4 = GameObject.Find("FleePoint 4").transform;
        HealthPoints = 10;
        Healthbar = gameObject.transform.GetChild(0); //Only one child on enemy so easiest to reference it this way
        Healthbar.transform.localScale = new Vector3(HealthPoints / 10, Healthbar.transform.localScale.y, Healthbar.transform.localScale.z);    
        this.transform.tag = "Enemy";
        LastKnownPosOrb = (GameObject)Resources.Load("Last Known Position Orb", typeof(GameObject));
        LevelGeneratorScript = GameObject.Find("Level Generator").GetComponent<LevelGenerator>();
        GameManagerScript = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        EnemyAINavMeshAgent = GetComponent<NavMeshAgent>();
        State = ENEMYSTATE.ENEMYSTATE_IDLE;
        transform.position = LevelGeneratorScript.ReturnRandomVector();
        PatrolPosition = transform.position;
    }
	
	// Update is called once per frame
	void Update () {
        //regardlss of current state, if the enemy reaches less than 3 hp, it should flee
        if(HealthPoints < 3 && State != ENEMYSTATE.ENEMYSTATE_FLEE)
        {
            State = ENEMYSTATE.ENEMYSTATE_PICKFLEEPOINT;
        }
        if (State != ENEMYSTATE.ENEMYSTATE_ALERT && LastKnownPos != null)
        {
            //if the enemy is no longer investigating the last known pos, delete the marker
            GameObject.Destroy(LastKnownPos);
        }
        switch (State)
        {
            case ENEMYSTATE.ENEMYSTATE_IDLE:
                {
                    if (Vector3.Distance(transform.position, Player.transform.position) < SightRange) //Player is close enough to enemy to see
                    {
                        if ((Physics.Raycast(transform.position, (Player.transform.position - transform.position), out hit, SightRange)))
                        {
                            if (hit.collider.tag == "Player")
                            {
                                //The enemy can see the player
                                //Upon seeing the player, it will move towards them, if they lose sight of the player, they will continue to the last known position of the player.
                                //TargetPosition = Player.transform.position;
                                State = ENEMYSTATE.ENEMYSTATE_FOLLOW;
                            }
                        }
                    }
                    break;
                }
            case ENEMYSTATE.ENEMYSTATE_FOLLOW:
                {
                    if ((Physics.Raycast(transform.position, (Player.transform.position - transform.position), out hit, SightRange)))
                    {
                        if (hit.collider.tag == "Player")
                        {
                            TargetPosition = Player.transform.position;
                            if(Vector3.Distance(transform.position, Player.transform.position) < 2)
                            {
                                //if close enough to the player, attack!
                                State = ENEMYSTATE.ENEMYSTATE_ATTACK;
                            }
                        }
                        else
                        {
                            //Creates an orb to let the player know where the AI says they last saw you
                            LastKnownPos = Instantiate(LastKnownPosOrb, position: TargetPosition, rotation:Quaternion.identity);
                            State = ENEMYSTATE.ENEMYSTATE_ALERT;
                        }
                    }
                    EnemyAINavMeshAgent.destination = TargetPosition;
                    
                    break;
                }
            case ENEMYSTATE.ENEMYSTATE_ALERT:
                {
                    //continue to the last known target position, and look out for the player
                    if ((Physics.Raycast(transform.position, (Player.transform.position - transform.position), out hit, SightRange)))
                    {
                        if (hit.collider.tag == "Player")
                        {
                            TargetPosition = Player.transform.position;
                            State = ENEMYSTATE.ENEMYSTATE_FOLLOW;
                        }
                    }
                    EnemyAINavMeshAgent.destination = TargetPosition;
                    
                    //if the enemy gets to last known player position and cannot see the player, he will return to the patrol positon.
                    if (Vector3.Distance(TargetPosition, transform.position) < 1)
                    {
                        State = ENEMYSTATE.ENEMYSTATE_RETURN;
                    }
                    break;
                }
            case ENEMYSTATE.ENEMYSTATE_RETURN:
                {
                    //look out for the player
                    if ((Physics.Raycast(transform.position, (Player.transform.position - transform.position), out hit, SightRange)))
                    {
                        if (hit.collider.tag == "Player")
                        {
                            TargetPosition = Player.transform.position;
                            State = ENEMYSTATE.ENEMYSTATE_FOLLOW;
                        }
                        else
                        {
                            TargetPosition = PatrolPosition;
                        }
                    }
                    EnemyAINavMeshAgent.destination = TargetPosition;
                    break;
                }
            case ENEMYSTATE.ENEMYSTATE_PICKFLEEPOINT:
                {
                    //target postion should be a random one of 4 flee points.
                    //int randomNum = Random.Range(1, 5);
                    //switch (randomNum)
                    //{
                    //    case 1:
                    //        {
                    //            TargetPosition = FleePoint1.position;
                    //            State = ENEMYSTATE.ENEMYSTATE_FLEE;
                    //            break;
                    //        }
                    //    case 2:
                    //        {
                    //            TargetPosition = FleePoint2.position;
                    //            State = ENEMYSTATE.ENEMYSTATE_FLEE;
                    //            break;
                    //        }
                    //    case 3:
                    //        {
                    //            TargetPosition = FleePoint3.position;
                    //            State = ENEMYSTATE.ENEMYSTATE_FLEE;
                    //            break;
                    //        }
                    //    case 4:
                    //        {
                    //            TargetPosition = FleePoint4.position;
                    //            State = ENEMYSTATE.ENEMYSTATE_FLEE;
                    //            break;
                    //        }
                    //    default:
                    //        {
                    //            TargetPosition = FleePoint1.position;
                    //            State = ENEMYSTATE.ENEMYSTATE_FLEE;
                    //            break;
                    //        }
                    //}
                    //Debug.Log(randomNum);
                    TargetPosition = LevelGeneratorScript.ReturnRandomVector();
                    State = ENEMYSTATE.ENEMYSTATE_FLEE;
                    EnemyAINavMeshAgent.destination = TargetPosition;
                    break;
                }
            case ENEMYSTATE.ENEMYSTATE_FLEE:
                {
                    EnemyAINavMeshAgent.destination = TargetPosition;
                    break;
                }
            case ENEMYSTATE.ENEMYSTATE_ATTACK:
                {
                    if (Vector3.Distance(transform.position, Player.transform.position) >= 2)
                    {
                        AttackCooldown = 0.0f;
                        State = ENEMYSTATE.ENEMYSTATE_FOLLOW;
                    }
                    else
                    {
                        AttackCooldown += Time.deltaTime;
                        if(AttackCooldown >= 2)
                        {
                            AttackCooldown = 0.0f;
                            Player.GetComponent<PlayerController>().Damage(2);
                        }
                    }
                    break;
                }
            default:
                {
                    break;
                }
        }
    }

    public bool Damage(int DamageDealt)//returns whether the attack killed the unit or not
    {
        HealthPoints -= DamageDealt;
        Healthbar.transform.localScale = new Vector3(HealthPoints / 10, Healthbar.transform.localScale.y, Healthbar.transform.localScale.z);
        if (HealthPoints <= 0)
        {
            GameObject.Destroy(this.gameObject);
            GameManagerScript.DecreaseEnemyCount();
            return true;
        }
        else
        {
            return false;
        }
    }
}

enum ENEMYSTATE
{
    ENEMYSTATE_ATTACK, //hits player
    ENEMYSTATE_FOLLOW, //follows player
    ENEMYSTATE_ALERT, //Loses sight of player, continues onward in search
    ENEMYSTATE_RETURN, //returns to its patrol position
    ENEMYSTATE_PICKFLEEPOINT, //picks a location to flee to
    ENEMYSTATE_FLEE, //runs away to 1 of 4 flee points (randomly chosen)
    ENEMYSTATE_IDLE,    //Stays in one place
    ENEMYSTATE_COUNT    //number of possible states
};
