using Dispatch.Helpers;

namespace Dispatch.ViewModels
{
    public class S3ConnectionViewModel : ObservableForm
    {
        private string server;
        public string Server
        {
            get
            {
                return server;
            }
            set
            {
                server = value;
                Notify();
            }
        }

        private string key;
        public string Key
        {
            get
            {
                return key;
            }
            set
            {
                key = value;
                Notify();
            }
        }

        private string secret;
        public string Secret
        {
            get
            {
                return secret;
            }
            set
            {
                secret = value;
                Notify();
            }
        }

        private string root;
        public string Root
        {
            get
            {
                return root;
            }
            set
            {
                root = value;
                Notify();
            }
        }

        internal override string GetError(string propertyName)
        {
            string errorMessage = null;

            switch (propertyName)
            {
                case "Server":
                    if (string.IsNullOrEmpty(Server))
                        errorMessage = "Server must not be empty";

                    break;

                case "Key":
                    if (string.IsNullOrEmpty(Key))
                        errorMessage = "Key must not be empty";

                    break;

                case "Secret":
                    if (string.IsNullOrEmpty(Secret))
                        errorMessage = "Secret must not be empty";

                    break;
            }

            return errorMessage;
        }
    }
}
