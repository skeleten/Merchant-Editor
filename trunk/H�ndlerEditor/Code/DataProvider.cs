using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using FiestaLib;

namespace HändlerEditor.Code
{
    public static class DataProvider
    {
        public static List<Item> Items = new List<Item>();

        public static void Initialize()
        {
            Items = new List<Item>();

            var start = DateTime.Now;
            TimeSpan end;
            using (var itemInfo = new SHNFile(Settings.DataPath + "\\ItemInfo.shn"))
            {
                using (var itemViewInfo = new SHNFile(Settings.DataPath + "\\ItemViewInfo.shn"))
                {
                    end = DateTime.Now.Subtract(start);

                    Trace.WriteLine("Shn's loaded in " + end.TotalMilliseconds + "ms");
                    start = DateTime.Now;


                    var iiEnum = itemInfo.Rows.GetEnumerator();
                    var iviEnum = itemViewInfo.Rows.GetEnumerator();

                    while(iiEnum.MoveNext())
                    {   
                        int resetTimes = 0;
                        while ((iviEnum.Current == null) || 
                            (((string)((DataRow)iviEnum.Current)["InxName"]) != ((string)((DataRow)iiEnum.Current)["InxName"])))
                        {
                            if (!iviEnum.MoveNext())
                            {
                                iviEnum.Reset();
                                resetTimes++;
                            }
                            if (resetTimes > 1)
                                break;
                        }

                        if(resetTimes > 1)
                            continue;

                        var i = new Item
                        {
                            ID = (ushort)(((DataRow)iiEnum.Current)["ID"] ?? -1),
                            InxName = (string)(((DataRow)iiEnum.Current)["InxName"] ?? ""),
                            Name = (string)(((DataRow)iiEnum.Current)["Name"] ?? ""),
                            DemandLevel = (uint)(((DataRow)iiEnum.Current)["DemandLv"] ?? -1),
                            IconFile = ((string)(((DataRow)iviEnum.Current)["IconFile"]) ?? "").ToLower(),
                            IconIndex = (uint)(((DataRow)iviEnum.Current)["IconIndex"] ?? 0)
                        };
                        Items.Add(i);
                    }
                }
            }
            end = DateTime.Now.Subtract(start);
            Trace.WriteLine("Items initialized in " + end.TotalMilliseconds + "ms");
        }
    }
}