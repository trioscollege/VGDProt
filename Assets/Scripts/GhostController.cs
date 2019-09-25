using System.Collections.Generic;
using UnityEngine;

public enum GhostName {	Blinky, Pinky,  Inky, Clyde }

public enum GhostState { DEAD, WANDER, SCATTER, CHASE, SCARED }

public class GhostController : MonoBehaviour {
    
    public GhostName persona { get { return m_persona; } set { m_persona = value; }}


    private GhostName m_persona; 
    private GhostState m_state;
    private Color m_ghostColor; 
    private Renderer m_renderer;

    public float m_moveSpeed = 8f;
    private Vector3 m_dest = Vector3.zero;
    private Vector3 m_dir = Vector3.zero;
    private Vector3 m_nextDir = Vector3.zero;
    private float m_distance = 0.5f; // look ahead 1 tile

    private PlayerController m_pacMan; 
    public void setPacMan(PlayerController player) { m_pacMan = player; }

    
    private void Start() {
        m_dest = transform.position;
        m_renderer = GetComponent<Renderer>(); 
        ResetGhost();
    }


    private void Update() {
        GetComponent<Pathfinding>().SetTarget(m_pacMan.transform);
        FollowPath();
    }


    private void FollowPath() {
		
		List<Node> path  = GetComponent<Pathfinding>().m_path; 

		if (path != null) {
			Vector3 p = Vector3.MoveTowards(transform.position, m_dest, m_moveSpeed * Time.deltaTime);
			GetComponent<Rigidbody>().MovePosition(p);

			if(path.Count > 0) {
				m_nextDir = path[0].m_worldPosition; 	
				transform.LookAt(m_nextDir);
				m_nextDir = transform.forward;
				path.RemoveAt(0);
			} else {
				// target reached
			}

			if (Vector3.Distance(m_dest, transform.position) < 0.00001f) {
			
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
		} else {
            Debug.Log("Path is NULL");
        }
	}


    bool Valid(Vector3 direction) {
        bool retVal = false;
	
		// cast line from 'next to pacman' to pacman not from directly the center of next tile but just a little further from center of next tile
        Vector3 pos = transform.position;
        direction += new Vector3(direction.x * m_distance, 0, direction.z * m_distance);
        RaycastHit hit; 
        Physics.Linecast(pos + direction, pos, out hit);
        
        if(hit.collider != null) {
            retVal = hit.collider.tag == "Player" || hit.collider.tag == "Ghost" || hit.collider.name == "Energizer" || hit.collider.name == "Dot" || (hit.collider == GetComponent<Collider>());
        } 
		return retVal;
    }



    public void ResetGhost() {

        switch(persona) {
			case GhostName.Blinky: 
				m_ghostColor = new Color32(255, 0, 0, 204);	
				break; 
			case GhostName.Pinky:
				m_ghostColor = new Color32(255, 0, 247, 204); 
				break;
			case GhostName.Inky: 
				m_ghostColor = new Color32(73 , 179, 219, 204);		
				break;
			case GhostName.Clyde:
				m_ghostColor = new Color32(255, 132, 0, 204);
				break;
		}
		m_renderer.material.color = m_ghostColor;

    }

}
