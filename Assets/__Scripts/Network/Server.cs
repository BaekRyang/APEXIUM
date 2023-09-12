using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using PimDeWitte.UnityMainThreadDispatcher;
using ProtoBuf;
using UnityEngine;

public class Server : MonoBehaviour
{
    [SerializeField] [Inject] private PlayerManager _playerManager;
    
    public static Dictionary<EndPoint, int> clients = new();

    private const string PREFIX                      = "<color=red>Server</color> -";
    private const int    PACKET_ADDITIONAL_DATA_SIZE = 2;

    private          Socket       _serverSocket;             //서버 소켓
    private readonly List<Socket> _clientSocketList = new(); //연결된 클라이언트 소켓 목록
    private readonly ArrayList    _receivedDataList = new(); //각 클라이언트의 송신 데이터를 저장할 버퍼

    private const int PORT = 11211;

    private UdpClient _udpClient;

    private int _clientID;

    public bool bootServer = false;

    private void StartServer()
    {
        if (!SetupTcpServer())
            Debug.Log($"{PREFIX} Initialize TCP - Server Failed");
        else
            Debug.Log($"{PREFIX} Initialize Server Success");

        SetupUDPServer();
        
        DIContainer.Inject(this);
    }

    private void Update()
    {
        switch (bootServer)
        {
            case true when _serverSocket == null:
                StartServer();
                break;
            case false when _serverSocket != null:
                Shutdown();
                break;
        }

        //서버 소켓이 null이거나 연결되지 않았다면 리턴
        if (_serverSocket == null) return;

        //서버 소켓이 연결되었다면 연결 요청이 있는지 감시한다.

        ArrayList _listenList = new();

        //외부에서 ArrayList를 선언하고 Update에서 Clear()하는 방식이 더 좋을거 같다.
        _listenList.Add(_serverSocket);


        Socket.Select(_listenList, null, null, 1000);

        //처음의 각 Argument는 읽기, 쓰기, 에러 감시용 소켓 목록이다.
        //첫번째 Argument에 _listenList가 있으므로, _listenList에 있는 소켓들이 읽기 가능한지 감시하고,
        //읽기가 가능한 소켓만 남기고 나머지는 제거한다.
        //이 곳에서는 연결 요청이 있는지 감시한다. (_listenList)

        //1000은 타임아웃으로, "1000μs => 1ms" 동안 데이터가 없으면 Select()는 null을 반환한다.

        foreach (Socket _socket in _listenList) //연결 요청이 들어온 소켓 목록을 순회
            HandleNewConnection(_socket);

        if (_clientSocketList.Count > 0) //연결된 클라이언트가 있다면
        {
            ArrayList _copySockets = new(_clientSocketList); //연결된 클라이언트 목록을 새 리스트에 복사하고
            Socket.Select(_copySockets, null, null, 1000);   //연결된 클라이언트 목록을 감시한다.

            //이 부분은 위와 다르게 _copySockets => _clientSockets 를 감시한다.
            //_clientSockets은 연결된 클라이언트가 있는곳으로, 연결된 클라이언트가 데이터를 보내는지 감시하고
            //데이터를 보낸 클라이언트만 남기고 나머지는 제거한다.


            //데이터를 보낸 소켓을 순회한다.
            foreach (Socket _socket in _copySockets) 
                HandleClientData(_socket, _copySockets.IndexOf(_socket));
        }

    #region UDP Receive

        //UDP 메세지 처리
        if (_udpClient is not null && _udpClient.Available > 0)
            HandleUDPData();

    #endregion
    }

    private void Shutdown()
    {
        foreach (var _socket in _clientSocketList)
            _socket.Shutdown(SocketShutdown.Both);

        _serverSocket.Close();
        _serverSocket = null;

        _udpClient.Close();
        _udpClient = null;

        _clientSocketList.Clear();
        _receivedDataList.Clear();
    }

