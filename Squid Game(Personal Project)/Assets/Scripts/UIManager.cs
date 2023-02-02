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

    // Ÿ�̸� �ؽ�Ʈ ����
    public void UpdateTimerText(float min, float sec)
    {
        timerText.text = min + ":" + sec;
    }

    // ������ �ؽ�Ʈ ����
    public void UpdateSurvivorText(int survivor)
    {
        survivorText.text = "Survivor : " + survivor;
    }

    // ���� ���� UI Ȱ��ȭ
    public void SetActiveGameoverUI(bool active)
    {
        gameoverUI.SetActive(active);
    }

    // ���� �����
    public void GameRestart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}