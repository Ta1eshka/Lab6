using System;
using System.Drawing;
using System.Windows.Forms;
using BusinessObjects;
using ServiceLayer;

namespace HospitalApp
{
    // Главная форма приложения «Больница»
    public class MainForm : Form
    {
        // Инициализация сервисного слоя (Паттерн Façade)
        private static readonly IService service = new Service();

        // Текущая выбранная медицинская карта
        private MedicalRecord? _selectedRecord;

        // Элементы интерфейса
        private TreeView _treeHospital = null!;
        private Label _lblPatientInfo = null!;
        private Label _lblDoctors = null!;
        private Label _lblDiagnosis = null!;
        private Label _lblStatus = null!;
        private DataGridView _dgvPrescriptions = null!;

        private Button _btnAddPrescription = null!;
        private Button _btnExecutePrescription = null!;
        private Button _btnDischarge = null!;
        private Button _btnViewEpicrisis = null!;

        public MainForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Больница — Моделирование стационара";
            Size = new Size(1000, 600);
            StartPosition = FormStartPosition.CenterScreen;
            Font = new Font("Segoe UI", 9f);

            // Главный сплиттер (Слева: дерево, Справа: ЭМК)
            var split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                SplitterDistance = 300
            };

            // Левая панель: Дерево отделений и пациентов
            var pnlLeft = new Panel { Dock = DockStyle.Fill, Padding = new Padding(5) };

