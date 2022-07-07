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

    private GameObject m_loginUI;
    private TMP_InputField  m_accountInput;
    private Button m_loginButton;

    private GameObject m_lobbyUI;
    private TMP_Dropdown m_mapSelector;
    private TMP_Dropdown m_gameModeSelector;
    private Button m_createGameButton;
    private Button m_joinGameButton;

    private GameObject m_roomUI;

    void Awake()
    {
      if (instance != null)
      {
        DestroyImmediate(gameObject);
        return;
      }

      instance = this;

      m_loginUI = transform.FindAnyChild<Transform>("LoginUI").gameObject;
      m_accountInput = transform.FindAnyChild<TMP_InputField>("AccountInput");
      m_loginButton = transform.FindAnyChild<Button>("LoginButton");

      m_lobbyUI = transform.FindAnyChild<Transform>("LobbyUI").gameObject;
      m_mapSelector = transform.FindAnyChild<TMP_Dropdown>("MapSelector");
      m_gameModeSelector = transform.FindAnyChild<TMP_Dropdown>("GameModeSelector");
      m_createGameButton = transform.FindAnyChild<Button>("CreateGameButton");
      m_joinGameButton = transform.FindAnyChild<Button>("JoinGameButton");

      m_roomUI = transform.FindAnyChild<Transform>("RoomUI").gameObject;

      ResetUI();
    }

    private void ResetUI()
    {
      m_loginUI.SetActive(true);
      m_accountInput.interactable = true;
      m_loginButton.interactable = true;

      m_lobbyUI.SetActive(false);
      m_mapSelector.interactable = true;
      m_gameModeSelector.interactable = true;
      m_createGameButton.interactable = true;
      m_joinGameButton.interactable = true;

      m_roomUI.SetActive(false);
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
        m_accountInput.interactable = true;
        m_loginButton.interactable = true;
      }
    }

    public override void OnConnectedToMaster()
    {
      m_loginUI.SetActive(false);
      m_lobbyUI.SetActive(true);
    }

    public void CreateGame()
    {
      GameManager.instance.CreateGame(m_mapSelector.value + 1, m_gameModeSelector.value + 1);
    }

    public void JoinRandomGame()
    {
      GameManager.instance.JoinRandomGame(m_mapSelector.value + 1, m_gameModeSelector.value + 1);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      if (!PhotonNetwork.InRoom)
      {
        ResetUI();
      }
      else
      {
        m_lobbyUI.SetActive(false);
        m_roomUI.SetActive(false);
      }
    }
  }
}
