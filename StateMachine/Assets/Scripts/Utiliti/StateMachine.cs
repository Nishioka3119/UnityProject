using System;
using System.Collections.Generic;

// ジェネリック
// Tの部分は自由に変更可能
public class StateMachine<T>
{
    private class State
    {
        // readonly・・・最代入不可な変数修飾子
        // 関数呼び出しやメンバ変数への代入は可能

        // 開始時に呼び出されるデリゲート(関数)
        private readonly Action mEnterAction;
        // 更新時に呼び出されるデリゲート(関数)
        private readonly Action mUpdateAction;
        // 終了時に呼び出されるデリゲート(関数)
        private readonly Action mExitAction;

        // コンストラクタ
        public State(Action updateAct = null,
                     Action enterAct = null,
                     Action exitAct = null)
        {
            // ??演算子・・・代入する変数の中身がnullの場合??の右側にあるものが代入される
            // nullを許容したくない時に使える
            mUpdateAction = updateAct ?? delegate { };
            mEnterAction = enterAct ?? delegate { };
            mExitAction = exitAct ?? delegate { };
        }

        public void Enter()
        {
            mEnterAction();
        }

        public void Update()
        {
            mUpdateAction();
        }

        public void Exit()
        {
            mExitAction();
        }
    }

    private Dictionary<T, State> mStateDictionary = new Dictionary<T, State>();

    private State mCurrentState;


    public void Add(T Key, Action updateAct = null,
                           Action enterAct = null,
                           Action exitAct = null)
    {
        mStateDictionary.Add(Key, new State(updateAct, enterAct, exitAct));
    }

    public void ChangeState(T Key)
    {
        // ?演算子・・・変数の中身がnullでなければ変数にアクセスする
        mCurrentState?.Exit();
        mCurrentState = mStateDictionary?[Key];
        mCurrentState?.Enter();
    }

    public void Update()
    {
        if (mCurrentState == null)
        {
            return;
        }
        mCurrentState.Update();
    }

    public void Clear()
    {
        mStateDictionary.Clear();
        mCurrentState = null;
    }
}