            var pnlLeftButtons = new FlowLayoutPanel { Dock = DockStyle.Top, Height = 40 };
            var btnHosp = new Button { Text = "🏥 Госпитализация", Width = 140, Height = 30 };
            btnHosp.Click += (s, e) =>
            {
                using var dlg = new HospitalizePatientForm(service);
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    LoadTree();
                }
            };

            var btnNewPat = new Button { Text = "👤 Новый пациент", Width = 135, Height = 30 };
            btnNewPat.Click += (s, e) =>
            {
                using var dlg = new NewPatientForm(service);
                dlg.ShowDialog(this);
            };

            pnlLeftButtons.Controls.AddRange(new Control[] { btnHosp, btnNewPat });

            _treeHospital = new TreeView { Dock = DockStyle.Fill };
            _treeHospital.AfterSelect += TreeHospital_AfterSelect;

            pnlLeft.Controls.Add(_treeHospital);
            pnlLeft.Controls.Add(pnlLeftButtons);
            split.Panel1.Controls.Add(pnlLeft);

            // Правая панель: Информация об ЭМК и назначения
            var pnlRight = new Panel { Dock = DockStyle.Fill, Padding = new Padding(10) };

            // Группа 1: Данные пациента и ЭМК
            var grpEmk = new GroupBox
            {
                Text = "Электронная медицинская карта (ЭМК)",
                Dock = DockStyle.Top,
                Height = 150
            };

            _lblPatientInfo = new Label { Location = new Point(15, 25), AutoSize = true, Font = new Font(Font, FontStyle.Bold) };
            _lblDoctors = new Label { Location = new Point(15, 52), AutoSize = true };
            _lblDiagnosis = new Label { Location = new Point(15, 80), AutoSize = true };
            _lblStatus = new Label { Location = new Point(15, 108), AutoSize = true, ForeColor = Color.DarkBlue };

            grpEmk.Controls.AddRange(new Control[] { _lblPatientInfo, _lblDoctors, _lblDiagnosis, _lblStatus });

            // Группа 2: Лист назначений
            var grpPresc = new GroupBox
            {
                Text = "Назначения врача и их выполнение",
                Dock = DockStyle.Fill
            };

            _dgvPrescriptions = new DataGridView
            {
                Dock = DockStyle.Fill,
                AllowUserToAddRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = Color.White,
                RowHeadersVisible = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
            };

            _dgvPrescriptions.Columns.Add("Type", "Тип");
            _dgvPrescriptions.Columns.Add("Name", "Название");
            _dgvPrescriptions.Columns.Add("Details", "Параметры");
            _dgvPrescriptions.Columns.Add("Status", "Статус");
            _dgvPrescriptions.Columns.Add("ExecutedBy", "Кто выполнил");
            _dgvPrescriptions.Columns.Add("Result", "Отметка / Результат");

            var pnlPrescButtons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 45, Padding = new Padding(5) };

            _btnAddPrescription = new Button { Text = "➕ Добавить назначение...", Width = 190, Height = 32 };
            _btnAddPrescription.Click += BtnAddPrescription_Click;

            _btnExecutePrescription = new Button { Text = "✔ Отметить выполнение...", Width = 180, Height = 32 };
            _btnExecutePrescription.Click += BtnExecutePrescription_Click;

            _btnDischarge = new Button { Text = "📋 Выписать (Эпикриз)...", Width = 170, Height = 32 };
            _btnDischarge.Click += BtnDischarge_Click;

            _btnViewEpicrisis = new Button { Text = "📄 Просмотр эпикриза", Width = 160, Height = 32, Visible = false };
            _btnViewEpicrisis.Click += (s, e) =>
            {
                if (_selectedRecord != null)
                {
                    var ep = new DischargeEpicrisis(_selectedRecord, "Наблюдение участкового терапевта.");
                    using var epDlg = new EpicrisisForm(ep);
                    epDlg.ShowDialog(this);
                }
            };

            pnlPrescButtons.Controls.AddRange(new Control[] { _btnAddPrescription, _btnExecutePrescription, _btnDischarge, _btnViewEpicrisis });

            grpPresc.Controls.Add(_dgvPrescriptions);
            grpPresc.Controls.Add(pnlPrescButtons);

            pnlRight.Controls.Add(grpPresc);
            pnlRight.Controls.Add(grpEmk);
            split.Panel2.Controls.Add(pnlRight);

            Controls.Add(split);

            Load += (s, e) => LoadTree();
        }

        // Заполнение дерева отделений и пациентов
        private void LoadTree()
        {
            _treeHospital.BeginUpdate();
            _treeHospital.Nodes.Clear();

            var departments = service.GetDepartments();
            var activeRecords = service.GetActiveRecords();
            var dischargedRecords = service.GetDischargedRecords();

            // Добавляем узлы отделений
            foreach (var dep in departments)
            {
                var depNode = new TreeNode($"🏥 {dep.Name}");
                var records = activeRecords.FindAll(r => r.DepartmentName == dep.Name);

                foreach (var rec in records)
                {
                    var patNode = new TreeNode($"🛌 {rec.Patient.FullName} ({rec.Id})")
                    {
                        Tag = rec
                    };
                    depNode.Nodes.Add(patNode);
                }

                _treeHospital.Nodes.Add(depNode);
                depNode.Expand();
            }

            // Узел архива выписанных
            var archNode = new TreeNode("📁 Архив выписанных");
            foreach (var rec in dischargedRecords)
            {
                var node = new TreeNode($"✓ {rec.Patient.FullName} ({rec.DepartmentName})")
                {
                    Tag = rec
                };
                archNode.Nodes.Add(node);
            }
            _treeHospital.Nodes.Add(archNode);

            _treeHospital.EndUpdate();
        }

        // Выбор пациента в дереве
        private void TreeHospital_AfterSelect(object? sender, TreeViewEventArgs e)
        {
            if (e.Node?.Tag is MedicalRecord record)
            {
                _selectedRecord = record;
                ShowMedicalRecord(record);
            }
        }

        // Отображение данных выбранной ЭМК
        private void ShowMedicalRecord(MedicalRecord record)
        {
            _lblPatientInfo.Text = $"Пациент: {record.Patient.FullName} ({record.Patient.Age} лет, {record.Patient.Gender}) | Паспорт: {record.Patient.Passport}";
            _lblDoctors.Text = $"Отделение: {record.DepartmentName} | Дежурный врач: {record.DutyDoctor} | Лечащий врач: {record.AttendingDoctor}";
            _lblDiagnosis.Text = $"Диагноз поступления: {record.AdmissionDiagnosis}" + (!string.IsNullOrEmpty(record.FinalDiagnosis) ? $" | Заключительный: {record.FinalDiagnosis}" : "");
            _lblStatus.Text = $"Статус: {record.Status} | Дата поступления: {record.AdmissionDate:dd.MM.yyyy}";

            bool isActive = record.Status == "На лечении";
            _btnAddPrescription.Enabled = isActive;
            _btnExecutePrescription.Enabled = isActive;
            _btnDischarge.Enabled = isActive;
            _btnViewEpicrisis.Visible = !isActive;

            // Заполняем таблицу назначений
            _dgvPrescriptions.Rows.Clear();
            foreach (var p in record.Prescriptions)
            {
                string status = p.IsCompleted ? "Выполнено" : "Ожидает";
                string who = p.IsCompleted ? $"{p.ExecutedBy} ({p.ExecutedRole})" : "—";
                string res = p.IsCompleted ? p.ExecutionResult : "—";

                int rowIdx = _dgvPrescriptions.Rows.Add(p.Type, p.Name, p.Details, status, who, res);
                _dgvPrescriptions.Rows[rowIdx].Tag = p;

                if (p.IsCompleted)
                {
                    _dgvPrescriptions.Rows[rowIdx].DefaultCellStyle.ForeColor = Color.DarkGreen;
                }
            }
        }

        private void BtnAddPrescription_Click(object? sender, EventArgs e)
        {
            if (_selectedRecord == null) return;

            using var dlg = new AddPrescriptionForm(service, _selectedRecord);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                ShowMedicalRecord(_selectedRecord);
            }
        }

        private void BtnExecutePrescription_Click(object? sender, EventArgs e)
        {
            if (_selectedRecord == null) return;

            if (_dgvPrescriptions.SelectedRows.Count == 0)
            {
                MessageBox.Show(this, "Выберите назначение из таблицы", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_dgvPrescriptions.SelectedRows[0].Tag is not Prescription prescription) return;

            if (prescription.IsCompleted)
            {
                MessageBox.Show(this, "Это назначение уже выполнено!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using var dlg = new ExecuteForm(service, _selectedRecord, prescription);
            if (dlg.ShowDialog(this) == DialogResult.OK)
            {
                ShowMedicalRecord(_selectedRecord);
            }
        }

        private void BtnDischarge_Click(object? sender, EventArgs e)
        {
            if (_selectedRecord == null || _selectedRecord.Status != "На лечении") return;

            using var dlg = new DischargeForm(_selectedRecord.AdmissionDiagnosis);
            if (dlg.ShowDialog(this) != DialogResult.OK) return;

            if (service.DischargePatient(_selectedRecord.Id, dlg.FinalDiagnosis, dlg.Recommendations, out var epicrisis, out string error))
            {
                LoadTree();
                ShowMedicalRecord(_selectedRecord);

                // Показываем выписной эпикриз
                if (epicrisis != null)
                {
                    using var epDlg = new EpicrisisForm(epicrisis);
                    epDlg.ShowDialog(this);
                }
            }
            else
            {
                MessageBox.Show(this, error, "Ошибка выписки", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
