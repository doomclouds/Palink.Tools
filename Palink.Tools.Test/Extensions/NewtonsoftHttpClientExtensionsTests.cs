using System;
using System.Net.Http;
using Newtonsoft.Json;
using Palink.Tools.Extensions.PLNet;
using Xunit;

namespace Palink.Tools.Test.Extensions;

public class NewtonsoftHttpClientExtensionsTests
{
    [Fact]
    public async void GetTest()
    {
        var http = new HttpClient();
        http.BaseAddress = new Uri("http://127.0.0.1");
        var person =
            await http.GetFromJsonAsync<Person>("/person?id=1",
                new JsonSerializerSettings());
    }

    [Fact]
    public async void PostTest()
    {
        var person = new Person()
        {
            Name = "name",
            Age = 20
        };
        var http = new HttpClient();
        http.BaseAddress = new Uri("http://127.0.0.1");
        var response =
            await http.PostAsJsonAsync("/person?id=1", person,
                new JsonSerializerSettings());
        var resp =
            await response.Content.ReadFromJsonAsync<ResponseResult>(
                new JsonSerializerSettings());
    }

    [Fact]
    public async void PutTest()
    {
        var person = new Person()
        {
            Name = "name",
            Age = 20
        };
        var http = new HttpClient();
        http.BaseAddress = new Uri("http://127.0.0.1");
        var response =
            await http.PutAsJsonAsync("/person?id=1", person,
                new JsonSerializerSettings());

        var resp =
            await response.Content.ReadFromJsonAsync<ResponseResult>(
                new JsonSerializerSettings());
    }

    public class Person
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Age { get; set; }
    }

    public class ResponseResult
    {
    }
}