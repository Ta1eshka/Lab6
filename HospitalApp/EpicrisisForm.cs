using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using BusinessObjects;

namespace HospitalApp
{
    // Форма просмотра выписного эпикриза
    public class EpicrisisForm : Form
    {
        private readonly DischargeEpicrisis _epicrisis;
        private TextBox _txtDoc = null!;

        public EpicrisisForm(DischargeEpicrisis epicrisis)
        {
            _epicrisis = epicrisis;
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            Text = $"Выписной эпикриз — {_epicrisis.Record.Patient.FullName}";
            Size = new Size(650, 550);
            StartPosition = FormStartPosition.CenterParent;

            _txtDoc = new TextBox
            {
                Multiline = true,
                ReadOnly = true,
                ScrollBars = ScrollBars.Vertical,
                Dock = DockStyle.Fill,
                Font = new Font("Courier New", 9.5f),
                Text = _epicrisis.GetText(),
                BackColor = Color.White
            };

            var pnlBottom = new Panel { Dock = DockStyle.Bottom, Height = 45 };

            var btnSave = new Button { Text = "💾 Сохранить в файл (.txt)", Location = new Point(15, 8), Width = 190, Height = 30 };
            btnSave.Click += (s, e) =>
            {
                using var sfd = new SaveFileDialog
                {
                    Filter = "Текстовые файлы (*.txt)|*.txt",
                    FileName = $"Эпикриз_{_epicrisis.Record.Patient.FullName}_{_epicrisis.Record.Id}.txt"
                };

                if (sfd.ShowDialog(this) == DialogResult.OK)
                {
                    File.WriteAllText(sfd.FileName, _txtDoc.Text);
                    MessageBox.Show(this, "Файл успешно сохранен!", "Успешно", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            };

            var btnClose = new Button { Text = "Закрыть", Location = new Point(520, 8), Width = 95, Height = 30, Anchor = AnchorStyles.Top | AnchorStyles.Right };
            btnClose.Click += (s, e) => Close();

            pnlBottom.Controls.AddRange(new Control[] { btnSave, btnClose });

            Controls.Add(_txtDoc);
            Controls.Add(pnlBottom);
        }
    }
}
