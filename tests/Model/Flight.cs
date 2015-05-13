
namespace SharpSapRfc.Test.Model
{
    public class Flight
    {
        [RfcStructureField("CARRID")]
        public string Carrier { get; set; }
        [RfcStructureField("CONNID")]
        public string ConnectionId { get; set; }
        [RfcStructureField("COUNTRYFR")]
        public string ContryFrom { get; set; }
        [RfcStructureField("CITYFROM")]
        public string CityFrom { get; set; }
        [RfcStructureField("AIRPFROM")]
        public string AirportFrom { get; set; }
        [RfcStructureField("COUNTRYTO")]
        public string ContryTo { get; set; }
        [RfcStructureField("CITYTO")]
        public string CityTo { get; set; }
        [RfcStructureField("AIRPTO")]
        public string AirportTo { get; set; }
        [RfcStructureField("FLTIME")]
        public int FlightTime { get; set; }
        [RfcStructureField("DISTANCE")]
        public decimal FlightDistance { get; set; }
    }
}