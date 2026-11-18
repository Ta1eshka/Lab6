using System;
using System.Drawing;
using System.Windows.Forms;
using BusinessObjects;
using ServiceLayer;

namespace HospitalApp
{
    // Форма отметки о выполнении назначения
    public class ExecuteForm : Form
    {
        private readonly IService _service;
        private readonly MedicalRecord _record;
        private readonly Prescription _prescription;

        private ComboBox _cmbRole = null!;
        private ComboBox _cmbEmployee = null!;
        private TextBox _txtResult = null!;

        public ExecuteForm(IService service, MedicalRecord record, Prescription prescription)
        {
            _service = service;
            _record = record;
            _prescription = prescription;
            InitializeComponent();
            LoadData();
        }

        private void InitializeComponent()
        {
            Text = "Выполнение назначения";
            Size = new Size(500, 360);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            int y = 20;

            var lblInfo = new Label
            {
                Text = $"Назначение: {_prescription.Name}\nТип: {_prescription.Type}\nПараметры: {_prescription.Details}",
                Location = new Point(20, y),
                Size = new Size(440, 50),
                Font = new Font(Font, FontStyle.Bold)
            };
            Controls.Add(lblInfo);

            y += 60;
            var lbl1 = new Label { Text = "Роль исполнителя:*", Location = new Point(20, y + 3), Width = 150 };
            _cmbRole = new ComboBox { Location = new Point(180, y), Width = 270, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbRole.Items.AddRange(new object[] { "Медсестра", "Врач" });
            _cmbRole.SelectedIndexChanged += (s, e) => UpdateEmployees();
            Controls.AddRange(new Control[] { lbl1, _cmbRole });

            y += 40;
            var lbl2 = new Label { Text = "Сотрудник:*", Location = new Point(20, y + 3), Width = 150 };
            _cmbEmployee = new ComboBox { Location = new Point(180, y), Width = 270, DropDownStyle = ComboBoxStyle.DropDownList };
            Controls.AddRange(new Control[] { lbl2, _cmbEmployee });

            y += 40;
            var lbl3 = new Label { Text = "Отметка / Результат:", Location = new Point(20, y + 3), Width = 150 };
            _txtResult = new TextBox { Location = new Point(180, y), Width = 270, Text = "Выполнено успешно" };
            Controls.AddRange(new Control[] { lbl3, _txtResult });

            y += 50;
            var btnSave = new Button { Text = "Подтвердить выполнение", Location = new Point(180, y), Width = 180, Height = 32 };
            btnSave.Click += BtnSave_Click;

            var btnCancel = new Button { Text = "Отмена", Location = new Point(370, y), Width = 80, Height = 32 };
            btnCancel.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { btnSave, btnCancel });
        }

        private void LoadData()
        {
            // По умолчанию выбираем нужную роль для удобства
            if (_prescription.Type.StartsWith("Медикаментозное"))
            {
                _cmbRole.SelectedItem = "Медсестра";
            }
            else
            {
                _cmbRole.SelectedItem = "Врач";
            }
            UpdateEmployees();
        }

        private void UpdateEmployees()
        {
            _cmbEmployee.Items.Clear();
            string role = _cmbRole.SelectedItem?.ToString() ?? "";

            if (role == "Медсестра")
            {
                foreach (var n in _service.GetNurses())
                {
                    _cmbEmployee.Items.Add(n.FullName);
                }
            }
            else
            {
                foreach (var d in _service.GetDoctors())
                {
                    _cmbEmployee.Items.Add(d.FullName);
                }
            }

            if (_cmbEmployee.Items.Count > 0) _cmbEmployee.SelectedIndex = 0;
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            string role = _cmbRole.SelectedItem?.ToString() ?? "";
            string emp = _cmbEmployee.SelectedItem?.ToString() ?? "";
            string res = _txtResult.Text.Trim();

            if (_service.ExecutePrescription(_record.Id, _prescription, emp, role, res, out string error))
            {
                MessageBox.Show(this, "Отметка о выполнении успешно внесена в ЭМК!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                // Показываем сообщение об ошибке нарушения регламента
                MessageBox.Show(this, error, "Нарушение регламента выполнения", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
