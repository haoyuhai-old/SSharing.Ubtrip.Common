using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace SSharing.Ubtrip.Common.Sql
{
    class LinqParser<T>
    {
        public static Filter<T> GetSql(Expression exp)
        {
            Filter<T> result = Filter.Create<T>();
            GetSql(exp, result);
            return result;
        }

        private static void GetSql(Expression exp, Filter<T> filter)
        {
            if (exp is LambdaExpression)//LAMBDA表达式
            {
                var le = exp as LambdaExpression;
                GetSql(le.Body, filter);
            }
            else if (exp is BinaryExpression)
            {
                var be = exp as BinaryExpression;
                if (be.NodeType == ExpressionType.AndAlso) //只支持and 条件
                {
                    GetSql(be.Left, filter);
                    GetSql(be.Right, filter);
                }
                else
                {
                    GetSqlForCompare(be, filter);
                }
            }
            else
            {
                throw new Exception("不支持的表达式类型:" + exp.NodeType.ToString());
            }
        }

        private static void GetSqlForCompare(BinaryExpression exp, Filter<T> filter)
        {
            string columnName = GetColumnName(exp.Left);
            object propValue = GetExpValue(exp.Right);
            Operator optr = GetOperator(exp.NodeType);

            filter.FilterItems.Add(new FilterItem(columnName, optr, propValue));
        }

        private static string GetColumnName(Expression left)
        {
            return MyReflection.GetColName(left);
        }

        private static Operator GetOperator(ExpressionType nodeType)
        {
            switch (nodeType)
            {
                case ExpressionType.LessThan:
                    return Operator.Lt;
                case ExpressionType.LessThanOrEqual:
                    return Operator.Le;
                case ExpressionType.GreaterThan:
                    return Operator.Gt;
                case ExpressionType.GreaterThanOrEqual:
                    return Operator.Ge;
                case ExpressionType.Equal:
                    return Operator.Eq;
                case ExpressionType.NotEqual:
                    return Operator.NotEq;
                default:
                    throw new NotSupportedException(nodeType.ToString());
            }
        }

        private static object GetExpValue(Expression exp)
        {
            if (exp is ConstantExpression)
            {
                var ce = exp as ConstantExpression;
                return ce.Value;
            }
            else if (exp is MethodCallExpression)//方法
            {
                var me = exp as MethodCallExpression;
                return me.Method.Invoke(me.Object, me.Arguments.Cast<object>().ToArray());
            }
            else if (exp is UnaryExpression)
            {
                //对于枚举类型，类型为转换运算
                UnaryExpression unaryExpression = exp as UnaryExpression;
                return GetExpValue(unaryExpression.Operand);
            }
            else if (exp is MemberExpression)
            {
                #region 解析成员的值
                var mexp = exp as MemberExpression;
                object o = null;
                if (mexp.Expression is ParameterExpression)
                {
                    throw new Exception("表达式的右侧不能是参数表达");
                }
                else if (mexp.Expression is MethodCallExpression)  //???不太理解
                {
                    var me2 = mexp.Expression as MethodCallExpression;
                    o = me2.Method.Invoke(me2.Object, me2.Arguments.Cast<object>().ToArray());
                }
                else if (mexp.Expression is ConstantExpression)
                {
                    var ce2 = mexp.Expression as ConstantExpression;
                    o = ce2.Value;
                }
                else
                {
                    o = GetExpValue(mexp.Expression);   //递归求对象的值
                }
                
                if (mexp.Member is System.Reflection.FieldInfo)
                {                    
                    var fd = mexp.Member as System.Reflection.FieldInfo;
                    return fd.GetValue(o);
                }
                else if (mexp.Member is System.Reflection.PropertyInfo)
                {
                    var pf = mexp.Member as System.Reflection.PropertyInfo;
                    return pf.GetValue(o, null);
                }
                else
                {
                    throw new Exception("不支持的成员访问类型:" + mexp.Member.Name);
                }
                #endregion
            }
            else
            {
                throw new Exception("不支持的表达式类型:" + exp.NodeType);
            }
        }
    }
}

