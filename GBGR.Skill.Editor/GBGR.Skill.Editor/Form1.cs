using System.Data;
using Newtonsoft.Json;

namespace GBGR.Skill.Editor
{
    public partial class Form1 : Form
    {
        internal static string StartStr = "";
        internal static string OrignalStr = "";
        internal static string EditedStr = "";
        internal static bool InitToken = true;
        internal static DataTable Dt = new();
        internal static int SkillIndex = 0;
        internal static int SkillLength = 0;
        internal static List<string> SkillList = [];
        internal static string NullStr = "00-00-00-00-";
        internal static string SkillId = "";
        internal static OpenFileDialog? OpenFileDialog;
        internal static SaveFileDialog? SaveFileDialog;
        internal bool isNewSave = true;

        public Form1()
        {
            InitializeComponent();
        }

        private void CustomInit()
        {
            Dt = new DataTable();
            Dt.Columns.Add("Level (lv)");
            Dt.Columns.Add("Param 1");
            Dt.Columns.Add("Param 2");
            Dt.Columns.Add("Param 3");
            Dt.Columns.Add("Param 4");
            Dt.Columns.Add("Param 5");
            Dt.Columns.Add("Param 6");

            dataGridView1.DataSource = Dt;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveTblFile();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenTblFile();
        }

        private void SkillList_SelectedValueChanged(object sender, EventArgs e)
        {
            if (InitToken && OrignalStr == "")
            {
                return;
            }

            var skill = (Skill)SkillListComboBox.SelectedItem!;

            if (skill.Id == "B0 E0 7A 88" || skill.Id == "")
                return;

            var skillId = skill.Id.Replace(" ", "-");
            int lastIndex = 0;

            var indexList = new List<int>();
            var skillList = new List<string>();
            // 每個Skill Level 字串長度為108

            while ((lastIndex = OrignalStr.IndexOf(skillId, lastIndex)) != -1)
            {
                var currentIndex = lastIndex - 72;

                indexList.Add(currentIndex);
                skillList.Add(OrignalStr.Substring(currentIndex, 108));
                lastIndex += skillId.Length;
            }

            SkillIndex = indexList[0];
            SkillLength = indexList.Count * 108;
            SkillList = skillList;
            SkillId = skillList[0].Substring(72, 24);

            Dt.Clear();
            ShowDetail();
        }

        private void ShowDetail()
        {
            foreach (var item in SkillList)
            {
                var temp = item.Split("-");

                var row = Dt.NewRow();
                row[0] = Convert.ToInt32(temp[32], 16);
                row[1] = BitConverter.ToSingle([Convert.ToByte(temp[0], 16), Convert.ToByte(temp[1], 16), Convert.ToByte(temp[2], 16), Convert.ToByte(temp[3], 16)]);
                row[2] = BitConverter.ToSingle([Convert.ToByte(temp[4], 16), Convert.ToByte(temp[5], 16), Convert.ToByte(temp[6], 16), Convert.ToByte(temp[7], 16)]);
                row[3] = BitConverter.ToSingle([Convert.ToByte(temp[8], 16), Convert.ToByte(temp[9], 16), Convert.ToByte(temp[10], 16), Convert.ToByte(temp[11], 16)]);
                row[4] = BitConverter.ToSingle([Convert.ToByte(temp[12], 16), Convert.ToByte(temp[13], 16), Convert.ToByte(temp[14], 16), Convert.ToByte(temp[15], 16)]);
                row[5] = BitConverter.ToSingle([Convert.ToByte(temp[16], 16), Convert.ToByte(temp[17], 16), Convert.ToByte(temp[18], 16), Convert.ToByte(temp[19], 16)]);
                row[6] = BitConverter.ToSingle([Convert.ToByte(temp[20], 16), Convert.ToByte(temp[21], 16), Convert.ToByte(temp[22], 16), Convert.ToByte(temp[23], 16)]);

                Dt.Rows.Add(row);
            }
        }

        private string ReturnHexString()
        {
            // 9 SET
            // 1 Param 1
            // 2 Param 2
            // 3 Param 3
            // 4 None
            // 5 None
            // 6 None
            // 7 Skill Id
            // 8 Unique Skill Id
            // 9 Skill Level

            var editedSkill = string.Empty;

            foreach (DataRow row in Dt.Rows)
            {
                var param1 = ConvertToHex(row, 1) + "-";
                var param2 = ConvertToHex(row, 2) + "-";
                var param3 = ConvertToHex(row, 3) + "-";
                var param4 = ConvertToHex(row, 4) + "-";
                var param5 = ConvertToHex(row, 5) + "-";
                var param6 = ConvertToHex(row, 6) + "-";

                var level = Convert.ToInt32(row[0]).ToString("X2") + "-00-00-00-";

                // SkillId: 9B-1A-F1-71-FC-05-A8-7B-
                // level: 01-00-00-00-
                var tempSkill = param1 + param2 + param3 + param4 + param5 + param6 + SkillId + level;

                editedSkill += tempSkill;
            }

            return editedSkill;
        }

