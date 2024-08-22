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
        #region Constants
        private const int STATIC_DATA_SIZE = 0x0154;
        private const int ITEM_CONFIG_SIZE = 0x1A;
        private const byte PADDING_VALUE = 0xDC;
        #endregion

        #region Fields
        /// <summary>
        /// Copy of the (mostly) static data at the beginning of ActorDat.bin, the first 0x154 bytes.
        /// Storing it here when we load the file so we can modify it and write it back out without much fuss.
        /// </summary>
        private byte[]? mStaticFileData = null;
        /// <summary>
        /// List of the currently-loaded item drop configs.
        /// </summary>
        private List<ItemDropConfig> mConfigs = new List<ItemDropConfig>();
        #endregion

        #region Properties
        /// <summary>
        /// Whether any item drop configs have been loaded.
        /// </summary>
        public bool HasLoaded
        {
            get => mConfigs.Count != 0;
        }

        public int ConfigCount
        {
            get => mConfigs.Count;
        }
        #endregion

        public Model() { }

        #region Getters
        /// <summary>
        /// Constructs and returns an ObservableCollection containing the actor names of the currently-loaded configs.
        /// </summary>
        /// <returns>ObservableCollection of actor names.</returns>
        public ObservableCollection<string> GetActorNames()
        {
            ObservableCollection<string> names = new ObservableCollection<string>();

            foreach (ItemDropConfig cfg in mConfigs)
            {
                names.Add(cfg.ActorName);
            }

            return names;
        }

        /// <summary>
        /// Attempts to return the item drop config at the given index.
        /// </summary>
        /// <param name="idx">Index of the config to return.</param>
        /// <returns>The config at the given index, or null if the index is invalid.</returns>
        public ItemDropConfig? GetConfigAtIndex(int idx)
        {
            if (idx < 0 || idx >= mConfigs.Count)
            {
                return null;
            }

            return mConfigs[idx];
        }
        #endregion

        #region Mutators
        public void AddConfig()
        {
            mConfigs.Add(new ItemDropConfig());
        }

        public void DeleteConfig(int idx)
        {
            if (idx < 0 || idx >= mConfigs.Count)
            {
                return;
            }

            mConfigs.RemoveAt(idx);
        }
        #endregion

        #region IO
        /// <summary>
        /// Attempts to load the item drop configs from the file at the given path.
        /// </summary>
        /// <param name="fileName">File path of the file to load config data from.</param>
        /// <returns>Whether the load succeeded.</returns>
        public bool Deserialize(string fileName)
        {
            if (!File.Exists(fileName))
            {
                return false;
            }

            using (EndianFileStream reader = new EndianFileStream(fileName))
            {
                // Store (mostly) unchanging part of the file
                mStaticFileData = reader.ReadBytes(STATIC_DATA_SIZE);

                reader.Seek(0x14);
                uint actorCount = reader.ReadUInt();
                uint actorNamesOffset = reader.ReadUInt();

                reader.Skip(8);
                uint configDataOffset = reader.ReadUInt();

                // Read the actor names
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

                // Read the config data
                foreach (ItemDropConfig cfg in mConfigs)
                {
                    cfg.Deserialize(reader);
                }
            }

            return true;
        }

        /// <summary>
        /// Attempts to save the currently-loaded item drop configs to the file at the given path.
        /// </summary>
        /// <param name="fileName">File path to save the config data to.</param>
        /// <returns>Whether the save succeeded.</returns>
        public bool Serialize(string fileName)
        {
            if (mStaticFileData is null)
            {
                System.Console.WriteLine("Attempted to save before opening a file... huh?");
                return false;
            }

            using (EndianFileStream writer = new EndianFileStream(fileName, EFileMode.Write))
            {
                // Write the (mostly) static portion of the file to the stream
                writer.WriteBytes(mStaticFileData, STATIC_DATA_SIZE);

                // Update config count
                writer.Seek(0x14);
                writer.WriteInt(mConfigs.Count);

                // Update config data size
                writer.Skip(8);
                writer.WriteInt(mConfigs.Count * ITEM_CONFIG_SIZE);

                writer.Seek(0, SeekOrigin.End);

                // Write the actor names
                int stringDataOffset = STATIC_DATA_SIZE + sizeof(uint) * mConfigs.Count;
                List<byte> nameBytes = [];

                foreach (ItemDropConfig cfg in mConfigs)
                {
                    writer.WriteInt(stringDataOffset + nameBytes.Count);

                    nameBytes.AddRange(Encoding.UTF8.GetBytes(cfg.ActorName));
                    nameBytes.Add(0);
                }

                writer.WriteBytes([.. nameBytes], nameBytes.Count);
                writer.Pad(8, PADDING_VALUE);

                int configDataOffset = (int)writer.Tell();

                // Update config data offset
                writer.Seek(0x24);
                writer.WriteInt(configDataOffset);

                writer.Seek(0, SeekOrigin.End);

                // Write the config data itself
                foreach (ItemDropConfig cfg in mConfigs)
                {
                    cfg.Serialize(writer);
                }

                writer.Pad(32, PADDING_VALUE);
            }

            return true;
        }
        #endregion
    }
}
