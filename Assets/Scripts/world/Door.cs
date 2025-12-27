using UnityEngine;
using ZombieHouseDefense.Interfaces;

namespace ZombieHouseDefense.World
{
    [RequireComponent(typeof(BoxCollider2D))]
    public class Door : MonoBehaviour, IDamageable, IInteractable
    {
        
        [SerializeField] 
        private int hp = 10;

        [SerializeField]
        private bool isOpen;

        private bool navmeshUpdateScheduled;
        private const float NavMeshDebounceDelay = 0.2f;

        public void Damage(int amt)
        {
            hp -= amt;
            
            if (hp > 0) 
                return;
            
            GetComponent<BoxCollider2D>().enabled = false;
            foreach (var boxCollider2D in gameObject.GetComponents<BoxCollider2D>())
            {
                boxCollider2D.enabled = false;
            }
            
            Destroy(gameObject);
        }


        public void Interact()
        {
            if (isOpen)
            {
                Debug.Log("Closing Door");
                isOpen = false;
                this.gameObject.transform.Rotate(0,0,-90);
            }else{
                Debug.Log("Opening Door");
                isOpen = true;
                this.gameObject.transform.Rotate(0,0,90);
            }
        }
    }
}
