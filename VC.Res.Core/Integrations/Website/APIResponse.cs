namespace VC.Res.Core.Integrations.Website
{
    public class APIResponse<T>
    {
        public bool Success { get; set; } = false;

        public bool Unauthorised { get; set; } = false;

        public bool Error { get; set; } = false;

        public string Error_Message { get; set; } = "We're having trouble contacting our servers, please try again later."; // default generic error

        public T? Result { get; set; }

        public async Task ProcessError(HttpResponseMessage response)
        {
            if (!response.IsSuccessStatusCode)
            {
                Error = true;
                Error_Message = "Error communicating with website."; // generic error

                if (!string.IsNullOrWhiteSpace(response.ReasonPhrase))
                {
                    Error_Message = response.ReasonPhrase;
                }

                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Unauthorised = true;

                    Error_Message = "Invalid authentication credentials.";
                }

                var str = await response.Content.ReadAsStringAsync();

                if (!string.IsNullOrWhiteSpace(str))
                {
                    Error_Message = str;
                }
            }
        }
    }
}
