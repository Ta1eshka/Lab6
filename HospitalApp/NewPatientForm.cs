using System;
using System.Drawing;
using System.Windows.Forms;
using BusinessObjects;
using ServiceLayer;

namespace HospitalApp
{
    // Форма добавления нового пациента
    public class NewPatientForm : Form
    {
        private readonly IService _service;
        private TextBox _txtFullName = null!;
        private NumericUpDown _nudAge = null!;
        private ComboBox _cmbGender = null!;
        private TextBox _txtPassport = null!;
        private TextBox _txtPhone = null!;

        public NewPatientForm(IService service)
        {
            _service = service;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = "Новый пациент";
            Size = new Size(400, 310);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            int y = 20;

            var lbl1 = new Label { Text = "ФИО:", Location = new Point(20, y + 3), Width = 100 };
            _txtFullName = new TextBox { Location = new Point(130, y), Width = 220 };
            Controls.AddRange(new Control[] { lbl1, _txtFullName });

            y += 40;
            var lbl2 = new Label { Text = "Возраст:", Location = new Point(20, y + 3), Width = 100 };
            _nudAge = new NumericUpDown { Location = new Point(130, y), Width = 80, Minimum = 1, Maximum = 120, Value = 35 };
            Controls.AddRange(new Control[] { lbl2, _nudAge });

            y += 40;
            var lbl3 = new Label { Text = "Пол:", Location = new Point(20, y + 3), Width = 100 };
            _cmbGender = new ComboBox { Location = new Point(130, y), Width = 120, DropDownStyle = ComboBoxStyle.DropDownList };
            _cmbGender.Items.AddRange(new object[] { "Мужской", "Женский" });
            _cmbGender.SelectedIndex = 0;
            Controls.AddRange(new Control[] { lbl3, _cmbGender });

            y += 40;
            var lbl4 = new Label { Text = "Паспорт:", Location = new Point(20, y + 3), Width = 100 };
            _txtPassport = new TextBox { Location = new Point(130, y), Width = 220 };
            Controls.AddRange(new Control[] { lbl4, _txtPassport });

            y += 40;
            var lbl5 = new Label { Text = "Телефон:", Location = new Point(20, y + 3), Width = 100 };
            _txtPhone = new TextBox { Location = new Point(130, y), Width = 220 };
            Controls.AddRange(new Control[] { lbl5, _txtPhone });

            y += 45;
            var btnSave = new Button { Text = "Сохранить", Location = new Point(130, y), Width = 110, Height = 30 };
            btnSave.Click += (s, e) =>
            {
                var patient = new Patient(
                    _txtFullName.Text.Trim(),
                    (int)_nudAge.Value,
                    _cmbGender.SelectedItem?.ToString() ?? "Мужской",
                    _txtPassport.Text.Trim(),
                    _txtPhone.Text.Trim()
                );

                if (_service.AddPatient(patient, out string error))
                {
                    MessageBox.Show(this, "Пациент успешно добавлен в базу!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MessageBox.Show(this, error, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            };

            var btnCancel = new Button { Text = "Отмена", Location = new Point(250, y), Width = 100, Height = 30 };
            btnCancel.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { btnSave, btnCancel });
        }
    }
}