    private void HandleUDPData()
    {
        IPEndPoint _remoteEP = new(IPAddress.Any, 0);             //모든 IP, Port에서 데이터를 받는다.
        byte[]     _data     = _udpClient.Receive(ref _remoteEP); //데이터를 받는다. _remoteEP에 데이터를 보낸 클라이언트의 IP, Port가 저장된다.

        byte       _playerID   = _data[0];             //플레이어 ID를 받는다.
        PacketType _packetType = (PacketType)_data[1]; //패킷 타입을 받는다.

        byte[] _pureData = new byte[_data.Length - PACKET_ADDITIONAL_DATA_SIZE];

        //두 헤더를 제외한 나머지 데이터를 복사한다.
        Array.Copy(_data, PACKET_ADDITIONAL_DATA_SIZE, _pureData, 0, _pureData.Length);


        if (_packetType is PacketType.Movement)
        {
            //데이터 역직렬화
            using (MemoryStream _stream = new(_pureData))
            {
                Vector3 _position = Serializer.Deserialize<Vector3Packet>(_stream);
                Debug.Log($"{PREFIX} UDP Data Received - {_remoteEP.Address}:{_remoteEP.Port} - {_packetType}  #{_playerID}: {_position}");
            }
        }
        else
            Debug.Log($"{PREFIX} UDP Data Received - {_remoteEP.Address}:{_remoteEP.Port} - {_packetType}  #{_playerID}: {_pureData}");
    }

    private void HandleNewConnection(Socket _socket)
    {
        Socket _newConnection = _socket.Accept(); //연결 요청을 수락하고
        _clientSocketList.Add(_newConnection);    //클라이언트 소켓 목록에 추가한다음
        _receivedDataList.Add(new ArrayList());   //데이터를 저장할 버퍼를 생성한다. (같은 클라이언트는 clientSocketList와 receivedDataList의 인덱스가 같다.)
        Debug.Log($"{PREFIX} New Client Connected - {_newConnection.RemoteEndPoint}");

        //클라이언트 ID를 증가시키고, 클라이언트에게 ID를 보낸다.
        Debug.Log($"{PREFIX} {_clientID - 1} ID is given to new client");
        Send(_newConnection, new byte[] { (byte)(_clientID++) }, PacketType.GiveID, ConnectType.TCP);

    }

    private void SendPlacePlayer(Socket _receivedSocket, int _playerID)
    {
        Debug.Log($"{PREFIX} Response Received");

        clients.Add(_receivedSocket.RemoteEndPoint, _playerID);
        Debug.Log($"{PREFIX} {_receivedSocket.RemoteEndPoint}'s Player Instantiate Command Created");

        //해당 클라이언트 플레이어 객체 생성 메세지를 뿌린다. (접속중인 모든 플레이어에게)
        foreach (var _socket in _clientSocketList)
        {
            Debug.Log($"{PREFIX} {_playerID} Object instantiate packet sent to {_socket.RemoteEndPoint}");
            Send(_socket, new byte[] { (byte)(_playerID) }, PacketType.PlacePlayer, ConnectType.TCP);
        }
    }

