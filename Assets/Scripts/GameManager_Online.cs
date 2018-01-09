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

    private void Start()
    {
        print("Start connecting");

        //byte[] buffer = System.BitConverter.GetBytes(GameStats.Instance.BoxesInWidth);
        print("byte : " + sizeof(byte));

        WaitingRespondScreen.SetActive(true);

        Connect();
    }

    public void Connect()
    {

        NetworkTransport.Init();

        ConnectionConfig config = new ConnectionConfig();
        channelId = config.AddChannel(QosType.Reliable);

        //Here we created a topology that allows up to 10 connections, each of them are configured by the parameters defined in previous step.
        HostTopology topology = new HostTopology(config, 10);

        //As all preliminary steps are done, we can create the host (open socket):
        //Here we add a new host on port 8888 and any ip addresses. This host will support up to 10 connections, and each connection will have parameters as we defined in config object.
        if (GameStats.Instance.isServer)
            hostId = NetworkTransport.AddHost(topology, GameStats.Instance.PORT);
        else
        {
            hostId = NetworkTransport.AddHost(topology, 0); // !!! can be problems !!!
            connectionId = NetworkTransport.Connect(hostId, GameStats.Instance.IP, GameStats.Instance.PORT, 0, out error);
        }
        print("End connecting");
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
                print("OnConnected");
                if (GameStats.Instance.isServer)
                {

                    connectionId = recConnectionId;

                    NewGame();
                }
                else
                    if (connectionId != recConnectionId)
                    connectionId = recConnectionId;
                break;
            case NetworkEventType.DataEvent:
                
                if (recBuffer[0] == (byte)E_Messages.M_NewGame)
                {
                    print("GetNewGame");
                    RecNewGame();
                }
                if (recBuffer[0] == (byte)E_Messages.M_GameOver)
                {
                    if (!GameStats.Instance.isServer)
                        RecGameOver();
                }
                if (recBuffer[0] == (byte)E_Messages.M_Turn)
                {
                    print("Get turn");
                    RecTurn();
                }
                if(recBuffer[0] == (byte)E_Messages.M_GameInvite)
                {
                    print("GameInvite");
                    GameInvite();
                }
                break;
            case NetworkEventType.DisconnectEvent:
                print("OnDisconected");
                Disconnect();
                break;


        }
    }

    void TestRecive()
    {
        int recHostId;
        int recConnectionId;
        int recChannelId;
        byte[] recBuffer = new byte[4];

        int dataSize;
        byte error;
        NetworkEventType recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, recBuffer.Length * sizeof(byte), out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:
                if (BitConverter.IsLittleEndian)
                {
                    print("little endian indeed");
                    Array.Reverse(recBuffer);
                }
                print("TestRecive : " + System.BitConverter.ToInt32(recBuffer, 0));
                break;
        }
    }

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
                //if (System.BitConverter.IsLittleEndian)
                //    Array.Reverse(recBuffer);
                x = System.BitConverter.ToInt32(recBuffer,0);
                break;
        }
        recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, sizeof(int), out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:
                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(recBuffer);
                y = BitConverter.ToInt32(recBuffer, 0);
                break;
        }

        recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, sizeof(bool), out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:
                //if (BitConverter.IsLittleEndian)
                //    Array.Reverse(recBuffer);
                T = BitConverter.ToBoolean(recBuffer, 0);
                break;
        }

        GameStats.Instance.BoxesInWidth = x;
        GameStats.Instance.BoxesInHight = y;
        IsFirstTurn = CanTurn = T;

        NewGame();
    }

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
                //if (System.BitConverter.IsLittleEndian)
                //    Array.Reverse(recBuffer);
                
                l = System.BitConverter.ToInt32(recBuffer, 0);
                
                break;
        }

        MakeTurnByLine(l);

    }

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
                //if (System.BitConverter.IsLittleEndian)
                //    Array.Reverse(recBuffer);

                OtherWins = System.BitConverter.ToInt32(recBuffer, 0);

                break;
        }
        recData = NetworkTransport.Receive(out recHostId, out recConnectionId, out recChannelId, recBuffer, sizeof(int), out dataSize, out error);
        switch (recData)
        {
            case NetworkEventType.DataEvent:
                //if (System.BitConverter.IsLittleEndian)
                //    Array.Reverse(recBuffer);

                MyWins = System.BitConverter.ToInt32(recBuffer, 0);

                break;
        }

        if(GameStats.Instance.FirstWins != MyWins)        
            GameStats.Instance.FirstWins = MyWins;

        if (GameStats.Instance.SecondWins != OtherWins)
            GameStats.Instance.SecondWins = OtherWins;


        if(!IsGameOverState)
            GameOver();
        
    }

    public void Disconnect ()
    {
        NetworkTransport.Disconnect(hostId, connectionId, out error);
        SceneManager.LoadScene(0);
    }

    public override void NewGame()
    {
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

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);

            buffer = System.BitConverter.GetBytes(GameStats.Instance.BoxesInWidth);
            
            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(int), out error);

            buffer = System.BitConverter.GetBytes(GameStats.Instance.BoxesInHight);

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(int), out error);

            buffer = System.BitConverter.GetBytes(!IsFirstTurn);

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte) * buffer.Length, out error);
        }

        foreach (S_Box b in MyBoxes)
            Destroy(b.gameObject);
        foreach (S_Line l in MyLines)
            Destroy(l.gameObject);
        foreach (S_Dot d in MyDots)
            Destroy(d.gameObject);


        GridGenerator.GenerateGrid(out MyDots, out MyLines, out MyBoxes, GameStats.Instance.BoxWide, GameStats.Instance.BoxesInWidth, GameStats.Instance.BoxesInHight, MyBox, MyLine, MyDot);

        MyHUD.SetActive(true);
    }

    protected override void GameOver()
    {
        IsGameOverState = true;

        IsIReady = false;
        IsOponentReady = false;

        // Game over, start again

        MyHUD.GetComponent<HUD>().UpdateHUD();
        MyHUD.SetActive(false);

        GameOverScreen.SetActive(true);
    }

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

                NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);

                GameOverScreen.SetActive(false);

                WaitingRespondScreen.SetActive(true);
            }
        }
        else
        {
            byte[] buffer = { (byte)E_Messages.M_GameInvite };

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);

            GameOverScreen.SetActive(false);

            WaitingRespondScreen.SetActive(true);
        }

    }

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

                NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);
            }
    }


    public override void MakeTurnByLine(int inIndex)
    {
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

        if (IsFirstTurn)
        {
            byte[] buffer = { (byte)E_Messages.M_Turn };

            NetworkTransport.Send(hostId, connectionId, channelId, buffer, sizeof(byte), out error);

            buffer = System.BitConverter.GetBytes(inIndex);

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


        IsFirstTurn = CanTurn = !IsFirstTurn;
    }


    

}
