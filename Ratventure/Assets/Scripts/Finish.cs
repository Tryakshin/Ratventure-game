using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Finish : MonoBehaviour
{
    private AudioSource finishSound;
    private Vector2 initialPosition = new Vector2(1, 1.16f);
    private bool LevelCompleted = false;

    // Start is called before the first frame update
    void Start()
    {
        finishSound = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "Player" && !LevelCompleted)
        {
            finishSound.Play();
            LevelCompleted = true;
            Invoke("CompleteLevel", 2f);
            // Устанавливаем игрока на изначальные координаты
            transform.position = initialPosition;

            // Сбрасываем сохраненные позиции чекпоинта
            PlayerPrefs.DeleteKey("CheckpointX");
            PlayerPrefs.DeleteKey("CheckpointY");

            Debug.Log("Checkpoint reset!");

        }
    }
    private void CompleteLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
}
