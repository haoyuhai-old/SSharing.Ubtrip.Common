# SSharing.Ubtrip.Common
包含C#开发中常用的一些工具类，主要内容是数据访问框架。

数据库访问扩展功能使用简介：

原有功能：基于配置的Sql模板生成实际执行的Sql脚本

操作示例：

DbMgr db = new DbMgr();

//不返回结果

ExecuteNonQuery("$Employee_Update", null, param1, param2);

//返回查询结果

PageList<Employee> pageList = 
    db.ExecutePageListObject<Employee>("$Employee_Select", "OrderKey", pageSize, curPage, param1);

扩展功能：为简化一些常用的操作，增加ORM(对象数据表映射)功能, 在实体类上直接标记与表(或视图)的对应关系，
         在操作时不用配置Sql语句，程序自动生成相应的Sql语句

映射示例
 
    //标记对应的表或视图
    //可以使用工具自动生成
    [Table("dbo.employee")] 
    class Employee
    {
        //主键
        [Column("emp_id", Category.IdentityKey)]
        public int Id { get; set; }

        //普通列
        [Column("emp_code")]
        public string Code { get; set; }
                
        //版本控制列,在更新时根据该值进行版本检查
        [Column("last_update_date", Category.Version)]				
        public DateTime LastUpdateDate { get; set; }

        //...
    }

操作简介

.Insert

    Employee emp = new Employee();
    emp.Code = "1001";
    //...

    //在db中插入记录，并设置emp.Id为新产生的值
    //sql: INSERT INTO dbo.employee (emp_code, ...)VALUES('1001',...);
    DbService.Insert(emp);

.Update

    Employee emp = DbService.Get<Employee>(100);
    emp.Code = "2008";
    //...

    //根据对象的值更新对应的数据库记录的所有字段,如果存在版本控制列，也将检查该列的值是否匹配
    //sql: UPDATE dbo.employee SET emp_code='2008', ... 
    //     WHERE emp_id = 100 AND last_update_date = #emp.LastUpdateDate#
    DbService.Update(emp);

    调用Update方法时emp对象应该是从数据库中读取出来的，而不要构造一个新的对象来更新
    比如:
    Employee emp = new Employee();
    emp.Code = "1001";
    //...
    DbService.Update(emp);  //将抛出异常！

.Delete

    //删除id为100的记录
    //sql: DELETE FROM dbo.employee WHERE emp_id = 100;
    DbService.Delete<Employee>(100);

.Get

    //获取主键为100的记录
    //sql: SELECT * FROM dbo.employee WHERE emp_id = 100;

    Employee emp = DbService.Get<Employee>(100);

    //获取Code为'1001'的记录,如果结果多于一条，将抛出异常！

    //sql: SELECT * FROM dbo.employee WHERE emp_code = '1001';

    Employee emp = DbService.GetUnique<Employee>(x => x.Code, Operator.Eq, "1001");

.GetList

    //获取Code大于'1001', 名称包含'王'的所有记录
    //sql: SELECT * FROM dbo.employee WHERE emp_code > '1001' AND emp_name like '%王%';

    Filter filter = Filter.Create<Employee>();
    filter.Gt(x => x.Code, "1001");
    filter.Like(x => x.Name, "王");
    List<Employee> list = DbService.GetList<Employee>(filter);

.GetPageList
    //每页10条，当前第3页, 获取Code大于'1001'的记录,按Name排序
    //sql: SELECT * FROM dbo.employee WHERE emp_code > '1001' ORDER BY emp_name;

    Filter filter = Filter.Create<Employee>();
    filter.Gt(x => x.Code, "1001");
    PageList<Employee> pageList = DbService.GetPageList<Employee>(filter, new PageInfo(10, 3, "emp_name"));

    获取总记录数和获取当前页数据将只连接一次数据库!

.更多用法
    //在更新时忽略版本检查
    DbService.UpdateIgnoreVersion(emp);

    //是否存在Code为'1001'的记录?
    bool exist = DbService.Exist<Employee>(x => x.Code, Operator.Eq, "1001");

    //一次查询中获取两个结果集
    List<Employee> employees;
    List<Order> orders;
    DbService.GetMultiList<Employee, Order>(filter1, filter2, out employees, out orders);

.事务

    TransactionScope类的使用
    1. ts.Complete()必须是using语句块的最后一条语句
    2. using语句块内如果发生数据库相关的异常，必须向外抛出
    3. TransactionScope可以嵌套，嵌套后内层的事务和外层的事务将合并成一个事务

    using (TransactionScope ts = new TransactionScope())
    {
        DbService.Insert(emp);

        DbService.Update(order);

        Funtion1();
        //...
        
        ts.Complete();
    }

总结：

    数据访问层的扩展功能：
    优点: 减少配置的Sql语句，操作更方便, 工作量更少, 容易维护
    缺点：灵活性不够, 不支持数据库中的函数，存储过程等，而且只支持单表或视图

    应结合具体情况选择使用原有功能或扩展功能
    只应该在Dao层使用,
    在使用时应该清楚地知道最终生成的Sql语句是什么, 以免误用!

