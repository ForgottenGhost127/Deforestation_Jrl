using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Deforestation
{

    public class SceneController : MonoBehaviour
    {
        [SerializeField] private int _currentScene;

        void Update()
        {
            if (Input.GetKeyUp(KeyCode.Escape))
            {

                if (_currentScene == 0)
                    Application.Quit();

            }

            if (_currentScene == 0)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }

        }

        public void SceneToGame()
        {
            SceneManager.LoadScene(1);
            Cursor.visible = false;
            _currentScene = 1;
        }

        public void SceneToSettings()
        {
            SceneManager.LoadScene(2);
            _currentScene = 2;
        }
        public void SceneToMainMenu()
        {
            Cursor.visible = true;
            SceneManager.LoadScene(0);
            _currentScene = 0;
        }
    }
}
