using System.Data;
using System.Text;
using Newtonsoft.Json;

namespace GBGR.Skill.Editor
{
    public partial class Form1 : Form
    {
        // Constants

        // Each Skill Level string length is 108
        private const int SKILL_LEVEL_LENGTH = 108;
        private const int SKILL_ID_OFFSET = 72;
        private const int HEADER_LENGTH = 24;
        private const int PARAM_BYTE_COUNT = 4;
        private const string LOCALE_FOLDER = "locale";
        private const string UI_JSON_FILE = "Ui.json";

        // Instance Fields
        private string _currentSkillId = "";
        private string _currentLocale = "";
        private string _editedHexString = "";
        // private readonly string _emptyPattern = "00-00-00-00-";
        private string _originalHexString = "";
        private string _fileHeader = "";

        private int _skillIndex = 0;
        private int _skillLength = 0;

        private readonly bool _isInitialized = true;
        private bool _isNewSave = true;

        private List<string> _skillList = [];
        private List<Ui> _uiLocales = [];

        private DataTable _skillDataTable = new();
        private OpenFileDialog? _openFileDialog;
        private SaveFileDialog? _saveFileDialog;

        public Form1()
        {
            InitializeComponent();
            LoadUiText();
            AllowDrop = true;
            DragEnter += Form1_DragEnter;
            DragDrop += Form1_DragDrop;
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            SaveTblFile();
        }

        private void OpenToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenTblFile();
        }

        private void SkillList_SelectedValueChanged(object sender, EventArgs e)
        {
            GetCurrentSkillId();

            if (_currentSkillId == "B0 E0 7A 88" || string.IsNullOrEmpty(_currentSkillId))
            {
                return;
            }

            LoadSkillParameters();

            PopulateSkillDataTable();
        }

        private void LocaleList_SelectedValueChanged(object sender, EventArgs e)
        {
            GetCurrentLocale();

            if (!string.IsNullOrEmpty(_currentLocale))
            {
                ApplyLocale(_currentLocale);
            }

            if (!string.IsNullOrEmpty(_originalHexString))
            {
                LoadSkillList();
            }
        }

        private void SaveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            _isNewSave = true;

