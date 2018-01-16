using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoomPresenceFunctions
{
    public class RoomStatus : TableEntity
    {
        public bool IsOccupied { get; set; }
    }
}
