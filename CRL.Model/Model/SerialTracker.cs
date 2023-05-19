using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CRL.Infrastructure.Domain;
using CRL.Model.Infrastructure;

namespace CRL.Model
{
    public enum SerialTrackerEnum { Registration = 1 , FSActivities=2, TempRegistration=3, TempFSActivities =4, Search =5, 
       Payment=6,ClientCode=7};
    public class SerialTracker : EntityBase<SerialTrackerEnum>, IAggregateRoot
    {
        public string Name { get; set; }
        public int Value { get; set; }
        [Timestamp]
        public byte[] RowVersion { get; set; }

        public static string GetSerialValue(ISerialTrackerRepository _serialTrackerRepository,SerialTrackerEnum valueType)
        {
            string SerialValue;
            SerialTracker _serial = _serialTrackerRepository.FindBy(valueType);
            //  CONVERT(CHAR(4),YEAR(GETDATE()),1) + RIGHT('000000' + CAST(@idd AS NVARCHAR(6)), 6)
            string YearPart = DateTime.Now.Year.ToString().Substring(2, 2);
            string IdentityPart = _serial.Value.ToString("00000000");
            string RegCode = YearPart+ "-" + IdentityPart;
            long code = Convert.ToInt64(YearPart+IdentityPart );
            int checksum = (int)((98 - (code * 100 % 97) % 97));
            SerialValue = RegCode + "-"+checksum.ToString("00");
            _serial.Value++;
            return SerialValue;

        }
        protected override void CheckForBrokenRules()
        {
            throw new NotImplementedException();
        }
    }
}
