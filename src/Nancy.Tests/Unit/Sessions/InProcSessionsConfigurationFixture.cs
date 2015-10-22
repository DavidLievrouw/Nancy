namespace Nancy.Tests.Unit.Sessions
{
    using System;
    using Nancy.Cryptography;
    using Nancy.Session;
    using Xunit;

    public class InProcSessionsConfigurationFixture
    {
        private readonly InProcSessionsConfiguration config;

        public InProcSessionsConfigurationFixture()
        {
            var cryptographyConfiguration = new CryptographyConfiguration(
                new RijndaelEncryptionProvider(new PassphraseKeyGenerator("SuperSecretPass",
                    new byte[] {1, 2, 3, 4, 5, 6, 7, 8})),
                new DefaultHmacProvider(new PassphraseKeyGenerator("UberSuperSecure",
                    new byte[] {1, 2, 3, 4, 5, 6, 7, 8})));

            this.config = new InProcSessionsConfiguration()
            {
                CryptographyConfiguration = cryptographyConfiguration
            };
        }

        [Fact]
        public void Should_be_valid_with_all_properties_set()
        {
            var result = this.config.IsValid;

            result.ShouldBeTrue();
        }

        [Fact]
        public void Should_not_be_valid_with_null_identificationmethod_configuration()
        {
            this.config.SessionIdentificationMethodConfiguration = null;

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_identificationmethod()
        {
            this.config.SessionIdentificationMethodConfiguration.SessionIdentificationMethod = null;

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_cryptography_configuration()
        {
            this.config.CryptographyConfiguration = null;

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_encryption_provider()
        {
            this.config.CryptographyConfiguration = new CryptographyConfiguration(null,
                this.config.CryptographyConfiguration.HmacProvider);

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_null_hmac_provider()
        {
            this.config.CryptographyConfiguration =
                new CryptographyConfiguration(this.config.CryptographyConfiguration.EncryptionProvider, null);

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_empty_session_timeout()
        {
            this.config.SessionTimeout = TimeSpan.Zero;

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }

        [Fact]
        public void Should_not_be_valid_with_negative_session_timeout()
        {
            this.config.SessionTimeout = TimeSpan.FromSeconds(-1);

            var result = this.config.IsValid;

            result.ShouldBeFalse();
        }
    }
}