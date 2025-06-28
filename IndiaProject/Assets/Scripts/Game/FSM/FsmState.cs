using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FSM
{
    /// <summary>
    /// 有限状态机基类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class FsmState<T> where T : class
    {
        /// <summary>
        /// 初始化有线状态机基类的新实例
        /// </summary>
        public FsmState()
        {
        }

        /// <summary>
        /// 有限状态机状态初始化时调用
        /// </summary>
        /// <param name="fsm"></param>
        protected internal virtual void OnInit(IFsm<T> fsm)
        {
        }

        /// <summary>
        /// 有限状态机状态进入时调用
        /// </summary>
        /// <param name="fsm"></param>
        protected internal virtual void OnEnter(IFsm<T> fsm)
        {
        }

        /// <summary>
        /// 有线状态机状态轮询时调用
        /// </summary>
        /// <param name="fsm"></param>
        /// <param name="elapseSeconds"></param>
        /// <param name="realElapseSecond"></param>
        protected internal virtual void OnUpdate(IFsm<T> fsm, float elapseSeconds, float realElapseSecond)
        {
        }

        /// <summary>
        /// 有限状态机状态离开时调用
        /// </summary>
        /// <param name="fsm"></param>
        /// <param name="isShutdown"></param>
        protected internal virtual void OnLeave(IFsm<T> fsm, bool isShutdown)
        {
        }

        /// <summary>
        /// 有限状态机状态销毁时调用
        /// </summary>
        /// <param name="fsm"></param>
        protected internal virtual void OnDestroy(IFsm<T> fsm)
        {
        }

        /// <summary>
        /// 切换当前有限状态机状态
        /// </summary>
        /// <param name="fsm"></param>
        /// <typeparam name="TState"></typeparam>
        protected void ChangeState<TState>(IFsm<T> fsm) where TState : FsmState<T>
        {
            // Fsm<T> fsmImplement = (Fsm<T>)fsm;
        }
    }
}