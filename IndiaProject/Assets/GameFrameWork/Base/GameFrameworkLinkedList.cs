using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace GameFramework
{
    /// <summary>
    /// 游戏框架链表类
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class GameFrameworkLinkedList<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable
    {
        private readonly LinkedList<T> m_LinkedList;
        private readonly Queue<LinkedListNode<T>> m_CacheNodes;

        /// <summary>
        /// 初始化游戏框架类的新实例
        /// </summary>
        public GameFrameworkLinkedList()
        {
            m_LinkedList = new();
            m_CacheNodes = new();
        }

        /// <summary>
        /// 获取链表中实际包含的节点数量
        /// </summary>
        /// <value></value>
        public int Count
        {
            get
            {
                return m_LinkedList.Count;
            }
        }

        /// <summary>
        /// 获取链表节点缓存数量
        /// </summary>
        /// <value></value>
        public int CachedNodeCount
        {
            get
            {
                return m_CacheNodes.Count;
            }
        }

        /// <summary>
        /// 获取链表的第一个结点
        /// </summary>
        /// <value></value>
        public LinkedListNode<T> First
        {
            get
            {
                return m_LinkedList.First;
            }
        }

        /// <summary>
        /// 获取链表的最后一个节点
        /// </summary>
        /// <value></value>
        public LinkedListNode<T> Last
        {
            get
            {
                return m_LinkedList.Last;
            }
        }
        /// <summary>
        /// 获取一个值，该值表示ICollection是否为只读
        /// </summary>
        /// <value></value>
        public bool IsReadOnly
        {
            get
            {
                return ((ICollection<T>)m_LinkedList).IsReadOnly;
            }
        }

        /// <summary>
        /// 获取可用于同步对ICollection的访问的对象
        /// </summary>
        /// <value></value>
        public object SyncRoot
        {
            get
            {
                return ((ICollection)m_LinkedList).SyncRoot;
            }
        }

        /// <summary>
        /// 获取一个值，该值指示是否同步ICollection的访问(线程安全)
        /// </summary>
        /// <value></value>
        public bool IsSynchronized
        {
            get
            {
                return ((ICollection)m_LinkedList).IsSynchronized;
            }
        }

        /// <summary>
        /// 在链表中指定的现有节点后添加包含指定的新结点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = AcquireNode(value);
            m_LinkedList.AddAfter(node, newNode);
            return newNode;
        }
        /// <summary>
        /// 在链表中指定的现有结点后添加指定的新结点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="newNode"></param>
        public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            m_LinkedList.AddAfter(node, newNode);
        }

        /// <summary>
        /// 在链表中指定的现有结点前添加包含指定值的新结点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
        {
            LinkedListNode<T> newNode = AcquireNode(value);
            m_LinkedList.AddBefore(node, newNode);
            return newNode;
        }

        /// <summary>
        /// 在链表中指定的现有结点前添加指定的新结点
        /// </summary>
        /// <param name="node"></param>
        /// <param name="newNode"></param>
        public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
        {
            m_LinkedList.AddBefore(node, newNode);
        }

        /// <summary>
        /// 在链表的开头添加包含指定值的新结点
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public LinkedListNode<T> AddFirst(T value)
        {
            var node = AcquireNode(value);
            m_LinkedList.AddFirst(node);
            return node;
        }

        /// <summary>
        /// 在链表开头处添加指定的新结点
        /// </summary>
        /// <param name="node"></param>
        public void AddFirst(LinkedListNode<T> node)
        {
            m_LinkedList.AddFirst(node);
        }

        /// <summary>
        /// 在链表的结尾处添加包含指定值的新结点。
        /// </summary>
        /// <param name="value">指定值。</param>
        /// <returns>包含指定值的新结点。</returns>
        public LinkedListNode<T> AddLast(T value)
        {
            LinkedListNode<T> node = AcquireNode(value);
            m_LinkedList.AddLast(node);
            return node;
        }

        /// <summary>
        /// 在链表的结尾处添加指定的新结点。
        /// </summary>
        /// <param name="node">指定的新结点。</param>
        public void AddLast(LinkedListNode<T> node)
        {
            m_LinkedList.AddLast(node);
        }

        /// <summary>
        /// 从链表中移除所有结点
        /// </summary>
        public void Clear()
        {
            LinkedListNode<T> current = m_LinkedList.First;
            while (current != null)
            {
                ReleaseNode(current);
                current = current.Next;
            }
            m_LinkedList.Clear();
        }

        /// <summary>
        /// 清除链表结点缓存
        /// </summary>
        public void ClearCachedNodes()
        {
            m_CacheNodes.Clear();
        }

        /// <summary>
        /// 确定某值是否在链表中
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public bool Contains(T value)
        {
            return m_LinkedList.Contains(value);
        }

        /// <summary>
        /// 从目标数组的指定索引处开始将整个链表复制到兼容的一维数组
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(T[] array, int index)
        {
            m_LinkedList.CopyTo(array, index);
        }

        /// <summary>
        /// 从特定的ICollection索引开始，将数组的元素复制到一个数值中
        /// </summary>
        /// <param name="array"></param>
        /// <param name="index"></param>
        public void CopyTo(Array array, int index)
        {
            ((ICollection)m_LinkedList).CopyTo(array, index);
        }

        /// <summary>
        /// 查找包含指定值的第一个结点
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public LinkedListNode<T> Find(T value)
        {
            return m_LinkedList.Find(value);
        }

        /// <summary>
        /// 查找包含指定值的最后一个结点
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public LinkedListNode<T> FindLast(T value)
        {
            return m_LinkedList.FindLast(value);
        }
        /// <summary>
        /// 从链表中移除指定值的第一个匹配项
        /// </summary>
        /// <param name="value"></param>
        public bool Remove(T value)
        {
            LinkedListNode<T> node = m_LinkedList.Find(value);
            if (node != null)
            {
                m_LinkedList.Remove(node);
                ReleaseNode(node);
                return true;
            }
            return false;
        }

        /// <summary>
        /// 从链表中移除指定的结点
        /// </summary>
        /// <param name="node"></param>
        public void Remove(LinkedListNode<T> node)
        {
            m_LinkedList.Remove(node);
            ReleaseNode(node);
        }

        /// <summary>
        /// 移除位于链表开头处的结点。
        /// </summary>
        public void RemoveFirst()
        {
            LinkedListNode<T> first = m_LinkedList.First;
            if (first == null)
            {
                //TODO: throw new GameFrameworkException("First is invalid.");
            }

            m_LinkedList.RemoveFirst();
            ReleaseNode(first);
        }

        /// <summary>
        /// 移除位于链表结尾处的结点。
        /// </summary>
        public void RemoveLast()
        {
            LinkedListNode<T> last = m_LinkedList.Last;
            if (last == null)
            {
                //TODO: throw new GameFrameworkException("Last is invalid.");
            }

            m_LinkedList.RemoveLast();
            ReleaseNode(last);
        }

        /// <summary>
        /// 返回循环访问集合的枚举器
        /// </summary>
        /// <returns></returns>
        public Enumerator GetEnumerator()
        {
            return new Enumerator(m_LinkedList);
        }


        private LinkedListNode<T> AcquireNode(T value)
        {
            LinkedListNode<T> node = null;
            if (m_CacheNodes.Count > 0)
            {
                node = m_CacheNodes.Dequeue();
                node.Value = value;
            }
            else
            {
                node = new LinkedListNode<T>(value);
            }

            return node;
        }
        private void ReleaseNode(LinkedListNode<T> node)
        {
            node.Value = default(T);
            m_CacheNodes.Enqueue(node);
        }

        /// <summary>
        /// 将值添加到ICollection的结尾处
        /// </summary>
        /// <param name="value"></param>
        void ICollection<T>.Add(T value)
        {
            AddLast(value);
        }
        /// <summary>
        /// 返回循环访问集合的枚举数
        /// </summary>
        /// <returns></returns>        
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 循环访问集合的枚举数
        /// </summary>
        [StructLayout(LayoutKind.Auto)]
        public struct Enumerator : IEnumerator<T>, IEnumerator
        {
            private LinkedList<T>.Enumerator m_Enumerator;
            internal Enumerator(LinkedList<T> linkedList)
            {
                if (linkedList == null)
                {
                    //TODO: throw new GameFrameworkException("Linked list is invalid.");
                }

                m_Enumerator = linkedList.GetEnumerator();
            }

            public T Current => m_Enumerator.Current;

            object IEnumerator.Current => m_Enumerator.Current;

            public void Dispose()
            {
                m_Enumerator.Dispose();
            }

            public bool MoveNext()
            {
                return m_Enumerator.MoveNext();
            }

            public void Reset()
            {
                ((IEnumerator<T>)m_Enumerator).Reset();
            }
        }
    }
}