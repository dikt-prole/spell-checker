using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using YandexLinguistics.NET.Speller;

namespace SpellChecker
{
    public partial class SpellCheckerForm : Form
    {
        private readonly SpellerService _speller = new SpellerService();

        public SpellCheckerForm()
        {
            InitializeComponent();

            using (var stream = File.OpenRead("spell-check.ico"))
            {
                Icon = new Icon(stream);
            }
        }

        private async void check_btn_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(text_tb.Text)) return;

            var wordsQueue = new Queue<string>(text_tb.Text.Split(' ', StringSplitOptions.RemoveEmptyEntries));

            try
            {
                var index = 0;
                while (wordsQueue.Any())
                {
                    var sb = new StringBuilder();
                    while (sb.Length < 500 && wordsQueue.Any())
                    {
                        sb.Append(wordsQueue.Dequeue());
                        sb.Append(' ');
                    }

                    var token = sb.ToString();
                    var errors = await _speller.CheckTextAsync(token);
                    foreach (var error in errors)
                    {
                        text_tb.SelectionStart = index + error.Position;
                        text_tb.SelectionLength = error.Length;
                        text_tb.SelectionBackColor = Color.Yellow;
                    }

                    index += token.Length;
                }

                MessageBox.Show("Проверка завершена", "Проверка", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception exc)
            {
                MessageBox.Show(exc.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
