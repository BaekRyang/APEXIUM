using System;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class State : IState
{
    protected EnemyAI enemyAI;

    public IState Initialize(EnemyAI p_enemyAI)
    {
        enemyAI = p_enemyAI;
        return this;
    }

    public abstract void Enter();
    public abstract void Execute();
    public abstract void Exit();

    private const float CLIFF_DETECT_DISTANCE = 0.5f;

    protected int CliffDetect() //절벽 감지 함수. 
    {
        int     _layerMask = 1 << LayerMask.NameToLayer("Floor");
        Vector3 _position  = enemyAI._transform.position;

        Vector2 _checkPoint = new Vector2(
            _position.x + enemyAI._targetDirection.x * enemyAI.bodySize.x / 2,
            _position.y - enemyAI.bodySize.y                              / 2
        );

        RaycastHit2D _hit = Physics2D.Raycast(_checkPoint, Vector2.down, CLIFF_DETECT_DISTANCE, _layerMask);
        return _hit.collider == null ? 1 : 0;
    }
}

public class SWander : State
{
    private enum WanderState
    {
        Moving,
        Waiting
    }

    private struct FloatPair
    {
        public float x;
        public float y;

        public FloatPair(float xValue, float yValue2)
        {
            x = xValue;
            y = yValue2;
        }
    }

    private WanderState _currentState;
    private float       _nextMovableTime, _nextWaitTime;

    private readonly FloatPair _randomMovingTime = new FloatPair(3, 5),
                               _randomWaitTime   = new FloatPair(1, 3);

    public override void Enter()
    {
        _currentState = WanderState.Moving;
    }

    public override void Execute()
    {
        switch (_currentState)
        {
            case WanderState.Moving:
                if (Time.time >= _nextMovableTime)
                {
                    //Waiting 상태로 진입하고 다시 Move 상태로 진입할 시간을 랜덤으로 설정
                    _nextMovableTime = Time.time + Random.Range(_randomWaitTime.x, _randomWaitTime.y);
                    _currentState    = WanderState.Waiting;
                }

                if (CliffDetect() == 1)
                    enemyAI._targetDirection *= -1; //절벽 감지시 방향 반전

                break;
            
            case WanderState.Waiting:
                if (Time.time >= _nextMovableTime)
                {
                    //Move 상태로 진입하고 다시 Waiting 상태로 진입할 시간을 랜덤으로 설정
                    _nextWaitTime = Time.time + Random.Range(_randomMovingTime.x, _randomMovingTime.y);
                    _currentState = WanderState.Moving;
                }

                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}

public class SChase : State
{
    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}

public class SAttack : State
{
    public override void Enter()
    {
        throw new System.NotImplementedException();
    }

    public override void Execute()
    {
        throw new System.NotImplementedException();
    }

    public override void Exit()
    {
        throw new System.NotImplementedException();
    }
}