using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using UnityEngine;
using ProtoBuf;

public class Client : MonoBehaviour
{
    private const string PREFIX = "<color=blue>Client</color> - ";
    
    private        string    _serverIP   = "127.0.0.1";
    private        int       _serverPort = 11211;
    private static Socket    ClientSocket;
    private static UdpClient UDPClient;

    private readonly byte[] _receiveBuffer = new byte[1024]; //1KB

    private void Start()
    {

    #region TCP Init

        ClientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); //클라이언트 소켓 준비
        //Bind는 필요없음 : 클라이언트는 자동으로 포트를 골라서 서버에 연결함

        IPAddress  _serverAddress  = IPAddress.Parse(_serverIP);                  //서버 주소를 IPAddress로 변환
        IPEndPoint _serverEndPoint = new IPEndPoint(_serverAddress, _serverPort); //서버 주소와 포트를 IPEndPoint로 변환

    #region RequestConnection

        try
        {
            ClientSocket.Connect(_serverEndPoint); //서버에 연결을 요청한다.
            Debug.Log($"{PREFIX} Connecting to Server");

            BeginReceive(); //서버로부터 데이터를 받기 시작한다.
        }
        catch (SocketException e)
        {
            Debug.Log($"{PREFIX} Failed to Connect to Server");
            Debug.Log(e.Message);
        }

    #endregion

    #endregion

    #region UDP Init

        UDPClient = new UdpClient();
        UDPClient.Connect(_serverEndPoint); 
        //UDP는 TCP의 Connect와 다르게 실제로 연결을 만드는것은 아니고
        //UdpClient.Send()에 매번 EndPoint를 넘겨줘야 하는데, 이 때 EndPoint의 기본값을 설정해준다.
        //여기서는 언제나 서버와 통신을 하기 때문에 서버의 EndPoint를 기본값으로 설정해준다.

    #endregion
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3Packet _mousePosition = new Vector3Packet(Camera.main.ScreenToWorldPoint(Input.mousePosition)); //마우스 클릭 위치를 얻어온다.

