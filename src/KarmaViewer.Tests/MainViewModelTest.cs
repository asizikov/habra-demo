using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;
using KarmaViewer.Domain;
using KarmaViewer.Networking;
using KarmaViewer.ViewModels;
using Microsoft.Reactive.Testing;
using Moq;
using NUnit.Framework;

namespace KarmaViewer.Tests
{
    [TestFixture]
    public class MainViewModelTest : ReactiveTest
    {
        private Subject<KarmaModel> KarmaStream { get; set; }
        private const string USER_NAME = "userName";
        private Mock<IHabraClient> Client { get; set; }

        [SetUp]
        public void SetUp()
        {
            Client = new Mock<IHabraClient>();
            KarmaStream = new Subject<KarmaModel>();
        }

        [Test]
        public void Test()
        {
            Client.Setup(client => client.GetKarmaForUser(USER_NAME)).Returns(KarmaStream);
            var viewModel = new MainViewModel(Client.Object, USER_NAME);

            Assert.IsTrue(viewModel.IsLoading);
            KarmaStream.OnNext(new KarmaModel {Karma = 10});
            Assert.IsTrue(viewModel.IsLoading);
            Assert.AreEqual(10, viewModel.Karma);
            KarmaStream.OnCompleted();
            Assert.IsFalse(viewModel.IsLoading);
        }
    }
}