using System.Security.Cryptography.X509Certificates;

namespace Telemetryy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var TelemetryDataFile = File.ReadAllLines("C:\\Users\\sbp\\source\\repos\\LINQ\\Telemetryy\\TelemetryDataFile.txt")
                .Select(line => line.Split(','))
                .Select(x => new
                {
                    deviceId = x[0],
                    deviceTypeCode = x[1],
                    sequenceNumber = x[2],
                    timeStamp = x[3],
                    telemetryData = x[4]
                }).OrderBy(data => data.sequenceNumber)
                .ToList();

            var TelemetryTypeMapper = File.ReadLines("C:\\Users\\sbp\\source\\repos\\LINQ\\Telemetryy\\TelemetryTypeMapperFile.txt")
                .Select(line => line.Split(','))
                .ToDictionary(x =>$"{x[0]}{x[1]}", x => x[2]);


            var Mapperdata = TelemetryDataFile.Select(data =>
            {
            var telemetryType = data.telemetryData.Substring(0, 3);
            var key = $"{data.deviceTypeCode}{telemetryType}";
            var eventType = TelemetryTypeMapper.ContainsKey(key) ? TelemetryTypeMapper[key] : "DISCARD";

                return new
                {
                    data.deviceId,
                    data.sequenceNumber,
                    EventType = eventType,
                    timeStamp = data.timeStamp,
                    Telemetry = data.telemetryData
                };
            }).ToList();


            using (var writer = new StreamWriter("C:\\Users\\sbp\\source\\repos\\LINQ\\Telemetryy\\OutputFile.txt"))
            {
                writer.WriteLine("deviceId , TelemetrySequenceNumber, EventType, TelemetryData");
                foreach(var map in Mapperdata)
                {
                    if (map.EventType != "DISCARD")
                    {
                        writer.WriteLine($"{map.deviceId},{map.sequenceNumber},{map.EventType},{map.Telemetry}");
                    }
                }
                using (var discardWriter = new StreamWriter("C:\\Users\\sbp\\source\\repos\\LINQ\\Telemetryy\\DiscardFile.txt"))

                {
                    discardWriter.WriteLine("deviceId , TelemetrySequenceNumber, EventType, TelemetryData");
                    foreach (var map in Mapperdata)
                    {
                        if(map.EventType == "DISCARD")
                        {
                            discardWriter.WriteLine($"{map.deviceId},{map.sequenceNumber},{map.EventType},{map.Telemetry}");
                        }
                    }
                }
                   
                
            };






        }
    }
}