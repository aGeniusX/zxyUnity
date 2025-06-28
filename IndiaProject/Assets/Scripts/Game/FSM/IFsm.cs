using System;
using System.Collections.Generic;
using GameFramework;

namespace FSM
{
    /// <summary>
    /// 有线状态机接口
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IFsm<T> where T : class
    {
        /// <summary>
        /// 获取有限状态机名称
        /// </summary>
        /// <value></value>
        string Name
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机名称完成名称
        /// </summary>
        /// <value></value>
        string FullName
        {
            get;
        }
        /// <summary>
        /// 获取有限状态机持有者
        /// </summary>
        /// <value></value>
        T Owner
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机状态的数量
        /// </summary>
        /// <value></value>
        int FsmStateCount
        {
            get;
        }

        /// <summary>
        /// 获取有限状态机是否正在运行
        /// </summary>
        /// <value></value>
        bool IsRunning
        {
            get;
        }

        /// <summary>
        /// 获取当前有限状态机状态是否被销毁
        /// </summary>
        /// <value></value>
        bool IsDestroyed
        {
            get;
        }

        /// <summary>
        /// 获取当前有限状态机状态
        /// </summary>
        /// <value></value>
        FsmState<T> CurrentState
        {
            get;
        }

        /// <summary>
        /// 获取当前有限状态机状态持续时间
        /// </summary>
        /// <value></value>
        float CurrentStateTime
        {
            get;
        }

        /// <summary>
        /// 开始有限状态机
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        void Start<TState>() where TState : FsmState<T>;

        /// <summary>
        /// 开始有限状态机
        /// </summary>
        /// <param name="stateType"></param>
        void Start(Type stateType);

        /// <summary>
        /// 是否存在有限状态机
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        bool HasState<TState>() where TState : FsmState<T>;

        /// <summary>
        /// 是否存在有限状态机状态
        /// </summary>
        /// <param name="stateType"></param>
        /// <returns></returns>
        bool HasState(Type stateType);

        /// <summary>
        /// 获取有限状态机状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        TState GetState<TState>() where TState : FsmState<T>;

        /// <summary>
        /// 获取有限状态机状态
        /// </summary>
        /// <param name="stateType"></param>
        /// <returns></returns>
        FsmState<T> GetState(Type stateType);

        /// <summary>
        /// 获取有限状态机的所有状态
        /// </summary>
        /// <returns></returns>
        FsmState<T>[] GetAllStates();

        /// <summary>
        /// 获取有限状态机的所有状态
        /// </summary>
        /// <param name="results"></param>
        void GetAllStates(List<FsmState<T>> results);

        /// <summary>
        /// 是否存在有限状态机数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool HasData(string name);

        /// <summary>
        /// 获取有限状态机数据
        /// </summary>
        /// <param name="name"></param>
        /// <typeparam name="TData"></typeparam>
        /// <returns></returns>
        TData GetData<TData>(string name) where TData : Variable;

        /// <summary>
        /// 获取有限状态机数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        Variable GetData(string name);

        /// <summary>
        /// 设置有限状态机数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        /// <typeparam name="TData"></typeparam>
        void SetData<TData>(string name, TData data) where TData : Variable;

        /// <summary>
        /// 设置有限状态机数据
        /// </summary>
        /// <param name="name"></param>
        /// <param name="data"></param>
        void SetData(string name, Variable data);

        /// <summary>
        /// 移除有限状态机数据
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        bool Remove(string name);
    }
}