//static string GetSql(Expression exp)
//       {
//           while (true)
//           {
//               if (exp is LambdaExpression)//LAMBDA表达式
//               {
//                   var le = exp as LambdaExpression;
//                   return GetSql(le.Body);
//               }
//               else if (exp is NewExpression)//NEW表达式,一般用于SELECT语句中选择字段
//               {
//                   var ne = exp as NewExpression;
//                   StringBuilder sb = new StringBuilder();
//                   foreach (var s in ne.Arguments)
//                   {
//                       sb.Append(s + ",");
//                   }
//                   sb.Length--;
//                   return sb.ToString();
//               }
//               else if (exp is BinaryExpression)
//               {
//                   StringBuilder sb = new StringBuilder();
//                   var be=exp as BinaryExpression;
//                   sb.Append(GetSql(be.Left));
//                   sb.Append(GetExp(be.NodeType));
//                   sb.Append(GetSql(be.Right));
//                   return sb.ToString();
//               }
//               else if(exp is ConstantExpression)//变量
//               {
//                   var ce=exp as ConstantExpression;
                    
//                   return "'"+ce.Value.ToString()+"'";
//               }
//               else if (exp is MethodCallExpression)//方法
//               {
//                   var me = exp as MethodCallExpression;
//                   return "'"+me.Method.Invoke(me.Object, me.Arguments.Cast<object>().ToArray()).ToString() + "'";
//               }
//               else if (exp is ParameterExpression)
//               {
//                   var pe = exp as ParameterExpression;
//                   return pe.Name;
//               }
//               else if (exp is MemberExpression)//成员解析比较麻烦
//               {
//                   var ex = exp as MemberExpression;
//                   object o = null;
//                   if (ex.Expression is ParameterExpression && ex.Member is System.Reflection.PropertyInfo)
//                   {
//                       return exp.ToString();
//                   }
//                   if (ex.Expression is ConstantExpression)
//                   {
//                       var ce = ex.Expression as ConstantExpression;
//                       o = ce.Value;
//                   }
//                   if (ex.Expression is MethodCallExpression)
//                   {
//                       var me = ex.Expression as MethodCallExpression;
//                       return "'" + me.Method.Invoke(me.Object, me.Arguments.Cast<object>().ToArray()).ToString() + "'";
//                   }
//                   if (ex.Member is System.Reflection.FieldInfo)
//                   {
//                       var fd = ex.Member as System.Reflection.FieldInfo;
//                       return "'" + fd.GetValue(o).ToString() + "'";
//                   }
//                   if (ex.Member is System.Reflection.PropertyInfo)
//                   {
//                       var pf = ex.Member as System.Reflection.PropertyInfo;
//                       return "'" + pf.GetValue(o, null).ToString() + "'";
//                   }
//                   return string.Empty;
//               }
//               else if (exp is UnaryExpression)
//               {
//                   var ue=exp as UnaryExpression;
//                   return GetSql(ue.Operand);
//               }
//               else
//               {
//                   return string.Empty;
//               }
//           }
//       }
//       static string GetExp(ExpressionType e)
//       {
//           switch (e)
//           {
//               case ExpressionType.And: { return " AND "; }
//               case ExpressionType.AndAlso: { return " AND "; }
//               case ExpressionType.Or: { return " OR "; }
//               case ExpressionType.OrElse: { return " OR "; }
//               case ExpressionType.LessThan: { return "<"; }
//               case ExpressionType.GreaterThan: { return ">"; }
//               case ExpressionType.Equal: { return "="; }
//               case ExpressionType.GreaterThanOrEqual: { return ">="; }
//               case ExpressionType.LessThanOrEqual: { return "=<"; }
//               case ExpressionType.Not: { return "<>"; }
//           }
//           return string.Empty;
//       }

//void Where(Expression<Func<T, bool>> ex) 
//        {
//            Pn = ex.Parameters[0].Name;
//            if (string.IsNullOrEmpty(WhereStr))
//            {
//                WhereStr = ex == null ? "" : GetSql(ex).Replace(Pn+".","");
//            }
//            else
//            {
//                WhereStr += " AND " + (ex == null ? "" : GetSql(ex).Replace(Pn + ".", ""));
//            }
//        }