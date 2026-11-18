using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using BusinessObjects;
using ServiceLayer;

namespace HospitalApp
{
    // Форма госпитализации пациента
    public class HospitalizePatientForm : Form
    {
        private readonly IService _service;
        private ComboBox _cmbPatient = null!;
        private ComboBox _cmbDutyDoctor = null!;
        private ComboBox _cmbDepartment = null!;
        private ComboBox _cmbAttendingDoctor = null!;
        private TextBox _txtDiagnosis = null!;
        private TextBox _txtNotes = null!;

        public HospitalizePatientForm(IService service)
        {
            _service = service;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Госпитализация пациента (Первичный осмотр)";
            Size = new Size(520, 420);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            int y = 20;

            var lbl1 = new Label { Text = "Пациент:*", Location = new Point(20, y + 3), Width = 170 };
            _cmbPatient = new ComboBox { Location = new Point(200, y), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.AddRange(new Control[] { lbl1, _cmbPatient });

            y += 40;
            var lbl2 = new Label { Text = "Дежурный врач (осмотр):*", Location = new Point(20, y + 3), Width = 170 };
            _cmbDutyDoctor = new ComboBox { Location = new Point(200, y), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.AddRange(new Control[] { lbl2, _cmbDutyDoctor });

            y += 40;
            var lbl3 = new Label { Text = "Отделение:*", Location = new Point(20, y + 3), Width = 170 };
            _cmbDepartment = new ComboBox { Location = new Point(200, y), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbDepartment.SelectedIndexChanged += (s, e) => UpdateAttendingDoctors();
            Controls.AddRange(new Control[] { lbl3, _cmbDepartment });

            y += 40;
            var lbl4 = new Label { Text = "Лечащий врач в отделении:*", Location = new Point(20, y + 3), Width = 170 };
            _cmbAttendingDoctor = new ComboBox { Location = new Point(200, y), Width = 280, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.AddRange(new Control[] { lbl4, _cmbAttendingDoctor });

            y += 40;
            var lbl5 = new Label { Text = "Диагноз при поступлении:*", Location = new Point(20, y + 3), Width = 170 };
            _txtDiagnosis = new TextBox { Location = new Point(200, y), Width = 280 };
            Controls.AddRange(new Control[] { lbl5, _txtDiagnosis });

            y += 40;
            var lbl6 = new Label { Text = "Первичный осмотр (жалобы):", Location = new Point(20, y + 3), Width = 170 };
            _txtNotes = new TextBox { Location = new Point(200, y), Width = 280, Height = 55, Multiline = true };
            Controls.AddRange(new Control[] { lbl6, _txtNotes });

            y += 70;
            var btnSave = new Button { Text = "Госпитализировать", Location = new Point(200, y), Width = 150, Height = 32 };
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button { Text = "Отмена", Location = new Point(360, y), Width = 120, Height = 32 };
            btnCancel.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private void LoadData()
        {
            // Пациенты
            _cmbPatient.Items.Clear();
            foreach (var p in _service.GetPatients())
            {
                _cmbPatient.Items.Add(p);
            }
            if (_cmbPatient.Items.Count > 0) _cmbPatient.SelectedIndex = 0;

            // Дежурные врачи
            _cmbDutyDoctor.Items.Clear();
            var dutyDocs = _service.GetDutyDoctors();
            if (dutyDocs.Count == 0) dutyDocs = _service.GetDoctors();
            foreach (var d in dutyDocs)
            {
                _cmbDutyDoctor.Items.Add(d.FullName);
            }
            if (_cmbDutyDoctor.Items.Count > 0) _cmbDutyDoctor.SelectedIndex = 0;

            // Отделения
            _cmbDepartment.Items.Clear();
            foreach (var dep in _service.GetDepartments())
            {
                _cmbDepartment.Items.Add(dep.Name);
            }
            if (_cmbDepartment.Items.Count > 0) _cmbDepartment.SelectedIndex = 0;
        }

        private void UpdateAttendingDoctors()
        {
            _cmbAttendingDoctor.Items.Clear();
            string dep = _cmbDepartment.SelectedItem?.ToString() ?? "";
            var docs = _service.GetDoctorsByDepartment(dep);
            if (docs.Count == 0) docs = _service.GetDoctors();

            foreach (var d in docs)
            {
                _cmbAttendingDoctor.Items.Add(d.FullName);
            }
            if (_cmbAttendingDoctor.Items.Count > 0) _cmbAttendingDoctor.SelectedIndex = 0;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            var patient = _cmbPatient.SelectedItem as Patient;
            string duty = _cmbDutyDoctor.SelectedItem?.ToString() ?? "";
            string dep = _cmbDepartment.SelectedItem?.ToString() ?? "";
            string att = _cmbAttendingDoctor.SelectedItem?.ToString() ?? "";
            string diag = _txtDiagnosis.Text.Trim();
            string notes = _txtNotes.Text.Trim();

            if (patient == null)
            {
                MessageBox.Show(this, "Выберите пациента", "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_service.Hospitalize(patient, duty, dep, att, diag, notes, out var record, out string error))
            {
                MessageBox.Show(this, $"Пациент успешно госпитализирован!\nСоздана медицинская карта: {record?.Id}", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(this, error, "Ошибка госпитализации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
