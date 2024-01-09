using UnityEngine;
using UnityEngine.SceneManagement;

namespace SG.Game
{
    public class GameStart : MonoBehaviour
    {
        public void StartGame()
        {
            SceneManager.LoadScene(1);
        }
    }
}
