using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

namespace Tanks
{
  public class StatisticsUI : MonoBehaviourPunCallbacks
  {
    private int CountOfPlayers = -1;
    public TMP_Text CountOfPlayersText;

    private int CountOfRooms = -1;
    public TMP_Text CountOfRoomsText;

    private int CountOfPlayersInRooms = -1;
    public TMP_Text CountOfPlayersInRoomsText;

    private int CountOfPlayersOnMaster = -1;
    public TMP_Text CountOfPlayersOnMasterText;

    private bool InLobby = false;
    public TMP_Text InLobbyText;

    private string LobbyName;
    public TMP_Text LobbyNameText;

    private LobbyType? LobbyType = null;
    public TMP_Text LobbyTypeText;

    private int CountOfRoomOnLobby = -1;
    public TMP_Text CountOfRoomOnLobbyText;

    private int CountOfPlayerOnLobby = -1;
    public TMP_Text CountOfPlayerOnLobbyText;

    private Dictionary<string, RoomInfo> cachedRoomList = new Dictionary<string, RoomInfo>();

    // Start is called before the first frame update
    void Start()
    {
      cachedRoomList.Clear();
    }

    public override void OnJoinedLobby()
    {
      cachedRoomList.Clear();
    }

    public override void OnLeftLobby()
    {
      cachedRoomList.Clear();
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
      UpdateCachedRoomList(roomList);
      printRoomList();
    }

    // Update is called once per frame
    void Update()
    {
      UpdateApplicationInfo();
      UpdateLobbyInfo();
    }

    private void UpdateApplicationInfo()
    {
      if (PhotonNetwork.NetworkingClient.Server == ServerConnection.MasterServer)
      {
        RefreshApplicationInfo();
      }
      else
      {
        ResetApplicationInfo();
      }
    }

    private void ResetApplicationInfo()
    {
      if (CountOfPlayers != -1)
      {
        CountOfPlayers = -1;
        CountOfPlayersText.text = "n/a";
      }
      if (CountOfRooms != -1)
      {
        CountOfRooms = -1;
        CountOfRoomsText.text = "n/a";
      }
      if (CountOfPlayersInRooms != -1)
      {
        CountOfPlayersInRooms = -1;
        CountOfPlayersInRoomsText.text = "n/a";
      }
      if (CountOfPlayersOnMaster != -1)
      {
        CountOfPlayersOnMaster = -1;
        CountOfPlayersOnMasterText.text = "n/a";
      }
    }

    private void RefreshApplicationInfo()
    {
      if (!PhotonNetwork.IsConnected)
      {
        ResetApplicationInfo();
        return;
      }

      if (CountOfPlayers != PhotonNetwork.CountOfPlayers)
      {
        CountOfPlayers = PhotonNetwork.CountOfPlayers;
        CountOfPlayersText.text = CountOfPlayers.ToString();
      }

      if (CountOfRooms != PhotonNetwork.CountOfRooms)
      {
        CountOfRooms = PhotonNetwork.CountOfRooms;
        CountOfRoomsText.text = CountOfRooms.ToString();
      }

      if (CountOfPlayersInRooms != PhotonNetwork.CountOfPlayersInRooms)
      {
        CountOfPlayersInRooms = PhotonNetwork.CountOfPlayersInRooms;
        CountOfPlayersInRoomsText.text = CountOfPlayersInRooms.ToString();
      }

      if (CountOfPlayersOnMaster != PhotonNetwork.CountOfPlayersOnMaster)
      {
        CountOfPlayersOnMaster = PhotonNetwork.CountOfPlayersOnMaster;
        CountOfPlayersOnMasterText.text = CountOfPlayersOnMaster.ToString();
      }
    }

    private void UpdateCachedRoomList(List<RoomInfo> roomList)
    {
      for(int i=0; i < roomList.Count; i++)
      {
        RoomInfo info = roomList[i];
        if (info.RemovedFromList) // 不紀錄已關閉、滿了、或是隱藏的房間
          cachedRoomList.Remove(info.Name);
        else
          cachedRoomList[info.Name] = info;
      }
    }

    private void printRoomList()
    {
      var message = $"Room List: {cachedRoomList.Count} rooms\n";
      foreach (var roomInfo in cachedRoomList)
      {
        message += $"  {roomInfo.Key}, {roomInfo.Value.IsOpen}, " +
                   $"{roomInfo.Value.PlayerCount}/{roomInfo.Value.MaxPlayers}\n";
      }

      Debug.Log(message);
    }

    private void UpdateLobbyInfo()
    {
      if (InLobby != PhotonNetwork.InLobby)
      {
        InLobby = PhotonNetwork.InLobby;
        InLobbyText.text = InLobby ? "true" : "false";
      }

      if (!InLobby || PhotonNetwork.CurrentLobby == null)
      {
        ResetLobbyInfo();
      }
      else
      {
        if (LobbyName != PhotonNetwork.CurrentLobby.Name)
        {
          LobbyName = PhotonNetwork.CurrentLobby.Name;
          LobbyNameText.text = LobbyName;
        }

        if (LobbyType != PhotonNetwork.CurrentLobby.Type)
        {
          LobbyType = PhotonNetwork.CurrentLobby.Type;
          LobbyTypeText.text = LobbyType.ToString();
        }

        if (CountOfRoomOnLobby != cachedRoomList.Count)
        {
          CountOfRoomOnLobby = cachedRoomList.Count;
          CountOfRoomOnLobbyText.text = CountOfRoomOnLobby.ToString();
        }

        var count = cachedRoomList.Sum(keyValuePair => keyValuePair.Value.PlayerCount);
        if (CountOfPlayerOnLobby != count)
        {
          CountOfPlayerOnLobby = count;
          CountOfPlayerOnLobbyText.text = count.ToString();
        }
      }
    }

    private void ResetLobbyInfo()
    {
      if (!string.IsNullOrEmpty(LobbyName))
      {
        LobbyName = null;
        LobbyNameText.text = "n/a";
      }

      if (LobbyType != null)
      {
        LobbyType = null;
        LobbyNameText.text = "n/a";
      }

      if (CountOfRoomOnLobby != -1)
      {
        CountOfRoomOnLobby = -1;
        CountOfRoomOnLobbyText.text = "n/a";
      }

      if (CountOfPlayerOnLobby != -1)
      {
        CountOfPlayerOnLobby = -1;
        CountOfPlayerOnLobbyText.text = "n/a";
      }
    }
  }
}
