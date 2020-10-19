using System.IO;
using System.Security.Cryptography;
using System.Text;
using Xunit;

namespace IniParserLibrary.Tests
{
    /// <summary>
    /// IniParser_Tests class.
    /// </summary>
    public class IniParser_Tests
    {
        #region Constants

        const string EXAMPLE_INI_SHA512 = "82bc54b04dd2932b64418b44f9b7ba519b0f8c62735b4e2ee2730559bceb8dc1c49fbf451bcc8f5a2f4ae4eba403f48b01fff33ec2fdb3ae84ccf43c2d971ded";

        const string SECTION_1 = "Numbers";
        const string SECTION_2 = "Strings";
        const string SECTION_3 = "Images";

        const string KEY_1 = "Binary";
        const string KEY_2 = "Octal";
        const string KEY_3 = "Decimal";
        const string KEY_4 = "Hex";
        const string KEY_5 = "Text";
        const string KEY_6 = "Base64";
        const string KEY_7 = "MD5";
        const string KEY_8 = "SHA1";
        const string KEY_9 = "Image";
        const string KEY_10 = "ImageSHA512";

        const string VALUE_1 = "11001010";
        const string VALUE_2 = "312";
        const string VALUE_3 = "202";
        const string VALUE_4 = "CA";
        const string VALUE_5 = "Hello";
        const string VALUE_6 = "SGVsbG8=";
        const string VALUE_7 = "8b1a9953c4611296a827abf8c47804d7";
        const string VALUE_8 = "f7ff9e8b7bb2e09b70935a5d785e0cc5d9d0abf0";
        const string VALUE_9 = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAJcEhZcwAADsMAAA7DAcdvqGQAAAAZdEVYdFNvZnR3YXJlAHBhaW50Lm5ldCA0LjAuMjHxIGmVAAAElElEQVRYR9VXW0hsZRQe65xRx/ttdC6OM6OOHm8J3htwvJR2vIsoIoyKD2ooSqJoIoyjaB68RGSWVCg9BBH4EBGCWD2cRzkPig9BRASdc6DoBiXEjKv1/e1tG9NxztEGWvDx7/1f1lr/+tf6/r1V/wv58Y3iSHI4bhGpgqSuwAmpVEGe9wxO7yfWhT9X9M+Ty3FLGgqM0Gb/bc/7hlc83xY+8u4lf/5LlylGGgqc0GumAu9nOfe99xJWqT1LLXUHTn5bscWfulJKT3eGbO3t7c9yV4CP4VOtte1uyV17adGHxcXFj3Nych5wd+ASsqGhwWQ2WzzR0dGk0WgoNjaWrFZrFQ/hOJ4Rk/5Lcblc6tbW1keJiYkUExNDBoOB8vLyPFlZWZ7MzMw/UlNTHyQnJ69rtdoXefrNH4/ZbH6O5Ss2Rna7nUZHR2ljY4O2trZodXWVxsbGqK2tjYqKishkMh0xXpCWXl9sNtvL7MBJZWUlDQ4O0uTkJK2trdHe3p7A9vY2zc66aWRkhIaHh6mxsZFSU9O8HJXXpYR9agnKzc11p6WlUU1NDfX09FBXVxe1tLQQ5wQ1NTURGxBjDoeD6urqxHtvb68YwzqO2AdP7QSHs4d3f4qQQ3F1dTXOnTIyMpCAZ7BYLAIcJbLZMsQRyfNTUsxUUFCwKKn0X3hxHJfbTzhz7Ky0tJSVpVBSUtKFQHICWm0C6fV64nwR67hcKTs729PZ2VkiqfZP+vv7l7EYKCkpIZ1OJ7IfZYj2ImAsKiqKIiMjRZlyhYj1cIz17Uqqr5bl5eWw2dnZX/V6AyUkJAiEh4cLhIWF/atVAhwB4BkOxcfHi3nd3d2nR0dHdyQTvmV/f79uenpa7CQkJMQvBAcHCyjflW1hYSEdHh66JBO+5fj42I2aDg0NPVMMqNXqs1Z+Pv9+Wcu8QLu7u19IJnzLwcHBO2VlZSKUWHwRlDv1B8gR5ovHrP7q+2NnZ2cLiYczjIiIODt/+Rnne75POaYE+nCUyKOlpXs/M6VffW+sr6+/VVtbK2ocnM/8LhQgoeSkVL4r+/EsIy4uTlQAwo9ynpqaesjqr47A3Nzcq2A8JhAmFhsZjUZR26iKv1vf0OkAnXAeBMVsSuXl5cSJfV8y4VvcbnfZ0NCQWITFUAInnhQgrszMO4JHmpubaWZm5k3JhG/Z3Ny8zd5+X19fT8yGgnqhjK/bM8CA3BqNcvsPEHbcBfn5+VRVVUV9fX20uLj4kmTiallaWppyOrupoqJC0CrfbMIgwgpcFHoZGIfDYEJQOC6miYmJI78SUBaOgmZ8fPwb3HRwwGKxCuVISGWiKRMOwDPmIArp6eki/E6n07uwsFAtqfZf5ufny9n7EyQikgoGZK5H2V0GzIEjcBjODwwMrEgqn1w6OjpaOAlPQCRQjtqW+R4AW56HXP9YwxT8MYf+ep9ozAd23vXD8wZlNpQh92EenOBr+l3+ULmZ70NWqGW8zcp/v2zngCZUc8rjx3wdt/Oym/9s52OIY0MDbOQjxtf8/AO33zG+ZAfnOfR2nhawnxbsEMau8V+gUv0FJyWg+o5wz/8AAAAASUVORK5CYII=";
        const string VALUE_10 = "7df39ab6d5ea46e87b8a83870d05b202d946aa3999198ed543350c6ed211289e4fd1b289bc82190daf2ed41e1005b0bfaf98fc6e7d761d558129a35a529ff5b5";

