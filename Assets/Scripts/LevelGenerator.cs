using UnityEngine;

public class LevelGenerator : MonoBehaviour {


    public Transform m_boardContainer;
    private GameObject m_tmpObject;


    // Prefabs
    public GameObject m_playerPrefab;
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
                    default: 
                        break;
                }
            }
        }
    } 
}
