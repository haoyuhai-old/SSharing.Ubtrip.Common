using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;

namespace SSharing.Ubtrip.Common.Sql
{
    //引用自Jeffrey Zhao
    public class DynamicPropertyAccessor
    {
        private Func<object, object> m_getter;
        private DynamicMethodExecutor m_dynamicSetter;
        
        public DynamicPropertyAccessor(Type type, string propertyName)
            : this(type.GetProperty(propertyName))
        { }

        public DynamicPropertyAccessor(PropertyInfo propertyInfo)
        {
            // target: (object)((({TargetType})instance).{Property})

            // preparing parameter, object type
            ParameterExpression instance = Expression.Parameter(
                typeof(object), "instance");

            // ({TargetType})instance
            Expression instanceCast = Expression.Convert(
                instance, propertyInfo.ReflectedType);

            // (({TargetType})instance).{Property}
            Expression propertyAccess = Expression.Property(
                instanceCast, propertyInfo);

            // (object)((({TargetType})instance).{Property})
            UnaryExpression castPropertyValue = Expression.Convert(
                propertyAccess, typeof(object));

            // Lambda expression
            Expression<Func<object, object>> lambda =
                Expression.Lambda<Func<object, object>>(
                    castPropertyValue, instance);

            this.m_getter = lambda.Compile();

            MethodInfo setMethod = propertyInfo.GetSetMethod();
            if (setMethod != null)
            {
                this.m_dynamicSetter = new DynamicMethodExecutor(setMethod);
            }
        }

        public object GetValue(object o)
        {
            return this.m_getter(o);
        }

        public void SetValue(object o, object value)
        {
            if (this.m_dynamicSetter == null)
            {
                throw new NotSupportedException("Cannot set the property.");
            }

            this.m_dynamicSetter.Execute(o, new object[] { value });
        }
    }    

    //实现强类型的属性反射
    public class MyReflection
    {
        public static string GetColName(Expression property)
        {
            PropertyInfo pi = GetPropertyInfo(property);

            ColumnAttribute[] columnAttributes = (ColumnAttribute[])pi.GetCustomAttributes(typeof(ColumnAttribute), false);
            if (columnAttributes.Length > 0)
            {
                return string.IsNullOrEmpty(columnAttributes[0].ColumnName) ? pi.Name : columnAttributes[0].ColumnName;
            }
            else
            {
                throw new ArgumentException("指定的属性没有Column特性标记", "property");
            }
        }

        private static PropertyInfo GetPropertyInfo(Expression property)
        {   
            Expression exp;
            LambdaExpression lambda = property as LambdaExpression;
            if (lambda != null)
            {
                exp = lambda.Body;
            }
            else
            {
                exp = property;
            }

            if (exp.NodeType == ExpressionType.MemberAccess)
            {
                PropertyInfo pi = (exp as MemberExpression).Member as PropertyInfo;
                if (pi != null)
                    return pi;
            }
            else if (exp.NodeType == ExpressionType.Convert) 
            {
                //对于枚举类型，类型为转换运算
                UnaryExpression unaryExpression = exp as UnaryExpression;
                if(unaryExpression != null && unaryExpression.Operand.NodeType == ExpressionType.MemberAccess)                
                {
                    PropertyInfo pi = (unaryExpression.Operand as MemberExpression).Member as PropertyInfo;
                    if (pi != null)
                        return pi;
                }
            }

            throw new ArgumentException("请指定有效的属性访问表达式", "property");
        }
    }
}
