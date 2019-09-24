
using System.Collections.Generic;
using UnityEngine;

/* 
    A* Pathfinding algorithm 
    ------------------------

    OPEN    //  the set of nodes to be evaluated 
    CLOSED  // the set of nodes already evaluated 
    add the start node to OPEN 

    loop 
        current = node in OPEN with the lowest f_cost 
        remove current from OPEN 
        add current to CLOSED 

        if current is the target // path found 
            return 

        for each neighbour of the current node 
            if neighbour is not traversable or neighbour is in CLOSED 
                skip to the next neighbour 

            if new path to neighbour is shorter OR neighbour is not in OPEN 
                set f_cost of neighbour 
                set parent of neighbour to current 
                if neighbour is not in OPEN 
                    add neighbour to OPEN 

    --------- 
    Note: NodeFromWorldPoint and GetNeighbours are in the LevelGenerator
*/ 

public class Pathfinding : MonoBehaviour {

    public bool m_showPath = true;
    private LevelGenerator m_levelScript; 
    private Transform m_target;

    public List<Node> m_path; 

    private Transform m_seeker; 
    private Color m_seekerColor; 

    public void SetLevel(LevelGenerator levelScript) { m_levelScript = levelScript; }
    public void SetTarget(Transform newTarget) { m_target = newTarget; }

    void Start() {
       m_seeker = transform;
       m_seekerColor = new Color32(255, 132, 0, 204);
    }

    void Update() {
        if (m_target != null) {
            FindPath(m_seeker.position, m_target.position);
        }
    }



    private void FindPath(Vector3 startPos, Vector3 targetPos) {
        if (m_levelScript != null) {
            Node startNode = m_levelScript.NodeFromWorldPoint(startPos);
            Node targetNode = m_levelScript.NodeFromWorldPoint(targetPos);

            // OPEN    //  the set of nodes to be evaluated 
            List<Node> openSet = new List<Node>();
            // CLOSED  // the set of nodes already evaluated 
            HashSet<Node> closedSet = new HashSet<Node>();

            // add the start node to OPEN 
            openSet.Add(startNode);

            // loop 
            while(openSet.Count > 0) {
                
                // current = node in OPEN with the lowest f_cost 
                Node currentNode = openSet[0]; 
                
                for(int i = 1; i < openSet.Count; i++) {
                    if(openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].m_hCost < currentNode.m_hCost) {
                        currentNode = openSet[i];
                    }
                }

                // remove current from OPEN 
                openSet.Remove(currentNode);
                // add current to CLOSED 
                closedSet.Add(currentNode);
                //  if current is the target // path found 
                //      return 
                if(currentNode == targetNode) {
                    RetracePath(startNode, targetNode);
                    return; 
                }

                // for each neighbour of the current node 
                foreach (Node neighbour in m_levelScript.GetNeighbours(currentNode)) {
                    if(!neighbour.m_walkable || closedSet.Contains(neighbour)) {
                        continue; 
                    }

                    // if new path to neighbour is shorter OR neighbour is not in OPEN 
                    int newMovementCostToNeighbour = currentNode.m_gCost + GetDistance(currentNode, neighbour); 
                    if(newMovementCostToNeighbour < neighbour.m_gCost || !openSet.Contains(neighbour)) {
                        //  set f_cost of neighbour (remember we re-calculate this, so we need to update the g and h costs)
                        neighbour.m_gCost = newMovementCostToNeighbour;
                        neighbour.m_hCost = GetDistance(neighbour, targetNode);
                        // set parent of neighbour to current 
                        neighbour.m_parent = currentNode;

                        // if neighbour is not in OPEN 
                        //      add neighbour to OPEN 
                        if(!openSet.Contains(neighbour)) {
                            openSet.Add(neighbour); 
                        }
                    }

                }

            }

        }
    }


    private void RetracePath(Node startNode, Node endNode) {
        List<Node> path = new List<Node>();
        Node currentNode = endNode; 

        while (currentNode != startNode) {
            path.Add(currentNode);
            currentNode = currentNode.m_parent;
        }
        path.Reverse();
        m_path = path;
    }


    private int GetDistance(Node nodeA, Node nodeB) {
        int retVal = 0;
        int distX = Mathf.Abs(nodeA.m_gridX - nodeB.m_gridX);
        int distZ = Mathf.Abs(nodeA.m_gridZ - nodeB.m_gridZ);
        
        if (distX > distZ) {
            retVal = 14 * distZ + 10 * (distX - distZ);
        } else {
            retVal = 14 * distX + 10 * (distZ - distX);
        }

        return retVal;
    }


    void OnDrawGizmos() {
        Vector3 pathHeightAdjusment = Vector3.zero; 
        pathHeightAdjusment.y += 1;
        if(m_showPath) { 
	 		Node seekerNode = m_levelScript.NodeFromWorldPoint(m_seeker.position);
            foreach( Node n in m_levelScript.getGrid()) { 
	 			if(m_path != null) {
	 				if (m_path.Contains(n)) {
	 					Gizmos.color = m_seekerColor;
                        Gizmos.DrawCube(n.m_worldPosition + pathHeightAdjusment, Vector3.one * (.6f));   
	 				}
	 			}
	 			if(seekerNode == n) { 
                    Gizmos.color = m_seekerColor;
                    Gizmos.DrawCube(n.m_worldPosition, Vector3.one * (.9f));     
                }
             }
         }
    }


}
