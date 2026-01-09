using UnityEngine;
using UnityEngine.UI;

public class HealthHud : MonoBehaviour
{
    public Slider myHp;
    public Slider enemyHp;

    NetworkFighter me;
    NetworkFighter enemy;

    [System.Obsolete]
    void Update()
    {
        if (me == null)
        {
            foreach (var f in FindObjectsOfType<NetworkFighter>())
                if (f.isLocalPlayer) me = f;
        }

        if (me != null && enemy == null)
        {
            foreach (var f in FindObjectsOfType<NetworkFighter>())
                if (f != me) enemy = f;

            if (enemy != null)
            {
                myHp.maxValue = me.Health;
                enemyHp.maxValue = enemy.Health;
            }
        }

        if (me != null) myHp.value = me.Health;
        if (enemy != null) enemyHp.value = enemy.Health;
    }
}
