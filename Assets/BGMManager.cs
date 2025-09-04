using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BGMManager : MonoBehaviour
{
    [System.Serializable]
    public class BGMEntry
    {
        public string sceneName;   // シーン
        public AudioClip bgmClip;
    }

    [SerializeField] private List<BGMEntry> bgmList = new List<BGMEntry>();
    [SerializeField] private AudioSource audioSource;

    public static BGMManager Instance { get; private set; }

    private void Awake()
    {
        // シングルトン化
        if (Instance == null)
        {
            Instance = this;

        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayBGMForCurrentScene();
    }

    private void PlayBGMForCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        PlayBGMForScene(currentScene);
    }

    // シーン名（または任意の登録名）でBGMを切り替え
    public void PlayBGMForScene(string sceneOrSpecialName)
    {
        foreach (BGMEntry entry in bgmList)
        {
            if (entry.sceneName == sceneOrSpecialName)
            {
                if (entry.bgmClip != null && audioSource != null)
                {
                    audioSource.clip = entry.bgmClip;
                    audioSource.loop = true;
                    audioSource.Play();
                }
                break;
            }
        }
    }

    // 直接AudioClip指定でBGMを切り替え
    public void PlayBGM(AudioClip clip)
    {
        if (clip != null && audioSource != null)
        {
            audioSource.clip = clip;
            audioSource.loop = true;
            audioSource.Play();
        }
    }
}