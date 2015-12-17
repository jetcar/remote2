using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Xml.Serialization;
using IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using remote;
using remote.Services;

namespace UnitTestProject1
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestActions()
        {
            var directory = new Mock<IDirectory>();
            var process = new Mock<IProcess>();
            IocKernel.registeredServices[typeof(IProcess)] = process;
            IocKernel.registeredServices[typeof(IDirectory)] = directory;
            IActions Actions = new Actions();
            Actions.ListButton();
            Assert.AreEqual(1,Actions.Explorer.SelectedIndex);
            Actions.OkButton();
            Assert.AreEqual(1, Actions.Explorer.SelectedIndex);

            Actions.OkButton();
        }

        [TestMethod]
        [DeploymentItem("XMLFile1.xml")]
        public void DeserializeXml()
        {
            var xsSubmit = new XmlSerializer(typeof(PlayerStatus));
            using (var sww = new StreamReader("XMLFile1.xml"))
            using (var xrr = XmlReader.Create(sww))
            {
                var status = (PlayerStatus)xsSubmit.Deserialize(xrr);
                Assert.AreEqual(PlayerStatus.States.paused, status.state);
                Assert.AreEqual(false, status.fullscreen);
                Assert.AreEqual(123, status.volume);
                Assert.AreEqual(0.8540027141571, status.position);
//                Assert.AreEqual(3, status.information.Count);
//                Assert.AreEqual(3, status.information.Count);

            }

        }
    }
}
