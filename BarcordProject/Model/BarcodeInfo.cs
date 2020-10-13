using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcordProject.Model
{
    public class BarcodeInfo
    {
        private string _name;
        private string _value;
        private string _target;

        public string Name {
            get { return _name; }
            set { _name = value; }
        }
        public string Value
        {
            get { return _value; }
            set { _value = value; }
        }
        public string Target
        {
            get { return _target; }
            set { _target = value; }
        }
    }
}
