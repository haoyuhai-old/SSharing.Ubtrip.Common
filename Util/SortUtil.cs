using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

namespace SSharing.Ubtrip.Common.Util
{
    /// <summary>
    /// 对象比较时使用的信息类
    /// </summary>
    public struct SortInfo
    {
        /// <summary>
        /// 比较的方向，如下：
        /// ASC：升序
        /// DESC：降序
        /// </summary>
        public enum Direction
        {
            ASC = 0,
            DESC,
        };

        public enum Target
        {
            CUSTOMER = 0,
            FORM,
            FIELD,
            SERVER,
        };

        public string name;
        public Direction direction;
        public Target target;
    }
    /// <summary>
    /// 自定义排序类：继承IComparer<T>接口，实现同一自定义类型　对象比较
    /// </summary>
    public class SortUtil<T> : IComparer<T>
    {
        private Type type = null;
        private SortInfo info;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="type">进行比较的类类型</param>
        /// <param name="name">进行比较对象的属性名称</param>
        /// <param name="direction">比较方向(升序/降序)</param>
        public SortUtil(Type type, string name, SortInfo.Direction direction)
        {
            this.type = type;
            this.info.name = name;
            if (direction != SortInfo.Direction.ASC)
                this.info.direction = direction;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="className">进行比较的类名称</param>
        /// <param name="name">进行比较对象的属性名称</param>
        /// <param name="direction">比较方向(升序/降序)</param>
        public SortUtil(string className, string name, SortInfo.Direction direction)
        {
            try
            {
                this.type = Type.GetType(className, true);
                this.info.name = name;
                this.info.direction = direction;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="t">进行比较的类型的实例</param>
        /// <param name="name">进行比较对象的属性名称</param>
        /// <param name="direction">比较方向(升序/降序)</param>
        public SortUtil(T t, string name, SortInfo.Direction direction)
        {
            this.type = t.GetType();
            this.info.name = name;
            this.info.direction = direction;
        }

        /// <summary>
        /// 必须！实现IComparer<T>的比较方法。
        /// </summary>
        /// <param name="t1"></param>
        /// <param name="t2"></param>
        /// <returns></returns>
        int IComparer<T>.Compare(T t1, T t2)
        {
            object x = this.type.InvokeMember(this.info.name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, t1, null);
            object y = this.type.InvokeMember(this.info.name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty, null, t2, null);
            if (this.info.direction != SortInfo.Direction.ASC)
                Swap(ref x, ref y);
            return (new CaseInsensitiveComparer()).Compare(x, y);
        }

        /// <summary>
        /// 交换操作数
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void Swap(ref object x, ref object y)
        {
            object temp = null;
            temp = x;
            x = y;
            y = temp;
        }

    }// 类结束
}// 命名空间结束
