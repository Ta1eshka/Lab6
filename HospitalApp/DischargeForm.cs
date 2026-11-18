using System.Drawing;
using System.Windows.Forms;

namespace HospitalApp
{
    // Простая форма ввода данных для выписки
    public class DischargeForm : Form
    {
        private TextBox _txtFinalDiag = null!;
        private TextBox _txtRecs = null!;

        public string FinalDiagnosis => _txtFinalDiag.Text.Trim();
        public string Recommendations => _txtRecs.Text.Trim();

        public DischargeForm(string admissionDiagnosis)
        {
            Text = "Выписка пациента";
            Size = new Size(450, 260);
            StartPosition = FormStartPosition.CenterParent;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;

            var lbl1 = new Label { Text = "Заключительный клинический диагноз:*", Location = new Point(20, 15), Width = 380 };
            _txtFinalDiag = new TextBox { Location = new Point(20, 40), Width = 390, Text = admissionDiagnosis };

            var lbl2 = new Label { Text = "Рекомендации при выписке:", Location = new Point(20, 75), Width = 380 };
            _txtRecs = new TextBox
            {
                Location = new Point(20, 100),
                Width = 390,
                Height = 50,
                Multiline = true,
                Text = "Диспансерное наблюдение терапевта по месту жительства."
            };

            var btnOk = new Button { Text = "Выписать", Location = new Point(200, 170), Width = 100, Height = 30 };
            btnOk.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(_txtFinalDiag.Text))
                {
                    MessageBox.Show(this, "Укажите заключительный диагноз!", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
                DialogResult = DialogResult.OK;
                Close();
            };

            var btnCancel = new Button { Text = "Отмена", Location = new Point(310, 170), Width = 100, Height = 30 };
            btnCancel.Click += (s, e) => Close();

            Controls.AddRange(new Control[] { lbl1, _txtFinalDiag, lbl2, _txtRecs, btnOk, btnCancel });
        }
    }
}
