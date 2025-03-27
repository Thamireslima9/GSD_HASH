using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Net.Mail;
using System.Net;
using System.Runtime.Remoting.Contexts;

namespace GSD_HASH
{
            public partial class Form1 : Form
            {
     

        public Form1()
                {
                    InitializeComponent();
                }

                private void Form1_Load(object sender, EventArgs e)
                {
                    this.acessosTableAdapter.Fill(this.tRABALHOHASHcj3022099DataSet.Acessos);
                    informacoesSMTP();

                }

                private void informacoesSMTP()
                {
                    txtEnderecoSMTP.Text = Properties.Settings.Default.enderecoSMTP;
                    nupPortaSMTP.Value = Convert.ToInt32(Properties.Settings.Default.portaSMTP);
                    txtEmailUsuarioSMTP.Text = Properties.Settings.Default.emailSMTP;
                    txtSenhaEmailSMTP.Text = Properties.Settings.Default.senhaSMTP;
                }

                private void acessosBindingSource_CurrentChanged(object sender, EventArgs e)
                {

                }

                private void bindingNavigator1_RefreshItems(object sender, EventArgs e)
                {

                }

                private void label4_Click(object sender, EventArgs e)
                {

                }

                private void tabPage4_Click(object sender, EventArgs e)
                {


                }


                private void btnLogin_Click(object sender, EventArgs e)
                {
                    //variaveis locais para tratar o usuario e a senha
                    string usuario = txtUsuarioLogin.Text;
                    string senha = Crypto.sha256encrypt(txtSenhaLogin.Text);

                    //percorre cada tabela do banco de dados
                    foreach (DataRow row in tRABALHOHASHcj3022099DataSet.Acessos)
                    {
                        //e verifica pelo usuário e senha que coincidem
                        if (row.ItemArray[1].Equals(usuario) && row.ItemArray[2].Equals(senha))
                        {
                            txtUsuarioLogin.Text = String.Empty;
                            txtSenhaLogin.Text = String.Empty;
                            MessageBox.Show("Login realizado com sucesso !");
                            break;
                        }
                        //Se não achar então
                        else
                        {
                            MessageBox.Show("Usuário/Senha incorretos");
                            break;
                        }
                    }
                }

                private void btnRegistrar_Click(object sender, EventArgs e)
                {
                    AdicionarUsuario(txtUsuario.Text, txtSenha.Text, txtConfirmaSenha.Text, txtEmail.Text);
                }

                private void AdicionarUsuario(string text1, string text2, object text3, string text4)
                {
                    throw new NotImplementedException();
                }

                //private void AdicionarUsuario(string _nomeUsuario, string _senha, string _confirmaSenha, string _email)
                //{
                //}


                private void AdicionarUsuario(string _nomeUsuario, string _senha, string _confirmaSenha, string _email)
                {
                    //variaveis locais para tratar os valores
                    string smtpEmail = txtEmailUsuarioSMTP.Text;
                    string smtpPassword = txtSenhaEmailSMTP.Text;
                    int smtpPorta = (int)nupPortaSMTP.Value;
                    string smtpAddress = txtEnderecoSMTP.Text;

                    //Regex para validar o email
                    Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
                    Match match = regex.Match(_email);

                    //Percorre as tabelas do banco de dados
                    foreach (DataRow row in tRABALHOHASHcj3022099DataSet.Acessos)
                    {
                        //E procura por nomes de usuários existentes
                        if (row.ItemArray[1].Equals(_nomeUsuario))
                        {
                            //Se achar um então avisa
                            MessageBox.Show("O nome do usuário já existe, tente informar outro nome.");
                            return;
                        }
                    }
                    //Confirma a senha
                    if (_senha != _confirmaSenha)
                    {
                        MessageBox.Show("A senha não confere.");
                    }
                    // A senha tem que ter no minimo 8 caracteres
                    else if (_senha.Length < 8)
                    {
                        MessageBox.Show("A senha deve conter no mínimo 8 caracteres");
                    }
                    //Se o email não for válido
                    else if (!match.Success)
                    {
                        MessageBox.Show("Email inválido");
                    }
                    //Se não informou o usuário
                    else if (_nomeUsuario == null)
                    {
                        MessageBox.Show("VOcê deve informar um usuário");
                    }
                    //Se estiver tudo certo então cria o usuário
                    else
                    {
                        string _hashSenha = Crypto.sha256encrypt(_senha);
                        AdicionaUsuarioNoBD(_nomeUsuario, _hashSenha, _email);

                        txtUsuario.Text = String.Empty;
                        txtSenha.Text = String.Empty;
                        txtConfirmaSenha.Text = String.Empty;
                        txtEmail.Text = String.Empty;

                        MessageBox.Show("Obrigado por seu registro !");

                        if (String.IsNullOrWhiteSpace(smtpEmail) || String.IsNullOrWhiteSpace(smtpPassword) ||
                        String.IsNullOrWhiteSpace(smtpAddress) || smtpPorta <= 0)
                        {
                            MessageBox.Show("A configuração do Email não foi definida corretamente! \nNão é possível enviar emails!");
                        }
                        else
                        {
                            EnviaMensagem(_email.ToString(), _nomeUsuario.ToString(), _senha.ToString());
                        }
                    }
                }

                //private void AdicionaUsuarioNoBD(string nomeUsuario, string hashSenha, string email)
                //{
                //    throw new NotImplementedException();
                //}

                private void AdicionaUsuarioNoBD(string _nomeUsuario, string _senha, string _email)
                {
                    string ConnectString = Properties.Settings.Default.LoginsConnectionString;
        SqlCeConnection cn = new SqlCeConnection(ConnectString);
        if (cn.State == ConnectionState.Closed)
        {
            cn.Open();
        }
        SqlCeCommand cmd;
        string sql = "insert into Acessos "
                   + "(usuario, senha, email) "
                   + "values (@usuario, @senha, @email)";
                try
                {
                    cmd = new SqlCeCommand(sql, cn);
                    // Forma correta para SQL Compact Edition (SqlCeCommand)
                    cmd.Parameters.AddWithValue("@usuario", _nomeUsuario);
                    cmd.Parameters.AddWithValue("@senha", _senha);
                    cmd.Parameters.AddWithValue("@email", _email);
                    cmd.ExecuteNonQuery();
                    MessageBox.Show("Usuario incluído.");
                }
                catch (SqlCeException sqlexception)
                {
                    MessageBox.Show(sqlexception.Message, "Arre.", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Arre Égua...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    cn.Close();
                }
                }

                public void EnviaMensagem(string ParaEndereco, string ParaNome, string _senha)
                {
                    var cliente = new SmtpClient(txtEnderecoSMTP.Text, (int)nupPortaSMTP.Value)
                    {
                        Credentials = new NetworkCredential(txtEmailUsuarioSMTP.Text, txtSenhaEmailSMTP.Text),
                        EnableSsl = true
                    };
                    cliente.Send(txtEmailUsuarioSMTP.Text, ParaEndereco, "Obrigado !", "Obrigado por seu registro ! \n Seu usuário/senha são: \n \nUsuário: "
                             + ParaNome.ToString() + "\nSenha: " + _senha.ToString());
                }
            }
         }

