using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace GameFramework
{
    [StructLayout(LayoutKind.Auto)]
    public struct GameFrameworkLinkedListRange<T> : IEnumerable<T>, IEnumerable
    {

        private readonly LinkedListNode<T> m_First;
        private readonly LinkedListNode<T> m_Terminal;

        /// <summary>
        /// 初始化游戏框架链表范围的新实例
        /// </summary>
        /// <param name="first"></param>
        /// <param name="terminal"></param>
        public GameFrameworkLinkedListRange(LinkedListNode<T> first, LinkedListNode<T> terminal)
        {
            if (first == null || terminal == null || first == terminal)
            {
                throw new GameFrameworkException("Range is invalid.");
            }
            m_First = first;
            m_Terminal = terminal;
        }

        /// <summary>
        /// 获取链表范围是否有效
        /// </summary>
        /// <value></value>
        public bool IsValid
        {
            get
            {
                return m_First != null && m_Terminal != null && m_First != m_Terminal;
            }
        }

        /// <summary>
        /// 获取链表范围的开始结点
        /// </summary>
        /// <value></value>
        public LinkedListNode<T> First
        {
            get
            {
                return m_First;
            }
        }
        /// <summary>
        /// 获取链表范围的终结标记的结点
        /// </summary>
        /// <value></value>
        public LinkedListNode<T> Terminal
        {
            get
            {
                return m_Terminal;
            }
        }

        /// <summary>
        /// 获取链表范围的结点数量
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                if (!IsValid)
                {
                    return 0;
                }

                int count = 0;
                for (var current = m_First; current != null && current != m_Terminal; current = current.Next)
                {
                    count++;
                }
                return count;
            }
        }

        /// <summary>
        /// 检查是否包含指定值
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value)
        {
            for (var current = m_First; current != null && current != m_Terminal; current = current.Next)
            {
                if (current.Value.Equals(value))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns></returns>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(this);
        }
        /// <summary>
        /// 返回循环访问集合的枚举数。
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        /// <summary>
        /// 循环访问集合的枚举值
        /// </summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private readonly GameFrameworkLinkedListRange<T> m_GameFrameworkLinkedListRange;
            private LinkedListNode<T> m_Current;
            private T m_CurrentValue;
            public T Current => m_CurrentValue;

            object IEnumerator.Current => m_CurrentValue;

            internal Enumerator(GameFrameworkLinkedListRange<T> range)
            {
                if (!range.IsValid)
                {
                    throw new GameFrameworkException("Range is invalid.");
                }

                m_GameFrameworkLinkedListRange = range;
                m_Current = range.m_First;
                m_CurrentValue = default;
            }
            /// <summary>
            /// 清理枚举值
            /// </summary>
            public void Dispose()
            {

            }

            /// <summary>
            /// 获取下一个节点
            /// </summary>
            /// <returns></returns>
            public bool MoveNext()
            {
                if (m_Current == null || m_Current == m_GameFrameworkLinkedListRange.m_Terminal)
                {
                    return false;
                }
                m_CurrentValue = m_Current.Value;
                m_Current = m_Current.Next;
                return true;
            }

            /// <summary>
            /// 充值枚举值
            /// </summary>
            public void Reset()
            {
                m_Current = m_GameFrameworkLinkedListRange.m_First;
                m_CurrentValue = default;
            }
        }
    }
}