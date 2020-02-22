using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    NavMeshAgent PlayerMeshAgent;
    [SerializeField]
    Camera MiniMapCamera;
    [SerializeField]
    Animation SwordAttack;
    [SerializeField]
    Transform HealthBar;
    private float HealthPoints;
    private const float MoveSpeed = 0.05f;
    private const float RotationSpeed = 2.40f;
    // Use this for initialization
    void Start()
    {
        HealthPoints = 10;
        PlayerMeshAgent = GetComponent<NavMeshAgent>();
        HealthBar.transform.localScale = new Vector3(HealthPoints / 10, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        //Will move with player but at a set height and at a locked rotation.
        //Showing entire map on minimap proved to make things too small to see :)
        MiniMapCamera.transform.position = new Vector3(this.transform.position.x, 24.5f, this.transform.position.z);
        if (Input.GetMouseButtonDown(0))  //Left click for main screen
        {
            PlayerMeshAgent.isStopped = false;
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                if (Vector3.Distance(this.transform.position, hit.transform.position) < 3)
                {
                    if (hit.transform.tag == "Enemy")
                    {
                        if (!SwordAttack.isPlaying) //Stops attack spam.
                        {
                            SwordAttack.Play();
                            hit.transform.GetComponent<EnemyAI>().Damage(4);
                        }
                    }
                }
                else
                {
                    PlayerMeshAgent.destination = hit.point;
                }
            }
        }
        else if (Input.GetMouseButtonDown(1)) //Right click for minimap
        {
            PlayerMeshAgent.isStopped = false;
            RaycastHit hit;

            if (Physics.Raycast(MiniMapCamera.ScreenPointToRay(Input.mousePosition), out hit, 100))
            {
                PlayerMeshAgent.destination = hit.point;
            }
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            if (!SwordAttack.isPlaying)
            {
                SwordAttack.Play();
                //SwordColliderScript will handle the damage dealt 
            }
        }
        else if (Input.GetKeyDown(KeyCode.E))
        {
            Collider[] collidersinrange = Physics.OverlapSphere(transform.position, 5);
            foreach(Collider col in collidersinrange)
            {
                if(col.transform.tag == "Enemy"){
                    Rigidbody rb = col.GetComponent<Rigidbody>();
                    if(rb != null)
                    {
                        rb.AddExplosionForce(10, transform.position, 5, 0, ForceMode.Impulse);
                    }
                }
            }
        }
        else if (Input.GetKey(KeyCode.W))
        {
            PlayerMeshAgent.isStopped = true;
            transform.position += transform.forward * MoveSpeed;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            PlayerMeshAgent.isStopped = true;
            transform.position -= transform.forward * MoveSpeed;
        }
        if (Input.GetKey(KeyCode.A))
        {
            PlayerMeshAgent.isStopped = true;
            transform.position -= transform.right * MoveSpeed;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            PlayerMeshAgent.isStopped = true;
            transform.position += transform.right * MoveSpeed;
        }
        if (Input.GetKey(KeyCode.Alpha1) || Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(0, -RotationSpeed, 0);
        }
        else if (Input.GetKey(KeyCode.Alpha2) || Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(0, RotationSpeed, 0);
        }
    }
    public void Damage(int damagedealt)
    {
        HealthPoints -= damagedealt;
        if(HealthPoints <= 0)
        {
            HealthPoints = 0;
            SceneManager.LoadScene("GameOver");
        }
        HealthBar.transform.localScale = new Vector3(HealthPoints / 10, HealthBar.transform.localScale.y, HealthBar.transform.localScale.z);
    }
    public void Reset()
    {
        transform.position = new Vector3(0, 0, 0);
    }
}