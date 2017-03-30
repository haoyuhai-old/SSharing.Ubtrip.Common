namespace SSharing.Ubtrip.Common.Sql
{
	/// <summary>
	/// SQL语句字符串抽象接口
	/// </summary>
	public interface ISql
	{
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        string ToString(object val);
        /// <summary>
        /// 返回计算总记录数的SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string CountSql(string sql);
        /// <summary>
        /// 返回指定页数据的SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string PageSql(string sql,string idKey, int startIndex, int endIndex);
	}
}