        const string INI_FILE_PATH = @".\Example\Example.ini";

        #endregion

        /// <summary>
        /// Test #1 - IniSuccessfullyParsed.
        /// </summary>
        [Fact]
        public void ExampleIniSuccessfullyParsed()
        {
            var ini = new IniParser(INI_FILE_PATH);

            Assert.True(ini != null);
        }

        /// <summary>
        /// Test #2 - IniHashMatchesBaseHash.
        /// </summary>
        [Fact]
        public void ExampleIniHashMatchesPrecomputedHash()
        {
            byte[] iniBytes = File.ReadAllBytes(INI_FILE_PATH);
            string resultHash = GenerateSHA512String(iniBytes);

            Assert.Equal(EXAMPLE_INI_SHA512, resultHash);
        }

        /// <summary>
        /// Test #3 - CheckIfKeyValuesMatch.
        /// </summary>
        [Fact]
        public void CheckIfKeyValuesMatch()
        {
            var ini = new IniParser(INI_FILE_PATH);

            string value_1 = ini.GetValue(SECTION_1, KEY_1);
            string value_2 = ini.GetValue(SECTION_1, KEY_2);
            string value_3 = ini.GetValue(SECTION_1, KEY_3);
            string value_4 = ini.GetValue(SECTION_1, KEY_4);
            string value_5 = ini.GetValue(SECTION_2, KEY_5);
            string value_6 = ini.GetValue(SECTION_2, KEY_6);
            string value_7 = ini.GetValue(SECTION_2, KEY_7);
            string value_8 = ini.GetValue(SECTION_2, KEY_8);
            string value_9 = ini.GetValue(SECTION_3, KEY_9);
            string value_10 = ini.GetValue(SECTION_3, KEY_10);

            Assert.Equal(VALUE_1, value_1);
            Assert.Equal(VALUE_2, value_2);
            Assert.Equal(VALUE_3, value_3);
            Assert.Equal(VALUE_4, value_4);
            Assert.Equal(VALUE_5, value_5);
            Assert.Equal(VALUE_6, value_6);
            Assert.Equal(VALUE_7, value_7);
            Assert.Equal(VALUE_8, value_8);
            Assert.Equal(VALUE_9, value_9);
            Assert.Equal(VALUE_10, value_10);
        }

