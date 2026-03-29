using System;
using System.Linq;

var validResponse = "$1\r\n$9\r\n$3\r\n0-0\r\n$8\r\nfield1\r\n$4\r\ndata\r\n";
Console.WriteLine("Input: " + validResponse.Replace("\r", "\\r").Replace("\n", "\\n"));

var streamEnd = "\r\n";
var parsedData = validResponse.Split(streamEnd, StringSplitOptions.RemoveEmptyEntries);
Console.WriteLine($"\nSplit by '{streamEnd}': {parsedData.Length} parts");
foreach (var p in parsedData) Console.WriteLine($"  '{p}'");
