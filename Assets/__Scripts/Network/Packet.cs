using ProtoBuf;
using UnityEngine;

public class Packet : MonoBehaviour
{

}

public enum PacketType : byte
{
    GiveID,
    PlacePlayer,
    CheckOK,
    RPC,
    Movement
}

public enum ConnectType
{
    TCP,
    UDP
}

//ProtoBuf을 사용하기 위해서 선언
[ProtoContract]
public readonly struct Vector3Packet
{
    //ProtoBuf을 통해 Serialize할 변수에 ProtoMember Attribute를 붙여준다.
    [ProtoMember(1)] private readonly float x;
    [ProtoMember(2)] private readonly float y;
    [ProtoMember(3)] private readonly float z;

    public Vector3Packet(Vector3Packet p_other) //복사 생성자
    {
        x = p_other.x;
        y = p_other.y;
        z = p_other.z;
    }

    public Vector3Packet(Vector3 p_other) //Vector3 => Vector3Packet
    {
        x = p_other.x;
        y = p_other.y;
        z = p_other.z;
    }

    public static implicit operator Vector3(Vector3Packet p_other) //Vector3Packet => Vector3 연산자
    {
        return new Vector3(p_other.x, p_other.y, p_other.z);
    }

    public override string ToString()
    {
        return $"({x}, {y}, {z})";
    }
}