            if (!string.IsNullOrEmpty(_originalHexString))
            {
                SaveTblFile();
            }
        }

        private void InitializeSkillDataTable()
        {
            _skillDataTable = new DataTable();

            _skillDataTable.Columns.Add("Level (lv)").ReadOnly = true;
            _skillDataTable.Columns.Add("Param 1");
            _skillDataTable.Columns.Add("Param 2");
            _skillDataTable.Columns.Add("Param 3");
            _skillDataTable.Columns.Add("Param 4");
            _skillDataTable.Columns.Add("Param 5");
            _skillDataTable.Columns.Add("Param 6");
            _skillDataTable.Columns.Add("Skill ID").ReadOnly = true;
            _skillDataTable.Columns.Add("Unique ID").ReadOnly = true;

            DataGridView1.DataSource = _skillDataTable;
        }

        private void GetCurrentSkillId()
        {
            if (_isInitialized && _originalHexString == "")
            {
                return;
            }

            if (SkillListComboBox.SelectedItem is not Skill skill)
            {
                return;
            }

            _currentSkillId = skill.Id;
        }

        private void LoadSkillParameters()
        {
            var skillId = _currentSkillId.Replace(" ", "-");
            int lastIndex = 0;

            var indexList = new List<int>();
            var skillList = new List<string>();

            while ((lastIndex = _originalHexString.IndexOf(skillId, lastIndex)) != -1)
            {
                var currentIndex = lastIndex - SKILL_ID_OFFSET;

                indexList.Add(currentIndex);
                skillList.Add(_originalHexString.Substring(currentIndex, SKILL_LEVEL_LENGTH));
                lastIndex += skillId.Length;
            }

            if (indexList.Count == 0)
            {
                return;
            }

            _skillIndex = indexList[0];
            _skillLength = indexList.Count * SKILL_LEVEL_LENGTH;
            _skillList = skillList;
        }

        /// <summary>
        /// Convert byte array to float
        /// </summary>
        private static float BytesToFloat(string[] bytes, int startIndex)
        {
            var byteArray = new byte[PARAM_BYTE_COUNT];
            for (int i = 0; i < PARAM_BYTE_COUNT; i++)
            {
                byteArray[i] = Convert.ToByte(bytes[startIndex + i], 16);
            }
            return BitConverter.ToSingle(byteArray);
        }

        /// <summary>
        /// Format hexadecimal string
        /// </summary>
        private static string FormatHexString(string[] hexValues, params int[] indices)
        {
            return string.Join("-", indices.Select(i => hexValues[i])) + "-";
        }

        private void PopulateSkillDataTable()
        {
            _skillDataTable.Clear();

            foreach (var item in _skillList)
            {
                var hexValues = item.Split("-");

                var row = _skillDataTable.NewRow();

                // - Level
                row[0] = Convert.ToInt32(hexValues[32], 16);
                // - Param 1
                row[1] = BytesToFloat(hexValues, 0);
                // - Param 2
                row[2] = BytesToFloat(hexValues, 4);
                // - Param 3
                row[3] = BytesToFloat(hexValues, 8);
                // - Param 4
                row[4] = BytesToFloat(hexValues, 12);
                // - Param 5
                row[5] = BytesToFloat(hexValues, 16);
                // - Param 6
                row[6] = BytesToFloat(hexValues, 20);
                // - Skill ID
                row[7] = FormatHexString(hexValues, 24, 25, 26, 27);
                // - Unique Id
                row[8] = FormatHexString(hexValues, 28, 29, 30, 31);

                _skillDataTable.Rows.Add(row);
            }
        }

        private string BuildHexString()
        {
            // 9 SET
            // 1 Param 1
            // 2 Param 2
            // 3 Param 3
            // 4 Param 4
            // 5 Param 5
            // 6 Param 6                                                            
            // 7 Skill Id
            // 8 Unique Skill Id
            // 9 Skill Level

            var editedSkill = new StringBuilder();

            foreach (DataRow row in _skillDataTable.Rows)
            {
                editedSkill.Append(ConvertToHex(row, 1)).Append('-');
                editedSkill.Append(ConvertToHex(row, 2)).Append('-');
                editedSkill.Append(ConvertToHex(row, 3)).Append('-');
                editedSkill.Append(ConvertToHex(row, 4)).Append('-');
                editedSkill.Append(ConvertToHex(row, 5)).Append('-');
                editedSkill.Append(ConvertToHex(row, 6)).Append('-');
                editedSkill.Append(row[7]).Append(row[8]);

                var level = Convert.ToInt32(row[0]).ToString("X2");
                // SkillId: 9B-1A-F1-71-FC-05-A8-7B-
                // level: 01-00-00-00-
                editedSkill.Append(level).Append("-00-00-00-");
            }

            return editedSkill.ToString();
        }

        private static string ConvertToHex(DataRow row, int column)
        {
            return BitConverter.ToString(BitConverter.GetBytes(Convert.ToSingle(row[column])));
        }

        private byte[] SaveToByteArray()
        {
            _editedHexString = _originalHexString[.._skillIndex] + BuildHexString() + _originalHexString[(_skillIndex + _skillLength)..];

            // Assign to save without reading new file bytes again
            var completeHexString = _fileHeader + _editedHexString;
            var hexTokens = completeHexString.Trim('-').Split("-");

            return [.. hexTokens.Select(item => Convert.ToByte(item, 16))];
        }

        /// <summary>
        /// Get locale file path
        /// </summary>
        private static string GetLocalePath(string fileName)
        {
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(basePath, LOCALE_FOLDER, fileName);
        }

        private void LoadSkillList()
        {
            try
            {
                var jsonPath = GetLocalePath($"{_currentLocale}.json");

                if (!File.Exists(jsonPath))
                {
                    MessageBox.Show($"Locale file not found: {jsonPath}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var json = File.ReadAllText(jsonPath);
                var skillDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

                if (skillDict == null)
                {
                    MessageBox.Show("Failed to parse skill list", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var skillList = skillDict.Select(item => new Skill
                {
                    Id = item.Key,
                    Name = item.Value
                }).ToList();

                SkillListComboBox.DataSource = skillList;
                SkillListComboBox.DisplayMember = "Name";
            }
            catch (IOException ex)
            {
                MessageBox.Show($"File read failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"JSON parsing failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LoadUiText()
        {
            try
            {
                var uiJsonPath = GetLocalePath(UI_JSON_FILE);

                if (!File.Exists(uiJsonPath))
                {
                    MessageBox.Show($"UI locale file not found: {uiJsonPath}", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var uiJson = File.ReadAllText(uiJsonPath);
                _uiLocales = JsonConvert.DeserializeObject<List<Ui>>(uiJson) ?? [];

                if (_uiLocales.Count == 0)
                {
                    MessageBox.Show("UI locale file is empty", "Warning",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                LocaleList.DataSource = _uiLocales.Select(x => x.Code).ToList();
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Unable to read UI locale file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (JsonException ex)
            {
                MessageBox.Show($"UI locale JSON parsing failed: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ApplyLocale(string locale)
        {
            var currentUiText = _uiLocales.FirstOrDefault(x => x.Code == locale);

            if (currentUiText == null)
            {
                MessageBox.Show($"Locale code not found: {locale}", "Warning",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            FileToolStripMenuItem.Text = currentUiText.FileToolStripMenuItem;
            OpenToolStripMenuItem.Text = currentUiText.OpenToolStripMenuItem;
            SaveAsToolStripMenuItem.Text = currentUiText.SaveAsToolStripMenuItem;
            SaveButton.Text = currentUiText.SaveButton;
            SkillLabel.Text = currentUiText.SkillLabel;
        }

        private void GetCurrentLocale()
        {
            _currentLocale = LocaleList.SelectedItem?.ToString() ?? string.Empty;
        }

        #region SaveTblFile Save Tbl File
        private void SaveTblFile()
        {
            try
            {
                _saveFileDialog ??= new SaveFileDialog
                {
                    Filter = "tbl files (*.tbl)|*.tbl",
                    Title = "Save File",
                    FileName = "skill_status.tbl" // Default file name
                };

                var byteArray = SaveToByteArray();

                if (!_isNewSave)
                {
                    File.WriteAllBytes(_saveFileDialog.FileName, byteArray);
                }
                // Show dialog and check if user clicked the Save button
                else if (_saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(_saveFileDialog.FileName, byteArray);
                    _isNewSave = false;
                }
                else
                {
                    return;
                }

                _originalHexString = _editedHexString;

                MessageBox.Show("Save Success", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Failed to save file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"No permission to write file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        #region OpenTblFile Open Tbl File
        private void OpenTblFile()
        {
            try
            {
                _isNewSave = true;

                _openFileDialog ??= new OpenFileDialog
                {
                    Title = "Select File",
                    Filter = "tbl files (*.tbl)|*.tbl"
                };

                if (_openFileDialog.ShowDialog() != DialogResult.OK)
                {
                    return;
                }

                OpenTblFileFromPath(_openFileDialog.FileName);
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Failed to open file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"No permission to read file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        #endregion

        private void Form1_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data?.GetDataPresent(DataFormats.FileDrop) != true)
            {
                e.Effect = DragDropEffects.None;
                return;
            }

            var files = (string[]?)e.Data.GetData(DataFormats.FileDrop);
            if (files is { Length: 1 } && IsTblFile(files[0]))
            {
                e.Effect = DragDropEffects.Copy;
                return;
            }

            e.Effect = DragDropEffects.None;
        }

        private void Form1_DragDrop(object? sender, DragEventArgs e)
        {
            var files = (string[]?)e.Data?.GetData(DataFormats.FileDrop);
            if (files is not { Length: 1 } || !IsTblFile(files[0]))
            {
                MessageBox.Show("Please drop a single .tbl file.", "Invalid File",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            OpenTblFileFromPath(files[0]);
        }

        private static bool IsTblFile(string filePath)
        {
            return File.Exists(filePath)
                   && string.Equals(Path.GetExtension(filePath), ".tbl", StringComparison.OrdinalIgnoreCase);
        }

        private void OpenTblFileFromPath(string filePath)
        {
            try
            {
                if (!IsTblFile(filePath))
                {
                    MessageBox.Show("Invalid file. Please select a .tbl file.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _isNewSave = true;
                InitializeSkillDataTable();
                LoadSkillList();

                using var stream = File.OpenRead(filePath);
                using var memoryStream = new MemoryStream();
                stream.CopyTo(memoryStream);

                byte[] dataBytes = memoryStream.ToArray();

                if (dataBytes.Length < HEADER_LENGTH)
                {
                    MessageBox.Show("Invalid file format: File too small", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var hexString = BitConverter.ToString(dataBytes, 0, dataBytes.Length);
                _fileHeader = hexString[..HEADER_LENGTH];

                // 2024/03/08 fix Crabvestment Returns issue
                _originalHexString = hexString[HEADER_LENGTH..] + "-";
            }
            catch (IOException ex)
            {
                MessageBox.Show($"Failed to open file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (UnauthorizedAccessException ex)
            {
                MessageBox.Show($"No permission to read file: {ex.Message}", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
