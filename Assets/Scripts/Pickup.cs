using UnityEngine;

public enum pickupType {
	None,
	Dot, 
	Energizer
}


public class Pickup : MonoBehaviour {
 
    public pickupType m_pickupType; 

	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {

			switch(m_pickupType) {
				case pickupType.Dot:
					other.GetComponent<PlayerController>().AteDot();
					break;
				case pickupType.Energizer:
					other.GetComponent<PlayerController>().Energize();
					break; 				
			}
			Destroy(gameObject);
		}
	}
}
