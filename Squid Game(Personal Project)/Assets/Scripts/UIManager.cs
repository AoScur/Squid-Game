using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    public static UIManager instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = FindObjectOfType<UIManager>();
            }

            return m_instance;
        }
    }

    private static UIManager m_instance;

    public Text timerText; 
    public Text survivorText; 
    
    public GameObject gameoverUI;

    // 타이머 텍스트 갱신
    public void UpdateTimerText(float min, float sec)
    {
        timerText.text = min + ":" + sec;
    }

    // 생존자 텍스트 갱신
    public void UpdateSurvivorText(int survivor)
    {
        survivorText.text = "Survivor : " + survivor;
    }

    // 게임 오버 UI 활성화
    public void SetActiveGameoverUI(bool active)
    {
        gameoverUI.SetActive(active);
    }

    // 게임 재시작
    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}