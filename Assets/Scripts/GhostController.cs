
using UnityEngine;

public enum GhostName {	Blinky, Pinky,  Inky, Clyde }

public enum GhostState { DEAD, WANDER, SCATTER, CHASE, SCARED }

public class GhostController : MonoBehaviour {
    
    public GhostName persona { get { return m_persona; } set { m_persona = value; }}


    private GhostName m_persona; 
    private GhostState m_state;
    private Color m_ghostColor; 
    private Renderer m_renderer;

    
    private void Start() {
        m_renderer = GetComponent<Renderer>(); 
        ResetGhost();
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
