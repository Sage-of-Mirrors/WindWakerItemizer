using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.IO;

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
            ObservableCollection<string> names = new ObservableCollection<string>();

            foreach (ItemDropConfig cfg in mConfigs)
            {
                names.Add(cfg.ActorName);
            }

            return names;
        }

        public ItemDropConfig? GetConfig(int idx)
        {
            if (idx < 0 || idx >= mConfigs.Count)
            {
                return null;
            }

            return mConfigs[idx];
        }

        public bool Deserialize(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            using (EndianStreamReader reader = new EndianStreamReader(fileName))
            {
                reader.Seek(0x14);
                uint actorCount = reader.ReadUInt();
                uint actorNamesOffset = reader.ReadUInt();
                reader.Skip(8);
                uint configDataOffset = reader.ReadUInt();

                reader.Seek(actorNamesOffset);
                for (uint i = 0; i < actorCount; i++)
                {
                    uint nameOffset = reader.ReadUInt();

                    long curPos = reader.Tell();
                    reader.Seek(nameOffset);

                    string name = reader.ReadString();
                    mConfigs.Add(new ItemDropConfig(name));

                    reader.Seek(curPos);
                }

                reader.Seek(configDataOffset);

                foreach(ItemDropConfig cfg in mConfigs)
                {
                    cfg.Deserialize(reader);
                }
            }

            return true;
        }
    }
}
