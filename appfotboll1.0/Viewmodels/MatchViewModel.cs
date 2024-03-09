namespace appfotboll5
{
    public class MatchViewModel : ViewModelBase
    {
        private int matchID;
        private string result;

        public int MatchID
        {
            get { return matchID; }
            set
            {
                if (matchID != value)
                {
                    matchID = value;
                    OnPropertyChanged(nameof(MatchID));
                }
            }
        }

        public string Result
        {
            get { return result; }
            set
            {
                if (result != value)
                {
                    result = value;
                    OnPropertyChanged(nameof(Result));
                }
            }
        }

        // Constructor
        public MatchViewModel()
        {
            // Initialization if needed
        }
    }
}
