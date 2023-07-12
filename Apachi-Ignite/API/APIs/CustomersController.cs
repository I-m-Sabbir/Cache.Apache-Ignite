﻿using API.HelperFunctions;
using Cache.Operations;
using Cache.Operations.Cache.Entity;
using Microsoft.AspNetCore.Mvc;

namespace API.APIs;

[Route("api/[controller]/[action]")]
[ApiController]
public class CustomersController : ControllerBase
{
    private readonly ICacheServicce _cacheService;
    private readonly string _cacheName;

    public CustomersController(ICacheServicce cacheService)
    {
        _cacheService = cacheService;
        _cacheName = _cacheService.GetOrCreateCache<int, Person>(Person.GetPersonCacheConfiguration()).Name;
    }

    [HttpPost]
    public IActionResult CreateCustomer(string name, int age, string address)
    {
        string message = string.Empty;
        try
        {
            var key = _cacheService.GetCacheSize<int, Person>(_cacheName);
            key = key + 1;
            var person = new Person { Id = key, Name = name, Age = age, Address = address };
            _cacheService.Put(_cacheName, (int)key, person);

            message = "Successfully Customer Created.";
            return Ok(message);
        }
        catch (Exception ex)
        {
            message = ex.Message;

            return BadRequest(message);
        }
    }

    [HttpGet]
    public IActionResult Get()
    {
        var str = new List<Person>();
        try
        {
            var result = _cacheService.ExecuteQuery<int, Person>(_cacheName, "Select * from Person");

            if (result is not null)
                str = CommonHelper.FieldsQueryCursorToList<Person>(result);

            return Ok(str);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpGet("{id}")]
    public IActionResult Get(long id)
    {
        var str = new List<Person>();
        try
        {
            var result = _cacheService.ExecuteQuery<int, Person>(_cacheName, $"Select * from Person where id={id}");

            if (result is not null)
                str = CommonHelper.FieldsQueryCursorToList<Person>(result);

            return Ok(str);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPost]
    public IActionResult CreateCustomerWithSql(string name, int age, string address)
    {
        try
        {
            var key = _cacheService.GetCacheSize<int, Person>(_cacheName);
            key = key + 1;
            var query = $"Insert into Person(_key, Id, Name, Age, Address) Values({key}, {key}, '{name}', {age}, '{address}')";
            var result = _cacheService.ExecuteQuery<int, Person>(_cacheName, query);

            return Ok(result);
        }
        catch(Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}