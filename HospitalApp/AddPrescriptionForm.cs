using System;
using System.Drawing;
using System.Windows.Forms;
using BusinessObjects;
using ServiceLayer;

namespace HospitalApp
{
    // Форма добавления назначения
    public class AddPrescriptionForm : Form
    {
        private readonly IService _service;
        private readonly MedicalRecord _record;

        private ComboBox _cmbType = null!;
        private TextBox _txtName = null!;
        private TextBox _txtDetails = null!;
        private ComboBox _cmbDoctor = null!;
        private Label _lblHint = null!;

        public AddPrescriptionForm(IService service, MedicalRecord record)
        {
            _service = service;
            _record = record;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = $"Назначение лечения — {_record.Patient.FullName}";
            Size = new Size(500, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            int y = 20;

            var lbl1 = new Label { Text = "Тип назначения:*", Location = new Point(20, y + 3), Width = 150 };
            _cmbType = new ComboBox { Location = new Point(180, y), Width = 270, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbType.Items.AddRange(new object[] {
                "Медикаментозное (таблетки)",
                "Медикаментозное (инъекции)",
                "Диагностическое",
                "Профилактическое"
            });
            _cmbType.SelectedIndex = 0;
            _cmbType.SelectedIndexChanged += CmbType_SelectedIndexChanged;
            Controls.AddRange(new Control[] { lbl1, _cmbType });

            y += 40;
            var lbl2 = new Label { Text = "Название:*", Location = new Point(20, y + 3), Width = 150 };
            _txtName = new TextBox { Location = new Point(180, y), Width = 270, Text = "Аспирин Кардио" };
            Controls.AddRange(new Control[] { lbl2, _txtName });

            y += 40;
            var lbl3 = new Label { Text = "Параметры / дозировка:*", Location = new Point(20, y + 3), Width = 150 };
            _txtDetails = new TextBox { Location = new Point(180, y), Width = 270, Text = "100 мг, 1 раз в день, 10 дней" };
            Controls.AddRange(new Control[] { lbl3, _txtDetails });

            y += 40;
            var lbl4 = new Label { Text = "Назначил врач:*", Location = new Point(20, y + 3), Width = 150 };
            _cmbDoctor = new ComboBox { Location = new Point(180, y), Width = 270, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.AddRange(new Control[] { lbl4, _cmbDoctor });

            y += 40;
            _lblHint = new Label
            {
                Text = "Примечание: Медикаментозные назначения осуществляет медсестра.",
                Location = new Point(20, y),
                Width = 430,
                Height = 35,
                ForeColor = Color.DarkBlue
            };
            Controls.Add(_lblHint);

            y += 45;
            var btnSave = new Button { Text = "Назначить", Location = new Point(180, y), Width = 130, Height = 32 };
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button { Text = "Отмена", Location = new Point(320, y), Width = 130, Height = 32 };
            btnCancel.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private void LoadData()
        {
            _cmbDoctor.Items.Clear();
            foreach (var d in _service.GetDoctors())
            {
                _cmbDoctor.Items.Add(d.FullName);
            }

            int idx = _cmbDoctor.Items.IndexOf(_record.AttendingDoctor);
            _cmbDoctor.SelectedIndex = idx >= 0 ? idx : 0;
        }

        private void CmbType_SelectedIndexChanged(object? sender, EventArgs e)
        {
            string t = _cmbType.SelectedItem?.ToString() ?? "";
            if (t.StartsWith("Медикаментозное (таблетки)"))
            {
                _txtName.Text = "Аспирин Кардио";
                _txtDetails.Text = "100 мг, 1 раз в день, 10 дней";
                _lblHint.Text = "Примечание: Медикаментозные назначения осуществляет медсестра.";
                _lblHint.ForeColor = Color.DarkBlue;
            }
            else if (t.StartsWith("Медикаментозное (инъекции)"))
            {
                _txtName.Text = "Цефтриаксон";
                _txtDetails.Text = "5 инъекций (по 1.0 г в/м)";
                _lblHint.Text = "Примечание: Медикаментозные назначения осуществляет медсестра.";
                _lblHint.ForeColor = Color.DarkBlue;
            }
            else if (t == "Диагностическое")
            {
                _txtName.Text = "УЗИ брюшной полости";
                _txtDetails.Text = "Печень, желчный пузырь, поджелудочная";
                _lblHint.Text = "Примечание: Диагностические назначения осуществляет врач.";
                _lblHint.ForeColor = Color.DarkGreen;
            }
            else if (t == "Профилактическое")
            {
                _txtName.Text = "Лечебный массаж спины";
                _txtDetails.Text = "10 сеансов по 30 минут";
                _lblHint.Text = "Примечание: Профилактические назначения осуществляет врач.";
                _lblHint.ForeColor = Color.DarkOrange;
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            var p = new Prescription
            {
                Type = _cmbType.SelectedItem?.ToString() ?? "",
                Name = _txtName.Text.Trim(),
                Details = _txtDetails.Text.Trim(),
                PrescribedBy = _cmbDoctor.SelectedItem?.ToString() ?? "",
                PrescribedDate = DateTime.Now
            };

            if (_service.AddPrescription(_record.Id, p, out string error))
            {
                MessageBox.Show(this, "Назначение успешно внесено в ЭМК!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show(this, error, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
