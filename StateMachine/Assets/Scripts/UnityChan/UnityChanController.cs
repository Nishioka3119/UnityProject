using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// AddComponentしたときにGameObjectにRequireComponentで指定したComponentが
// なければAddComponentしてくれる命令
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Rigidbody))]

public class UnityChanController : MonoBehaviour
{
    enum AnimationState : int
    {
        Idle,
        Run,
        Jump
    }

    StateMachine<AnimationState> stateMachine = new StateMachine<AnimationState>();

    Animator animator = null;

    AnimatorBehaviour animatorBehaviour = null;

    Rigidbody rigidbody = null;

    [SerializeField] float rotateSpeed = 1.0f;
    [SerializeField] float moveSpeed = 1.0f;
    [SerializeField] float jumpPower = 5.0f;

    Vector3 velocity;

    // Start is called before the first frame update
    void Start()
    {
        InitializeComponent();
        InitializeState();
    }

    void InitializeComponent()
    {
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();
        animatorBehaviour = animator.GetBehaviour<AnimatorBehaviour>();
    }

    void InitializeState()
    {
        // アニメーション追加
        stateMachine.Add(AnimationState.Idle, IdleUpdate, IdleInitialize);
        stateMachine.ChangeState(AnimationState.Idle);

        stateMachine.Add(AnimationState.Jump, JumpUpdate, JumpInitialize);

        stateMachine.Add(AnimationState.Run, RunUpdate, RunInitialize);
    }

    //----------------------------------------------------------------------------

    void IdleUpdate()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            stateMachine.ChangeState(AnimationState.Jump);
        }
        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, 1);   // 正面方向に進む
            stateMachine.ChangeState(AnimationState.Run);
        }

        //JumpChange();
        DirectionChange();
    }

    void JumpUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            if (animatorBehaviour.NormalizedTime > 0.65f)
            {
                stateMachine.ChangeState(AnimationState.Run);
            }
        }
        else {

        }

        //var animeState = animator.GetCurrentAnimatorStateInfo(0);
        //if (animeState.normalizedTime > 1.0f)
        //{
        //    stateMachine.ChangeState(AnimationState.Idle);
        //}
    }

    void RunUpdate()
    {
        DirectionChange();

        if (Input.GetKeyUp(KeyCode.W))
        {
            stateMachine.ChangeState(AnimationState.Idle);
        }
    }

    //----------------------------------------------------------------------------

    void IdleInitialize()
    {
        // アニメーション遷移
        animator.CrossFadeInFixedTime("Idle", 0.0f);
    }

    void RunInitialize()
    {
        animator.CrossFadeInFixedTime("Run", 0.0f);
    }

    void JumpInitialize()
    {
        animator.CrossFadeInFixedTime("Jump", 0.0f);
        rigidbody.AddForce(0.0f, jumpPower, 0.0f, ForceMode.Impulse);
        animatorBehaviour.EndCallBack = () => { stateMachine.ChangeState(AnimationState.Idle); };
    }

    //----------------------------------------------------------------------------

    // 回転
    void DirectionChange()
    {
        if (Input.GetKey(KeyCode.A))
        {
            this.transform.Rotate(0, -10, 0);
        }
        if (Input.GetKey(KeyCode.D))
        {
            this.transform.Rotate(0, 10, 0);
        }
    }

    void JumpChange()
    {

    }

    void Move(float aMoveSpeed)
    {
        // ローカル座標のZ方向へ向いてくれる
        velocity = transform.TransformDirection(new Vector3(0, 0, aMoveSpeed));
        var position = transform.position + velocity;
        rigidbody.MovePosition(position);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();

        var animeState = animator.GetCurrentAnimatorStateInfo(0);
        // unityのデフォルトで取得できるアニメーション再生時間
        Debug.LogFormat($"before normalizedTime{animeState.normalizedTime}");
        // 新たに作り直して0~1の間で取得できるようにした
        // アニメーション再生時間
        Debug.LogFormat($"after normalizedTime={animatorBehaviour.NormalizedTime}");
        if (animatorBehaviour.NormalizedTime > 1.0f)
        {
            animatorBehaviour.ResetTime();
        }
    }
}
