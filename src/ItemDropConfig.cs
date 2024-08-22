using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace WindWakerItemizer
{
    internal class ItemDropConfig : INotifyPropertyChanged
    {
        #region Constants
        private const int LOOSE_ITEM_COUNT = 16;
        private const int ITEM_BALL_CONTENTS_COUNT = 8;
        #endregion

        #region Fields
        private string mActorName = "";
        private byte mArg;

        private byte[] mLooseItems = new byte[16];

        private byte mItemBallDropChance;
        private byte[] mItemBallContents = new byte[8];
        #endregion

        #region Properties
        public string ActorName
        {
            get => mActorName;
            set
            {
                mActorName = value;
                OnPropertyChanged();
            }
        }

        public byte Arg
        {
            get => mArg;
            set
            {
                mArg = value;
                OnPropertyChanged();
            }
        }

        public byte[] LooseItems
        {
            get => mLooseItems;
            set
            {
                mLooseItems = value;
                OnPropertyChanged();
            }
        }

        public byte ItemBallDropChance
        {
            get => mItemBallDropChance;
            set
            {
                mItemBallDropChance = value;
                OnPropertyChanged();
            }
        }

        public byte[] ItemBallContents
        {
            get => mItemBallContents;
            set
            {
                mItemBallContents = value;
                OnPropertyChanged();
            }
        }
        #endregion

        public ItemDropConfig()
            : this("dummy")
        {

        }

        public ItemDropConfig(string actorName)
        {
            mActorName = actorName;
        }

        #region IO
        /// <summary>
        /// Attempts to load a single item drop config from the given stream.
        /// </summary>
        /// <param name="reader">Stream to load the config data from.</param>
        /// <returns>Whether the load succeeded.</returns>
        public bool Deserialize(EndianFileStream reader)
        {
            Arg = reader.ReadByte();
            for (int i = 0; i < LOOSE_ITEM_COUNT; i++)
            {
                LooseItems[i] = reader.ReadByte();
            }

            ItemBallDropChance = reader.ReadByte();
            for (int i = 0; i < ITEM_BALL_CONTENTS_COUNT; i++)
            {
                ItemBallContents[i] = reader.ReadByte();
            }

            return true;
        }

        /// <summary>
        /// Attempts to write this item drop config to the given stream.
        /// </summary>
        /// <param name="writer">Stream to write the config data to.</param>
        /// <returns>Whether the write succeeded.</returns>
        public bool Serialize(EndianFileStream writer)
        {
            writer.WriteByte(Arg);
            for (int i = 0; i < LOOSE_ITEM_COUNT; i++)
            {
                writer.WriteByte(LooseItems[i]);
            }

            writer.WriteByte(ItemBallDropChance);
            for (int i = 0; i < ITEM_BALL_CONTENTS_COUNT; i++)
            {
                writer.WriteByte(ItemBallContents[i]);
            }

            return true;
        }
        #endregion

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        #endregion
    }
}
