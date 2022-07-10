using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tanks
{
  public class GameManager : MonoBehaviourPunCallbacks
  {
    public static GameManager instance;
    public static GameObject localPlayer;
    private GameObject defaultSpawnPoint;

    private const string MAP_PROP_KEY = "C0";
    private const string GAME_MODE_PROP_KEY = "C1";
    private const string AI_PROP_KEY = "C2";

    string gameVersion = "1";

    void Awake()
    {
      if (instance != null)
      {
        Debug.LogErrorFormat(gameObject, "Multiple instances of {0} is not allow", GetType().Name);
        DestroyImmediate(gameObject);
        return;
      }

      PhotonNetwork.AutomaticallySyncScene = true;
      DontDestroyOnLoad(gameObject);
      instance = this;

      defaultSpawnPoint = new GameObject("Default SpawnPoint");
      defaultSpawnPoint.transform.position = new Vector3(0, 0, 0);
      defaultSpawnPoint.transform.SetParent(transform, false);
    }

    void Start()
    {
      SceneManager.sceneLoaded += OnSceneLoaded;

      PhotonNetwork.GameVersion = gameVersion;
    }

    public bool ConnectToServer(string account)
    {
      PhotonNetwork.NickName = account;
      return PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnected()
    {
      Debug.Log("PUN Connected");
    }

    public override void OnConnectedToMaster()
    {
      Debug.Log("PUN Connected to Master");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
      Debug.LogWarningFormat("PUN Disconnected was called by PUN with reason {0}", cause);
    }

    public void CreateGame(int map, int gameMode, TypedLobby type)
    {
      var roomOptions = new RoomOptions();
      roomOptions.CustomRoomPropertiesForLobby
        = new[] { MAP_PROP_KEY, GAME_MODE_PROP_KEY, AI_PROP_KEY };
      roomOptions.CustomRoomProperties
        = new ExitGames.Client.Photon.Hashtable
        {
          { MAP_PROP_KEY, map },
          { GAME_MODE_PROP_KEY, gameMode }
        };
      roomOptions.MaxPlayers = 4;

      PhotonNetwork.CreateRoom(null, roomOptions, type);
    }

    public void JoinRandomGame(int map, int gameMode, TypedLobby type, string sqlFilter)
    {
      byte expectedMaxPlayers = 0;
      ExitGames.Client.Photon.Hashtable expectedCustomRoomProperties = null;

      if (type.Type == LobbyType.Default)
      {
        expectedCustomRoomProperties = new ExitGames.Client.Photon.Hashtable
        {
          { MAP_PROP_KEY, map },
          { GAME_MODE_PROP_KEY, gameMode }
        };
      }

      PhotonNetwork.JoinRandomRoom(expectedCustomRoomProperties, expectedMaxPlayers, MatchmakingMode.FillRoom, type, sqlFilter);
    }

    public void JoinGameRoom()
    {
      var options = new RoomOptions
      {
        MaxPlayers = 6
      };

      PhotonNetwork.JoinOrCreateRoom("Kingdom", options, null);
    }

    public override void OnJoinedRoom()
    {
      Debug.Log($"Joined room: {PhotonNetwork.CurrentRoom.Name} " +
                $"{PhotonNetwork.CurrentRoom.CustomProperties}");
    }

    public void EnterGame()
    {
      if (PhotonNetwork.IsMasterClient)
      {
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.LoadLevel("GameScene");
      }
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
      Debug.Log($"Join Random Room Failed: ({returnCode}) {message}");
    }

    public void LeaveGame()
    {
      PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
      PhotonNetwork.LoadLevel("MainScene");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
      if (!PhotonNetwork.InRoom)
      {
        return;
      }

      var spawnPoint = GetRandomSpawnPoint();

      localPlayer = PhotonNetwork.Instantiate(
        "TankPlayer",
        spawnPoint.position,
        spawnPoint.rotation,
        0);

      Debug.Log("Player Instance ID: " + localPlayer.GetInstanceID());
    }

    private Transform GetRandomSpawnPoint()
    {
      var spawnPoints = GetAllObjectsOfTypeInScene<SpawnPoint>();
      return spawnPoints.Count == 0
        ? defaultSpawnPoint.transform
        : spawnPoints[Random.Range(0, spawnPoints.Count)].transform;
    }

    public static List<GameObject> GetAllObjectsOfTypeInScene<T>()
    {
      var objectsInScene = new List<GameObject>();

      foreach (var go in (GameObject[])Resources.FindObjectsOfTypeAll(typeof(GameObject)))
      {
        if (go.hideFlags == HideFlags.NotEditable ||
            go.hideFlags == HideFlags.HideAndDontSave)
          continue;

        if (go.GetComponent<T>() != null)
          objectsInScene.Add(go);
      }

      return objectsInScene;
    }
  }
}
