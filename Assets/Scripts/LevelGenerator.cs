using UnityEngine;

public class LevelGenerator : MonoBehaviour {

    public Transform m_boardContainer;
    private GameObject m_tmpObject;

    public LayerMask m_unwalkableMask; // pathfinding
    public bool m_showPathfindingGrid = true; // pathfinding 
    public bool m_wireframe = false; // pathfinding
    public float m_nodeDiameter = 1f; // pathfinding

    private Node[,] m_grid; // pathfinding


    // Prefabs
    public GameObject m_playerPrefab;
    public GameObject m_ghostPrefab; 
    public GameObject m_dotPrefab;
    public GameObject m_energizerPrefab;
    public GameObject m_wallPrefab;
    public GameObject m_cornerPrefab;


    // Level / Maze data 
    private const int m_levelDepth = 31; // rows 
    private const int m_levelWidth = 28; // columns 

    private char[,] m_levelMap = new char[m_levelDepth, m_levelWidth] { 
        { 'd','x','x','x','x','x','x','x','x','x','x','x','x','a','d','x','x','x','x','x','x','x','x','x','x','x','x','a'}, 
        { 'y','.','.','.','.','.','.','.','.','.','.','.','.','y','y','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
        { 'y','.','d','x','x','a','.','d','x','x','x','a','.','y','y','.','d','x','x','x','a','.','d','x','x','a','.','y'}, 
        { 'y','*','y',' ',' ','y','.','y',' ',' ',' ','y','.','y','y','.','y',' ',' ',' ','y','.','y',' ',' ','y','*','y'}, 
        { 'y','.','c','x','x','b','.','c','x','x','x','b','.','c','b','.','c','x','x','x','b','.','c','x','x','b','.','y'}, 
        { 'y','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
        { 'y','.','d','x','x','a','.','d','a','.','d','x','x','x','x','x','x','a','.','d','a','.','d','x','x','a','.','y'}, 
        { 'y','.','c','x','x','b','.','y','y','.','c','x','x','a','d','x','x','b','.','y','y','.','c','x','x','b','.','y'}, 
        { 'y','.','.','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','.','.','y'}, 
        { 'c','x','x','x','x','a','.','y','c','x','x','a',' ','y','y',' ','d','x','x','b','y','.','d','x','x','x','x','b'}, 
        { ' ',' ',' ',' ',' ','y','.','y','d','x','x','b',' ','c','b',' ','c','x','x','a','y','.','y',' ',' ',' ',' ',' '}, 
        { ' ',' ',' ',' ',' ','y','.','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
        { ' ',' ',' ',' ',' ','y','.','y','y',' ','d','x','x','z','z','x','x','a',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
        { 'x','x','x','x','x','b','.','c','b',' ','y',' ',' ',' ',' ',' ',' ','y',' ','c','b','.','c','x','x','x','x','x'}, 
        { 'w',' ',' ',' ',' ',' ','.',' ',' ',' ','y',' ','B','P','I','C',' ','y',' ',' ',' ','.',' ',' ',' ',' ',' ','w'}, 
        { 'x','x','x','x','x','a','.','d','a',' ','y',' ',' ',' ',' ',' ',' ','y',' ','d','a','.','d','x','x','x','x','x'}, 
        { ' ',' ',' ',' ',' ','y','.','y','y',' ','c','x','x','x','x','x','x','b',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
        { ' ',' ',' ',' ',' ','y','.','y','y',' ',' ',' ',' ',' ',' ',' ',' ',' ',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
        { ' ',' ',' ',' ',' ','y','.','y','y',' ','d','x','x','x','x','x','x','a',' ','y','y','.','y',' ',' ',' ',' ',' '}, 
        { 'd','x','x','x','x','b','.','c','b',' ','c','x','x','a','d','x','x','b',' ','c','b','.','c','x','x','x','x','a'}, 
        { 'y','.','.','.','.','.','.','.','.','.','.','.','.','y','y','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
        { 'y','.','d','x','x','a','.','d','x','x','x','a','.','y','y','.','d','x','x','x','a','.','d','x','x','a','.','y'}, 
        { 'y','.','c','x','a','y','.','c','x','x','x','b','.','c','b','.','c','x','x','x','b','.','y','d','x','b','.','y'}, 
        { 'y','*','.','.','y','y','.','.','.','.','.','.','.','s',' ','.','.','.','.','.','.','.','y','y','.','.','*','y'}, 
        { 'c','x','a','.','y','y','.','d','a','.','d','x','x','x','x','x','x','a','.','d','a','.','y','y','.','d','x','b'}, 
        { 'd','x','b','.','c','b','.','y','y','.','c','x','x','a','d','x','x','b','.','y','y','.','c','b','.','c','x','a'}, 
        { 'y','.','.','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','y','y','.','.','.','.','.','.','y'}, 
        { 'y','.','d','x','x','x','x','b','c','x','x','a','.','y','y','.','d','x','x','b','c','x','x','x','x','a','.','y'},  
        { 'y','.','c','x','x','x','x','x','x','x','x','b','.','c','b','.','c','x','x','x','x','x','x','x','x','b','.','y'},  
        { 'y','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','.','y'}, 
        { 'c','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','x','b'}
	}; 


    private void Start() {
        BuildLevel();
    }

    private void BuildLevel() {

        // pathfinding grid
		m_grid = new Node[m_levelDepth, m_levelWidth];


        for(int r = 0, z = m_levelDepth - 1; r < m_levelDepth; r++, z--) {
		    for (int c = 0, x = 0; c < m_levelWidth; c++, x++) {		
                switch(m_levelMap[r,c]) {
					case 'a': // corner 180 rotation (top right / NE)
						m_tmpObject = Instantiate(m_cornerPrefab, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 180, 0)));
                        m_tmpObject.transform.parent = m_boardContainer; 
						break;          
                    case 'b': // corner -90 rotation (bottom right / SE)
						m_tmpObject = Instantiate(m_cornerPrefab, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, -90, 0)));
                        m_tmpObject.transform.parent = m_boardContainer; 
						break;
                	case 'c': // corner // no rotation (bottom left / SW) 
						m_tmpObject = Instantiate(m_cornerPrefab, new Vector3(x, 0, z), Quaternion.identity);
                        m_tmpObject.transform.parent = m_boardContainer; 
						break;
                	case 'd': // corner 90 rotation (top left / NW) 
						m_tmpObject = Instantiate(m_cornerPrefab, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 90, 0)));
                        m_tmpObject.transform.parent = m_boardContainer; 
						break;
                	case 'x': // horozantal wall 90 deg rotation
						m_tmpObject = Instantiate(m_wallPrefab, new Vector3(x, 0, z), Quaternion.Euler(new Vector3(0, 90, 0)));
                        m_tmpObject.transform.parent = m_boardContainer; 
						break;
                	case 'y': // vertical wall no rotation 
						m_tmpObject = Instantiate(m_wallPrefab, new Vector3(x, 0, z), Quaternion.identity);
                        m_tmpObject.transform.parent = m_boardContainer; 
						break;
                    case '.': // dot
						m_tmpObject = Instantiate(m_dotPrefab, new Vector3(x, 0, z), Quaternion.identity);
                        m_tmpObject.name = "Dot";
                        m_tmpObject.transform.parent = m_boardContainer; 
						break;
                    case '*': // energizer
						m_tmpObject = Instantiate(m_energizerPrefab, new Vector3(x, 0, z), Quaternion.identity);
                        m_tmpObject.name = "Energizer";
                        m_tmpObject.transform.parent = m_boardContainer; 
						break;
                    case 's': // PacMan
						m_tmpObject = Instantiate(m_playerPrefab, new Vector3(x, 0, z), Quaternion.identity);
						break;
                    // Ghosts 
					case 'B':
                        m_tmpObject = Instantiate(m_ghostPrefab, new Vector3(x, 0, z), Quaternion.identity);						
                        m_tmpObject.GetComponent<GhostController>().persona = GhostName.Blinky;
                        break; 
                    case 'P':
                        m_tmpObject = Instantiate(m_ghostPrefab, new Vector3(x, 0, z), Quaternion.identity);						
                        m_tmpObject.GetComponent<GhostController>().persona = GhostName.Pinky;
                        break; 
                    case 'I':
                        m_tmpObject = Instantiate(m_ghostPrefab, new Vector3(x, 0, z), Quaternion.identity);						
                        m_tmpObject.GetComponent<GhostController>().persona = GhostName.Inky;
                        break; 
                    case 'C':
						m_tmpObject = Instantiate(m_ghostPrefab, new Vector3(x, 0, z), Quaternion.identity);						
                        m_tmpObject.GetComponent<GhostController>().persona = GhostName.Clyde;
                        break; 
                    default: 
                        break;
                }

                bool walkable = !(Physics.CheckSphere(new Vector3(x, 0, z), .4f, m_unwalkableMask));
				m_grid[z, x] = new Node(walkable, new Vector3(x, 0, z));

            }
        }
    } 

    private void OnDrawGizmos() {

        if(m_grid != null && m_showPathfindingGrid) {  
			
            foreach( Node n in m_grid) {
                Gizmos.color = (n.m_walkable) ? Color.white : Color.red;                

				if (m_wireframe) {
					Gizmos.DrawWireCube(n.m_worldPosition, Vector3.one * (m_nodeDiameter - .1f));
				} else {
					Gizmos.DrawCube(n.m_worldPosition, Vector3.one * (m_nodeDiameter - .1f));
				}
            }
        }
    }
}


public class Node {
	
	public bool m_walkable;
	public Vector3 m_worldPosition;
	
	public Node(bool walkable, Vector3 worldPos) {
		m_walkable = walkable;
		m_worldPosition = worldPos;
	}
}