using System.Text;
using Newtonsoft.Json;
using OfficeOpenXml;

namespace SkillExportJSON;

internal class Program
{
    static void Main(string[] args)
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

        var filePath = "D:\\Desktop\\Granblue Relink Sigils_Traits.xlsx";

        using ExcelPackage excelPackage = new ExcelPackage(filePath);

        ExcelWorksheet ws = excelPackage.Workbook.Worksheets[0];

        var usedRow = ws.Dimension.End.Row;

        var skill = new Dictionary<string, string>();

        var currentRow = 2;

        while (currentRow < usedRow)
        {
            var traitName = ws.Cells[currentRow, 4].Value?.ToString() ?? "";
            var traitId = ws.Cells[currentRow, 5].Value?.ToString() ?? "";

            if (traitName != "" && traitId != "")
            {
                skill.Add(traitId, traitName);
            }

            currentRow++;
        }

        var x = JsonConvert.SerializeObject(skill, Formatting.Indented);
        var y = Encoding.UTF8.GetBytes(x);
        var file = File.OpenWrite("E:\\repo\\GBFR.Skill.Editor\\en-US.json");
        file.Write(y, 0, y.Length);
        file.Dispose();
    }
}
