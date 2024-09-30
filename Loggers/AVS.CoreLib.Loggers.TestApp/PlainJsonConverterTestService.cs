using System;
using System.Collections.Generic;
using AVS.CoreLib.BootstrapTools;
using AVS.CoreLib.Json;
using AVS.CoreLib.REST.Responses;
using Microsoft.Extensions.Logging;

namespace AVS.CoreLib.Loggers.TestApp;

public class PlainJsonConverterTestService : TestService
{
    private readonly ILogger _logger;

    public PlainJsonConverterTestService(ILogger<PlainJsonConverterTestService> logger)
    {
        _logger = logger;
    }

    public override void Test(string[] args)
    {
        try
        {
            object? obj = null;
            var json = obj!.ToPlainJson();

            TestSerializeTypedList();
            TestSerializeTypedDictionary();
            //var obj = new
            //{

            //    Prop1 = new[] { 1, 2, 3, 4, 5 },
            //    Prop2 = new { PropA = new string[] { "a", "B", "C", "d" }, PropB = "str1" }
            //};
                
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Tests Failed");
        }
    }

    private void TestSerializeTypedList()
    {
        try
        {
            using (var scope = _logger.BeginScope("TypedList tests (TValue = primitive)"))
            {
                var list = new List<string> { "item1","item2","item3" };
                var plainJson = list.ToBriefJson();
                _logger.LogInformation("{@@source} => {result}", list, plainJson);

                var list2 = new List<decimal> { 1.002m ,2.3m, 3.03m };
                plainJson = list2.ToBriefJson();
                _logger.LogInformation("{@@source} => {result}", list2, plainJson);

                var list3 = new List<object> { null, 1.002m, "abc" };
                plainJson = list3.ToBriefJson();
                _logger.LogInformation("{@@source} => {result}", list3, plainJson);

                var arr = new [] { DateTime.Today, DateTime.Now.AddDays(-1), DateTime.Now.AddDays(-2) };
                var json = arr.ToJson();
                plainJson = arr.ToBriefJson();
                _logger.LogInformation("{@@source} => {result}", arr, plainJson);

                var response = new Response<string[]>("Binance", new[]
                {
                    "1) ``&&`` \"item1\" ",
                    "6) @\r\n@ 12333123123123123123123123 213123123123123 ================================ ---------------------------------",
                    "7) \"\r\n\" 12333123123123123123123123 213123123123123 ================================ ---------------------------------",
                });

                json = response.ToJson();
                plainJson = response.ToBriefJson();
                _logger.LogInformation("{@source} => {result}", response, plainJson);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TestSerializeTypedList Failed");
        }
    }

    private void TestSerializeTypedDictionary()
    {
        try
        {
            using (var scope = _logger.BeginScope("TypedDictionary tests (TValue = primitive)"))
            {
                var dict = new Dictionary<string, string> { { "key1", "val1" }, { "key2", "val2" }, { "key3", "val3" } };
                var plainJson = dict.ToPlainJson();
                _logger.LogInformation("{@source} => {result}", dict, plainJson);

                var dict2 = new Dictionary<string, int> { { "key1", 15 }, { "key2", 20 }, { "key3", 50 } };
                plainJson = dict2.ToPlainJson();
                _logger.LogInformation("{@source} => {result}", dict2, plainJson);

                var dict3 = new Dictionary<string, decimal> { { "key1", 15.1273m }, { "key2", 0.22m }, { "key3", 50.333m } };
                plainJson = dict3.ToPlainJson();
                _logger.LogInformation("{@source} => {result}", dict3, plainJson);
            }

            using (var scope = _logger.BeginScope("TypedDictionary tests (TValue = object)"))
            {
                var dict = new Dictionary<string, object> { { "key1", new { Prop1 = "val1" } }, { "key2", new { Prop2 = "val2" } }, { "key3", new { Prop3 = 33.445m } } };
                var plainJson = dict.ToPlainJson();
                _logger.LogInformation("{@source} => {result}", dict, plainJson);

                var dict2 = new Dictionary<string, object> { { "key1", new[] { "item1", "item2" } }, { "key2", new[] { 1, 33 } }, { "key3", null } };
                plainJson = dict2.ToPlainJson();
                _logger.LogInformation("{@source} => {result}", dict2, plainJson);

                var dict3 = new Dictionary<string, object> { { "key1", null},
                    { "key2", new[] { "str2", "..................................................................." } }, 
                    { "key3", null } };

                plainJson = dict3.ToBriefJson();
                _logger.LogInformation("{@source} => {result}", dict3, plainJson);

                var dict4 = new Dictionary<string, object> {
                    { "``Key1", "&&& ``` value''```&&"}, // new object[] { "nested item", new Dictionary<string, string>() {{"nested key", "val"}}, 2m}
                    { "&Key2", new []
                    {
                        new [] {"nested item1.1", "nested item1.2" },
                        new [] {"nested item2.1", "nested item2.2" }
                    }},
                    { "Key3", new[] { 
                        " 1. =========================================================== ",
                        " 2. ----------------------------------------------------------- ",
                        " 3. ########################################################### ",
                        " 4. =========================================================== ",
                        " 5. ........................................................... " } },
                    { "key4", new []{ "item1", "item2", "item3","item4", "item5", "item6", "item7","item8" }},
                    { "key5", new []{ "item1", "item2", "item3","item4", "item5", "item6", "item7","item8" }},
                    { "key6", new []{ "item1", "item2", "item3","item4", "item5", "item6", "item7","item8" }},
                    { "key7", new []{ "item1", "item2", "item3","item4", "item5", "item6", "item7","item8" }},
                    { "key8", new []{ "item1", "item2", "item3","item4", "item5", "item6", "item7","item8" }},
                    { "key9", new []{ "item1", "item2", "item3","item4", "item5", "item6", "item7","item8" }},
                    { "key10", new []{ "item1", "item2", "item3","item4", "item5", "item6", "item7","item8" }},
                    { "key11", new []{ "item1", "item2", "item3","item4", "item5", "item6", "item7","item8" }},
                };

                var arr = new string[] { "``quotes", "& ampersand" };
                _logger.LogInformation("test special symbols <Green>` &</Green> <Red> red text {@arg1} {@arg2};</Red>", 1, arr);

                plainJson = dict4.ToBriefJson();
                _logger.LogInformation("{@source} =>\r\nRESULT:\r\n {result}", dict4, plainJson);

            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "TestSerializeTypedDictionary Failed");
        }
    }
}