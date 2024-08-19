using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

namespace WindWakerItemizer
{
    internal record Items(int Id, string Name)
    {
        public static Items None           { get; } = new(255, "Nothing");
        public static Items HeartPickup    { get; } = new(0, "Heart (Pickup)");
        public static Items GreenRupee     { get; } = new(1, "Green Rupee (1)");
        public static Items BlueRupee      { get; } = new(2, "Blue Rupee (5)");
        public static Items YellowRupee    { get; } = new(3, "Yellow Rupee (10)");
        public static Items RedRupee       { get; } = new(4, "Red Rupee (20)");
        public static Items PurpleRupee    { get; } = new(5, "Purple Rupee (50)");
        public static Items OrangeRupee    { get; } = new(6, "Orange Rupee (100)");
        public static Items HeartPiece     { get; } = new(7, "Piece of Heart");
        public static Items HeartContainer { get; } = new(8, "Heart Container");
        public static Items MagicJarSmall  { get; } = new(9, "Small Magic Jar");
        public static Items MagicJarLarge  { get; } = new(10, "Large Magic Jar");
        public static Items Bombs5         { get; } = new(11, "5 Bombs");
        public static Items Bombs10        { get; } = new(12, "10 Bombs");
        public static Items Bombs20        { get; } = new(13, "20 Bombs");
        public static Items Bombs30        { get; } = new(14, "30 Bombs");
        public static Items SilverRupee    { get; } = new(15, "Silver Rupee (200)");
        public static Items Arrows10       { get; } = new(16, "10 Arrows");
        public static Items Arrows20       { get; } = new(17, "20 Arrows");
        public static Items Arrows30       { get; } = new(18, "30 Arrows");

        public static Array GetValues()
        {
            List<string> names = new List<string>();

            foreach (PropertyInfo p in typeof(Items).GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (p.GetValue(p) is Items itm)
                {
                    names.Add(itm.Name);
                }
            }

            return names.ToArray();
        }

        public override string ToString() => Name;
    }
}
