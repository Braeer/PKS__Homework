using System;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
namespace ПКС6
{
    public partial class Form1 : Form
    {
        private DataTable dataTable;
        public Form1()
        {
            InitializeComponent();
            this.txtSearchLastName = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            dataTable = new DataTable();
            this.Controls.Add(this.txtSearchLastName);
            this.Controls.Add(this.btnSearch);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnOpenFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";
            openFileDialog.Title = "Select XML File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = openFileDialog.FileName;
                LoadDataFromFile(filePath);
            }
        }

        private void LoadDataFromFile(string filePath)
        {
            try
            {
                DisplayDataInGridView(filePath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading data: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void DisplayDataInGridView(string filePath)
        {
            dataTable.Columns.Add("ФИО", typeof(string));
            dataTable.Columns.Add("Год рождения", typeof(DateTime));
            dataTable.Columns.Add("Название должности", typeof(string));
            dataTable.Columns.Add("Дата начала работы", typeof(DateTime));
            dataTable.Columns.Add("Дата окончания работы", typeof(DateTime));
            dataTable.Columns.Add("Отдел", typeof(string));
            dataTable.Columns.Add("Год", typeof(int));
            dataTable.Columns.Add("Месяц", typeof(int));
            dataTable.Columns.Add("Итого", typeof(int));
            dataTable.Columns.Add("Доля_работающих", typeof(double));

            XDocument doc = XDocument.Load(filePath);

            var employees = doc.Descendants("Сотрудник")
            .SelectMany(employee => employee.Elements("Список_Работ")
                .Elements("Работа")
                .SelectMany(job => employee.Elements("Список_Зарплат")
                    .Elements("Зарплата")
                    .Select(salary => new
                    {
                        ФИО = employee.Element("ФИО").Value,
                        Год_рождения = DateTime.Parse(employee.Element("Год_рождения").Value),
                        Название_должности = job.Element("Название_должности")?.Value ?? "",
                        Дата_начала_работы = DateTime.Parse(job.Element("Дата_начала").Value),
                        Дата_окончания_работы = DateTime.Parse(job.Element("Дата_окончания").Value),
                        Отдел = job.Element("Отдел")?.Value ?? "",
                        Год = int.Parse(salary.Element("Год").Value),
                        Месяц = int.Parse(salary.Element("Месяц").Value),
                        Итого = int.Parse(salary.Element("Итого").Value),
                        Доля_работающих = CalculateWorkRatio(DateTime.DaysInMonth(int.Parse(salary.Element("Год").Value), int.Parse(salary.Element("Месяц").Value)), job.Element("Дата_начала").Value, job.Element("Дата_окончания").Value)
                    })))
            .Distinct();

            var uniqueEmployees = employees
                .GroupBy(e => new
                {
                    e.ФИО,
                    e.Дата_начала_работы,
                    e.Дата_окончания_работы,
                    e.Отдел,
                    e.Год,
                    e.Месяц,
                    e.Итого,
                    e.Доля_работающих
                })
                .Select(g => g.First());

            foreach (var employee in uniqueEmployees)
            {
                // Проверяем, существует ли запись с такими данными в таблице
                DataRow existingRow = dataTable.Rows.Cast<DataRow>().FirstOrDefault(row =>
                    row["ФИО"].ToString() == employee.ФИО &&
                    (DateTime)row["Дата начала работы"] == employee.Дата_начала_работы &&
                    (DateTime)row["Дата окончания работы"] == employee.Дата_окончания_работы &&
                    row["Отдел"].ToString() == employee.Отдел &&
                    (int)row["Год"] == employee.Год &&
                    (int)row["Месяц"] == employee.Месяц &&
                    (int)row["Итого"] == employee.Итого &&
                    (double)row["Доля_работающих"] == employee.Доля_работающих);

                // Если записи нет, добавляем новую запись в таблицу
                if (existingRow == null)
                {
                    DataRow newRow = dataTable.NewRow();
                    newRow["ФИО"] = employee.ФИО;
                    newRow["Год рождения"] = employee.Год_рождения;
                    newRow["Название должности"] = employee.Название_должности;
                    newRow["Дата начала работы"] = employee.Дата_начала_работы;
                    newRow["Дата окончания работы"] = employee.Дата_окончания_работы;
                    newRow["Отдел"] = employee.Отдел;
                    newRow["Год"] = employee.Год;
                    newRow["Месяц"] = employee.Месяц;
                    newRow["Итого"] = employee.Итого;
                    newRow["Доля_работающих"] = employee.Доля_работающих;

                    dataTable.Rows.Add(newRow);
                }
            }



            // Очищаем текстовое поле перед добавлением новой информации
            Task3.Clear();

            // Выводим информацию о сотрудниках в более чем одном отделе в текстовое поле Task3
            Task3.AppendText("Сотрудники, работающие в более чем одном отделе:\n" + Environment.NewLine);

            var addedEmployees = new HashSet<string>();

            foreach (var employee in uniqueEmployees)
            {
                var jobs = doc.Descendants("Сотрудник")
                              .First(e => e.Element("ФИО").Value == employee.ФИО)
                              .Element("Список_Работ")
                              .Elements("Работа");

                var multipleDepartments = jobs.Select(job => job.Element("Отдел")?.Value).Where(department => department != null).Distinct();

                if (addedEmployees.Add(employee.ФИО))
                {
                    StringBuilder employeeInfo = new StringBuilder();
                    employeeInfo.AppendLine($"{employee.ФИО}:");

                    foreach (var department in multipleDepartments)
                    {
                        employeeInfo.AppendLine($"Отдел: {department.Replace(" ", "")}");
                    }

                    Task3.AppendText(employeeInfo.ToString() + "\n");
                }


                if (addedEmployees.Add(employee.ФИО)) // Проверяем, был ли сотрудник уже добавлен
                {
                    StringBuilder employeeInfo = new StringBuilder();
                    employeeInfo.AppendLine($"{employee.ФИО}:");

                    foreach (var department in multipleDepartments)
                    {
                        employeeInfo.AppendLine($"Отдел: {department.Replace(" ", "")}");
                    }

                    Task3.AppendText(employeeInfo.ToString() + "\n");
                }
            }

            var departmentsWithFewEmployees = uniqueEmployees
                .GroupBy(e => e.Отдел)
                .Where(g => g.Count() <= 3)
                .Select(g => g.Key);

            Task3.AppendText("\nОтделы, в которых работает не более 3 сотрудников:" + Environment.NewLine);
            foreach (var department in departmentsWithFewEmployees)
            {
                Task3.AppendText(department + Environment.NewLine);
            }

            // Задание 5: Вывод годов с наибольшим и наименьшим количеством сотрудников

            // Группировка сотрудников по году принятия на работу
            var hiringYears = uniqueEmployees
                .GroupBy(employee => employee.Год)
                .Select(group => new
                {
                    Год = group.Key,
                    Количество = group.Count()
                })
                .OrderByDescending(group => group.Количество);

            // Вывод года с наибольшим количеством принятых сотрудников
            int maxHiringYear = hiringYears.First().Год;
            Task3.AppendText($"Год с наибольшим количеством принятых сотрудников: {maxHiringYear}{Environment.NewLine}");

            // Вывод года с наименьшим количеством принятых сотрудников
            int minHiringYear = hiringYears.Last().Год;
            Task3.AppendText($"Год с наименьшим количеством принятых сотрудников: {minHiringYear}{Environment.NewLine}");

            // Группировка сотрудников по году увольнения
            var firingYears = uniqueEmployees
                .GroupBy(employee => employee.Дата_окончания_работы.Year)
                .Select(group => new
                {
                    Год = group.Key,
                    Количество = group.Count()
                })
                .OrderByDescending(group => group.Количество);


            // Вывод года с наибольшим количеством уволенных сотрудников
            int maxFiringYear = firingYears.First().Год;
            Task3.AppendText($"Год с наибольшим количеством уволенных сотрудников: {maxFiringYear}{Environment.NewLine}");

            // Вывод года с наименьшим количеством уволенных сотрудников
            int minFiringYear = firingYears.Last().Год;
            Task3.AppendText($"Год с наименьшим количеством уволенных сотрудников: {minFiringYear}{Environment.NewLine}");

            DisplayAnniversaryEmployees();

            dataGridView1.DataSource = dataTable.DefaultView.ToTable(true, "ФИО", "Год рождения", "Название должности", "Дата начала работы", "Дата окончания работы", "Отдел", "Год", "Месяц", "Итого", "Доля_работающих");
        }


        private void DisplayAnniversaryEmployees()
        {
            int currentYear = DateTime.Now.Year;

            // Фильтруем сотрудников, у которых юбилей в текущем году
            var anniversaryEmployees = dataTable.AsEnumerable()
                .Where(row => (currentYear - row.Field<DateTime>("Год рождения").Year) % 5 == 0)
                .Distinct(new EmployeeDistinctComparer());

            // Выводим информацию о сотрудниках с юбилеем в текстовое поле Task3
            Task3.AppendText($"Сотрудники с юбилеем ({currentYear} год):\n");

            foreach (var employee in anniversaryEmployees)
            {
                int age = currentYear - employee.Field<DateTime>("Год рождения").Year;

                Task3.AppendText($"{employee.Field<string>("ФИО")}: {age} лет\n");
            }

            var departments = dataTable.AsEnumerable()
        .GroupBy(row => row.Field<string>("Отдел"))
        .Select(group => new
        {
            Department = group.Key,
            Employees = group.Select(row => row.Field<string>("ФИО")).Distinct().ToList(),
            TotalEmployees = group.Select(row => row.Field<string>("ФИО")).Distinct().Count(),
            WorkingEmployees = group.Select(row => row.Field<double>("Доля_работающих")).Max()
        });

            foreach (var department in departments)
            {
                double workRatioPercentage = (department.WorkingEmployees * 100);

                Task3.AppendText($"{department.Department}: {workRatioPercentage:F2}%{Environment.NewLine}");
            }








        }

        private void CalculateAndDisplayWorkRatio()
        {
            var departments = dataTable.AsEnumerable()
                .GroupBy(row => row.Field<string>("Отдел"))
                .Select(group => new
                {
                    Department = group.Key,
                    Employees = group.Select(row => row.Field<string>("ФИО")).Distinct().ToList(),
                    TotalEmployees = group.Select(row => row.Field<string>("ФИО")).Distinct().Count(),
                    WorkingEmployees = group.Select(row => row.Field<double>("Доля_работающих")).FirstOrDefault()
                });

            foreach (var department in departments)
            {
                double workRatioPercentage = (department.WorkingEmployees * 100);

                Task3.AppendText($"{department.Department}: {workRatioPercentage:F2}%{Environment.NewLine}");
            }
        }



        private void DisplayWorkRatioByDepartment()
        {

            // Group employees by department
            var departments = dataTable.AsEnumerable()
                .GroupBy(row => row.Field<string>("Отдел"))
                .Select(group => new
                {
                    Department = group.Key,
                    TotalEmployees = group.Count(),
                    WorkingEmployees = group.Count(row => row.Field<double>("Доля_работающих") > 0)
                });

            // Display the work ratio for each department in the TextBox
            foreach (var department in departments)
            {
                double workRatioPercentage = (department.WorkingEmployees / (double)department.TotalEmployees) * 100;

                Task3.AppendText($"Отдел: {department.Department}\n");
                Task3.AppendText($"Всего сотрудников: {department.TotalEmployees}\n");
                Task3.AppendText($"Работающих сотрудников: {department.WorkingEmployees}\n");
                Task3.AppendText($"Доля работающих: {workRatioPercentage:F2}%\n");
                Task3.AppendText("------------------------------\n");
            }
        }




        private double CalculateWorkRatio(int totalDaysInMonth, string startDate, string endDate)
        {
            DateTime start = DateTime.Parse(startDate);
            DateTime end = DateTime.Parse(endDate);

            int workingDays = 0;
            for (DateTime date = start; date <= end; date = date.AddDays(1))
            {
                if (date.DayOfWeek != DayOfWeek.Saturday && date.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }

            return (double)workingDays / totalDaysInMonth;
        }



        private void SearchEmployee(string lastName)
        {
            // Используем DataTable.Select для поиска сотрудников
            DataRow[] rows = dataTable.Select($"ФИО LIKE '% {lastName}'");

            if (rows.Length > 0)
            {
                // Создаем новую таблицу для отображения результатов поиска
                DataTable searchResultTable = dataTable.Clone();

                foreach (var row in rows)
                {
                    searchResultTable.ImportRow(row);
                }

                dataGridView1.DataSource = searchResultTable;

                // Выполняем расчет статистики
                CalculateStatistics(rows.ToList());
            }
            else
            {
                MessageBox.Show("Сотрудник не найден", "Поиск", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }



        private void CalculateStatistics(List<DataRow> employees)
        {
            if (employees.Any())
            {
                int maxSalary = employees.Max(row => row.Field<int>("Итого"));
                int minSalary = employees.Min(row => row.Field<int>("Итого"));
                double avgSalary = employees.Average(row => row.Field<int>("Итого"));

                MessageBox.Show($"Максимальная зарплата: {maxSalary}\nМинимальная зарплата: {minSalary}\nСредняя зарплата: {avgSalary:F2}",
                    "Статистика", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnSearch_Click_1(object sender, EventArgs e)
        {
            string searchLastName = txtSearchLastName.Text.Trim();
            SearchEmployee(searchLastName);
        }

        private void Task3_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnExport_Click(object sender, EventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "XML Files (*.xml)|*.xml|All files (*.*)|*.*";
                saveFileDialog.Title = "Save XML File";

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    string filePath = saveFileDialog.FileName;

                    var uniqueEmployees = dataTable.AsEnumerable()
                        .GroupBy(row => row.Field<string>("ФИО"))
                        .Select(group => group.First());

                    XDocument xmlDocument = new XDocument(
                        new XElement("Отделы",
                            from row in uniqueEmployees
                            group row by row.Field<string>("Отдел") into departmentGroup
                            select new XElement("Отдел",
                                new XAttribute("Название", departmentGroup.Key),
                                new XElement("Количество_работающих_сотрудников", departmentGroup.Count()),
                                new XElement("Количество_работающих_сотрудников_молодежь", departmentGroup.Count(row => (DateTime.Now.Year - row.Field<DateTime>("Год рождения").Year) < 30))
                            )
                        )
                    );

                    xmlDocument.Save(filePath);
                    MessageBox.Show($"XML файл успешно сохранен по пути: {filePath}", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting data to XML: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

    }

    public class EmployeeDistinctComparer : IEqualityComparer<DataRow>
    {
        public bool Equals(DataRow x, DataRow y)
        {
            return x.Field<string>("ФИО") == y.Field<string>("ФИО");
        }

        public int GetHashCode(DataRow obj)
        {
            return obj.Field<string>("ФИО").GetHashCode();
        }
    }

}