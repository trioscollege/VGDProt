using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float m_speed = 10.0f;
    private Vector3 m_dest = Vector3.zero;
    private Vector3 m_dir = Vector3.zero;
    private Vector3 m_nextDir = Vector3.zero;
    private float m_distance = 1f; // look ahead 1 tile

    private List<GameObject> m_ghosts;

    private AudioSource[] m_pacmanSounds;
    private AudioSource m_dotSound;
    private AudioSource m_energizerSound;
    private AudioSource m_deadSound;

    public void Init()
    {
        m_ghosts = new List<GameObject>();
    }

    public void AddGhost(GameObject ghost)
    {
        m_ghosts.Add(ghost);
    }

    private void Start() {
        m_dest = transform.position;
        //m_pacmanSounds = GetComponents<AudioSource>();
        //m_dotSound = m_pacmanSounds[0];
        //m_energizerSound = m_pacmanSounds[1];
        //m_deadSound = m_pacmanSounds[2];
    }

    private void FixedUpdate() {

        // move closer to destination
        Vector3 p = Vector3.MoveTowards(transform.position, m_dest, m_speed * Time.deltaTime);
        GetComponent<Rigidbody>().MovePosition(p);  
       
        // up moves up, down is down, right is right, etc .. (uses world space)
        if (Input.GetAxis("Horizontal") > 0) { 
            m_nextDir = Vector3.right; 
        }
        
        if (Input.GetAxis("Horizontal") < 0) { 
            m_nextDir = -Vector3.right; 
        }
        
        if (Input.GetAxis("Vertical") > 0) { 
            m_nextDir = Vector3.forward; 
        }
        
        if (Input.GetAxis("Vertical") < 0) { 
            m_nextDir = -Vector3.forward; 
        }

        if (Vector3.Distance(m_dest, transform.position) < 0.0001f) {
            if (Valid(m_nextDir)) { 
                m_dest = (Vector3)transform.position + m_nextDir;
                m_dir = m_nextDir;    
            } else {   // nextDir NOT valid
                if (Valid(m_dir)) {  // and the prev. direction is valid
                    m_dest = (Vector3)transform.position + m_dir;   // continue on that direction
                } 
                    
            }
        }
        transform.LookAt(m_dest);
    }


    private bool Valid(Vector3 direction) {
        // cast line from 'next to pacman' to pacman 
        Vector3 pos = transform.position;
        bool retVal = false; 

        direction += new Vector3(direction.x * m_distance, 0, direction.z * m_distance);
        RaycastHit hit; 
        Physics.Linecast(pos + direction, pos, out hit);
        
        if(hit.collider != null) {
            retVal = hit.collider.tag == "Ghost" || hit.collider.name == "Energizer" || hit.collider.name == "Dot" || hit.collider.name == "warp" || (hit.collider == GetComponent<Collider>());
        } 
        return retVal;
    }


    public void AteDot(){
        //m_dotSound.Play();
    }

    public void Energize() {
        foreach(GameObject go in m_ghosts)
        {
            go.GetComponent<GhostController>().setState(GhostState.SCARED);
        }
        //energizedSound.Play();
    }

    public void GotCaught()
    {
        //m_deadSound.Play();
    }

    public void setDest(Vector3 newDest) { m_dest = newDest; }
}
