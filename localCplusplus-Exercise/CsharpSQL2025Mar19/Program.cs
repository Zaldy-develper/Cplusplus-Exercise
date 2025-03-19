namespace WinFormsApp1SQL
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.

            ApplicationConfiguration.Initialize();

            // Show the login form as a dialog
            using (LoginForm login = new LoginForm())
            {
                if (login.ShowDialog() == DialogResult.OK)
                {
                    // Retrieve connection string from the login form if needed
                    //string fbConnectionString = login.FbConnectionString;
                    string fbConnectionString = ConnectionString.FbConnectionString;

                    // Run the main form; when it closes, the application exits 
                    Application.Run(new Form1(fbConnectionString));
                }
            }

            //Application.Run(new LoginForm());
        }
    }
}