    private void HandleClientData(Socket _socket, int _index)
    {
        if (_socket.Available == 0) //읽을 수 있는 데이터의 크기가 0이라면 해당 클라이언트가 연결을 끊었다는 뜻이다.
        {
            ClientDisconnect(_socket);
            return;
        }

        Debug.Log($"{PREFIX} {_receivedDataList.Count} => {_index}"); //가끔씩 오류 뜨는데 디버그 찍으면 안뜸 (???)
        
        // if (_receivedDataList[_index] is null) return;
        byte[]    _receivedByteData = new byte[512];                                                                     //받은 데이터를 저장할 버퍼 생성
        ArrayList _receivedData     = _receivedDataList[_index] as ArrayList;                                            //해당 소켓의 데이터 저장 공간을 가져온다.
        int       _receiveDataSize  = _socket.Receive(_receivedByteData, 0, _receivedByteData.Length, SocketFlags.None); //데이터를 받는다. (Arg1에, Arg2부터, Arg3만큼 받는다.) 

        //Arg3이 버퍼의 Length니까 버퍼에 담길 수 있는 만큼만 최대로 받는다.
        
        Debug.Log($"{PREFIX} TCP Packet Received [ {_socket.RemoteEndPoint} ]: {_receiveDataSize} bytes");

        if (_receivedData == null) //이럴일은 없을 것 같긴한데, 해당 소켓의 데이터 저장공간이 없으면 return
        {
            Debug.Log($"{PREFIX} Received Data is null");
            UpdateClientDataList();
            return;
        }

        for (int _i = 0; _i < _receiveDataSize; _i++)
            _receivedData.Add(_receivedByteData[_i]); //받은 데이터를 _receivedData에 복사한다.

        while (_receivedData.Count > 0) 
        {
            int        _packetSize = (byte)_receivedData[0];       //패킷의 첫번째 정보는 패킷의 크기이다. 이를 이용해 패킷을 분할한다.
            PacketType _packetType = (PacketType)_receivedData[1]; //패킷의 두번째 정보는 패킷의 타입이다.

            if (_packetSize >= _receivedData.Count) //받은 데이터의 크기가 패킷에 명시된 것 보다 작으면 => "완전한 패킷을 받지 못했다면" 
                return;                             //다음에 받은 데이터와 합쳐 완전한 패킷을 만들어야 데이터를 처리할 수 있다.

            byte[] _pureData = _receivedData.GetRange(PACKET_ADDITIONAL_DATA_SIZE, _packetSize).ToArray(typeof(byte)) as byte[]; //패킷의 데이터 부분만 복사 - "헤더의 끝부터 -> 데이터의 사이즈 만큼",
            _receivedData.RemoveRange(0, _packetSize + PACKET_ADDITIONAL_DATA_SIZE);                                             //패킷을 분할했으므로 _buffer에서는 해당 패킷을 제거한다.

            if (_pureData == null)
                throw new NullReferenceException("pureData is null");

            ProcessPacket(_socket, _packetType, _pureData);

            foreach (var _byte in _pureData)
                Debug.Log($"{PREFIX} Packet Contents : {_byte}");
        }
    }

    private void UpdateClientDataList()
    {
        if (_clientSocketList.Count == _receivedDataList.Count)
            return;
        throw new Exception("Client Socket List and Received Data List are not matched");
        //두 리스트의 크기가 다르면 처리할 메서드
        //근데 두 리스트의 생성과 삭제는 언제나 같이 일어나므로 이럴일은 없을 것 같다.
    }

