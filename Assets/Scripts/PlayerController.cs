using UnityEngine;

public class PlayerController : MonoBehaviour {

    public float m_speed = 10.0f;
    private Vector3 m_dest = Vector3.zero;
    private Vector3 m_dir = Vector3.zero;
    private Vector3 m_nextDir = Vector3.zero;
    private float m_distance = 1f; // look ahead 1 tile


    private void Start() {
        m_dest = transform.position;   
    }

    private void Update() {

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
            m_dest = (Vector3)transform.position + m_nextDir;
            m_dir = m_nextDir;
        }

        transform.LookAt(m_dest);
    }
}