        private string ConvertToHex(DataRow row, int column)
        {
            return BitConverter.ToString(BitConverter.GetBytes(Convert.ToSingle(row[column])));
        }

        private byte[] SaveToByteArray()
        {
            EditedStr = OrignalStr[..SkillIndex] + ReturnHexString() + OrignalStr[(SkillIndex + SkillLength)..];

            // 賦值給存檔時不用再讀取新檔案位元
            var replacedString = StartStr + EditedStr;

            var replaced = replacedString.Trim('-').Split("-");
            var byteArray = new List<byte>();

            foreach (var item in replaced)
            {
                byteArray.Add(Convert.ToByte(Convert.ToInt32(item, 16)));
            }

            return [.. byteArray];
        }

        private void LocaleInit()
        {
            var locale = LocaleList.SelectedItem!.ToString();
#if DEBUG            
            var json = File.ReadAllText($"E:\\repo\\GBFR.Skill.Editor\\locale\\{locale}.json");
#else
            var json = File.ReadAllText($".\\locale\\{locale}.json");
#endif

            var skillDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json) ?? throw new Exception("DeserializeObject Failed");
            var skillList = new List<Skill>();

            foreach (var item in skillDict)
            {
                var skill = new Skill
                {
                    Id = item.Key,
                    Name = item.Value,
                };

                skillList.Add(skill);
            }

            SkillListComboBox.DataSource = skillList;
            SkillListComboBox.DisplayMember = "Name";
        }

        private void JaInit()
        {
            fileToolStripMenuItem.Text = "ファイル";
            openToolStripMenuItem.Text = "開く";
            saveAsToolStripMenuItem.Text = "名前を付けて保存";
            SaveButton.Text = "セーブ";
            label1.Text = "スキル";
        }

        private void EnInit()
        {
            fileToolStripMenuItem.Text = "File";
            openToolStripMenuItem.Text = "Open";
            saveAsToolStripMenuItem.Text = "Save As";
            SaveButton.Text = "Save";
            label1.Text = "Skill";
        }

        private void ZhInit()
        {
            fileToolStripMenuItem.Text = "檔案";
            openToolStripMenuItem.Text = "開啟";
            saveAsToolStripMenuItem.Text = "另存新檔";
            SaveButton.Text = "儲存";
            label1.Text = "技能";
        }

        private void LocaleList_SelectedValueChanged(object sender, EventArgs e)
        {
            var locale = LocaleList.SelectedIndex;

            switch (locale)
            {
                case 0:
                    EnInit();
                    break;
                case 1:
                    JaInit();
                    break;
                case 2:
                    ZhInit();
                    break;
            };

            if (OrignalStr != "")
            {
                LocaleInit();
            }
        }

        private void SaveTblFile()
        {
            SaveFileDialog ??= new SaveFileDialog
            {
                Filter = "tbl files (*.tbl)|*.tbl",
                Title = "Save File",
                FileName = "skill_status.tbl" // 預設檔案名稱
            };

            var byteArray = SaveToByteArray();

            if (!isNewSave)
            {
                File.WriteAllBytes(SaveFileDialog.FileName, [.. byteArray]);
            }
            // 顯示對話框並檢查使用者是否按下了「保存」按鈕
            else if (SaveFileDialog.ShowDialog() == DialogResult.OK)
            {
                File.WriteAllBytes(SaveFileDialog.FileName, [.. byteArray]);
                isNewSave = false;
            }
            else
            {
                return;
            }

            OrignalStr = EditedStr;

            MessageBox.Show("Save Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.None);
        }

        private void OpenTblFile()
        {
            isNewSave = true;

            OpenFileDialog ??= new OpenFileDialog
            {
                Title = "Select File",
#if DEBUG
                InitialDirectory = "D:\\Desktop\\Reloaded-II\\Mods\\gbfr.custom.skill.level\\GBFR\\data\\system\\table",
#endif
                Filter = "tbl files (*.tbl)|*.tbl"
            };

            if (OpenFileDialog.ShowDialog() == DialogResult.OK)
            {
                CustomInit();
                LocaleInit();
            }
            else
            {
                return;
            }

            if (SaveFileDialog != null)
            {
                SaveFileDialog = new SaveFileDialog
                {
                    Filter = "tbl files (*.tbl)|*.tbl",
                    Title = "Save File",
                    FileName = "skill_status.tbl" // 預設檔案名稱
                };
            }

            var ms = new MemoryStream();

            OpenFileDialog.OpenFile().CopyTo(ms);

            byte[] dataByte = ms.ToArray();

            var str = BitConverter.ToString(dataByte, 0, dataByte.Length);
            StartStr = str[..24];
            OrignalStr = str[24..];
            // 前有8組byte 要先減掉
            // 9 SET
            // 1 Param 1
            // 2 Param 2
            // 3 Param 3
            // 4 None
            // 5 None
            // 6 None
            // 7 Skill Id
            // 8 Unique Skill Id
            // 9 Skill Level
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            isNewSave = true;
            SaveTblFile();
        }
    }
}
