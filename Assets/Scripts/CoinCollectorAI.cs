using UnityEngine;

namespace CoinCollector
{
    public class CoinCollectorAI : AIController
    {
        protected override GameObject FindTarget()
        {
            // Поиск ближайшей монеты
            GameObject[] coins = GameObject.FindGameObjectsWithTag("Coin");
            GameObject closestCoin = null;
            float minDistance = Mathf.Infinity;
            Vector2 currentPosition = transform.position;

            foreach(GameObject coin in coins)
            {
                float distance = Vector2.Distance(currentPosition, coin.transform.position);
                if(distance < minDistance)
                {
                    closestCoin = coin;
                    minDistance = distance;
                }
            }

            return closestCoin;
        }
    }
}
