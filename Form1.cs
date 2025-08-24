using Microsoft.Toolkit.Uwp.Notifications;
using Microsoft.Win32;
using System;
using System.Windows.Forms;

namespace WaterDrinkSolution
{
    public partial class Form1 : Form
    {
        DateTime _ultimaNotificacao;
        public Form1()
        {
            InitializeComponent();

            // Define valor padrão do NumericUpDown
            nIntervalo.Minimum = 1;  //Intervalo mínimo: 1 Min
            nIntervalo.Maximum = 60; // Intervalo máximo: 1 hora
            nIntervalo.Value = 5;    // Padrão: 5 minutos

            this.Resize += Form1_Resize; // liga o evento Resize ao método
            notifyIcon1.MouseDoubleClick += notifyIcon1_MouseDoubleClick; // liga duplo clique no ícone

            // Liga o menu de contexto
            notifyIcon1.ContextMenuStrip = contextMenuStrip1;

            btnParar.Visible = false;
        }
        private void Form1_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                notifyIcon1.Visible = true;   // mostra ícone na bandeja
                this.ShowInTaskbar = false;   // tira da barra de tarefas
            }
        }

        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            RestaurarJanela();
        }

        private void Timer1_Tick(object sender, EventArgs e)
        {
            int intervaloMinutos = (int)nIntervalo.Value;

            // Verifica se já passou o tempo escolhido
            if ((DateTime.Now - _ultimaNotificacao).TotalMinutes >= intervaloMinutos)
            {
                MostrarNotificacao();
                _ultimaNotificacao = DateTime.Now;
            }
        }

        private void MostrarNotificacao()
        {
            string caminhoIcone = System.IO.Path.Combine(Application.StartupPath, "img", "beberAgua.ico");
            string caminhoImagem = System.IO.Path.Combine(Application.StartupPath, "img", "beberAgua.png");

            new ToastContentBuilder()
                .AddAppLogoOverride(new Uri(caminhoIcone), ToastGenericAppLogoCrop.Circle) // Ícone pequeno ao lado do título
                .AddText("Hora de beber água!")
                .AddText("Mantenha-se hidratado!")
                .Show();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            string nomeApp = "Water Drink";
            RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", false);

            if (rk.GetValue(nomeApp) != null)
                chkIniciarComWindows.Checked = true;
            else
                chkIniciarComWindows.Checked = false;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            string nomeApp = "Water Drink";
            string caminhoApp = Application.ExecutablePath;

            RegistryKey rk = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Run", true);

            if (chkIniciarComWindows.Checked)
                rk.SetValue(nomeApp, "\"" + caminhoApp + "\""); // Adiciona ao registro
            else
                rk.DeleteValue(nomeApp, false); // Remove do registro
        }

        private void btnRegistrar_Click(object sender, EventArgs e)
        {
            // Configuração inicial do Timer
            timer1.Interval = 30000; // checa a cada 30 segundos
            timer1.Tick += Timer1_Tick;
            timer1.Start();

            _ultimaNotificacao = DateTime.Now;

            btnRegistrar.Enabled = false;
            btnParar.Visible = true;
        }

        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            RestaurarJanela();
        }

        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            Application.Exit(); // fecha o aplicativo
        }

        private void RestaurarJanela()
        {
            this.WindowState = FormWindowState.Normal; // volta a janela ao normal
            this.ShowInTaskbar = true;                 // volta a aparecer na barra de tarefas
            this.Activate();                           // traz para frente
        }

        private void btnParar_Click(object sender, EventArgs e)
        {
            btnParar.Visible = false;
            btnRegistrar.Enabled = true;

            timer1.Stop();
        }
    }
}
