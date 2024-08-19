using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WindWakerItemizer
{
    internal class Model
    {
        private List<ItemDropConfig> mConfigs = new List<ItemDropConfig>();

        public Model()
        {

        }

        public ObservableCollection<string> GetActorNames()
        {
            return new ObservableCollection<string>() { "test" };
        }

        public ItemDropConfig? GetConfig(int idx)
        {
            if (idx < 0 || idx <= mConfigs.Count)
            {
                return null;
            }

            return mConfigs[idx];
        }

        public bool Deserialize(string fileName)
        {
            return true;
        }
    }
}
