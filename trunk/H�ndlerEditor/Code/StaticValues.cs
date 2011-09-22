using System.Data;

namespace HändlerEditor.Code
{
    public static class StaticValues
    {
        public static string GetNameLine(int tab)
        {
            // NameLine for a merchant-table by table number
            return string.Format("#Table	Tab{0:00}	        									\n",
                                    tab);
        }
        public static string TypeLine
        {
            get
            {
                // Type-Line for a merchant table
                return "#ColumnType	BYTE	String[33]	String[33]	String[33]	String[33]	String[33]	String[33]					\n";
            }
        }
        public static string ColumnLine
        {
            get
            {
                // Column-Line for a mechant-table
                return "#ColumnName	Rec	Column00	Column01	Column02	Column03	Column04	Column05					\n";
            }
        }
        public static DataTable DataTableTemplate
        {
            get
            {
                var tab = new DataTable();

                // Table columns for a merchant-table.
                tab.Columns.Add(new DataColumn("Rec", typeof(string)));
                tab.Columns.Add(new DataColumn("Column00", typeof(string)));
                tab.Columns.Add(new DataColumn("Column01", typeof(string)));
                tab.Columns.Add(new DataColumn("Column02", typeof(string)));
                tab.Columns.Add(new DataColumn("Column03", typeof(string)));
                tab.Columns.Add(new DataColumn("Column04", typeof(string)));
                tab.Columns.Add(new DataColumn("Column05", typeof(string)));

                return tab;
            }
        }
        public static void AddFileHeader(ShineTableFile table)
        {
            // Add standart header
            table.FileHeader.Add("#ignore	\\o042			; ÀÎ¿ëºÎÈ£ ¹«½Ã		");
            table.FileHeader.Add("#exchange	#	\\x20		; # => space	");
        }
    }
}
