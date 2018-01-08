using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public enum E_Messages
{
    M_Turn = 0,
    M_NewGame = 1,
    M_GameOver = 2
}

public class GameManager_Online : GameManager {

    int hostId;
    int channelId;
    int connectionId;

    byte error;

    private void Start()
    {
        print("Start connecting");

        //byte[] buffer = System.BitConverter.GetBytes(GameStats.Instance.BoxesInWidth);
        print("byte : " + sizeof(byte));

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
                    print("Get Game over");
                    RecGameOver();
                }
                if (recBuffer[0] == (byte)E_Messages.M_Turn)
                {
                    print("Get turn");
                    RecTurn();
                }
                break;
            case NetworkEventType.DisconnectEvent:
                print("OnDisconected");
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

    }

    private void Disconnect ()
    {
        
    }

    protected override void NewGame()
    {
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

        
    }

    protected override void GameOver()
    {
        base.GameOver();
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
            GameOver();
            return;
        }


        IsFirstTurn = CanTurn = !IsFirstTurn;
    }

}
