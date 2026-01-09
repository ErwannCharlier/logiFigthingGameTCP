using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMechanism : MonoBehaviour
{
    private static float round = 1;
    public PlayerOneController player_1;
    public PlayerTwoController player_2;
    public Text roundText;
    private bool isPlaying;
    public List<Potion> potions;

    void Start()
    {
        isPlaying = true;
        roundText.text = "Round: " + round;
        StartCoroutine(PlacePotions());
    }

    void Update()
    {
        if (isPlaying && (player_1.Health < 1 || player_2.Health < 1))
        {
            isPlaying = false;
            round++;
            roundText.text = "Round: " + round;
            StartCoroutine(NextRound(3f));
        }
    }
    IEnumerator NextRound(float delay)
    {
        yield return new WaitForSeconds(delay);
        SceneManager.LoadScene("SampleScene");
    }

    IEnumerator PlacePotions()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            Potion choosedPotion = potions[Random.Range(0, 2)];
            choosedPotion.transform.position = new Vector3(
                Random.Range(-20, 20),
                choosedPotion.transform.position.y,
                Random.Range(-20, 20)
                );
            Instantiate(choosedPotion);
        }
    }
}
