using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public enum E_Messages
{
    M_Turn = 0,
    M_NewGame = 1,
    M_GameOver = 2,
    M_GameInvite
}

public class GameManager_Online : GameManager {

    bool IsGameOverState = false;

    bool IsIReady = false;
    bool IsOponentReady = false;

    public GameObject WaitingRespondScreen;

    int hostId;
    int channelId;
    int connectionId;

    byte error;

    // waiting for connection
    private void Start()
    {

        WaitingRespondScreen.SetActive(true);

        Connect();
    }

    // set up connection
    public void Connect()
    {

        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Reliable);        // only reliable channel

        //Here we created a topology that allows up to 10 connections, each of them are configured by the parameters defined in previous step.
        HostTopology topology = new HostTopology(config, 10);

       if (GameStats.Instance.isServer)
            hostId = NetworkTransport.AddHost(topology, GameStats.Instance.PORT);
        else
        {
            hostId = NetworkTransport.AddHost(topology, 0);
            connectionId = NetworkTransport.Connect(hostId, GameStats.Instance.IP, GameStats.Instance.PORT, 0, out error);
        }
        
    }

    private void Update()
    {
        
        int recHostId;
        int recConnectionId;
        int recChannelId;
        byte[] recBuffer = new byte[1];
        int bufferSize = 1;
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, bufferSize, out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.ConnectEvent:
                if (GameStats.Instance.isServer)
                {
                    // server starts game, and client waiting for it
                    connectionId = recConnectionId;
                    NewGame();
                }
                else
                    if (connectionId != recConnectionId)
                         connectionId = recConnectionId;
                break;
            case NetworkEventType.DataEvent:                     // receiving data about packet
                
                if (recBuffer[0] == (byte)E_Messages.M_NewGame)
                {
                    RecNewGame();
                }

                if (recBuffer[0] == (byte)E_Messages.M_GameOver)
                {
                    RecGameOver();
                }

                if (recBuffer[0] == (byte)E_Messages.M_Turn)
                {
                    RecTurn();
                }

                if(recBuffer[0] == (byte)E_Messages.M_GameInvite)
                {
                    GameInvite();
                }
                break;
            case NetworkEventType.DisconnectEvent:
                Disconnect();
                break;


        }
    }
    
    // this packet receives only client, here we get data about grid and which turn is first
    void RecNewGame()
    {
        int x = 3;
        int y = 3;
        bool T = false;

        int recHostId;
        int recConnectionId;
        int recChannelId;
        byte[] recBuffer = new byte[4];
        
        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, sizeof(int), out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:

                if (System.BitConverter.IsLittleEndian)
                    Array.Reverse(recBuffer);

                x = System.BitConverter.ToInt32(recBuffer,0);
                break;
        }
        recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, sizeof(int), out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(recBuffer);

                y = BitConverter.ToInt32(recBuffer, 0);
                break;
        }

        recBuffer = new byte[1];
        recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, sizeof(bool), out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:

                if (BitConverter.IsLittleEndian)
                    Array.Reverse(recBuffer);

                T = BitConverter.ToBoolean(recBuffer, 0);
                break;
        }

        GameStats.Instance.BoxesInWidth = x;
        GameStats.Instance.BoxesInHight = y;
        IsFirstTurn = CanTurn = T;

        NewGame();      // here client starts game
    }


    // here we receiving data about turn made by our opponent
    void RecTurn()
    {
        int l = 0;
        

        int recHostId;
        int recConnectionId;
        int recChannelId;
        byte[] recBuffer = new byte[4];

        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, sizeof(int), out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:

                if (System.BitConverter.IsLittleEndian)
                    Array.Reverse(recBuffer);
                
                l = System.BitConverter.ToInt32(recBuffer, 0);
                
                break;
        }

        MakeTurnByLine(l);

    }

    // here we comparing scores, and if our score inright we change it
    void RecGameOver()
    {
        int MyWins = 0;
        int OtherWins = 0;

        int recHostId;
        int recConnectionId;
        int recChannelId;
        byte[] recBuffer = new byte[4];

        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, sizeof(int), out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:

                if (System.BitConverter.IsLittleEndian)
                    Array.Reverse(recBuffer);

                OtherWins = System.BitConverter.ToInt32(recBuffer, 0);

                break;
        }
        recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, sizeof(int), out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:

               if (System.BitConverter.IsLittleEndian)
                   Array.Reverse(recBuffer);

                MyWins = System.BitConverter.ToInt32(recBuffer, 0);

                break;
        }

        if(GameStats.Instance.FirstWins != MyWins)        
            GameStats.Instance.FirstWins = MyWins;

        if (GameStats.Instance.SecondWins != OtherWins)
            GameStats.Instance.SecondWins = OtherWins;


        if(!IsGameOverState)    // if something goes wrong and we still didn't start GameOver state
            GameOver();
        
    }

    // On oponent disconnected
    public void Disconnect ()
    {
        NetworkTransport.Disconnect(hostId, connectionId, out error);
        NetworkTransport.RemoveHost(hostId);
        SceneManager.LoadScene(0);
    }

    // We generating grid and, if we server, we sending data about turn and grid
    public override void NewGame()
    {
        FirstScore = 0;
        SecondScore = 0;

        IsGameOverState = false;

        WaitingRespondScreen.SetActive(false);

        GameOverScreen.SetActive(false);

        int MainLength = GameStats.Instance.BoxesInWidth > GameStats.Instance.BoxesInHight ? GameStats.Instance.BoxesInWidth : GameStats.Instance.BoxesInHight;

        GameStats.Instance.BoxWide = 6f / MainLength;

        if (MyCamera != null)
            MyCamera.transform.position = new Vector3(GameStats.Instance.BoxesInWidth * GameStats.Instance.BoxWide / 2f,
                GameStats.Instance.BoxesInHight * GameStats.Instance.BoxWide / 2, -10);

        if (GameStats.Instance.isServer)
        {
            byte[] buffer = { (byte)E_Messages.M_NewGame };
            if (System.BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);

            buffer = System.BitConverter.GetBytes(GameStats.Instance.BoxesInWidth);
            if (System.BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(int), out error);

            buffer = System.BitConverter.GetBytes(GameStats.Instance.BoxesInHight);
            if (System.BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(int), out error);

            buffer = new byte[1];
            buffer = System.BitConverter.GetBytes(!IsFirstTurn);
            if (System.BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte) * buffer.Length, out error);
        }

        // refresh grid
        foreach (S_Box b in MyBoxes)
            Destroy(b.gameObject);
        foreach (S_Line l in MyLines)
            Destroy(l.gameObject);
        foreach (S_Dot d in MyDots)
            Destroy(d.gameObject);

        GridGenerator.GenerateGrid(out MyDots, out MyLines, out MyBoxes, GameStats.Instance.BoxWide, GameStats.Instance.BoxesInWidth, GameStats.Instance.BoxesInHight, MyBox, MyLine, MyDot);

        MyHUD.SetActive(true);
    }

    // game over state, showing score and sending data about it
    protected override void GameOver()
    {
        IsGameOverState = true;

        IsIReady = false;
        IsOponentReady = false;

        byte[] buffer = { (byte)E_Messages.M_GameOver };
        if (System.BitConverter.IsLittleEndian)
            Array.Reverse(buffer);

        NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);

        buffer = System.BitConverter.GetBytes(GameStats.Instance.FirstWins);
        if (System.BitConverter.IsLittleEndian)
            Array.Reverse(buffer);

        NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(int), out error);

        buffer = System.BitConverter.GetBytes(GameStats.Instance.SecondWins);
        if (System.BitConverter.IsLittleEndian)
            Array.Reverse(buffer);

        NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(int), out error);

        MyHUD.GetComponent<HUD>().UpdateHUD();
        MyHUD.SetActive(false);

        GameOverScreen.SetActive(true);
    }

    // Telling what we are ready, and if oponent ready starting game (if client sending to server information what we ready)
    public void SetReady()
    {
        IsIReady = true;

        if (IsOponentReady)
        {
            if (GameStats.Instance.isServer)
                NewGame();
            else
            {
                byte[] buffer = { (byte)E_Messages.M_GameInvite };
                if (System.BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);

                NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);

                GameOverScreen.SetActive(false);

                WaitingRespondScreen.SetActive(true);
            }
        }
        else
        {
            byte[] buffer = { (byte)E_Messages.M_GameInvite };
            if (System.BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);

            GameOverScreen.SetActive(false);

            WaitingRespondScreen.SetActive(true);
        }

    }

    // oponent tell us what he ready
    void GameInvite()
    {
        if (!IsIReady)
            IsOponentReady = true;
        else
            if (GameStats.Instance.isServer)
                NewGame();
            else
            {
                byte[] buffer = { (byte)E_Messages.M_GameInvite };
                if (System.BitConverter.IsLittleEndian)
                    Array.Reverse(buffer);

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);
            }
    }

    // or we making making turn or our oponent (after RecTurn function)
    public override void MakeTurnByLine(int inIndex)
    {
        // if it's our oponent line must be occupy (if it's we, it will do nothing)
        MyLines[inIndex].HardOccupy();

        int BoxCount = MyBoxes.Count;
        
        bool AllBoxesFill = true;
        int BoxesFound = 0;

        for (int i = 0; i < BoxCount; ++i)
        {
            if (AllBoxesFill)
                if (!MyBoxes[i].CheckIsFill())
                    AllBoxesFill = false;

            if (MyBoxes[i].isHasLine(inIndex))
                if (MyBoxes[i].CheckIsFill())
                {
                    MyBoxes[i].GetComponent<SpriteRenderer>().color = IsFirstTurn ? GameStats.Instance.FirstColor : GameStats.Instance.SecondColor;
                    ++BoxesFound;
                }

        }

        // if it's our turn send turn packet to our oponent
        if (IsFirstTurn)
        {
            byte[] buffer = { (byte)E_Messages.M_Turn };
            if (System.BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);

            buffer = System.BitConverter.GetBytes(inIndex);
            if (System.BitConverter.IsLittleEndian)
                Array.Reverse(buffer);

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(int), out error);
        }
       
        if (IsFirstTurn)
            FirstScore += BoxesFound;
        else
            SecondScore += BoxesFound;

        if (AllBoxesFill)
        {
            if (FirstScore > SecondScore)
                GameStats.Instance.FirstWins += 1;
            if(FirstScore < SecondScore)
                GameStats.Instance.SecondWins += 1;
            GameOver();
            return;
        }

        // if next turn not our, block input
        IsFirstTurn = CanTurn = !IsFirstTurn;
    }


    

}
