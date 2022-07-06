using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tanks
{
  public class MainMenu : MonoBehaviourPunCallbacks
  {
    public static MainMenu instance;
    private GameObject m_ui;
    private TMP_InputField  m_accountInput;
    private Button m_loginButton;
    private Button m_joinGameButton;

    void Awake()
    {
      if (instance != null)
      {
        DestroyImmediate(gameObject);
        return;
      }

      instance = this;

      m_ui = transform.FindAnyChild<Transform>("UI").gameObject;
      m_accountInput = transform.FindAnyChild<TMP_InputField>("AccountInput");
      m_loginButton = transform.FindAnyChild<Button>("LoginButton");
      m_joinGameButton = transform.FindAnyChild<Button>("JoinGameButton");

      ResetUI();
    }

    private void ResetUI()
    {
      m_ui.SetActive(true);
      m_accountInput.gameObject.SetActive(true);
      m_loginButton.gameObject.SetActive(true);
      m_joinGameButton.gameObject.SetActive(false);
      
      m_accountInput.interactable = true;
      m_loginButton.interactable = true;
      m_joinGameButton.interactable = true;
    }

    public override void OnEnable()
    {
      // Always call the base to add callbacks
      base.OnEnable();

      SceneManager.sceneLoaded += OnSceneLoaded;
    }

    public override void OnDisable()
    {
      // Always call the base to remove callbacks
      base.OnDisable();

      SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    public void Login()
    {
      if (string.IsNullOrEmpty(m_accountInput.text))
      {
        Debug.Log("Please input your account!!");
        return;
      }

      m_accountInput.interactable = false;
      m_loginButton.interactable = false;

      if (!GameManager.instance.ConnectToServer(m_accountInput.text))
      {
        Debug.Log("Connect to PUN Failed!!");
      }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      m_ui.SetActive(!PhotonNetwork.InRoom);
    }

    public override void OnConnectedToMaster()
    {
      m_accountInput.gameObject.SetActive(false);
      m_loginButton.gameObject.SetActive(false);
      m_joinGameButton.gameObject.SetActive(true);
    }
  }
}
