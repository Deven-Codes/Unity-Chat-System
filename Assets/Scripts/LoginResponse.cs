namespace LoginSystem
{

    /// <summary>
    /// For login response recieved from the server 
    /// </summary>
    [System.Serializable]
    public class LoginResponse
    {
        public int code;
        public string msg;
        public GameAccount data;

    }
}