        /// <summary>
        /// Test #4 - SetKeyValues.
        /// </summary>
        [Fact]
        public void SetKeyValues()
        {
            var ini = new IniParser(INI_FILE_PATH);

            ini.SetValue(SECTION_1, KEY_1, VALUE_1);
            ini.SetValue(SECTION_1, KEY_2, VALUE_2);
            ini.SetValue(SECTION_1, KEY_3, VALUE_3);
            ini.SetValue(SECTION_1, KEY_4, VALUE_4);
            ini.SetValue(SECTION_2, KEY_5, VALUE_5);
            ini.SetValue(SECTION_2, KEY_6, VALUE_6);
            ini.SetValue(SECTION_2, KEY_7, VALUE_7);
            ini.SetValue(SECTION_2, KEY_8, VALUE_8);
            ini.SetValue(SECTION_3, KEY_9, VALUE_9);
            ini.SetValue(SECTION_3, KEY_10, VALUE_10);

            ini.SaveIni();
            ini.ReloadIni();

            string value_1 = ini.GetValue(SECTION_1, KEY_1);
            string value_2 = ini.GetValue(SECTION_1, KEY_2);
            string value_3 = ini.GetValue(SECTION_1, KEY_3);
            string value_4 = ini.GetValue(SECTION_1, KEY_4);
            string value_5 = ini.GetValue(SECTION_2, KEY_5);
            string value_6 = ini.GetValue(SECTION_2, KEY_6);
            string value_7 = ini.GetValue(SECTION_2, KEY_7);
            string value_8 = ini.GetValue(SECTION_2, KEY_8);
            string value_9 = ini.GetValue(SECTION_3, KEY_9);
            string value_10 = ini.GetValue(SECTION_3, KEY_10);

            Assert.Equal(VALUE_1, value_1);
            Assert.Equal(VALUE_2, value_2);
            Assert.Equal(VALUE_3, value_3);
            Assert.Equal(VALUE_4, value_4);
            Assert.Equal(VALUE_5, value_5);
            Assert.Equal(VALUE_6, value_6);
            Assert.Equal(VALUE_7, value_7);
            Assert.Equal(VALUE_8, value_8);
            Assert.Equal(VALUE_9, value_9);
            Assert.Equal(VALUE_10, value_10);
        }

        /// <summary>
        /// Test #5 - DeleteKeys.
        /// </summary>
        [Fact]
        public void DeleteKeys()
        {
            var ini = new IniParser(INI_FILE_PATH);

            bool deleted_1 = ini.DeleteKey(SECTION_1, KEY_1);
            bool deleted_2 = ini.DeleteKey(SECTION_1, KEY_2);
            bool deleted_3 = ini.DeleteKey(SECTION_1, KEY_3);
            bool deleted_4 = ini.DeleteKey(SECTION_1, KEY_4);
            bool deleted_5 = ini.DeleteKey(SECTION_2, KEY_5);
            bool deleted_6 = ini.DeleteKey(SECTION_2, KEY_6);
            bool deleted_7 = ini.DeleteKey(SECTION_2, KEY_7);
            bool deleted_8 = ini.DeleteKey(SECTION_2, KEY_8);
            bool deleted_9 = ini.DeleteKey(SECTION_3, KEY_9);
            bool deleted_10 = ini.DeleteKey(SECTION_3, KEY_10);

            ini.SaveIni();
            ini.ReloadIni();

            string value_1 = ini.GetValue(SECTION_1, KEY_1);
            string value_2 = ini.GetValue(SECTION_1, KEY_2);
            string value_3 = ini.GetValue(SECTION_1, KEY_3);
            string value_4 = ini.GetValue(SECTION_1, KEY_4);
            string value_5 = ini.GetValue(SECTION_2, KEY_5);
            string value_6 = ini.GetValue(SECTION_2, KEY_6);
            string value_7 = ini.GetValue(SECTION_2, KEY_7);
            string value_8 = ini.GetValue(SECTION_2, KEY_8);
            string value_9 = ini.GetValue(SECTION_3, KEY_9);
            string value_10 = ini.GetValue(SECTION_3, KEY_10);

            Assert.True(value_1 == null && deleted_1);
            Assert.True(value_2 == null && deleted_2);
            Assert.True(value_3 == null && deleted_3);
            Assert.True(value_4 == null && deleted_4);
            Assert.True(value_5 == null && deleted_5);
            Assert.True(value_6 == null && deleted_6);
            Assert.True(value_7 == null && deleted_7);
            Assert.True(value_8 == null && deleted_8);
            Assert.True(value_9 == null && deleted_9);
            Assert.True(value_10 == null && deleted_10);

            SetKeyValues();
        }

        /// <summary>
        /// Generates SHA512 hash represented as hex string (lowercase).
        /// </summary>
        private string GenerateSHA512String(byte[] inputBytes)
        {
            var sha512 = SHA512.Create();
            //byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hashBytes = sha512.ComputeHash(inputBytes);

            var sb = new StringBuilder();

            foreach (var hashByte in hashBytes)
            {
                sb.Append(hashByte.ToString("x2"));
            }

            return (sb.ToString());
        }
    }
}
