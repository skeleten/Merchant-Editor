using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using FiestaLib;

namespace HändlerEditor.Code {
    public static class DataProvider {
        public static List<Item> Items = new List<Item>();
        public static void Initialize() {
            Items = new List<Item>();

            DateTime start = DateTime.Now;
            TimeSpan end;
            using (var itemInfo = new SHNFile(Settings.DataPath + "\\ItemInfo.shn"))
            using (var itemViewInfo = new SHNFile(Settings.DataPath + "\\ItemViewInfo.shn")) {
                end = DateTime.Now.Subtract(start);

                Trace.WriteLine("Shn's loaded in " + end.TotalMilliseconds + "ms");
                start = DateTime.Now;
                ParseItems(itemInfo, itemViewInfo);
            }
            end = DateTime.Now.Subtract(start);
            Trace.WriteLine("Items initialized in " + end.TotalMilliseconds + "ms");
        }

        private static void ParseItems(SHNFile itemInfo, SHNFile itemViewInfo) {
            IEnumerator itemInfoEnum = itemInfo.Rows.GetEnumerator();
            IEnumerator itemViewInfoEnum = itemViewInfo.Rows.GetEnumerator();

            while (itemInfoEnum.MoveNext()) {
                int resetTimes = 0;
                while ((itemViewInfoEnum.Current == null) ||
                       (((string) ((DataRow) itemViewInfoEnum.Current)["InxName"]) !=
                        ((string) ((DataRow) itemInfoEnum.Current)["InxName"]))) {
                    if (!itemViewInfoEnum.MoveNext()) {
                        itemViewInfoEnum.Reset();
                        resetTimes++;
                    }
                    if (resetTimes > 1)
                        break;
                }

                if (resetTimes > 1)
                    continue;

                Item item = ParseItemFromEnumartors(itemInfoEnum, itemViewInfoEnum);
                Items.Add(item);
            }
        }
        private static Item ParseItemFromEnumartors(IEnumerator itemInfoEnum, IEnumerator itemViewInfoEnum) {
            return new Item {
                                Id = (ushort) (((DataRow) itemInfoEnum.Current)["ID"] ?? -1),
                                InxName = (string) (((DataRow) itemInfoEnum.Current)["InxName"] ?? ""),
                                Name = (string) (((DataRow) itemInfoEnum.Current)["Name"] ?? ""),
                                DemandLevel = (uint) (((DataRow) itemInfoEnum.Current)["DemandLv"] ?? -1),
                                IconFile = ((string) (((DataRow) itemViewInfoEnum.Current)["IconFile"]) ?? "").ToLower(),
                                IconIndex = (uint) (((DataRow) itemViewInfoEnum.Current)["IconIndex"] ?? 0)
                            };
        }
    }
}