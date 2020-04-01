using System.Collections.Generic;
using UnityEngine;

public enum GhostName {	Blinky, Pinky,  Inky, Clyde }

public enum GhostState { DEAD, WANDER, SCATTER, CHASE, SCARED }

public class GhostController : MonoBehaviour {
    
    public GhostName persona { get { return m_persona; } set { m_persona = value; }}


    private GhostName m_persona; 
    private GhostState m_state;
	private GhostState m_lastState;
	private Color m_ghostColor; 
    private Renderer m_renderer;

	private GameObject m_target;
	private GameObject m_homeTarget;
	private GameObject m_scatterTarget;

    public float m_moveSpeed = 8f;
    private Vector3 m_dest = Vector3.zero;
    private Vector3 m_dir = Vector3.zero;
    private Vector3 m_nextDir = Vector3.zero;
	private Vector3 reversed = Vector3.zero;
	private float m_distance = 0.5f; // look ahead 1 tile

	private float m_blinkTimer = 0.0f;
	private bool m_altColor = false;
	private float m_scaredTimer = 0.0f;

    private PlayerController m_pacMan; 
    public void setPacMan(PlayerController player) { m_pacMan = player; }

    
    private void Start() {
        m_dest = transform.position;
        m_renderer = GetComponent<Renderer>(); 
        ResetGhost();
    }

    private void Update() {
		switch(m_state) {
			case GhostState.DEAD:
				GoHome();
				FollowPath();
				break;
			case GhostState.SCATTER:
				Scatter();
				FollowPath();
				break;
			case GhostState.WANDER:
				Wander();
				break;
			case GhostState.CHASE:
				GetComponent<Pathfinding>().SetTarget(m_pacMan.transform);
				FollowPath();
				break;
			case GhostState.SCARED:
				m_scaredTimer += Time.deltaTime;
				m_blinkTimer += Time.deltaTime;
				if (m_blinkTimer > .2f) {
					m_altColor = !m_altColor;
					m_renderer.material.color = (m_altColor) ? Color.white : Color.blue;
					m_blinkTimer = 0;
				}
				if (m_scaredTimer > 10.0f) {
					m_renderer.material.color = m_ghostColor;
					m_state = m_lastState;
					if(m_state == GhostState.SCATTER) {
						FollowPath();
					}
					m_scaredTimer = 0;
				}
				Wander();
				break;
			default:
				Debug.LogWarning("Ghost Controller doesn't know what state it's in");
				break;
		}    
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
				switch(m_state)
				{
					case GhostState.DEAD:
						m_renderer.enabled = true;
						m_state = GhostState.SCATTER;
						Scatter();
						break;
					case GhostState.SCATTER:
						if (m_persona == GhostName.Clyde)
						{
							m_state = GhostState.CHASE;
						}
						else
						{
							m_state = GhostState.WANDER;
						}
						break;
				}
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

	private void Wander() {
		// move closer to destination
		Vector3 p = Vector3.MoveTowards(transform.position, m_dest, m_moveSpeed * Time.deltaTime);
		GetComponent<Rigidbody>().MovePosition(p);

		Vector3[] choices = { Vector3.right, -Vector3.right, Vector3.forward, -Vector3.forward };
		int myRandomIndex;

		if (!Valid(m_nextDir)) {
			do
			{
				myRandomIndex = Random.Range(0, 4);
			} while (choices[myRandomIndex] == reversed);

			m_nextDir = choices[myRandomIndex];

			if (m_nextDir == Vector3.forward) {
				reversed = -Vector3.forward;
			} else if (m_nextDir == -Vector3.forward) {
				reversed = Vector3.forward;
			} else if (m_nextDir == Vector3.right) {
				reversed = -Vector3.right;
			} else if (m_nextDir == -Vector3.right) {
				reversed = Vector3.right;
			}
		}

		if (Vector3.Distance(m_dest, transform.position) < 0.0001f) {
			if (Valid(m_nextDir)) {
				m_dest = (Vector3)transform.position + m_nextDir;
				m_dir = m_nextDir;
			}
			else {   // nextDir NOT valid
				if (Valid(m_dir)) {  // and the prev. direction is valid
					m_dest = (Vector3)transform.position + m_dir;   // continue on that direction
				}
			}
		}
		transform.LookAt(m_dest);
	}

	private void GoHome() { GetComponent<Pathfinding>().SetTarget(m_homeTarget.transform); }
	private void Scatter() { GetComponent<Pathfinding>().SetTarget(m_scatterTarget.transform); }

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

	public Color getColor() { return m_ghostColor; }

	public void setState(GhostState newState)
	{
		if (m_state != GhostState.DEAD)
		{
			m_state = newState;
		}

		if (m_state == GhostState.SCARED)
		{
			m_renderer.material.color = Color.blue;
			m_scaredTimer = 0;
		}
	}

    public void ResetGhost() {
		m_state = GhostState.SCATTER;
		m_lastState = m_state;

		if (m_target == null) {
			m_target = new GameObject();
			m_target.name = "Target";
		}

		if (m_homeTarget == null) {
			m_homeTarget = new GameObject();
			m_homeTarget.name = "Home Target";
			m_homeTarget.transform.position = transform.position;
		}

		if (m_scatterTarget == null) {
			m_scatterTarget = new GameObject();
			m_scatterTarget.name = "Scatter Target";
		}

		switch (persona) {
			case GhostName.Blinky: 
				m_ghostColor = new Color32(255, 0, 0, 204);
				m_scatterTarget.transform.position = new Vector3(25, 0, 29);
				break; 
			case GhostName.Pinky:
				m_ghostColor = new Color32(255, 0, 247, 204);
				m_scatterTarget.transform.position = new Vector3(3, 0, 29);
				break;
			case GhostName.Inky: 
				m_ghostColor = new Color32(73 , 179, 219, 204);
				m_scatterTarget.transform.position = new Vector3(24, 0, 1);
				break;
			case GhostName.Clyde:
				m_ghostColor = new Color32(255, 132, 0, 204);
				m_scatterTarget.transform.position = new Vector3(3, 0, 1);
				break;
		}
		m_renderer.material.color = m_ghostColor;

    }

	private void OnTriggerEnter(Collider other)
	{
		if (m_state != GhostState.DEAD)
		{
			if (other.tag == "Player")
			{
				if (m_state != GhostState.SCARED)
				{
					Debug.Log("PacMan Died!");
					other.GetComponent<PlayerController>().GotCaught();
				}
				else
				{
					Debug.Log("Ghost Died!");
					// m_deathSound.Play();
					m_state = GhostState.DEAD;
					m_renderer.enabled = false;
				}
			}
		}
	}

}
