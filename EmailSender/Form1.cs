using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Gmail.v1.Data;


namespace EmailSender
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            EnviarEmail();
        }
        public static string Base64UrlEncode(string input)
        {
            var inputBytes = System.Text.Encoding.UTF8.GetBytes(input);
            return Convert.ToBase64String(inputBytes)
              .Replace('+', '-')
              .Replace('/', '_')
              .Replace("=", "");
        }
        public bool EnviarEmail()
        {
            try
            {
                // Autenticação
                UserCredential credential;
                using (var stream = new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
                {
                    string credPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    credPath = Path.Combine(credPath, ".credentials/gmail-dotnet-quickstart.json");

                    credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                        GoogleClientSecrets.FromStream(stream).Secrets,
                        new[] { GmailService.Scope.GmailSend },
                        "user",
                        CancellationToken.None,
                        new FileDataStore(credPath, true)).Result;
                }

                // Criação do serviço
                var service = new GmailService(new BaseClientService.Initializer()
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "Gmail API"
                });

                // Criação da mensagem
                var email = txtEmail.Text;
                var assunto = txtAssunto.Text;
                var msg = txtMsg.Text;
                var message = new Google.Apis.Gmail.v1.Data.Message();
                message.Raw = Base64UrlEncode("From: damazio.store@gmail.com\r\n" +
                                              $"To: {email}\r\n" +
                                              $"Subject: {assunto}\r\n\r\n" +
                                              $"{msg}");

                // Envio da mensagem
                service.Users.Messages.Send(message, "me").Execute();
                MessageBox.Show("E-mail enviado com sucesso");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Email não foi enviado");
                return false;
            }
        }
    }
}