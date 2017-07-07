using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using SecretSharingLogic;
using ShamirGUI.Annotations;

namespace ShamirGUI
{
    public class SimViewModel : INotifyPropertyChanged
    {
        private TSSAlgorithm _algorithm = new TSSAlgorithm();

        private string _phrase;
        private int _threshold;
        private ObservableCollection<ShareName> _shares;
        private int _numberOfShares;
        private string _inputShares;
        private string _recoveredSecret;

        public string Phrase
        {
            get { return _phrase; }
            set
            {
                if (value == _phrase) return;
                _phrase = value;
                OnPropertyChanged();
            }
        }

        public string RecoveredSecret
        {
            get { return _recoveredSecret; }
            set
            {
                if (value == _recoveredSecret) return;
                _recoveredSecret = value;
                OnPropertyChanged();
            }
        }

        public int NumberOfShares
        {
            get { return _numberOfShares; }
            set
            {
                if (value == _numberOfShares) return;
                _numberOfShares = value;
                OnPropertyChanged();
            }
        }
        
        public int Threshold
        {
            get { return _threshold; }
            set
            {
                if (value == _threshold) return;
                _threshold = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<ShareName> Shares {
            get { return _shares; }

            set
            {
                if (value == _shares) return;
                _shares = value;
                OnPropertyChanged();
            }
        }

        public string InputShares
        {
            get { return _inputShares; }
            set
            {
                if (value == _inputShares) return;
                _inputShares = value;
                OnPropertyChanged();
            }
        }

        public SimViewModel()
        {
            Phrase = "Hello World!";
            NumberOfShares = 3;
            Threshold = 2;
            Shares = new ObservableCollection<ShareName>();

        }

        public void Clear()
        {
            _algorithm = new TSSAlgorithm();
            Phrase = String.Empty;
            NumberOfShares = 1;
            Threshold = 1;
            Shares = new ObservableCollection<ShareName>();
            InputShares = String.Empty;
            RecoveredSecret = String.Empty;
            Phrase = "";
        }

        public void CreateShares()
        {
            string [] shares = _algorithm.Share(Phrase, Threshold, NumberOfShares);
            Shares.Clear();
            foreach (var sh in shares)
            {
                ShareName shareName =  new ShareName(sh);
                Shares.Add(shareName);
            }         
        }

        public bool RecoverSecret()
        {
            if (InputShares == null)
                return false;

            string [] parsedStrings = InputShares.Split(new [] {Environment.NewLine}, StringSplitOptions.RemoveEmptyEntries);
            if (parsedStrings.Count() < Threshold)
                return false;

          
            RecoveredSecret secret = _algorithm.Recover(parsedStrings.Take(Threshold));
            if (secret == null)
            {
                return false;
            }

            RecoveredSecret = secret.RecoveredText;
            return true;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }        
    }

    public class ShareName
    {
        public string DisplayName { get; set; }

        public ShareName(string name)
        {
            DisplayName = name;
        }
    }
}
