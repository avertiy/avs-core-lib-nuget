using AVS.CoreLib.BootstrapTools;
using AVS.CoreLib.PowerConsole.ConsoleTable;
using AVS.CoreLib.PowerConsole.Utilities;
using System.Collections.Generic;
using System;
using AVS.CoreLib.PowerConsole.Enums;

namespace AVS.CoreLib.PowerConsole.DemoApp.Services
{
    internal class PrintTableTestService : TestService
    {
        public override void Test(string[] args)
        {
            try
            {
                PowerConsole.SwitchColorMode(ColorMode.AnsiCodes);
                PrintArrayAsHorizontalTable();
                PrintObjectAsVerticalTable();
                PrintObjectAsHorizontalTable();
                
                PrintTableTests();
            }
            catch (Exception ex)
            {
                PowerConsole.PrintError(ex, $"{nameof(PrintTableTestService)}:{nameof(Test)} failed");
            }

        }

        private void PrintArrayAsHorizontalTable()
        {
            PowerConsole.PrintHeader($"Print array as table (horizontal orientation)");
            var arr = CreateTestArray();
            PowerConsole.PrintTable(arr);
        }

        
        private void PrintObjectAsHorizontalTable()
        {
            PowerConsole.PrintHeader($"Print object as table (horizontal orientation)");
            var obj = GetTestObject();
            PowerConsole.PrintTable(obj, TableOrientation.Horizontal);
        }

        private void PrintObjectAsVerticalTable()
        {
            PowerConsole.PrintHeader($"Print object as table (horizontal orientation)");
            var obj = GetTestObject();
            PowerConsole.PrintTable(obj, TableOrientation.Vertical);
        }

        bool PrintTableTests()
        {
            PowerConsole.PrintHeader($"PrintTable tests");

            


            var tbl = Table.Create(new[] { "Col1", "Col2" },
                new ColorScheme[] { ColorScheme.Success, ColorScheme.Info });
            tbl.AddRow(new object[] { "1203", 1234567890 });
            tbl.AddRow(new object[] { 9876543210, 12.1234567890123m });
            tbl.CalculateWidth(true);
            PowerConsole.PrintTable(tbl);


            var arr = new[] {
                new
                {
                    Price = 0.051132156m,
                    Column2 = DateTime.Now,
                    Amount = 10.50m,
                    Column3="asd ads d qwd qw",
                    Column4 = new { Prop1 = 1, Prop2 = 100}
                },
                new
                {
                    Price = 0.003m,
                    Column2 = DateTime.Now,
                    Amount = 120.09879877m,
                    Column3 = "asd ads d qwd qw 213 21321 321 3213",
                    Column4 = new { Prop1 = 2, Prop2 = 200}
                } };

            PowerConsole.PrintTable(arr, colors: ConsoleColor.DarkCyan);
            PowerConsole.WriteLine();

            var table = Table.Create(new[] { "Buys", "Sells", "Column 3", "Column 4" }, new[] { ColorScheme.Info, ColorScheme.Success });
            table.AddRow().AddCell("row 1 colspan = 4", 4);
            table.AddRow().AddCell("row 2 colspan = 3", 3).AddCell("Cell 4");
            table.AddRow().AddCell("row 3 cell 1").AddCell("Cell 2       -        3", 2).AddCell("cell 4");
            table.AddRow().AddCell("row 4 cell 1").AddCell("Cell 2    -    3", 2);
            table.AddRow(new object[] { "buys trade2", "sell trade2", "adasdasd", "sadasd adqw 23 2323" });
            table.AddRow(new object[] { "cell 3 asdas dasdasd das d", "", "adasdasd", "sadasd adqw 23 2323" });
            table.AddRow(new object[] { "", "cell 4 sdfdsf eewewer", "adasdasd", "sadasd adqw 23 2323" });

            PowerConsole.PrintTable(table);
            return true;
        }

        private object GetTestObject()
        {
            var objArr = new object[] { "1", 2, 3.0m, ConsoleColor.Cyan, 6 };
            var obj = new
            {
                Prop1 = "asdas dasdasd\r\n asd  askjdhasdh asdh wq qeqe\r\n asdas\r\ndajdhasda qwe qwe qwe \r\n",
                Prop2 = "$17.02",
                Prop74 = "asdas dasdasd\r\n asd  askjdhasdh asdh wq qeqe\r\n asdas\r\ndajdhasda qwe qwe qwe \r\n",
                Prop3 = "$17.02",
                Prop4 = "$17.02",
                Prop5 = "$1701.00",
                Prop8 = DateTime.UtcNow,
                Prop22 = 151454.00,
                Prop33 = 9988.00m,
                Prop44 = "asdas dasdasd\r\n asd",
                Prop9Time = DateTime.UtcNow.AddDays(3),
                Prop10Time = DateTime.UtcNow.AddDays(31),
                EntryPrice = 17.255m,
                DateProp = DateTime.Now,
                PropArr = objArr,
                PropArr1 = new string[] { "apple", "strawberry" },
                PropArr2 = new string[] { },
                PropListEmpty = new List<int>(),
                PropList = new List<int>() { 1, 100, 999, 777_000, 555_555_000 }
            };
            return obj;
        }

        private NewOrderFuturesRequest[] CreateTestArray()
        {
            var arr = new List<NewOrderFuturesRequest>
            {
                new() { Symbol = "BTC_USDT", Price = 1.887123m, Qty = 0.01m, ClientOrderId = "client_order_Id:123213:0"},
                new() { Symbol = "BTC_USDT", Price = 0.892123m, Qty = 0.14m },
                new() {Symbol = "BTC_USDT", Price = 27200, Qty = 0.01m},
                new() {Symbol = "BTC_USDT", Price = 27200, Qty = 0.12m}
            };
            return arr.ToArray();
        }

    }

    public class NewOrderFuturesRequest
    {
        public string Symbol { get; set; } = null!;
        public string ClientOrderId { get; set; }
        public decimal Qty { get; set; }
        public decimal Price { get; set; }
        public decimal StopPrice { get; set; }
        public decimal ActivationPrice { get; set; }
        public decimal CallbackRate { get; set; }
        public bool ClosePosition { get; set; }
        public string PriceProtect { get; set; }
    }
}