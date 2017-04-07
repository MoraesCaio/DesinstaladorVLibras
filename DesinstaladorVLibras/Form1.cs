using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using IWshRuntimeLibrary;

/*This program uninstalls VLibras application following six steps:
 * 1º Deletes the shortcut for VLibras Player;
 * 2º Deletes the three main folders of the application;
 * 3º Deletes environment variables created by the application;
 * 4º Deletes ClickOnce's version files;
 * 5º Deletes registry entries;
 * 6º Delete stat menu entries.
 * Author: Caio Moraes
 * Email: caiomoraes@msn.com
 * GitHub: MoraesCaio
 */
namespace DesinstaladorVLibras
{
    public partial class Form1 : Form
    {
        /*Deletes the specified user environment variable.
         * Parameter: (string) name of the variable
         */
        private static void deleteEnvVarUSR(string variable)
        {
            try
            {
                Console.WriteLine("Variável " + variable + ":");
                string t = Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User);
                if (t == null)
                {
                    Console.WriteLine(variable + " não existe.");
                }
                else
                {
                    Console.WriteLine(Environment.GetEnvironmentVariable(variable, EnvironmentVariableTarget.User));
                    Environment.SetEnvironmentVariable(variable, "", EnvironmentVariableTarget.User);
                    Console.WriteLine("Apagando.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Erro {0}", e);
            }
        }

        private static void deleteDirectory(string directory){
            ProcessStartInfo info = new ProcessStartInfo();
            info.FileName = "cmd.exe";
            info.Arguments = "/C rmdir /S /Q " + directory;
            info.WindowStyle = ProcessWindowStyle.Hidden;
            info.CreateNoWindow = true;
            Process.Start(info).WaitForExit();
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DialogResult result1 = MessageBox.Show("Isso pode levar alguns minutos. Por favor, aguarde o aviso de desinstalação concluída.", "Aviso!", MessageBoxButtons.OKCancel);

            if (result1 == DialogResult.OK){

                //VARIABLES
                string vlibrasFolderPath = Environment.GetEnvironmentVariable("PATH_VLIBRAS", EnvironmentVariableTarget.User);
                string resetEnvVarPath = Path.Combine(vlibrasFolderPath, "ResetEnvVar.exe");
                string mainFolderPath = Directory.GetParent(vlibrasFolderPath).FullName; // %LOCALAPPDATA%/VLibras/
                string startMenuFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Programs);
                string lavidStartMenuFolderPath = startMenuFolderPath + @"\LAVID-UFPB"; // startMenuFolderPath/LAVID-UFPB/


                //DELETING SHORTCUTS
                try
                {
                    object shDesktop = (object)"Desktop";
                    WshShell shell = new WshShell();

                    //Deleting shortcut on Desktop
                    string shortcutAddress = (string)shell.SpecialFolders.Item(ref shDesktop) + @"\VLibras.lnk";
                    System.IO.File.Delete(shortcutAddress);

                    //Deleting start menu folder
                    Directory.Delete(lavidStartMenuFolderPath, true);
                }
                catch (Exception ex){
                    MessageBox.Show(ex.Message,
                        "Remoção de atalho",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    return;
                }


                //DELETING ENVIRONMENT VARIABLES
                ProcessStartInfo resetInfo = new ProcessStartInfo();
                resetInfo.FileName = resetEnvVarPath;
                resetInfo.Arguments = "-u";
                resetInfo.WindowStyle = ProcessWindowStyle.Hidden;
                resetInfo.CreateNoWindow = true;
                Process.Start(resetInfo);
                Close();


                //DELETING EXTRACTED FOLDERS
                try
                {
                    deleteDirectory(mainFolderPath);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                        "Remoção de conteúdo extraído",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    return;
                }


                //DELETING CLICKONCE'S VERSIONS AND REGISTRIES
                try
                {
                    var uninstallInfo = UninstallInfo.Find("Atualizador VLibras");
                    if (uninstallInfo != null)
                    {
                        var uninstaller = new Uninstaller();
                        uninstaller.Uninstall(uninstallInfo);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message,
                        "Desinstalação do atualizador",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Exclamation,
                        MessageBoxDefaultButton.Button1);
                    return;
                }


                //UNINSTALL FINISHED
                MessageBox.Show(@"Desinstalação concluída.", @"Operação concluída.");


                //DELETE ITSELF IN 3 SECONDS
                ProcessStartInfo info = new ProcessStartInfo();
                info.FileName = @"cmd.exe";
                string dir = Directory.GetCurrentDirectory();
                info.Arguments = @"/C ping 1.1.1.1 -n 1 -w 3000 > Nul & cd .. & rmdir /s /q " + dir; // Del DesinstaladorVLibras.exe";
                info.WindowStyle = ProcessWindowStyle.Hidden;
                info.CreateNoWindow = true;
                Process.Start(info);
                Close();

            }
        }
    }
}