    private void ProcessPacket(Socket _socket, PacketType _packetType, byte[] _pureData)
    {
        switch (_packetType)
        {
            case PacketType.Movement:
            {
                Vector3Packet _receivedClickPosition;

                //MemoryStream은 IDisposable을 구현하고 있는데 이는 C#의 Garbage Collector가 자동으로 메모리를 해제하지 않는다. (파일 입출력 등에 사용됨)
                //그래서 Dispose()를 호출하여 메모리를 해제해야 하는데, 이를 자동으로 하기 위하여 using을 사용한다. - using Range가 끝나면 알아서 Dispose()를 호출한다.
                using (var _stream = new MemoryStream(_pureData)) //그리고 byte[]로 변환된 패킷을 _receivedClickPosition으로 Deserialize하여 저장한다.
                    _receivedClickPosition = Serializer.Deserialize<Vector3Packet>(_stream);

                Debug.Log($"{PREFIX} Packet Received [ {_socket.RemoteEndPoint} ]: {_pureData.Length} bytes");
                Debug.Log($"{PREFIX} Packet Type : {_packetType}");
                Debug.Log($"{PREFIX} Packet Contents : {_receivedClickPosition}");
                break;
            }
            case PacketType.CheckOK:
            {
                int _receivedID = _pureData[0];
                SendPlacePlayer(_socket, _receivedID);
                break;
            }
            case PacketType.GiveID:
                break;
            case PacketType.PlacePlayer:
                break;
            case PacketType.RPC:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void ClientDisconnect(Socket _socket)
    {
        if (clients.TryGetValue(_socket.RemoteEndPoint, out int _playerID))
        {
            Debug.Log($"{PREFIX} Client Disconnected - {_socket.RemoteEndPoint} \n {clients.Count}");
            Debug.Log($"{PREFIX} Remove Client Command Sent to GameManager");
            _playerManager.RemovePlayer(_playerID);
        }
        else
            Debug.Log($"{PREFIX} Cannot find client - {_socket.RemoteEndPoint}. Is it already disconnected?");
        
        _receivedDataList.RemoveAt(_clientSocketList.IndexOf(_socket));
        _clientSocketList.Remove(_socket);
        
        clients.Remove(_socket.RemoteEndPoint);
        _socket.Shutdown(SocketShutdown.Both);
        _socket.Close();
    }

    private async void Send(Socket _socket, byte[] _data, PacketType _type, ConnectType _connectType)
    {
        switch (_connectType)
        {
            case ConnectType.TCP:
            {
                if (_socket == null)
                    return;

                //데이터 합치기
                byte[] _fullPacket = new byte[_data.Length + PACKET_ADDITIONAL_DATA_SIZE];
                _fullPacket[0] = (byte)_data.Length;
                _fullPacket[1] = (byte)_type;
                _data.CopyTo(_fullPacket, PACKET_ADDITIONAL_DATA_SIZE);

                Debug.Log($"{PREFIX} Packet Sent : {_data.Length} bytes / Type : {_type}( {(byte)_type} )");
                await _socket.SendAsync(_fullPacket, SocketFlags.None); //데이터를 보낸다.

            }
                break;
            case ConnectType.UDP:
            {
            }
                break;
        }
    }

    private bool SetupTcpServer()
    {
        Debug.Log($"{PREFIX} Start");

    #region Socket Initialization

        _serverSocket = new(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        //C++ : socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
        if (_serverSocket == null)
        {
            Debug.Log($"{PREFIX} Failed to create socket");
            return false;
        }

    #endregion

    #region Socket Binding

        IPEndPoint _ipEndPoint = new(IPAddress.Any, PORT);

        //C++ : SOCKADDR_IN addr = {AF_INET, htons(PORT), INADDR_ANY}
        //C#에서는 IPEndPoint를 사용한다. C#은 C++과 다르게 IPv4/6을 모두 지원하므로 IP버젼을 명시적으로 지정하지 않는다.
        //또한 IPEndPoint를 사용하면 포트번호를 설정할 때 내부적으로 바이트 순서 변환이 자동으로 이루어진다.

        _serverSocket.Bind(_ipEndPoint);

        //C++ : bind(socket, (SOCKADDR*)&saddr, sizeof(saddr));
        if (_serverSocket == null)
        {
            Debug.Log($"{PREFIX} Failed to bind socket");
            return false;
        }

    #endregion

    #region Socket Listening

        _serverSocket.Listen(0);

        //C++ : listen(socket, SOMAXCONN);
        //Listen()에 0을 주면 OS가 가능한 많은 연결을 처리하도록 한다. (SOMAXCONN) 일반적으로는 명시적으로 표시하는것이 좋다.
        if (_serverSocket == null)
        {
            Debug.Log($"{PREFIX} Failed to listen socket");
            return false;
        }

    #endregion

    #region Accept

        // _serverSocket.BeginAccept(AcceptCallback, null);
        //AcceptCallback은 콜백함수로서, 클라이언트가 접속하면 호출된다.
        //null은 BeginAccept의 state인자로서, 콜백함수에 전달되는 인자이다. (이 예제에서는 사용하지 않는다.)

    #endregion

        return true;
    }

    private void SetupUDPServer()
    {
        _udpClient = new(PORT);
    }
}