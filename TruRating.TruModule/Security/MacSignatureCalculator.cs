// The MIT License
// 
// Copyright (c) 2017 TruRating Ltd. https://www.trurating.com
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using TruRating.TruModule.Device;

namespace TruRating.TruModule.Security
{
    public class MacSignatureCalculator : IMacSignatureCalculator
    {
        private readonly ILogger _logger;
        private readonly string _transportKey;

        public MacSignatureCalculator(string transportKey, ILogger logger)
        {
            _transportKey = transportKey;
            _logger = logger;
        }

        public string Calculate(byte[] payload)
        {
            try
            {
                var payloadStream = new MemoryStream(payload) {Position = 0};
                //makr sure stream is at start
                //generate sugnature
                var des = TripleDES.Create();
                des.Key = Encoding.UTF8.GetBytes(_transportKey);
                des.Mode = CipherMode.ECB;
                var encryptor = des.CreateEncryptor();
                var hash = SHA256.Create().ComputeHash(payloadStream);
                var encryptedHash = encryptor.TransformFinalBlock(hash, 0, hash.Length);
                return BitConverter.ToString(encryptedHash).Replace("-", string.Empty);
            }
            catch (Exception e)
            {
                _logger.Error(e, "MacSignatureCalculator - Error during MAC calculation");
                return null;
            }
        }

        public string EncryptionScheme
        {
            get { return "3"; }
        }
    }
}