using UnityEngine;


public class Node : IHeapItem<Node> {
	
	public bool m_walkable;
	public Vector3 m_worldPosition;

    public int m_gridX; 
    public int m_gridZ; 

    public int m_gCost; 
	public int m_hCost;
    public Node m_parent;

    private int m_heapIndex;

    public int FCost { 
		get { return m_gCost + m_hCost; }
	}
	
	public Node(bool walkable, Vector3 worldPos, int gridX, int gridZ) {
		m_walkable = walkable;
		m_worldPosition = worldPos;
        m_gridX = gridX; 
        m_gridZ = gridZ;
	}

    public int HeapIndex {
		get { return m_heapIndex; }
		set { m_heapIndex = value; }
	}

    public int CompareTo(Node nodeToCompare) {
		int compare = FCost.CompareTo(nodeToCompare.FCost); 
		if (compare == 0) {
			compare = m_hCost.CompareTo(nodeToCompare.m_hCost);
		}
		return -compare;
	}
}