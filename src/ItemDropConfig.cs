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
        #region Backing fields
        private string mActorName = "";
        private byte mArg;

        private byte[] mLooseItems = new byte[16];

        private byte mItemBallDropChance;
        private byte[] mItemBallContents = new byte[8];
        #endregion

        #region Public members
        public event PropertyChangedEventHandler? PropertyChanged;

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
        {

        }

        public ItemDropConfig(string actorName)
        {
            mActorName = actorName;
        }

        public bool Deserialize()
        {
            return true;
        }

        public bool Serialize()
        {
            return true;
        }

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
