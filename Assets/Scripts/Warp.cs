using UnityEngine;

public class Warp : MonoBehaviour
{
    private GameObject m_warpMate;
    private bool m_allowWarp = false;

    private void OnTriggerEnter(Collider other) {
        if ((other.tag == "Player") && (m_allowWarp)) {
            m_warpMate.GetComponent<Warp>().AllowWarp(false);
            other.transform.position = m_warpMate.transform.position;
            other.GetComponent<PlayerController>().setDest(m_warpMate.transform.position);
        }
    }

    private void OnTriggerExit(Collider other) {
        m_allowWarp = true;
    }

    public void AllowWarp(bool allowed) { m_allowWarp = allowed; }
    public void setWarpMate(GameObject mate) { m_warpMate = mate; }
}
