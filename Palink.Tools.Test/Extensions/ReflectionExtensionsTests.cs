#nullable enable
using System.Collections.Generic;
using System.Diagnostics;
using Palink.Tools.Extensions.ReflectionExt;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class ReflectionExtensionsTests
{
    [Fact]
    public void ReflectionTest()
    {
        var myClass = typeof(MyClass<string>).GetInstance(); //创建对象
        var ret1 = myClass.InvokeMethod<string>("Test1"); //执行有返回值方法
        myClass.InvokeMethod("Test2"); //执行无返回值方法

        var name = myClass.GetField<string>("_name"); //私有字段
        myClass.SetField("_name", "tom"); //设置私有字段
        var fields = myClass.GetFields();
        foreach (var fieldInfo in fields)
        {
            var n = fieldInfo.Name;
        }
        name = myClass.GetField<string>("_name");
        var age = myClass.GetField<string>("_age"); //公开字段
        var phone = myClass.GetProperty<string>("Phone"); //公开属性
        var email = myClass.GetProperty<string>("Email"); //私有属性
        myClass.SetProperty("Email", "9999999@qq.com"); //设置私有属性
        var props = myClass.GetProperties();
        foreach (var propInfo in props)
        {
            var n = propInfo.Name;
        }
        myClass.SetField("_list", new List<string>()
        {
            "1","2","3"
        });
    }

    private class MyClass<T>
    {
        private string _name = "bob";
        public string _age = "10";

        public string Phone { get; set; } = "1928329382";
        private string Email { get; set; } = "1928329382@qq.com";

        private List<string> _list = new();

        public string Test1()
        {
            return nameof(Test1);
        }

        public void Test2()
        {
            Debug.WriteLine("Test2");
        }
    }
}