using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace Platformer
{
    public interface IState
    {
        void Enter();
        void Update();
        void FixedUpdate();

        void OnGUI();
        void Exit();
    }


    public class CustomState : IState
    {
        private Action mOnEnter;
        private Action mOnUpdate;
        private Action mOnFixedUpdate;
        private Action mOnGUI;
        private Action mOnExit;

        public CustomState OnEnter(Action onEnter)
        {
            mOnEnter = onEnter;
            return this;
        }


        public CustomState OnUpdate(Action onUpdate)
        {
            mOnUpdate = onUpdate;
            return this;
        }

        public CustomState OnFixedUpdate(Action onFixedUpdate)
        {
            mOnFixedUpdate = onFixedUpdate;
            return this;
        }

        public CustomState OnGUI(Action onGUI)
        {
            mOnGUI = onGUI;
            return this;
        }

        public CustomState OnExit(Action onExit)
        {
            mOnExit = onExit;
            return this;
        }

        public void Enter()
        {
            mOnEnter?.Invoke();
        }


        public void Update()
        {
            mOnUpdate?.Invoke();

        }

        public void FixedUpdate()
        {
            mOnFixedUpdate?.Invoke();
        }


        public void OnGUI()
        {
            mOnGUI?.Invoke();
        }

        public void Exit()
        {
            mOnExit?.Invoke();
        }
    }

    [Serializable]
    public class FSM<T>
    {
        [ShowInInspector, ReadOnly] protected Dictionary<T, IState> mStates = new Dictionary<T, IState>();

        public void AddState(T id, IState state)
        {
            mStates.Add(id, state);
        }


        public CustomState State(T t)
        {
            if (mStates.ContainsKey(t))
            {
                return mStates[t] as CustomState;
            }

            var state = new CustomState();
            mStates.Add(t, state);
            return state;
        }

        private IState mCurrentState;
        [ShowInInspector, ReadOnly] private T mCurrentStateId;

        public IState CurrentState => mCurrentState;
        public T CurrentStateId => mCurrentStateId;
        public T PreviousStateId { get; private set; }

        public long FrameCountOfCurrentState = 1;

        public void ChangeState(T t)
        {
            if (t.Equals(CurrentStateId)) return;

            if (mStates.TryGetValue(t, out var state))
            {
                if (mCurrentState != null)
                {
                    mCurrentState.Exit();
                    PreviousStateId = mCurrentStateId;
                    mCurrentState = state;
                    mCurrentStateId = t;
                    mOnStateChanged?.Invoke(PreviousStateId, CurrentStateId);
                    FrameCountOfCurrentState = 1;
                    mCurrentState.Enter();
                }
            }
        }

        private Action<T, T> mOnStateChanged = (_, __) => { };

        public void OnStateChanged(Action<T, T> onStateChanged)
        {
            mOnStateChanged += onStateChanged;
        }

        public void StartState(T t)
        {
            if (mStates.TryGetValue(t, out var state))
            {
                PreviousStateId = t;
                mCurrentState = state;
                mCurrentStateId = t;
                FrameCountOfCurrentState = 0;
                state.Enter();
            }
        }

        public void FixedUpdate()
        {
            mCurrentState?.FixedUpdate();
        }

        public void Update()
        {
            mCurrentState?.Update();
            FrameCountOfCurrentState++;
        }

        public void OnGUI()
        {
            mCurrentState?.OnGUI();
        }

        public void Clear()
        {
            mCurrentState = null;
            mCurrentStateId = default;
            mStates.Clear();
        }
    }

    public abstract class AbstractState<TStateId> : IState
    {
        void IState.Enter()
        {
            OnEnter();
        }

        void IState.Update()
        {
            OnUpdate();
        }

        void IState.FixedUpdate()
        {
            OnFixedUpdate();
        }

        public virtual void OnGUI()
        {
        }

        void IState.Exit()
        {
            OnExit();
        }

        protected virtual bool OnCondition() => true;

        protected virtual void OnEnter()
        {
        }

        protected virtual void OnUpdate()
        {

        }

        protected virtual void OnFixedUpdate()
        {

        }

        protected virtual void OnExit()
        {

        }
    }


}