            using (var _stream = new MemoryStream()) //메모리 스트림을 이용하여 직렬화한다. MemoryStream()은 데이터를 쓸 수 있는 상태
            {
                Serializer.Serialize(_stream, _mousePosition);                     //mousePosition을 stream에 serialize한다.
                Send(_stream.ToArray(), PacketType.Movement, ConnectType.UDP); //stream을 byte[]로 변환하여 Send한다. 두번째 Arg은 데이터 타입 명시용
            }
        }
    }

    private static void Send(byte[] p_packet, PacketType p_type, ConnectType p_connectType)
    {
        if (p_connectType is ConnectType.TCP)
        {
            if (ClientSocket == null)
                return;

            byte[] _prefSize   = { (byte)p_packet.Length }; //버퍼의 맨 앞부분에 이 버퍼의 길이정보가 있는데 이걸 먼저 보내고
            byte[] _packetType = { (byte)p_type };          //변수의 타입의 정보를 다음으로 보내기 위해 저장한다.

            //각각 순서대로 보낸다.
            ClientSocket.Send(_prefSize);
            ClientSocket.Send(_packetType);
            ClientSocket.Send(p_packet);

            Debug.Log($"{PREFIX} Packet Sent(TCP) : {p_packet.Length} bytes");
        } 
        else if (p_connectType is ConnectType.UDP)
        {
            if (UDPClient == null)
                return;
            
            //새로운 bye[]를 만들어서 필요한 데이터를 넣는다.
            //데이터의 순서는 [시퀀스 넘버, 패킷 타입, 패킷 데이터] 순서로 넣는다.
            byte[] _packet = new byte[p_packet.Length + 2]; //시퀀스 넘버와 패킷 타입을 넣을 공간을 미리 만들어둔다.
            _packet[0] = (byte)GameManager.Instance.playerID;
            _packet[1] = (byte)p_type;
            Array.Copy(p_packet, 0, _packet, 2, p_packet.Length); //p_packet의 데이터를 _packet에 복사한다.
            
            UDPClient.Send(_packet, _packet.Length); //데이터를 보낸다.
            Debug.Log($"{PREFIX} Packet Sent(UDP) : {_packet.Length} bytes");
        }
        
    }

    private void BeginReceive() => 
        ClientSocket.BeginReceive(_receiveBuffer, 0, _receiveBuffer.Length, SocketFlags.None, new AsyncCallback(ReceiveCallback), null);
    // 비동기적으로 데이터를 받는다. - 서버의 Receive와 유사한데, 여기는 비동기적으로 받아서 Callback으로 처리한다.
    
    private          int?      _totalPacketSize; //nullable int 타입
    private readonly ArrayList _pendingDataBuffer = new ArrayList();

    private void ReceiveCallback(IAsyncResult p_result)
    {
        //이 함수가 실행되었다는것 => 데이터를 받아서 _receiveBuffer에 넣었음

        // 데이터 수신을 완료하고, 수신된 데이터의 길이를 얻는다.
        int _received = ClientSocket.EndReceive(p_result);

        Debug.Log($"{PREFIX} Data Received / {_received}bytes");

        if (_received <= 0)
        {
            Debug.Log($"{PREFIX} 0 byte Received - Aborting");
            return;
        }

        if (_totalPacketSize is null)
        {
            Debug.Log($"{PREFIX} New Packet Size Received {_receiveBuffer[0]}");
            _totalPacketSize = _receiveBuffer[0];
        }

        if (_pendingDataBuffer.Count < _totalPacketSize + 2)
        {
            for (int i = 0; i < _received; i++)
                _pendingDataBuffer.Add(_receiveBuffer[i]);
            Debug.Log($"{PREFIX} Packet piece received {_received} - " +
                      $"Total : {_pendingDataBuffer.Count} / {_totalPacketSize + 2}");
        }
        else
        {
            Debug.Log($"{_pendingDataBuffer.Count} / {_totalPacketSize}");
        }

        if (_pendingDataBuffer.Count >= _totalPacketSize + 2)
        {
            Debug.Log($"{PREFIX} Full Packet Received - Pending : {_pendingDataBuffer.Count} / Total : {_totalPacketSize + 2} ");
            StringBuilder _sb = new StringBuilder();
            foreach (byte _b in _pendingDataBuffer)
                _sb.Append(_b + " ");
            Debug.Log($"{PREFIX} Data : {_sb}");

            // 수신된 데이터를 처리한다.
            byte[] _dataBuf = new byte[_pendingDataBuffer.Count];
            //PendingDataBuffer에 있는 데이터 전부를 _dataBuf에 복사한다.
            _pendingDataBuffer.CopyTo(_dataBuf);

            HandleReceivedData(_dataBuf);
            //다음 패킷을 위해 초기화한다.
            _pendingDataBuffer.Clear();
            _totalPacketSize = null;
        }

        // 다음 데이터 수신을 위해 BeginReceive를 다시 호출한다.
        BeginReceive();
    }

    private void HandleReceivedData(byte[] p_receivedData)
    {
        int        _additionalDataSize = 2;                             //헤더의 크기
        int        _packetSize         = p_receivedData[0];             //패킷의 첫번째 정보는 패킷의 크기
        PacketType _packetType         = (PacketType)p_receivedData[1]; //패킷의 두번째 정보는 패킷의 타입

        Debug.Log($"{PREFIX} Packet Type : {_packetType}");

    #region InitPlayerID

        //서버에서 보낸 초기화 메세지는 따로 처리해준다.
        if (_packetType is PacketType.GiveID)
        {
            GameManager.Instance.playerID = p_receivedData[2];
            Debug.Log($"{PREFIX} Your Client ID : {GameManager.Instance.playerID}");

            //서버에게 클라이언트가 준비되었다고 알린다.
            Send(new byte[] { (byte)GameManager.Instance.playerID }, PacketType.CheckOK, ConnectType.TCP);
            Debug.Log($"{PREFIX} Check OK Sent");
            return;
        }

    #endregion

    #region PlacePlayer

        if (_packetType is PacketType.PlacePlayer)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => { GameManager.Instance.InstantiatePlayer(p_receivedData[2]); });
            Debug.Log($"{PREFIX} Player Placed : {p_receivedData[2]}");
            return; //여기에 return 안하니까 BeginReceive가 먹통이 되버리는 문제가 있었음
        }

    #endregion

        byte[] _pureData = new byte[_packetSize]; //헤더를 제외한 데이터의 크기
        for (int i = _additionalDataSize; i < _pureData.Length; i++)
            _pureData[i] = p_receivedData[i]; //헤더를 제외한 데이터를 _pureData에 넣는다.

        //받은 데이터를 처리한다.
        using (var _stream = new MemoryStream(_pureData)) //받은 데이터를 MemoryStream에 넣는다.
        {
            Vector3Packet _receivedPacket = Serializer.Deserialize<Vector3Packet>(_stream); //받은 데이터를 역직렬화한다.
            Debug.Log($"{PREFIX} Received Data : {_receivedPacket}");
        }
    }
}