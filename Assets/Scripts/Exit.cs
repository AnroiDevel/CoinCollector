using UnityEngine;

namespace CoinCollector
{
    public class Exit : MonoBehaviour
    {
        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.CompareTag("Player"))
            {
                GameManager.Instance.CheckAllPlayersExit();
                collision.gameObject.SetActive(false);
            }
        }
    }

}