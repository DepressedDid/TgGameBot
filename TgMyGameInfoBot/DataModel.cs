using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TgMyGameInfoBot
{
    public class DataModel
    {
        public List<Data> DataList { get; set; }
    }
    public class Data
    {

        public string GameName { get; set; }
        public string StoreLink { get; set; }
    }
}
