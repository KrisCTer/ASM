using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace ASM
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            //richTextBox.KeyDown += codeTextBox_KeyDown;

        }
        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var newForm = new Form1();
            newForm.Show();
        }

        private void openFileCtrlOToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "CPP Files |*.cpp";
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                MessageBox.Show("Open file successfully!");
                string fileContent = File.ReadAllText(openFileDialog.FileName);
                richTextBox.Text = fileContent;

            }
        }

        private void saveFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var sdlg = new SaveFileDialog();

            sdlg.Filter = "C++ Files (*.cpp)|*.cpp|Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            sdlg.Title = "Save File";
            sdlg.DefaultExt = sdlg.FileName;
            sdlg.AddExtension = true;

            if (sdlg.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    File.WriteAllText(sdlg.FileName, richTextBox.Text);
                    MessageBox.Show("Luu thanh cong!");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
        private void exitFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show(
           "Do you want to exit?",
           "Exit",
           MessageBoxButtons.YesNo,
           MessageBoxIcon.Question
           );
            if (result == DialogResult.Yes)
            {
                this.Close();
            }
        }

        private void richTextBox_TextChanged(object sender, EventArgs e)
        {
            // Tập hợp từ khóa C và C++
            var keyWords = new HashSet<string>(new[]
            {
        // Từ khóa C
        "auto", "break", "case", "char", "const", "continue", "default", "do", "double",
        "else", "enum", "extern", "float", "for", "goto", "if", "int", "long", "register",
        "return", "short", "signed", "sizeof", "static", "struct", "switch", "typedef",
        "union", "unsigned", "void", "volatile", "while",
        
        // Từ khóa C++
        "alignas", "alignof", "and", "and_eq", "asm", "atomic_cancel", "atomic_commit",
        "atomic_noexcept", "bitand", "bitor", "bool", "catch", "char16_t", "char32_t",
        "class", "compl", "concept", "const_cast", "constexpr", "constinit", "consteval",
        "co_await", "co_return", "co_yield", "decltype", "delete", "dynamic_cast", "explicit",
        "export", "false", "friend", "inline", "mutable", "namespace", "new", "noexcept",
        "not", "not_eq", "nullptr", "operator", "or", "or_eq", "private", "protected",
        "public", "reflexpr", "reinterpret_cast", "requires", "static_assert", "static_cast",
        "template", "this", "thread_local", "throw", "true", "try", "typeid", "typename",
        "using", "virtual", "wchar_t", "xor", "xor_eq"
    });

            // Gọi hàm tô màu
            HighlightText(keyWords, Color.Blue, Color.Brown, Color.Green);
            UpdateLineNumbers(sender, e);
        }

        private void HighlightText(HashSet<string> keywords, Color keywordColor, Color quotesColor, Color commentColor)
        {
            string text = richTextBox.Text;
            int cursorPosition = richTextBox.SelectionStart;

            // Reset màu sắc toàn bộ văn bản
            richTextBox.Select(0, text.Length);
            richTextBox.SelectionColor = Color.Black;

            // Sử dụng Regex để tô màu keywords
            var keywordRegex = new Regex(@"\b(" + string.Join("|", keywords.Select(Regex.Escape)) + @")\b", RegexOptions.IgnoreCase);
            foreach (Match match in keywordRegex.Matches(text))
            {
                richTextBox.Select(match.Index, match.Length);
                richTextBox.SelectionColor = keywordColor;
            }

            // Tô màu chuỗi (nội dung trong dấu ngoặc kép)
            var quotesRegex = new Regex("\"(.*?)\"");
            foreach (Match match in quotesRegex.Matches(text))
            {
                richTextBox.Select(match.Index, match.Length);
                richTextBox.SelectionColor = quotesColor;
            }

            // Tô màu comment đơn dòng (// ...)
            var singleLineCommentRegex = new Regex("//.*?$", RegexOptions.Multiline);
            foreach (Match match in singleLineCommentRegex.Matches(text))
            {
                richTextBox.Select(match.Index, match.Length);
                richTextBox.SelectionColor = commentColor;
            }

            // Tô màu comment khối (/* ... */)
            var multiLineCommentRegex = new Regex(@"/\*.*?\*/", RegexOptions.Singleline);
            foreach (Match match in multiLineCommentRegex.Matches(text))
            {
                richTextBox.Select(match.Index, match.Length);
                richTextBox.SelectionColor = commentColor;
            }

            // Khôi phục vị trí con trỏ
            richTextBox.SelectionStart = cursorPosition;
            richTextBox.SelectionLength = 0;
            richTextBox.SelectionColor = richTextBox.ForeColor;
        }
        //private void codeTextBox_KeyDown(object sender, KeyEventArgs e)
        //{
        //    if (e.KeyCode == Keys.Enter)
        //    {
        //        e.SuppressKeyPress = true; // Ngăn Enter mặc định
        //        int cursorPosition = richTextBox.SelectionStart;

        //        // Xác định dòng hiện tại
        //        int currentLineIndex = richTextBox.GetLineFromCharIndex(cursorPosition);
        //        string currentLine = currentLineIndex > 0 ? richTextBox.Lines[currentLineIndex - 1] : "";

        //        // Đếm số tab ở dòng trước đó
        //        int tabCount = 0;
        //        foreach (char c in currentLine)
        //        {
        //            if (c == '\t') tabCount++;
        //            else break;
        //        }

        //        // Thêm tab nếu dòng trước chứa ngoặc mở chưa đóng
        //        if (currentLine.TrimEnd().EndsWith("{") || currentLine.TrimEnd().EndsWith("("))
        //        {
        //            tabCount++;
        //        }

        //        // Chèn dòng mới với tab tương ứng
        //        string newLine = Environment.NewLine + new string('\t', tabCount);
        //        richTextBox.SelectedText = newLine;

        //        // Đặt con trỏ sau dòng mới
        //        richTextBox.SelectionStart = cursorPosition + newLine.Length;
        //    }
        //    else if (e.KeyCode == Keys.OemCloseBrackets || e.KeyCode == Keys.Oem6) // Xử lý ngoặc đóng '}'
        //    {
        //        int cursorPosition = richTextBox.SelectionStart;
        //        int currentLineIndex = richTextBox.GetLineFromCharIndex(cursorPosition);
        //        string currentLine = currentLineIndex >= 0 ? richTextBox.Lines[currentLineIndex] : "";

        //        // Nếu dòng hiện tại chỉ có tab, giảm bớt một tab
        //        if (currentLine.Trim().Length == 0 && currentLine.StartsWith("\t"))
        //        {
        //            int lineStartIndex = richTextBox.GetFirstCharIndexFromLine(currentLineIndex);
        //            richTextBox.SelectionStart = lineStartIndex;
        //            richTextBox.SelectionLength = 1;
        //            richTextBox.SelectedText = ""; // Xóa một tab
        //        }
        //    }
        //}

        private void lineNumber_Paint(object sender, PaintEventArgs e)
        {
            e.Graphics.Clear(Color.LightGray);
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            int firstVisibleLine = richTextBox.GetLineFromCharIndex(richTextBox.GetCharIndexFromPosition(new Point(0, 0)));
            int lastVisibleLine = richTextBox.GetLineFromCharIndex(richTextBox.GetCharIndexFromPosition(new Point(0, richTextBox.Height)));

            for (int i = firstVisibleLine; i <= lastVisibleLine + 1; i++)
            {
                int y = richTextBox.GetPositionFromCharIndex(richTextBox.GetFirstCharIndexFromLine(i)).Y - richTextBox.GetPositionFromCharIndex(richTextBox.GetFirstCharIndexFromLine(firstVisibleLine)).Y;
                if (y >= 0 && y < panelNumber.Height)
                {

                    e.Graphics.DrawString((i + 1).ToString(), new Font("Cascadia Mono", 11), Brushes.Black, new PointF(5, y));
                }
            }
        }
        private void UpdateLineNumbers(object sender, EventArgs e)
        {
            panelNumber.Invalidate();
        }
        private void compile()
        {
            string gppPath = @"C:\MinGW\bin\gcc.exe";
            string cppCode = richTextBox.Text;
            string path = "C:\\Users\\phucl\\Downloads\\ASM\\Save";
            if (!cppCode.EndsWith(Environment.NewLine))
            {
                cppCode += Environment.NewLine;
            }
            string tempFilePath = Path.Combine(path, "program.cpp");
            File.WriteAllText(tempFilePath, cppCode);
            if (!File.Exists(gppPath))
            {

                MessageBox.Show("Compiler not found at: " + gppPath);
                return;
            }

            string exeFilePath = Path.Combine(path, "program.exe");
            var compileProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = gppPath,
                    Arguments = $"-o \"{exeFilePath}\" \"{tempFilePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            compileProcess.Start();
            string compileOutput = compileProcess.StandardOutput.ReadToEnd();
            string compileError = compileProcess.StandardError.ReadToEnd();
            compileProcess.WaitForExit();
            if (!string.IsNullOrEmpty(compileError))
            {
                status.ForeColor = Color.Red;
                status.Text = $"Compile Error:\n{compileError}";
                return;
            }
            else
            {
                status.ForeColor = Color.Green;
                status.Text = "Compilation succeeded";
                return;
            }

        }
        private void runFile()
        {
            string path = "C:\\Users\\phucl\\Downloads\\ASM\\Save";
            string exeFilePath = Path.Combine(path, "program.exe");
            if (!File.Exists(exeFilePath))
            {
                MessageBox.Show("Executable file not found: " + exeFilePath);
                return;
            }

            var runProcess = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = exeFilePath,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            runProcess.Start();
            string programOutput = runProcess.StandardOutput.ReadToEnd();
            string programError = runProcess.StandardError.ReadToEnd();
            runProcess.WaitForExit();

            status.Text = !string.IsNullOrEmpty(programError)
                ? $"Runtime Error:\n{programError}"
                : programOutput;
        }

        private void compileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compile();
        }

        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            compile();
            if (status.Text.Contains("Compilation succeeded"))
            {
                runFile();
            }
        }

    }
}


