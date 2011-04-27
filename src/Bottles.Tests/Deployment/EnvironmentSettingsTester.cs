using Bottles.Configuration;
using Bottles.Deployment;
using FubuCore.Configuration;
using NUnit.Framework;
using FubuTestingSupport;

namespace Bottles.Tests.Deployment
{
    [TestFixture]
    public class EnvironmentSettingsTester
    {
        private EnvironmentSettings theEnvironmentSettings;

        [SetUp]
        public void SetUp()
        {
            theEnvironmentSettings = new EnvironmentSettings();
        }

        [Test]
        public void read_text_with_no_equals()
        {
            Exception<EnvironmentSettingsException>.ShouldBeThrownBy(() =>
            {
                theEnvironmentSettings.ReadText("something");
            });
            
        }

        [Test]
        public void read_text_with_too_many_equals()
        {
            Exception<EnvironmentSettingsException>.ShouldBeThrownBy(() =>
            {
                theEnvironmentSettings.ReadText("something=else=more");
            });

        }

        [Test]
        public void read_text_with_equals_and_only_one_dot()
        {
            Exception<EnvironmentSettingsException>.ShouldBeThrownBy(() =>
            {
                theEnvironmentSettings.ReadText("arg.1=value");
            });
        }

        [Test]
        public void read_simple_value()
        {
            theEnvironmentSettings.ReadText("A=B");
            theEnvironmentSettings.ReadText("C=D");

            theEnvironmentSettings.Overrides["A"].ShouldEqual("B");
            theEnvironmentSettings.Overrides["C"].ShouldEqual("D");
        }

        [Test]
        public void read_host_directive()
        {
            theEnvironmentSettings.ReadText("Host1.OneDirective.Name=Jeremy");
            theEnvironmentSettings.ReadText("Host1.OneDirective.Age=45");
            theEnvironmentSettings.ReadText("Host2.OneDirective.Name=Tom");

            theEnvironmentSettings.DataForHost("Host1").Get("OneDirective.Name").ShouldEqual("Jeremy");
            theEnvironmentSettings.DataForHost("Host2").Get("OneDirective.Name").ShouldEqual("Tom");
            theEnvironmentSettings.DataForHost("Host1").Get("OneDirective.Age").ShouldEqual("45");
        }

        [Test]
        public void the_environment_settings_are_categorized_as_environment()
        {
            theEnvironmentSettings.DataForHost("Host1").Category.ShouldEqual(SettingCategory.environment);
            theEnvironmentSettings.DataForHost("Host2").Category.ShouldEqual(SettingCategory.environment);
        }
    }
}