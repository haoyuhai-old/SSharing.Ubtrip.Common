namespace SSharing.Ubtrip.Common.Sql
{
	/// <summary>
	/// SQL����ַ�������ӿ�
	/// </summary>
	public interface ISql
	{
        /// <summary>
        /// ת��Ϊ�ַ���
        /// </summary>
        /// <param name="val"></param>
        /// <returns></returns>
        string ToString(object val);
        /// <summary>
        /// ���ؼ����ܼ�¼����SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string CountSql(string sql);
        /// <summary>
        /// ����ָ��ҳ���ݵ�SQL
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        string PageSql(string sql,string idKey, int startIndex, int endIndex);
	}
}
