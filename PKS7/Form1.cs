using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ПКС7
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void Open_btn_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.Filter = "JSON files (*.json)|*.json|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    Task.Run(() => LoadDataFirstProgram(openFileDialog.FileName));

                    Task.Run(() => LoadDataSecondProgram(openFileDialog.FileName));


                    Task.Run(() => LoadDataFourthProgram(openFileDialog.FileName));

                    Task.Run(() => LoadDataFifthProgram(openFileDialog.FileName));
                }
            }
        }


        private string retype(string input)
        {
            if (input == "Б1") return "теоретическое обучение             ";
            if (input == "Б2") return "практика                           ";
            if (input == "Э") return "промежуточная аттестация           ";
            if (input == "К") return "каникулы                           ";
            if (input == "У") return "учебная практика                   ";
            if (input == "П") return "производственная практика          ";
            if (input == "НИР") return "научно-исследовательская работа    ";
            if (input == "Д") return "государственная итоговая аттестация";
            else return "Неопр.";
        }


        private async Task LoadDataFirstProgram(string filePath)
        {
            string json = File.ReadAllText(filePath);
            JObject opop = JObject.Parse(json);
            JArray professionalStandards = (JArray)opop["content"]["section4"]["professionalStandards"];

            List<ProfessionalStandardData> data = new List<ProfessionalStandardData>();

            int count = 0;

            foreach (JToken standard in professionalStandards)
            {
                count += 1;
                string content = (string)standard["content"];

                ProfessionalStandardData standardData = new ProfessionalStandardData
                {
                    Номер = count.ToString(),
                    Код = CheckFirstWord(content) ? content.Split(" ")[0] : "",
                    Название = CheckFirstWord(content) ? content.Substring(content.IndexOf(' ')) : content
                };

                data.Add(standardData);
            }

            await UpdateDataGridView(data);
        }

        private async Task LoadDataSecondProgram(string filePath)
        {
            string json = File.ReadAllText(filePath);
            JObject opop = JObject.Parse(json);
            JArray arr = (JArray)opop["content"]["section4"]["universalCompetencyRows"];

            List<StandardData> data = new List<StandardData>();

            foreach (JToken standard in arr)
            {
                StandardData standardData = new StandardData
                {
                    Код = standard["competence"]["code"].ToString(),
                    Название = standard["competence"]["title"].ToString(),
                    Indicators = new List<string>()
                };

                foreach (JToken trt in standard["indicators"])
                {
                    standardData.Indicators.Add(trt["content"].ToString());
                }

                data.Add(standardData);
            }

            await UpdateTextBox(data);
        }

        private async Task UpdateDataGridView(List<ProfessionalStandardData> data)
        {
            await Task.Run(() =>
            {
                dataGridView1.Invoke(new Action(() =>
                {
                    dataGridView1.DataSource = data;
                    dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
                }));
            });
        }

        private async Task UpdateTextBox(List<StandardData> data)
        {
            await Task.Run(() =>
            {
                Text_Out.Invoke(new Action(() =>
                {
                    Text_Out.Clear();
                    foreach (var item in data)
                    {
                        Text_Out.AppendText($"Код: {item.Код}{Environment.NewLine}");
                        Text_Out.AppendText($"Название: {item.Название}{Environment.NewLine}");
                        foreach (var indicator in item.Indicators)
                        {
                            Text_Out.AppendText($"{indicator}{Environment.NewLine}");
                        }
                        Text_Out.AppendText(add_minus(60) + Environment.NewLine);
                    }
                }));
            });
        }

        private string add_minus(int g = 1)
        {
            string S = "";
            for (int i = 0; i < g; i++)
            {
                S += "-";
            }
            return S;
        }

        private bool CheckFirstWord(string input)
        {
            string firstWord = input.Substring(0, input.IndexOf(' '));
            firstWord = firstWord.Replace(".", "");

            return int.TryParse(firstWord, out _);
        }

        private JArray JMerge(JArray array1, JArray array2)
        {
            JArray combinedArray = new JArray();

            foreach (var item in array1)
            {
                combinedArray.Add(item);
            }

            foreach (var item in array2)
            {
                combinedArray.Add(item);
            }

            return combinedArray;
        }

        private string ProcessString(string input)
        {
            return input
                .Replace('"', '*')
                .Replace("</p>", "\n")
                .Replace("<p>", "")
                .Replace("<div>", "")
                .Replace("</div>", "")
                .Replace("<br>", "")
                .Replace("<p class=*MsoNormal*>", "")
                .Replace("</br>", "");
        }

        private async Task LoadDataFourthProgram(string filePath)
        {
            string json = File.ReadAllText(filePath);
            JObject opop = JObject.Parse(json);
            JArray arr1 = (JArray)opop["content"]["section5"]["eduPlan"]["block2"]["subrows"];
            JArray arr2 = (JArray)opop["content"]["section5"]["eduPlan"]["gias"];
            JArray arr3 = (JArray)opop["content"]["section5"]["eduPlan"]["block1"]["subrows"];
            JArray arr4 = (JArray)opop["content"]["section5"]["eduPlan"]["block2Variety"]["subrows"];
            JArray arr5 = (JArray)opop["content"]["section5"]["eduPlan"]["block1Variety"]["subrows"];

            JArray all_arr = JMerge(arr1, JMerge(arr2, JMerge(arr3, JMerge(arr4, arr5))));

            List<FourthProgramData> data = new List<FourthProgramData>();

            foreach (JToken standard in all_arr)
            {
                FourthProgramData programData = new FourthProgramData
                {
                    Index = standard["index"]?.ToString() ?? "",
                    Title = standard["title"]?.ToString() ?? "",
                    Description = standard["description"]?.ToString() ?? "",
                    Competences = GetCompetences(standard["competences"]),
                    UnitsCost = standard["unitsCost"]?.ToString() ?? "",
                    Terms = GetTerms(standard["terms"])
                };

                data.Add(programData);
            }

            UpdateDataGridView3(data);
        }

        private string GetCompetences(JToken competences)
        {
            if (competences == null) return "";

            List<string> competenceList = competences.Select(com => com["code"]?.ToString() ?? "").ToList();
            return string.Join(" ", competenceList);
        }

        private string GetTerms(JToken terms)
        {
            if (terms == null) return "";

            List<string> termList = terms.Select(t => (bool)t ? "O" : "X").ToList();
            return string.Join(" ", termList);
        }

        private void UpdateDataGridView3(List<FourthProgramData> data)
        {
            dataGridView3.Invoke(new Action(() =>
            {
                dataGridView3.DataSource = data.ToList();




                dataGridView3.Columns["Index"].HeaderText = "Индекс";
                dataGridView3.Columns["Title"].HeaderText = "Название";
                dataGridView3.Columns["Description"].HeaderText = "Описание";
                dataGridView3.Columns["Competences"].HeaderText = "Компетенции";
                dataGridView3.Columns["UnitsCost"].HeaderText = "Зачётные единицы";
                dataGridView3.Columns["Terms"].HeaderText = "Семестры";

                dataGridView3.Refresh();
            }));
        }

        private async Task LoadDataFifthProgram(string filePath)
        {
            string json = File.ReadAllText(filePath);
            JObject opop = JObject.Parse(json);
            JArray arr = (JArray)opop["content"]["section5"]["calendarPlanTable"]["courses"];

            int count = 0;
            string inp;

            List<string> scheduleData = new List<string>();

            scheduleData.Add(add_minus(60));
            foreach (JToken standard in arr)
            {
                count += 1;

                scheduleData.Add("Курс - " + count.ToString());
                scheduleData.Add("");

                inp = "";
                foreach (String weeks in standard["weekActivityIds"])
                {
                    inp += " " + weeks;
                }

                var scheduleLines = await Task.Run(() => GetScheduleLines(inp.Substring(1)));
                scheduleData.AddRange(scheduleLines);

                scheduleData.Add(add_minus(60));
            }

            await UpdateTextBox2(scheduleData);
        }

        private async Task<List<string>> GetScheduleLines(string input)
        {
            List<string> scheduleLines = new List<string>();

            string[] tokens = input.Split();

            string currentType = tokens[0];
            DateTime startDate = new DateTime(2023, 9, 1);
            DateTime endDate = startDate;
            int weeksCount = 0;

            foreach (string token in tokens)
            {
                if (token == currentType)
                {
                    endDate = endDate.AddDays(7);
                    weeksCount++;
                }
                else
                {
                    scheduleLines.Add(GetScheduleLine(currentType, startDate, endDate, weeksCount));

                    currentType = token;
                    startDate = endDate;
                    endDate = startDate;
                    weeksCount = 0;
                }
            }

            scheduleLines.Add(GetScheduleLine(currentType, startDate, endDate, weeksCount));

            return scheduleLines;
        }

        private string GetScheduleLine(string type, DateTime startDate, DateTime endDate, int weeksCount)
        {
            return $"{retype(type)}  :  {startDate.ToShortDateString()} -> {endDate.AddDays(-1).ToShortDateString()} ({weeksCount + 1} недель)";
        }

        private async Task UpdateTextBox2(List<string> data)
        {
            await Task.Run(() =>
            {
                Text_Out2.Invoke(new Action(() =>
                {
                    Text_Out2.Lines = data.ToArray();
                }));
            });
        }

        private class ProfessionalStandardData
        {
            public string Номер { get; set; }
            public string Код { get; set; }
            public string Название { get; set; }
        }

        private class StandardData
        {
            public string Код { get; set; }
            public string Название { get; set; }
            public List<string> Indicators { get; set; }
        }

        private class FourthProgramData
        {
            public string Index { get; set; }
            public string Title { get; set; }
            public string Description { get; set; }
            public string Competences { get; set; }
            public string UnitsCost { get; set; }
            public string Terms { get; set; }
        }
    }
}
