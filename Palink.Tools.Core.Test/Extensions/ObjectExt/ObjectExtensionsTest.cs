using System.ComponentModel;
using Palink.Tools.Extensions.ObjectExt;

namespace Palink.Tools.Core.Test.Extensions.ObjectExt;

public class ObjectExtensionsTest
{
    [Fact]
    public void PropDescriptionDisplayNameTest()
    {
        var school = new School();

        var calss1 = new Class() { No = 1, };
        var calss2 = new Class() { No = 2, _veryGood = true };
        var calss3 = new Class() { No = 3, };
        var calss4 = new Class() { No = 4, };
        var calss5 = new Class() { No = 5, _veryGood = true };

        for (int i = 0; i < 20; i++)
        {
            var stu = new Student()
            {
                Name = $"AA{i}",
                _good = i % 3 == 0
            };

            switch (i % 5)
            {
                case 0:
                    stu.Class = calss1;
                    calss1.Students.Add(stu);
                    break;
                case 1:
                    stu.Class = calss2;
                    calss2.Students.Add(stu);
                    break;
                case 2:
                    stu.Class = calss3;
                    calss3.Students.Add(stu);
                    break;
                case 3:
                    stu.Class = calss4;
                    calss4.Students.Add(stu);
                    break;
                case 4:
                    stu.Class = calss5;
                    calss5.Students.Add(stu);
                    break;
            }
        }

        school.Classes.Add(calss1);
        school.Classes.Add(calss2);
        school.Classes.Add(calss3);
        school.Classes.Add(calss4);
        school.Classes.Add(calss5);
        var newSchool1 = school.DeepClone2();
        var newSchool2 = school.DeepClone();
        school.Classes.Clear();

        Assert.NotNull(newSchool1);
        Assert.NotNull(newSchool2);
        Assert.NotEqual(newSchool1, school);
        Assert.NotEqual(newSchool2, school);
        Assert.NotEqual(school.Classes.Count, newSchool1.Classes.Count);
        Assert.NotEqual(school.Classes.Count, newSchool2.Classes.Count);
        Assert.Equal(newSchool1.Classes.Count, newSchool2.Classes.Count);
    }

    [Fact]
    public void PropAttrTest()
    {
        var p = new Person();
        var name = p.PropertyAttr<DisplayNameAttribute>("Name")?.DisplayName;
        Assert.NotNull(name);
    }
}

public class Person
{
    [DisplayName("name")]
    public string Name { get; set; }

    [DisplayName("age")]
    public int Age { get; set; }
}

[Serializable]
public class School
{
    public List<Class> Classes { get; set; } = new();
}

[Serializable]
public class Class
{
    public bool _veryGood;
    public int No { get; set; }

    public List<Student> Students { get; set; } = new();
}

[Serializable]
public class Student
{
    public string Name { get; set; } = string.Empty;

    public bool _good;

    private int age = 20;

    public Class Class { get; set; }
}