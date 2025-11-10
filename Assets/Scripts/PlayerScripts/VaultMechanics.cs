using UnityEngine;

public class VaultMechanics : MonoBehaviour
{
    private PlayerAttributes pa;

    private void Start()
    {
        pa = GetComponentInParent<PlayerAttributes>();
    }

  void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Vaultable"))
        {
            pa.isInVaultZone = true;
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Vaultable"))
        {
            pa.isInVaultZone = false;
        }
    }



}
