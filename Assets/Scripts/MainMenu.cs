using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Tanks
{
  public class MainMenu : MonoBehaviourPunCallbacks
  {
    public static MainMenu instance;

    private bool IsInitialed = false;

    private GameObject m_loginUI;
    private TMP_InputField  m_accountInput;
    private TMP_InputField  m_passwordInput;
    private Button m_loginButton;

    private GameObject m_lobbyUI;
    private TMP_InputField m_lobbyInput;
    private Button m_joinLobbyButton;
    private Button m_leaveLobbyButton;
    private TMP_Dropdown m_mapSelector;
    private TMP_Dropdown m_gameModeSelector;
    private TMP_InputField m_lobbyFilter;
    private Button m_createGameButton;
    private Button m_joinGameButton;

    private GameObject m_roomUI;
    private List<TMP_Text> m_playerNameTexts = new List<TMP_Text>();
    private Button m_enterGameButton;

    private GameObject m_gameUI;
    private Button m_leaveGameButton;

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
      m_passwordInput = transform.FindAnyChild<TMP_InputField>("PasswordInput");
      m_loginButton = transform.FindAnyChild<Button>("LoginButton");

      m_lobbyUI = transform.FindAnyChild<Transform>("LobbyUI").gameObject;
      m_lobbyInput = transform.FindAnyChild<TMP_InputField>("LobbyInput");
      m_joinLobbyButton = transform.FindAnyChild<Button>("JoinLobbyButton");
      m_leaveLobbyButton = transform.FindAnyChild<Button>("LeaveLobbyButton");
      m_mapSelector = transform.FindAnyChild<TMP_Dropdown>("MapSelector");
      m_lobbyFilter = transform.FindAnyChild<TMP_InputField>("LobbyFilter");
      m_gameModeSelector = transform.FindAnyChild<TMP_Dropdown>("GameModeSelector");
      m_createGameButton = transform.FindAnyChild<Button>("CreateGameButton");
      m_joinGameButton = transform.FindAnyChild<Button>("JoinGameButton");

      m_roomUI = transform.FindAnyChild<Transform>("RoomUI").gameObject;
      m_playerNameTexts.Add(transform.FindAnyChild<TMP_Text>("PlayerName01")); 
      m_playerNameTexts.Add(transform.FindAnyChild<TMP_Text>("PlayerName02")); 
      m_playerNameTexts.Add(transform.FindAnyChild<TMP_Text>("PlayerName03")); 
      m_playerNameTexts.Add(transform.FindAnyChild<TMP_Text>("PlayerName04")); 
      m_enterGameButton = transform.FindAnyChild<Button>("EnterGameButton");

      m_gameUI = transform.FindAnyChild<Transform>("GameUI").gameObject;
      m_leaveGameButton = transform.FindAnyChild<Button>("LeaveGameButton");

      ResetUI();
    }

    private void ResetUI()
    {
      m_loginUI.SetActive(true);
      m_accountInput.interactable = true;
      m_passwordInput.interactable = true;
      m_loginButton.interactable = true;

      m_lobbyUI.SetActive(false);
      m_lobbyInput.interactable = true;
      m_joinLobbyButton.interactable = true;
      m_leaveLobbyButton.interactable = false;
      m_mapSelector.interactable = true;
      m_gameModeSelector.interactable = true;
      m_createGameButton.interactable = true;
      m_joinGameButton.interactable = true;

      m_roomUI.SetActive(false);
      foreach (var mPlayerNameText in m_playerNameTexts)
      {
        mPlayerNameText.text = "n/a";
      }
      m_enterGameButton.interactable = true;
      
      m_gameUI.SetActive(false);
      m_leaveGameButton.interactable = true;
    }

    public void BackLobby()
    {
      m_loginUI.SetActive(false);

      m_lobbyUI.SetActive(false);
      m_lobbyInput.interactable = true;
      m_joinLobbyButton.interactable = true;
      m_leaveLobbyButton.interactable = false;
      m_mapSelector.interactable = true;
      m_gameModeSelector.interactable = true;
      m_createGameButton.interactable = true;
      m_joinGameButton.interactable = true;

      m_roomUI.SetActive(false);
      m_gameUI.SetActive(false);
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

      if (string.IsNullOrEmpty(m_passwordInput.text))
      {
        Debug.Log("Please input your password!!");
        return;
      }

      m_accountInput.interactable = false;
      m_passwordInput.interactable = false;
      m_loginButton.interactable = false;

      GameManager.instance.ConnectToServer(m_accountInput.text, m_passwordInput.text);
      // if (!GameManager.instance.ConnectToServer(m_accountInput.text, m_passwordInput.text))
      // {
      //   Debug.Log("Connect to PUN Failed!!");
      //   m_accountInput.interactable = true;
      //   m_passwordInput.interactable = true;
      //   m_loginButton.interactable = true;
      // }
    }

    public void OnLoginSuccess()
    {
      Debug.Log("Main Menu: Login success");
      
      m_loginUI.SetActive(false);
      m_lobbyUI.SetActive(true);
    }

    public void OnLoginFailed(string reason)
    {
      Debug.Log("Main Menu: Login failed: " + reason);

      m_accountInput.interactable = true;
      m_passwordInput.interactable = true;
      m_loginButton.interactable = true;
    }

    public override void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics)
    {
      var message = $"Lobby List: {lobbyStatistics.Count} lobbies\n";
      foreach (var lobbyInfo in lobbyStatistics)
      {
        message += $"  {lobbyInfo.Name}, {lobbyInfo.Type}, {lobbyInfo.RoomCount} rooms, " +
                   $"{lobbyInfo.PlayerCount} players\n";
      }

      Debug.Log(message);
    }

    public void JoinLobby()
    {
      var typedLobby = new TypedLobby(m_lobbyInput.text, LobbyType.Default);
      PhotonNetwork.JoinLobby(typedLobby);
    }

    public void JoinSQLLobby()
    {
      var typedLobby = new TypedLobby(m_lobbyInput.text, LobbyType.SqlLobby);
      PhotonNetwork.JoinLobby(typedLobby);
    }

    public void LeaveLobby()
    {
      PhotonNetwork.LeaveLobby();
    }
    
    public override void OnJoinedLobby()
    {
      Debug.Log($"Joined Lobby: {PhotonNetwork.CurrentLobby.Name} {PhotonNetwork.CurrentLobby.Type}");
      m_leaveLobbyButton.interactable = true;
    }

    public override void OnLeftLobby()
    {
      // 離開 Lobby 的時候，會加回 Default Lobby
      Debug.Log($"Left Lobby: {PhotonNetwork.CurrentLobby.Name} {PhotonNetwork.CurrentLobby.Type}");
      m_leaveLobbyButton.interactable = false;
    }
    
    public void GetCustomRoomList()
    {
      var sqlLobby = new TypedLobby(m_lobbyInput.text, LobbyType.SqlLobby);
      var sqlLobbyFilter = m_lobbyFilter.text;

      StatisticsUI.instance.ClearRoomList();

      // C0 BETWEEN 0 AND 1 AND C1 = 0
      PhotonNetwork.GetCustomRoomList(sqlLobby, sqlLobbyFilter);
    }

    public void CreateGame()
    {
      var sqlLobby = new TypedLobby(m_lobbyInput.text, LobbyType.SqlLobby);
      GameManager.instance.CreateGame(m_mapSelector.value + 1, m_gameModeSelector.value + 1, sqlLobby);
    }

    public void JoinRandomGame()
    {
      var sqlLobby = new TypedLobby(m_lobbyInput.text, LobbyType.SqlLobby);
      var sqlLobbyFilter = m_lobbyFilter.text;
      GameManager.instance.JoinRandomGame(m_mapSelector.value + 1, m_gameModeSelector.value + 1, sqlLobby, sqlLobbyFilter);
    }
    
    public override void OnJoinedRoom()
    {
      m_lobbyUI.SetActive(false);
      m_roomUI.SetActive(true);

      // m_enterGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

      refreshPlayerList();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
      refreshPlayerList();
    }

    public override void OnPlayerLeftRoom(Player newPlayer)
    {
      refreshPlayerList();
    }

    private void refreshPlayerList()
    {
      // 可以試試看，把這行搬到 OnJoinedRoom event 裏面，會有什麼現象
      m_enterGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

      var i = 0;
      for (i = 0; i < PhotonNetwork.PlayerList.Length; i++)
      {
        m_playerNameTexts[i].text = PhotonNetwork.PlayerList[i].NickName;
      }
      for (; i < 4; i++)
      {
        m_playerNameTexts[i].text = "n/a";
      }
    }

    public void EnterGame()
    {
      GameManager.instance.EnterGame();
    }

    public void LeaveGame()
    {
      Debug.Log("LeaveGame");
      GameManager.instance.LeaveGame();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
      Debug.Log($"Join Random Failed: ({returnCode}) {message}");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      Debug.Log($"Scene Loaded: {scene.name}");
      if (!PhotonNetwork.InRoom)
      {
        if (!IsInitialed)
        {
          IsInitialed = true;
          ResetUI();
        }
        else
        {
          BackLobby();
        }
      }
      else
      {
        m_lobbyUI.SetActive(false);
        m_roomUI.SetActive(false);
        m_gameUI.SetActive(true);
      }
    }
  }
}
