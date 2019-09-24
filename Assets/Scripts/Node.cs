using UnityEngine;

public class Node {
	
	public bool m_walkable;
	public Vector3 m_worldPosition;

    public int m_gridX; 
    public int m_gridZ; 

    public int m_gCost; 
	public int m_hCost;
    public Node m_parent;

    public int FCost { 
		get { return m_gCost + m_hCost; }
	}
	
	public Node(bool walkable, Vector3 worldPos, int gridX, int gridZ) {
		m_walkable = walkable;
		m_worldPosition = worldPos;
        m_gridX = gridX; 
        m_gridZ = gridZ;
	}
}