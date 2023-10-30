using System;
using System.Collections.Generic;
using Assets.HeroEditor.Common.CharacterScripts;
using LiteNetLib;
using LiteNetLib.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

[CreateAssetMenu]
public class PlayerNetworkController : NetworkController
{
    public GameObject HumanPrefab;
    
    public Character CharacterPrefab;
    
    [NonSerialized]
    public readonly UnityEvent<int> OnPlayerJoin = new();

    [NonSerialized]
    public readonly UnityEvent<int> OnPlayerLeave = new();
    
    [NonSerialized]
    public readonly UnityEvent<int> OnPlayerCurrencyChanged = new();

    public static readonly Dictionary<int, Player> Players = new();

    [NonSerialized]
    public UnityEvent OnPlayerListRefresh = new();

    public static Player LocalPlayer => Players[NetworkManager.LocalPlayerPeerId];

    public override void OnPeerDisconnected(NetPeer peer)
    {
        Players.Clear();
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        NetworkManager.Instance.Send(new ReqPlayers());
    }

    public override void OnPeerConnectionRequest(NetPeer peer, string idendifier, string username)
    {
    }
    

    public override void Subscribe(NetPacketProcessor processor)
    {
        processor.SubscribeReusable<ResPlayers>(OnPlayersResponse);
        processor.SubscribeReusable<ResPlayerPosition>(OnUpdatePlayerPositionResponse);
        processor.SubscribeReusable<ResPlayerMove>(OnPlayerTargetChangeResponse);
        processor.SubscribeReusable<ResPlayerJoin>(OnPlayerJoinedResponse);
        processor.SubscribeReusable<ResPlayerLeave>(OnPlayerLeaveResponse);
        processor.SubscribeReusable<ResPlayerCurrency>(OnPlayerCurrencyResponse);
    }

    private void OnPlayerCurrencyResponse(ResPlayerCurrency res)
    {
        LocalPlayer.Currency = res.NewCurrency;
        OnPlayerCurrencyChanged?.Invoke(LocalPlayer.Currency);
    }

    public override void OnExit()
    {
    }

    public override void OnStart()
    {
    }

    public override void OnUpdate(float deltaTime)
    {
        foreach (var p in Players)
        {
            var player = p.Value;
            var newPos = Vector2.MoveTowards(new Vector2(player.x, player.y),
                new Vector2(player.targetX, player.targetY), deltaTime * player.speed);
            player.x = newPos.x;
            player.y = newPos.y;
        }
    }

    public Player GetPlayer(int i)
    {
        return Players[i];
    }


    private void OnPlayersResponse(ResPlayers res)
    {
        for (var i = 0; i < res.Players.Length; i++) Players.Add(res.Players[i].Id, res.Players[i]);
        OnPlayerListRefresh?.Invoke();
    }

    private void OnPlayerTargetChangeResponse(ResPlayerMove target)
    {
        var p = Players[target.Id];
        p.targetX = target.x;
        p.targetY = target.y;
    }

    private void OnUpdatePlayerPositionResponse(ResPlayerPosition target)
    {
        var p = Players[target.Id];
        p.x = target.x;
        p.y = target.y;
    }

    private void OnPlayerJoinedResponse(ResPlayerJoin resPlayer)
    {
        Players.Add(resPlayer.Player.Id, resPlayer.Player);
        OnPlayerJoin?.Invoke(resPlayer.Player.Id);
    }

    private void OnPlayerLeaveResponse(ResPlayerLeave resPlayer)
    {
        for (var i = 0; i < Players.Count; i++)
            if (Players[i].Id == resPlayer.Player.Id)
            {
                Players.Remove(resPlayer.Player.Id);
                OnPlayerLeave?.Invoke(resPlayer.Player.Id);
                break;
            }
    }
}