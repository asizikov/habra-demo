using System;
using System.Reactive.Linq;
using KarmaViewer.Domain;
using KarmaViewer.Networking;

namespace KarmaViewer.ViewModels
{
    public class MainViewModel
    {
        private IHabraClient HabraClient { get; set; }

        public MainViewModel(IHabraClient habraClient, string userName)
        {
            HabraClient = habraClient;
            Initialize(userName);
        }

        private void Initialize(string userName)
        {
            IsLoading = true;
            HabraClient.GetKarmaForUser(userName)
                .Subscribe(onNext: HandleData,
                    onError: HandleException,
                    onCompleted: () => IsLoading = false);
        }

        private void HandleException(Exception exception)
        {
            ErrorMessage = exception.Message;
            IsLoading = false;
        }

        private void HandleData(KarmaModel data)
        {
            Karma = data.Karma;
        }

        public bool IsLoading { get; set; }
        public int? Karma { get; set; }
        public string ErrorMessage { get; set; }
    }
}