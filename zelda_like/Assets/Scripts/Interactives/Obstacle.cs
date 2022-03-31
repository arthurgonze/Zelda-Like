using ZL.Combat;
using UnityEngine;

namespace ZL.Interactives
{
    public class Obstacle : MonoBehaviour
    {
        [SerializeField] Weapon.BreakObjects _obstacleType;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "PlayerWeapon")
            {
                Debug.Log("Acertou objeto quebravel");
                Weapon.BreakObjects breakableType = collision.gameObject.GetComponentInParent<Fighter>().GetCurrentWeapon().GetBreakableObject();
                if ((Weapon.BreakObjects)breakableType == _obstacleType)
                {
                    Debug.Log("Quebrou obstaculo");
                    Destroy(this.gameObject);
                }
            }
        }
    }
}