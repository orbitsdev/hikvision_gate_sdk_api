namespace HikvisionAPI.Models
{
    public class DeviceInfoResponse
    {
        public byte byAlarmInPortNum { get; set; }
        public byte byAlarmOutPortNum { get; set; }
        public byte byChanNum { get; set; }
        public byte byStartChan { get; set; }
        public byte byAudioChanNum { get; set; }
        public byte byIPChanNum { get; set; }
        public byte bySupport { get; set; }
        public byte bySupport1 { get; set; }
        public byte bySupport2 { get; set; }
    }
}
